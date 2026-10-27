using System.Collections;
using UnityEngine;

public class EndlessLevelHandler : MonoBehaviour
{
    [Header("Track Configuration")]
    [SerializeField] private GameObject[] sectionsPrefabs;
    [SerializeField] private float sectionLength = 500f;

    [Header("Ambient Wind SFX")]
    [SerializeField] private AudioClip windClip; // just assign the wind sound asset here, no AudioSource needed on prefabs
    [SerializeField] private float windBaseVolume = 0.3f;   // volume while idle/slow
    [SerializeField] private float windMaxVolume = 0.9f;    // volume at top speed
    [SerializeField] private float windBasePitch = 0.85f;
    [SerializeField] private float windMaxPitch = 1.2f;
    [SerializeField] private float windMaxReferenceSpeed = 30f; // speed at which wind maxes out

    private GameObject[] sectionsPool = new GameObject[20];
    private AudioSource[] sectionsPoolWindAS = new AudioSource[20]; // wind AudioSource per pooled section
    private GameObject[] sections = new GameObject[10];
    private WaitForSeconds waitFor100ms = new WaitForSeconds(0.1f);

    private Transform playerCarTransform;
    private Rigidbody playerRb;
    private float currentMaxZ = 0f; 

    void Start()
    {
        // 1. SAFEGUARD: Check if the prefab array is empty before doing any math
        if (sectionsPrefabs == null || sectionsPrefabs.Length == 0)
        {
            Debug.LogError("CRITICAL ERROR: Please assign at least one prefab to 'Sections Prefabs' in the Inspector!", this);
            return; 
        }

        // 2. TAG FIX: Changed to capital "Player" to match Unity's built-in tag
        GameObject playerObj = GameObject.FindWithTag("Player");
        if (playerObj != null)
        {
            playerCarTransform = playerObj.transform;
            playerRb = playerObj.GetComponent<Rigidbody>();
        }
        else
        {
            Debug.LogError("CRITICAL ERROR: No GameObject with tag 'Player' found in the scene!", this);
        }

        // 3. POOL FIX: Populates the pool cleanly using modulo math
        if (windClip == null)
            Debug.LogWarning("No 'Wind Clip' assigned on EndlessLevelHandler — wind sound won't play.", this);

        for (int i = 0; i < sectionsPool.Length; i++)
        {
            int prefabIndex = i % sectionsPrefabs.Length;
            sectionsPool[i] = Instantiate(sectionsPrefabs[prefabIndex]);
            sectionsPool[i].SetActive(false);

            // Add a wind AudioSource in code — no need for one on the prefab itself
            AudioSource windAS = sectionsPool[i].AddComponent<AudioSource>();
            windAS.clip = windClip;
            windAS.loop = true;
            windAS.spatialBlend = 0f; // treat as ambient, not positional
            windAS.playOnAwake = false;
            windAS.volume = windBaseVolume;
            windAS.pitch = windBasePitch;

            sectionsPoolWindAS[i] = windAS;
        }

        // 4. INITIAL SPAWN: Places the first 10 sections end-to-end
        for (int i = 0; i < sections.Length; i++)
        {
            GameObject randomSection = GetRandomSectionFromPool();

            randomSection.transform.position = new Vector3(0, 0, i * sectionLength);
            randomSection.SetActive(true);

            sections[i] = randomSection;
        }

        // Tracks the absolute end point of our current world track
        currentMaxZ = (sections.Length - 1) * sectionLength;

        // Start wind playing on every currently active section (each has its own AudioSource now)
        for (int i = 0; i < sectionsPool.Length; i++)
        {
            if (sectionsPool[i].activeInHierarchy && sectionsPoolWindAS[i] != null)
                sectionsPoolWindAS[i].Play();
        }

        StartCoroutine(UpdateLessOftenCO());
    }

    IEnumerator UpdateLessOftenCO()
    {
        while (true)
        {
            if (playerCarTransform != null)
            {
                UpdateSectionPositions();
                UpdateAmbientWind();
            }
            yield return waitFor100ms;
        }
    }

    void UpdateSectionPositions()
    {
        for (int i = 0; i < sections.Length; i++)
        {
            // 5. MATH FIX: Checks if player has fully passed this specific piece
            if (sections[i].transform.position.z - playerCarTransform.position.z < -sectionLength)
            {
                StopSectionWind(sections[i]);
                sections[i].SetActive(false);

                GameObject newSection = GetRandomSectionFromPool();

                // Move the marker to the new absolute front, then position the piece there
                currentMaxZ += sectionLength;
                newSection.transform.position = new Vector3(0, 0, currentMaxZ);
                newSection.SetActive(true);
                PlaySectionWind(newSection);

                // Save the new piece back into the tracker slot
                sections[i] = newSection;
            }
        }
    }

    void UpdateAmbientWind()
    {
        if (playerRb == null) return;

        float speedPercentage = Mathf.Clamp01(playerRb.linearVelocity.z / windMaxReferenceSpeed);
        float targetVolume = Mathf.Lerp(windBaseVolume, windMaxVolume, speedPercentage);
        float targetPitch = Mathf.Lerp(windBasePitch, windMaxPitch, speedPercentage);

        // Find whichever active section the player is currently closest to,
        // and drive that section's wind AudioSource. Only one section should be "loud"
        // at a time so wind doesn't stack/echo across pieces.
        int closestIndex = -1;
        float closestDist = float.MaxValue;

        for (int i = 0; i < sections.Length; i++)
        {
            float dist = Mathf.Abs(sections[i].transform.position.z - playerCarTransform.position.z);
            if (dist < closestDist)
            {
                closestDist = dist;
                closestIndex = i;
            }
        }

        for (int i = 0; i < sections.Length; i++)
        {
            AudioSource windAS = GetWindASForSection(sections[i]);
            if (windAS == null) continue;

            if (i == closestIndex)
            {
                windAS.volume = targetVolume;
                windAS.pitch = targetPitch;
            }
            else
            {
                windAS.volume = 0f;
            }
        }
    }

    AudioSource GetWindASForSection(GameObject section)
    {
        for (int i = 0; i < sectionsPool.Length; i++)
        {
            if (sectionsPool[i] == section)
                return sectionsPoolWindAS[i];
        }
        return null;
    }

    void PlaySectionWind(GameObject section)
    {
        AudioSource windAS = GetWindASForSection(section);
        if (windAS != null && !windAS.isPlaying)
            windAS.Play();
    }

    void StopSectionWind(GameObject section)
    {
        AudioSource windAS = GetWindASForSection(section);
        if (windAS != null)
            windAS.Stop();
    }

    GameObject GetRandomSectionFromPool()
    {
        int randomIndex = Random.Range(0, sectionsPool.Length);
        bool isNewSectionFound = false;
        int attempts = 0;

        // Loop safeguard to prevent Unity from freezing if pool is entirely empty
        while (!isNewSectionFound && attempts < sectionsPool.Length)
        {
            if (!sectionsPool[randomIndex].activeInHierarchy)
            {
                isNewSectionFound = true;
            }
            else
            {
                randomIndex++;
                if (randomIndex >= sectionsPool.Length)
                    randomIndex = 0;
            }
            attempts++;
        }

        return sectionsPool[randomIndex];
    }
}