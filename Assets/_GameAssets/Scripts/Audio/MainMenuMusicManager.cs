using UnityEngine;

public class MainMenuMusicManager : MonoBehaviour
{
    [SerializeField] private AudioClip _mainMenuMusic;

    private void Start()
    {
        if (BackgroundMusic.Instance != null && _mainMenuMusic != null)
        {
            BackgroundMusic.Instance.ChangeMusic(_mainMenuMusic);
        }
    }
}