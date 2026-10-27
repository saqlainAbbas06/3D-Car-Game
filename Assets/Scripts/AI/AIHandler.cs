using UnityEngine;
using System.Collections;

public class AIHandler : MonoBehaviour
{

    [SerializeField]
    CarHandler CarHandler;

    [SerializeField]
    LayerMask otherCarsLayerMask;


    [SerializeField]
    MeshCollider meshCollider;


    [SerializeField]
    AudioSource honkHornAS;

    float hornVolume = 1.0f; // Set louder than engine volume so the horn stands out

    RaycastHit[] raycastHits = new RaycastHit[1];
    bool isCarAhead = false;
    float carAheadDistance = 0;



    int drivingInLane = 0;

    [Header("Honk Reaction")]
    [SerializeField] private float honkCooldown = 2f; // prevents spamming lane changes
    private float lastHonkReactionTime = -999f;

    WaitForSeconds wait = new WaitForSeconds(0.2f);
    private void Awake(){
        if(CompareTag("Player")){
            Destroy(this);
            return;
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (honkHornAS != null)
            honkHornAS.volume = hornVolume;

        StartCoroutine(UpdateLessOftenCO());
    }

    // Update is called once per frame
    void Update()
    {
        float accelerationInput = 1.0f;
        float steerInput = 0.0f;

        if (isCarAhead){

            accelerationInput = -1;
            if(carAheadDistance < 10 && !honkHornAS.isPlaying){
                honkHornAS.pitch = Random.Range(0.5f, 1.1f);
                honkHornAS.volume = 100;
                honkHornAS.Play();
            }

        }

        float desiredPositionX = Utils.CarLines[drivingInLane];
        
        float difference = desiredPositionX - transform.position.x;

        if (Mathf.Abs(difference) > 0.05f)
            steerInput = 1.0f * difference;

        steerInput = Mathf.Clamp(steerInput, -1.0f, 1.0f);

        CarHandler.SetInput(new Vector2(steerInput, accelerationInput));
    }

    IEnumerator UpdateLessOftenCO(){
        while(true){
            isCarAhead = CheckIfOtherCarsIsAhead();
            yield return wait;
        }
    }


    bool CheckIfOtherCarsIsAhead(){
        meshCollider.enabled = false;

        int numberOfHits = Physics.BoxCastNonAlloc(transform.position, Vector3.one * 0.25f, transform.forward, raycastHits, Quaternion.identity, 2, otherCarsLayerMask);

        meshCollider.enabled = true;

        if(numberOfHits > 0)
        {
            carAheadDistance = (transform.position - raycastHits[0].point).magnitude;
            return true;
        }

        return false;
    }

    // NEW: Called by the player's CarHandler when honked at from behind
    public void OnHonkedAt()
    {
        if (Time.time - lastHonkReactionTime < honkCooldown)
            return;

        lastHonkReactionTime = Time.time;

        // Pick a different lane than the current one so the player can pass
        int newLane = drivingInLane;
        int attempts = 0;

        while (newLane == drivingInLane && attempts < 10)
        {
            newLane = Random.Range(0, Utils.CarLines.Length);
            attempts++;
        }

        drivingInLane = newLane;
    }

    private void OnEnable(){
        CarHandler.SetMaxSpeed(Random.Range(2,4));

        drivingInLane = Random.Range(0, Utils.CarLines.Length);
    }
}