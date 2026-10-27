using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUIHandler : MonoBehaviour
{
    [Header("Menu Music")]
    [SerializeField] private AudioClip menuMusicClip;
    [SerializeField] private float musicVolume = 1f;
    [SerializeField] private bool loopMusic = true;

    private AudioSource menuMusicAS;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        menuMusicAS = gameObject.AddComponent<AudioSource>();

        if (menuMusicClip != null)
        {
            menuMusicAS.clip = menuMusicClip;
            menuMusicAS.volume = musicVolume;
            menuMusicAS.loop = loopMusic;
            menuMusicAS.playOnAwake = false;
            menuMusicAS.Play();
        }
        else
        {
            Debug.LogWarning("MainMenuUIHandler: menuMusicClip is not assigned!");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnStartGameClicked()
    {
        StopMenuMusic();
        SceneManager.LoadScene("Stage");
    }
     

    void StopMenuMusic()
    {
        if (menuMusicAS != null)
            menuMusicAS.Stop();
    }
}