using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using MaskTransitions;

public class LosePopup : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TimerUI _timerUI;
    [SerializeField] private Button _tryAgainButton;
    [SerializeField] private Button _mainMenuButton;
    [SerializeField] private TMP_Text _timerText;

    private void OnEnable() 
    {
        // Kamera sarsıntısını durdur
        if (CameraShake.Instance != null)
        {
            CameraShake.Instance.ResetShake();
        }

        _timerText.text = _timerUI.GetFinalTime();
        
        _tryAgainButton.onClick.RemoveAllListeners();
        _tryAgainButton.onClick.AddListener(OnTryAgainButtonClicked);
        _mainMenuButton.onClick.AddListener(() =>
        {
            TransitionManager.Instance.LoadLevel(Consts.SceneNames.MENU_SCENE);
        });
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    private void OnTryAgainButtonClicked()
    {
        Time.timeScale = 1f; 
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        TransitionManager.Instance.LoadLevel(Consts.SceneNames.GAME_SCENE);
    }
}