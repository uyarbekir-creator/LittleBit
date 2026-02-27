using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using MaskTransitions;
using System;

public class SettingsUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject _settingsPopupObject;
    [SerializeField] private GameObject _blackBackgroundObject;

    [Header("Buttons")]
    [SerializeField] private Button _settingsButton;
    [SerializeField] private Button _musicButton;
    [SerializeField] private Button _soundButton;
    [SerializeField] private Button _resumeButton;
    [SerializeField] private Button _mainMenuButton;

    [Header("Sprites")]
    [SerializeField] private Sprite _musicActiveSprite;
    [SerializeField] private Sprite _musicPassiveSprite;
    [SerializeField] private Sprite _soundActiveSprite;
    [SerializeField] private Sprite _soundPassiveSprite;

    [Header("Settings")]
    [SerializeField] private float _animationDuration;

    private Image _blackBackgroundImage;

    private bool _isMusicActive = true;
    private bool _isSoundActive = true;
    private bool _isSettingsOpen;

    private void Awake()
    {
        _blackBackgroundImage = _blackBackgroundObject.GetComponent<Image>();
        _settingsPopupObject.transform.localScale = Vector3.zero;

        _settingsButton.onClick.AddListener(OnSettingsButtonClicked);
        _resumeButton.onClick.AddListener(OnResumeButtonClicked);

        _mainMenuButton.onClick.AddListener(() =>
        {
            AudioManager.Instance.Play(SoundType.TransitionSound);
            TransitionManager.Instance.LoadLevel(Consts.SceneNames.MENU_SCENE);
        });

        _musicButton.onClick.AddListener(OnMusicButtonClicked);
        _soundButton.onClick.AddListener(OnSoundButtonClicked);
    }

    private void Start()
    {
        _musicButton.image.sprite = _isMusicActive ? _musicActiveSprite : _musicPassiveSprite;
        _soundButton.image.sprite = _isSoundActive ? _soundActiveSprite : _soundPassiveSprite;
    }

    private void OnMusicButtonClicked()
    {
        AudioManager.Instance.Play(SoundType.ButtonClickSound);
        _isMusicActive = !_isMusicActive;
        _musicButton.image.sprite = _isMusicActive ? _musicActiveSprite : _musicPassiveSprite;
        BackgroundMusic.Instance.SetMusicMute(!_isMusicActive);
    }

    private void OnSoundButtonClicked()
    {
        AudioManager.Instance.Play(SoundType.ButtonClickSound);
        _isSoundActive = !_isSoundActive;
        _soundButton.image.sprite = _isSoundActive ? _soundActiveSprite : _soundPassiveSprite;
        AudioManager.Instance.SetSoundEffectsMute(!_isSoundActive);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (_isSettingsOpen)
            {
                OnResumeButtonClicked();
            }
            else
            {
                OnSettingsButtonClicked();
            }
        }
    }

    private void OnSettingsButtonClicked()
    {
        if (_isSettingsOpen) return;

        AudioManager.Instance.Play(SoundType.ButtonClickSound);
        
        _isSettingsOpen = true;
        GameManager.Instance.ChangeGameState(GameState.Pause);
        
        _blackBackgroundObject.SetActive(true);
        _settingsPopupObject.SetActive(true);

        _blackBackgroundImage.DOFade(0.8f, _animationDuration).SetEase(Ease.Linear);
        _settingsPopupObject.transform.DOScale(1.5f, _animationDuration).SetEase(Ease.OutBack).OnComplete(() => {
            ToggleCursor(true);
        });
    }

    private void OnResumeButtonClicked()
    {
        if (!_isSettingsOpen) return;

        AudioManager.Instance.Play(SoundType.TransitionSound);
        
        _isSettingsOpen = false;
        GameManager.Instance.ChangeGameState(GameState.Resume);

        _blackBackgroundImage.DOFade(0f, _animationDuration).SetEase(Ease.Linear);
        _settingsPopupObject.transform.DOScale(0f, _animationDuration).SetEase(Ease.OutExpo).OnComplete(() =>
        {
            _blackBackgroundObject.SetActive(false);
            _settingsPopupObject.SetActive(false);
            ToggleCursor(false);
        });
    }

    private void ToggleCursor(bool isVisible)
    {
        if (isVisible)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}