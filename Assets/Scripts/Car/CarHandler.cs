using UnityEngine;
using System.Collections;
using System;

public class CarHandler : MonoBehaviour 
{ 
    [SerializeField] Rigidbody rb; 
    [SerializeField] Transform gameModel; 

    [Header("SFX Configuration")]
    [SerializeField] private AudioSource carEngineAS;
    [SerializeField] private AnimationCurve carPitchAnimationCurve;
    [SerializeField] private AudioSource carSkidAS;
    [SerializeField] private AudioSource carCrashAS;

    [Header("Horn Settings")]
    [SerializeField] private AudioSource carHornAS;
    [SerializeField] private float hornRaycastDistance = 15f;
    [SerializeField] private LayerMask otherCarsLayerMask;

    [Header("Fire VFX Settings")]
    [SerializeField] private ParticleSystem fireVFXPrefab; 
    [SerializeField] private Transform fireSpawnPoint;      
    [SerializeField] private float fireDelay = 1.0f; // Delay before fire starts burning after crash

    [Header("Crash Settings")]
    [SerializeField] private Renderer carRenderer; 
    public Renderer CarMeshRender => carRenderer;  
    [SerializeField] private Color crashColor = Color.black; 
    [SerializeField] private float colorChangeDelay = 0.5f; // Delay before car changes color after crash


    bool isPlayer = true;

    float maxSteerVelocity = 2; 
    float maxForwardVelocity = 30; 
    float accelerationMultipler = 2; 
    float breaksMultiplier = 15; 
    float steeringMultipler = 5; 
    float carMaxSpeedPercentage = 0;

    Vector2 input = Vector2.zero; 
    bool isStopped = false;
    bool hasCrashed = false;
    bool hasCaughtFire = false;

    float carStartPositionZ;
    public float DistanceTravelled { get; private set; } = 0f;

    public event Action<CarHandler> OnPlayerCrashed;

    RaycastHit[] hornRaycastHits = new RaycastHit[1];

    void Start()
    {
        isPlayer = CompareTag("Player");
        if (rb == null) rb = GetComponent<Rigidbody>();

        carStartPositionZ = transform.position.z;

        if (carEngineAS != null)
            carEngineAS.Play();
        else
            Debug.LogWarning($"[{gameObject.name}] carEngineAS is missing from the Inspector!");
    }

    void Update() 
    { 
        if (isStopped)
        {
            FadeOutCarAudio();
            return; // Stop all updates if crashed
        }

        gameModel.transform.rotation = Quaternion.Euler(0, rb.linearVelocity.x * 5, 0); 

        UpdateCarAudio();

        DistanceTravelled = transform.position.z - carStartPositionZ;
    }

    private void FixedUpdate() 
    { 
        if (isStopped)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            return;
        }

        if (input.y > 0) 
            Accelerate(); 
        else 
            rb.linearDamping = 0.2f; 

        if (input.y < 0) 
            Brake(); 

        Steer(); 

