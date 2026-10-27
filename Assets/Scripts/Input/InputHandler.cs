using UnityEngine;
using UnityEngine.SceneManagement;

public class InputHandler : MonoBehaviour
{
    [SerializeField]
    private CarHandler carHandler;

    private void Update()
    {
        Vector2 input = Vector2.zero;

        // Get player input
        input.x = Input.GetAxis("Horizontal");
        input.y = Input.GetAxis("Vertical");

        // Send input to the car
        carHandler.SetInput(input);

        // Honk the horn on Space
        if (Input.GetKeyDown(KeyCode.Space))
        {
            carHandler.PlayHorn();
        }

        // Restart the current scene when R is pressed
        if (Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1.0f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}