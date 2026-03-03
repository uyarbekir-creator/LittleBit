using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Playables;
using MagicPigGames;

public class PlayerStateUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private RectTransform _playerWalkingTransform;
    [SerializeField] private RectTransform _playerSlidingTransform;
    [SerializeField] private PlayableDirector _playableDirector;
    
    [Header("Booster Icons (Yıldırım, Yay, Radyasyon)")]
    [SerializeField] private Image _speedIcon;
    [SerializeField] private Image _jumpIcon;
    [SerializeField] private Image _slowIcon;

    [Header("Sliding Stamina UI")]
    [SerializeField] private HorizontalProgressBar _staminaProgressBar;

    [Header("Sprites (Walk/Slide)")]
    [SerializeField] private Sprite _playerWalkingActiveSprite;
    [SerializeField] private Sprite _playerWalkingPassiveSprite;
    [SerializeField] private Sprite _playerSlidingActiveSprite;
    [SerializeField] private Sprite _playerSlidingPassiveSprite;

    [Header("Booster Settings")]
    [SerializeField] private Color _activeColor = Color.white;
    [SerializeField] private Color _passiveColor = new Color(1, 1, 1, 0.3f);
    [SerializeField] private float _scaleAmount = 1.3f;
    [SerializeField] private float _scaleDuration = 0.2f;

    [Header("Movement Settings")]
    [SerializeField] private float _moveDuration = 0.5f;
    [SerializeField] private Ease _moveEase = Ease.OutBack;

    private Image _playerWalkingImage;
    private Image _playerSlidingImage;

    private void Awake()
    {
        if (_playerWalkingTransform != null) _playerWalkingImage = _playerWalkingTransform.GetComponent<Image>();
        if (_playerSlidingTransform != null) _playerSlidingImage = _playerSlidingTransform.GetComponent<Image>();
        
        // Başlangıçta ikonları sönük yap
        ResetBoosterUI(_speedIcon);
        ResetBoosterUI(_jumpIcon);
        ResetBoosterUI(_slowIcon);
    }

    private void Start()
    {
        if (_playableDirector != null) _playableDirector.stopped += OnTimelineFinished;
        if (_playerController != null) _playerController.OnPlayerStateChanged += PlayerController_OnPlayerStateChanged;
    }

    private void Update() => UpdateStaminaUI();

    private void UpdateStaminaUI()
    {
        if (_staminaProgressBar != null && _playerController != null)
            _staminaProgressBar.SetProgress(_playerController.GetStaminaNormalized());
    }

    // --- BOOSTER UI MANTIĞI ---

    public void ActivateBoosterUI(string type, float duration)
    {
        switch (type)
        {
            case "Speed": StartCoroutine(BoosterRoutine(_speedIcon, duration)); break;
            case "Jump":  StartCoroutine(BoosterRoutine(_jumpIcon, duration));  break;
            case "Slow":  StartCoroutine(BoosterRoutine(_slowIcon, duration));  break;
        }
    }

    private IEnumerator BoosterRoutine(Image icon, float duration)
    {
        if (icon == null) yield break;

        // Aktif Et: Rengi parlat ve büyüt
        icon.DOKill();
        icon.color = _activeColor;
        icon.transform.DOScale(_scaleAmount, _scaleDuration).SetEase(Ease.OutBack);

        yield return new WaitForSeconds(duration);

        // Pasif Et: Sönükleştir ve boyutu sıfırla
        ResetBoosterUI(icon);
    }

    private void ResetBoosterUI(Image icon)
    {
        if (icon == null) return;
        icon.DOKill();
        icon.color = _passiveColor;
        icon.transform.DOScale(1f, _scaleDuration).SetEase(Ease.InBack);
    }

    // --- STATE UI MANTIĞI ---

    private void OnTimelineFinished(PlayableDirector director) => 
        SetStateUserInterfaces(_playerWalkingActiveSprite, _playerSlidingPassiveSprite, _playerWalkingTransform, _playerSlidingTransform);

    private void PlayerController_OnPlayerStateChanged(PlayerState playerState)
    {
        switch (playerState)
        {
            case PlayerState.Idle:
            case PlayerState.Move:
                SetStateUserInterfaces(_playerWalkingActiveSprite, _playerSlidingPassiveSprite, _playerWalkingTransform, _playerSlidingTransform);
                break;
            case PlayerState.SlideIdle:
            case PlayerState.Slide:
                SetStateUserInterfaces(_playerWalkingPassiveSprite, _playerSlidingActiveSprite, _playerSlidingTransform, _playerWalkingTransform);
                break;
        }
    }

    private void SetStateUserInterfaces(Sprite walkingSprite, Sprite slidingSprite, RectTransform active, RectTransform passive)
    {
        if (_playerWalkingImage == null || _playerSlidingImage == null) return;
        _playerWalkingImage.sprite = walkingSprite;
        _playerSlidingImage.sprite = slidingSprite;

        active.DOKill(); passive.DOKill();
        active.DOAnchorPosX(-25f, _moveDuration).SetEase(_moveEase);
        passive.DOAnchorPosX(-90f, _moveDuration).SetEase(_moveEase);
    }

    private void OnDestroy()
    {
        if (_playableDirector != null) _playableDirector.stopped -= OnTimelineFinished;
        if (_playerController != null) _playerController.OnPlayerStateChanged -= PlayerController_OnPlayerStateChanged;
    }
}