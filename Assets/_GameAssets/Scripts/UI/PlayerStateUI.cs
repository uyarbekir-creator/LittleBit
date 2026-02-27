using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Playables;
using System;
using MagicPigGames;

public class PlayerStateUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private RectTransform _playerWalkingTransform;
    [SerializeField] private RectTransform _playerSlidingTransform;
    [SerializeField] private RectTransform _boosterSpeedTransform;
    [SerializeField] private RectTransform _boosterJumpTransform;
    [SerializeField] private RectTransform _boosterSlowTransform;
    [SerializeField] private PlayableDirector _playableDirector;
    
    [Header("Sliding Stamina UI")]
    [SerializeField] private HorizontalProgressBar _staminaProgressBar;
    [SerializeField] private GameObject _shiftKeyIconObject;

    [Header("Images")]
    [SerializeField] private Image _goldboosterWheatImage;
    [SerializeField] private Image _holyboosterWheatImage;
    [SerializeField] private Image _rottenboosterWheatImage;

    [Header("Sprites")]
    [SerializeField] private Sprite _playerWalkingActiveSprite;
    [SerializeField] private Sprite _playerWalkingPassiveSprite;
    [SerializeField] private Sprite _playerSlidingActiveSprite;
    [SerializeField] private Sprite _playerSlidingPassiveSprite;

    [Header("Settings")]
    [SerializeField] private float _moveDuration = 0.5f;
    [SerializeField] private Ease _moveEase = Ease.OutBack;

    public RectTransform GetBoosterSpeedTransform => _boosterSpeedTransform;
    public RectTransform GetBoosterJumpTransform => _boosterJumpTransform;
    public RectTransform GetBoosterSlowTransform => _boosterSlowTransform;

    public Image GetGoldBoosterWheatImage => _goldboosterWheatImage;
    public Image GetHolyBoosterWheatImage => _holyboosterWheatImage;
    public Image GetRottenBoosterWheatImage => _rottenboosterWheatImage;

    private Image _playerWalkingImage;
    private Image _playerSlidingImage;

    private void Awake()
    {
        if (_playerWalkingTransform != null) _playerWalkingImage = _playerWalkingTransform.GetComponent<Image>();
        if (_playerSlidingTransform != null) _playerSlidingImage = _playerSlidingTransform.GetComponent<Image>();
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
            float staminaRatio = _playerController.GetStaminaNormalized();
            _staminaProgressBar.SetProgress(staminaRatio);
        }
    }

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

    private void SetStateUserInterfaces(Sprite playerWalkingSprite, Sprite playerSlidingSprite,
        RectTransform activeTransform, RectTransform passiveTransform)
    {
        if (_playerWalkingImage == null || _playerSlidingImage == null) return;

        _playerWalkingImage.sprite = playerWalkingSprite;
        _playerSlidingImage.sprite = playerSlidingSprite;

        activeTransform.DOKill();
        passiveTransform.DOKill();

        activeTransform.DOAnchorPosX(-25f, _moveDuration).SetEase(_moveEase).SetLink(activeTransform.gameObject);
        passiveTransform.DOAnchorPosX(-90f, _moveDuration).SetEase(_moveEase).SetLink(passiveTransform.gameObject);
    }

    private IEnumerator SetBoosterUserInterfaces(RectTransform activeTransform, Image boosterImage,
        Image wheatImage, Sprite activeSprite, Sprite passiveSprite, Sprite activeWheatSprite, Sprite passiveWheatSprite,
        float duration)
    {
        boosterImage.sprite = activeSprite;
        wheatImage.sprite = activeWheatSprite;

        activeTransform.DOKill();
        activeTransform.DOAnchorPosX(25f, _moveDuration).SetEase(_moveEase).SetLink(activeTransform.gameObject);
        
        yield return new WaitForSeconds(duration);
        
        boosterImage.sprite = passiveSprite;
        wheatImage.sprite = passiveWheatSprite;
        activeTransform.DOAnchorPosX(90f, _moveDuration).SetEase(_moveEase).SetLink(activeTransform.gameObject);
    }

    public void PlayBoosterUIAnimations(RectTransform activeTransform, Image boosterImage,
        Image wheatImage, Sprite activeSprite, Sprite passiveSprite, Sprite activeWheatSprite, Sprite passiveWheatSprite,
        float duration)
    {
        StartCoroutine(SetBoosterUserInterfaces(activeTransform, boosterImage, wheatImage, activeSprite, passiveSprite,
        activeWheatSprite, passiveWheatSprite, duration));
    }

    private void OnDestroy()
    {
        if (_playableDirector != null) _playableDirector.stopped -= OnTimelineFinished;
        if (_playerController != null) _playerController.OnPlayerStateChanged -= PlayerController_OnPlayerStateChanged;
    }
}