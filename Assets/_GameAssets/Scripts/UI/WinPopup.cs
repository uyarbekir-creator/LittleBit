using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MaskTransitions;

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
        
        _oneMoreButton.onClick.RemoveAllListeners();
        _oneMoreButton.onClick.AddListener(OnOneMoreButtonClicked);
        _mainMenuButton.onClick.AddListener(() =>
        {
            TransitionManager.Instance.LoadLevel(Consts.SceneNames.MENU_SCENE);
        });
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnOneMoreButtonClicked()
    {
        Time.timeScale = 1f; 
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        TransitionManager.Instance.LoadLevel(Consts.SceneNames.GAME_SCENE);
    }
}