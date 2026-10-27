using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class UIHandler : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI distanceTravelledText;

    [SerializeField]
    TextMeshProUGUI gameOverText;

    [SerializeField]
    CanvasGroup gameOverCanvasGroup;

    [SerializeField] private string mainMenuSceneName = "Main Menu";
    CarHandler playerCarHandler;

    

    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerCarHandler = player.GetComponent<CarHandler>();
            playerCarHandler.OnPlayerCrashed += PlayerCarHandler_OnPlayerCrashed;
        }
        else
        {
            Debug.LogWarning("UIHandler: No GameObject tagged 'Player' found in the scene.");
        }
        gameOverCanvasGroup.interactable = false;
        gameOverCanvasGroup.alpha = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (playerCarHandler == null || distanceTravelledText == null) return;

        distanceTravelledText.text = playerCarHandler.DistanceTravelled.ToString("000000");
    }

    IEnumerator StartGameOverAnimationCO()
    {
        yield return new WaitForSecondsRealtime(3.0f);

        gameOverCanvasGroup.interactable = true;

        while (gameOverCanvasGroup.alpha < 0.8f)
        {
            gameOverCanvasGroup.alpha = Mathf.MoveTowards(gameOverCanvasGroup.alpha, 1.0f, Time.deltaTime * 2);

            yield return null;
        }
    }

    void PlayerCarHandler_OnPlayerCrashed(CarHandler obj)
    {
        gameOverText.text = $"DISTANCE {playerCarHandler.DistanceTravelled.ToString("000000")}";

        StartCoroutine(StartGameOverAnimationCO());
    }

    public void OneRestartClicked()
    {
        Time.timeScale = 1.0f;

        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    

    public void OnReturnToMenuClicked()
    {
        Time.timeScale = 1.0f; // in case the game was paused/frozen on game over
        SceneManager.LoadScene(mainMenuSceneName);
    }
}