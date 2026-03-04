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

    [Header("Booster Visual Settings")]
    [Tooltip("HDR Parlama için Intensity'yi yüksek tutun")]
    [ColorUsage(true, true)] 
    [SerializeField] private Color _activeColor = new Color(4f, 4f, 4f, 1f); 
    [SerializeField] private Color _passiveColor = new Color(1f, 1f, 1f, 0.3f);
    
    [Space]
    [SerializeField] private float _passiveScale = 1.5f; // Pasif durumdaki sabit büyüklük
    [SerializeField] private float _activeScale = 3.0f;  // Aktif durumdaki devasa büyüklük
    [SerializeField] private float _scaleDuration = 0.25f;

    [Header("Movement Settings (Walk/Slide Icons)")]
    [SerializeField] private float _moveDuration = 0.5f;
    [SerializeField] private Ease _moveEase = Ease.OutBack;

    private Image _playerWalkingImage;
    private Image _playerSlidingImage;

    private void Awake()
    {
        // Image bileşenlerini al
        if (_playerWalkingTransform != null) _playerWalkingImage = _playerWalkingTransform.GetComponent<Image>();
        if (_playerSlidingTransform != null) _playerSlidingImage = _playerSlidingTransform.GetComponent<Image>();
        
        // Başlangıçta ikonları pasif (1.5x) durumuna getir
        ResetToPassiveState(_speedIcon);
        ResetToPassiveState(_jumpIcon);
        ResetToPassiveState(_slowIcon);
    }

    private void Start()
    {
        if (_playableDirector != null) _playableDirector.stopped += OnTimelineFinished;
        if (_playerController != null) _playerController.OnPlayerStateChanged += PlayerController_OnPlayerStateChanged;
    }

    private void Update()
    {
        UpdateStaminaUI();
    }

    private void UpdateStaminaUI()
    {
        if (_staminaProgressBar != null && _playerController != null)
        {
            _staminaProgressBar.SetProgress(_playerController.GetStaminaNormalized());
        }
    }

    // --- BOOSTER UI MANTIĞI ---

    public void ActivateBoosterUI(string type, float duration)
    {
        // Animasyonların çakışmaması için diğer rutinleri durdur
        StopAllCoroutines(); 

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

        // Önceki Tween'leri temizle
        icon.DOKill();
        icon.transform.DOKill();

        // AKTİF ET: Rengi parlat ve 3 katına çıkar (Booster süresince böyle kalacak)
        icon.color = _activeColor;
        icon.transform.DOScale(_activeScale, _scaleDuration).SetEase(Ease.OutBack);

        // Booster süresi kadar bekle
        yield return new WaitForSeconds(duration);

        // PASİF ET: Eski pasif boyutuna (1.5x) ve rengine dön
        ResetToPassiveState(icon);
    }

    private void ResetToPassiveState(Image icon)
    {
        if (icon == null) return;
        icon.DOKill();
        icon.transform.DOKill();
        
        icon.color = _passiveColor;
        // DOTween ile yumuşak geçiş yaparak 1.5x boyutuna döner
        icon.transform.DOScale(_passiveScale, _scaleDuration).SetEase(Ease.InBack);
    }

    // --- STATE UI MANTIĞI (Walk/Slide İkon Kaydırma) ---

    private void OnTimelineFinished(PlayableDirector director)
    {
        SetStateUserInterfaces(_playerWalkingActiveSprite, _playerSlidingPassiveSprite, _playerWalkingTransform, _playerSlidingTransform);
    }

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

        active.DOKill(); 
        passive.DOKill();

        active.DOAnchorPosX(-25f, _moveDuration).SetEase(_moveEase).SetLink(active.gameObject);
        passive.DOAnchorPosX(-90f, _moveDuration).SetEase(_moveEase).SetLink(passive.gameObject);
    }

    private void OnDestroy()
    {
        if (_playableDirector != null) _playableDirector.stopped -= OnTimelineFinished;
        if (_playerController != null) _playerController.OnPlayerStateChanged -= PlayerController_OnPlayerStateChanged;
    }
}