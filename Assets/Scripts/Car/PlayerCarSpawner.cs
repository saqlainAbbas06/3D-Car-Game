using UnityEngine;
using Unity.Cinemachine;

public class PlayerCarSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject[] carPrefabs;

    [Header("Camera")]
    [SerializeField]
    CinemachineCamera CinemachineCamera;

    [Header("Menu")]
    [SerializeField]
    bool isMainMenu = false;

    GameObject instantiatedPlayerCar = null;

    int carIndex = 0;

    static GameObject selectedCarPrefab = null;

    Quaternion carRotation = Quaternion.identity;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (!isMainMenu)
        {
            // Gameplay: spawn the FULL car prefab (CarHandler, Rigidbody, Collider, "Player" tag, etc.)
            GameObject prefabToSpawn = selectedCarPrefab != null ? selectedCarPrefab : carPrefabs[carIndex];

            instantiatedPlayerCar = Instantiate(prefabToSpawn);
            selectedCarPrefab = prefabToSpawn;
        }
        else
        {
            // Main menu: just show the spinning mesh preview, no physics needed
            GameObject prefabToPreview = selectedCarPrefab != null ? selectedCarPrefab : carPrefabs[0];

            instantiatedPlayerCar = Instantiate(prefabToPreview.GetComponent<CarHandler>().CarMeshRender.gameObject);
        }

        if (CinemachineCamera != null)
        {
            CinemachineCamera.Follow = instantiatedPlayerCar.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isMainMenu)
        {
            instantiatedPlayerCar.transform.Rotate(new Vector3(0, 20, 0) * Time.deltaTime);

            carRotation = instantiatedPlayerCar.transform.rotation;
        }
    }

    void ChangeCar()
    {
        Destroy(instantiatedPlayerCar);

        if (isMainMenu)
        {
            instantiatedPlayerCar = Instantiate(carPrefabs[carIndex].GetComponent<CarHandler>().CarMeshRender.gameObject);
        }
        else
        {
            instantiatedPlayerCar = Instantiate(carPrefabs[carIndex]);
        }

        selectedCarPrefab = carPrefabs[carIndex];

        instantiatedPlayerCar.transform.rotation = carRotation;
    }

    public void OnNextCarClicked()
    {
        carIndex++;

        if (carIndex > carPrefabs.Length - 1)
            carIndex = 0;

        ChangeCar();
    }

    public void OnPreviousCarClicked()
    {
        carIndex--;

        if (carIndex < 0)
            carIndex = carPrefabs.Length - 1;

        ChangeCar();
    }
}