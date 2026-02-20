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
        _blackBackgroundObject.SetActive(true);
        _winPopUp.SetActive(true);

        _blackBackgroundImage.DOFade(0.8f, _animationDuration).SetEase(Ease.Linear);
        _winPopUpTransform.DOScale(1.5f, _animationDuration).SetEase(Ease.OutBack);
    }

    public void OnGameLose()
    {
        _blackBackgroundObject.SetActive(true);
        _losePopUp.SetActive(true);

        _blackBackgroundImage.DOFade(0.8f, _animationDuration).SetEase(Ease.Linear);
        _losePopUpTransform.DOScale(1.5f, _animationDuration).SetEase(Ease.OutBack);
    }
}
