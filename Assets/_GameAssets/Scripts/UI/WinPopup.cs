using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class WinPopup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TimerUI _timerUI;
    [SerializeField] private Button _oneMoreButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private TMP_Text _timerText;


    private void OnEnable() 
    {
        _timerText.text = _timerUI.GetFinalTime();
        _oneMoreButton.onClick.AddListener(OnOneMoreButtonClicked);
        //_mainMenuButton.onClick.AddListener(OnMainMenuButtonClicked);
    }

    private void OnOneMoreButtonClicked()
    {
        SceneManager.LoadScene(Consts.SceneNames.GAME_SCENE);
    }
    
}
