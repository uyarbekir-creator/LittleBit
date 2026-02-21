using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class WinLoseUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _blackBackgroundObject;
    [SerializeField] private GameObject _winPopUp;
    [SerializeField] private GameObject _losePopUp;

    [Header("Settings")]
    [SerializeField] private float _animationDuration = 0.3f;

    private Image _blackBackgroundImage;
    private RectTransform _winPopUpTransform;
    private RectTransform _losePopUpTransform;

    private void Awake()
    {
        _blackBackgroundImage = _blackBackgroundObject.GetComponent<Image>();
        _winPopUpTransform = _winPopUp.GetComponent<RectTransform>();
        _losePopUpTransform = _losePopUp.GetComponent<RectTransform>();
    }

    public void OnGameWin()
    {
        ShowUI(_winPopUp, _winPopUpTransform);
    }

    public void OnGameLose()
    {
        ShowUI(_losePopUp, _losePopUpTransform);
    }

    private void ShowUI(GameObject popup, RectTransform popupTransform)
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Time.timeScale = 0f; 

        _blackBackgroundObject.SetActive(true);
        popup.SetActive(true);

        // Animasyonlar
        _blackBackgroundImage.DOFade(0.8f, _animationDuration).SetEase(Ease.Linear).SetUpdate(true);
        popupTransform.DOScale(1.5f, _animationDuration).SetEase(Ease.OutBack).SetUpdate(true);
        
    }
}