        if (rb.linearVelocity.z <= 0) 
            rb.linearVelocity = Vector3.zero; 
    } 

    void Accelerate() 
    { 
        rb.linearDamping = 0; 
        if (rb.linearVelocity.z >= maxForwardVelocity) return;
        rb.AddForce(rb.transform.forward * accelerationMultipler * input.y); 
    } 

    void Brake() 
    { 
        if (rb.linearVelocity.z <= 0) return; 
        rb.AddForce(rb.transform.forward * breaksMultiplier * input.y); 
    } 

    void Steer() 
    { 
        if (Mathf.Abs(input.x) > 0) 
        { 
            float speedBaseSteerLimit = rb.linearVelocity.z / 5.0f; 
            speedBaseSteerLimit = Mathf.Clamp01(speedBaseSteerLimit); 
            rb.AddForce(rb.transform.right * steeringMultipler * input.x * speedBaseSteerLimit); 

            float normalizedX = rb.linearVelocity.x / maxSteerVelocity; 
            normalizedX = Mathf.Clamp(normalizedX, -1.0f, 1.0f); 

            rb.linearVelocity = new Vector3(normalizedX * maxSteerVelocity, rb.linearVelocity.y, rb.linearVelocity.z); 
        } 
        else 
        { 
            Vector3 targetVelocity = new Vector3(0, rb.linearVelocity.y, rb.linearVelocity.z);
            rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, targetVelocity, Time.fixedDeltaTime * 3); 
        } 
    } 

    void UpdateCarAudio()
    {
        if (carEngineAS == null || carSkidAS == null) return;

        carMaxSpeedPercentage = rb.linearVelocity.z / maxForwardVelocity;
        carEngineAS.pitch = carPitchAnimationCurve.Evaluate(carMaxSpeedPercentage);

        // Check if player is hitting brakes while moving forward
        if (input.y < 0 && carMaxSpeedPercentage > 0.2f)
        {
            if (!carSkidAS.isPlaying)
            {
                carSkidAS.Play();
            }
            carSkidAS.volume = Mathf.Lerp(carSkidAS.volume, 2.0f, Time.deltaTime * 10f);
            carEngineAS.volume = Mathf.Lerp(carEngineAS.volume, 0.15f, Time.deltaTime * 20f);
        }
        else
        {
            carSkidAS.volume = Mathf.Lerp(carSkidAS.volume, 0f, Time.deltaTime * 30f);
            carEngineAS.volume = Mathf.Lerp(carEngineAS.volume, 1.0f, Time.deltaTime * 10f);
        }
    }

    void FadeOutCarAudio()
    {
        if (carEngineAS != null && carEngineAS.isPlaying)
            carEngineAS.volume = Mathf.Lerp(carEngineAS.volume, 0f, Time.deltaTime * 10f);

        if (carSkidAS != null && carSkidAS.isPlaying)
            carSkidAS.volume = Mathf.Lerp(carSkidAS.volume, 0f, Time.deltaTime * 10f);
    }

    public void SetInput(Vector2 inputVector) 
    { 
        if (isStopped) return;
        input = inputVector.normalized; 
    } 

    public void SetMaxSpeed(float newMaxSpeed)
    {
        maxForwardVelocity = newMaxSpeed;
    }

    // NEW: Called by InputHandler when the player presses the horn key (e.g. Space)
    public void PlayHorn()
    {
        if (isStopped) return;

        if (carHornAS != null && !carHornAS.isPlaying)
            carHornAS.Play();

        // Raycast forward to check if there's an AI car directly ahead
        int hitCount = Physics.RaycastNonAlloc(
            transform.position,
            transform.forward,
            hornRaycastHits,
            hornRaycastDistance,
            otherCarsLayerMask
        );

        if (hitCount > 0)
        {
            AIHandler aiAhead = hornRaycastHits[0].collider.GetComponentInParent<AIHandler>();
            if (aiAhead != null)
            {
                aiAhead.OnHonkedAt();
            }
        }
    }

    void SpawnFire()
    {
        if (!hasCaughtFire && fireVFXPrefab != null && fireSpawnPoint != null)
        {
            ParticleSystem spawnedFire = Instantiate(fireVFXPrefab, fireSpawnPoint.position, fireSpawnPoint.rotation);
            spawnedFire.transform.SetParent(transform);
            spawnedFire.Play();
            hasCaughtFire = true;
        }
    }

    void ChangeColor()
    {
        if (carRenderer != null)
        {
            carRenderer.material.color = crashColor;
        }
    }

    IEnumerator ChangeColorDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        ChangeColor();
    }

    IEnumerator SpawnFireDelayed(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnFire();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(!isPlayer){
            if(collision.transform.root.CompareTag("Untagged"))
                return;
            if(collision.transform.root.CompareTag("CarAI"))
                return;
        }

        Debug.Log($"Hit: {collision.gameObject.name} (tag: {collision.gameObject.tag})");
        TriggerCrashSequence();

        CarHandler otherCarHandler = collision.gameObject.GetComponent<CarHandler>();
        if (otherCarHandler != null)
        {
            otherCarHandler.TriggerCrashSequence();
        }

        OnPlayerCrashed?.Invoke(this);
    }

    public void TriggerCrashSequence()
    {
        if (isStopped) return;

        Debug.Log("Crash");
        isStopped = true;
        hasCrashed = true;
        input = Vector2.zero;

        // Color change and fire now happen after a short delay instead of instantly
        StartCoroutine(ChangeColorDelayed(colorChangeDelay));
        StartCoroutine(SpawnFireDelayed(fireDelay));

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeAll; // instantly locks position + rotation, kills residual bounce

        if (carCrashAS != null)
        {
            // Alter crashing audio tone/volume dynamically based on structural impact velocity
            carCrashAS.volume = Mathf.Clamp(carMaxSpeedPercentage, 0.25f, 1.0f);
            carCrashAS.pitch = Mathf.Clamp(carMaxSpeedPercentage, 0.3f, 1.0f);

            if (carEngineAS != null) carEngineAS.Stop();
            carCrashAS.Play();
        }

        Debug.Log($"{gameObject.name} stopped and crashed cleanly!");
    }
}