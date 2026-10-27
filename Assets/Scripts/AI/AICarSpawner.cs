using UnityEngine;
using System.Collections;

public class AICarSpawner : MonoBehaviour
{

    [SerializeField] GameObject[] carAIPrefabs;

    GameObject[] carAIPool = new GameObject[20];

    Transform playerCarTransform;
    WaitForSeconds wait = new WaitForSeconds(0.5f);


    [SerializeField] LayerMask otherCarsLayerMask;

    Collider[] overlappedCheckCollider = new Collider[1];
     
    float timeLastCarSpawned = 0;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        playerCarTransform = GameObject.FindGameObjectWithTag("Player").transform;
        int prefabIndex = 0;

        for (int i = 0; i < carAIPool.Length; i++){
            carAIPool[i] = Instantiate(carAIPrefabs[prefabIndex]);
            carAIPool[i].SetActive(false);

            prefabIndex++;

            if(prefabIndex > carAIPrefabs.Length - 1)
                prefabIndex = 0;
        }

        StartCoroutine(UpdateLessOftenCO());
    }

    IEnumerator UpdateLessOftenCO(){
        while(true){
            CleanUpCarsBeyondView();
            SpawnNewCars();
            yield return wait;
        }
    }

    void SpawnNewCars(){
        if (Time.time - timeLastCarSpawned < 2)
            return;

        GameObject carToSpawn = null;

        foreach(GameObject aiCar in carAIPool){
            if(aiCar.activeInHierarchy)
                continue;
            carToSpawn = aiCar;
            break;
        }

        if(carToSpawn == null)
            return;

        Vector3 spawnPosition = new Vector3(0, 0, playerCarTransform.position.z + 100);

        if(Physics.OverlapBoxNonAlloc(spawnPosition, Vector3.one * 2, overlappedCheckCollider, Quaternion.identity, otherCarsLayerMask) > 0){
            return;
        }

        carToSpawn.transform.position = spawnPosition;
        carToSpawn.SetActive(true);
        timeLastCarSpawned = Time.time;
    }

    void CleanUpCarsBeyondView(){
        foreach(GameObject aiCar in carAIPool){
            if(!aiCar.activeInHierarchy)
                continue;

            if(aiCar.transform.position.z - playerCarTransform.position.z > 200)
                aiCar.SetActive(false);

            if(aiCar.transform.position.z - playerCarTransform.position.z < -50)
                aiCar.SetActive(false);
        }
    }

}