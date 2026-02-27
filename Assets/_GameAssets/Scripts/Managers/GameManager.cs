using System;
using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action<GameState> OnGameStateChanged;

    [Header("References")]
    [SerializeField] private CatController _catController;
    [SerializeField] private EggCounterUI _eggCounterUI;
    [SerializeField] private WinLoseUI _winLoseUI;
    [SerializeField] private PlayerHealthUI _playerHealthUI;

    [Header("Settings")]
    [SerializeField] private int _maxEggcount = 5;
    [SerializeField] private float _delay;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip _gameSceneMusic; // Oyun sahnesi için seçtiğin müziği buraya sürükle

    private GameState _currentGameState;
    private int _currentEggCount;
    private bool _isCatCatched;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // BackgroundMusic referansını kullanarak müziği değiştiriyoruz
        if (BackgroundMusic.Instance != null && _gameSceneMusic != null)
        {
            BackgroundMusic.Instance.ChangeMusic(_gameSceneMusic);
        }
        else if (BackgroundMusic.Instance != null)
        {
            BackgroundMusic.Instance.PlayBackgroundMusic(true);
        }

        if (HealthManager.Instance != null)
        {
            HealthManager.Instance.OnPlayerDeath += HealthManager_OnPlayerDeath;
        }

        if (_catController != null)
        {
            _catController.OnCatCatched += CatController_OnCatCatched;
        }
    }

    private void OnEnable()
    {
        ChangeGameState(GameState.Cutscene);
    }

    private void CatController_OnCatCatched()
    {
        if (!_isCatCatched)
        {
            _isCatCatched = true;
            _playerHealthUI.AnimateDamageForAll();
            StartCoroutine(OnGameOver(true));
            CameraShake.Instance.ShakeCamera(1f, 1f);
        }
    }

    private void HealthManager_OnPlayerDeath()
    {
        StartCoroutine(OnGameOver(false));
    }

    public void ChangeGameState(GameState gamestate)
    {
        _currentGameState = gamestate;
        OnGameStateChanged?.Invoke(gamestate);

        // İpucu: Eğer Game Over olduğunda müziğin durmasını istersen buraya ekleyebilirsin:
        // if(gamestate == GameState.GameOver && BackgroundMusic.Instance != null) BackgroundMusic.Instance.PlayBackgroundMusic(false);
    }

    public void OnEggCollected()
    {
        _currentEggCount++;
        _eggCounterUI.SetEggCounterText(_currentEggCount, _maxEggcount);

        if (_currentEggCount == _maxEggcount)
        {
            _eggCounterUI.SetEggCompleted();
            ChangeGameState(GameState.GameOver);
            _winLoseUI.OnGameWin();
        }
    }

    private IEnumerator OnGameOver(bool isCatCatched)
    {
        yield return new WaitForSeconds(_delay);
        ChangeGameState(GameState.GameOver);
        _winLoseUI.OnGameLose();
        
        if (isCatCatched)
        {
            AudioManager.Instance.Play(SoundType.CatSound);
        }
    }

    public GameState GetCurrentGameState()
    {
        return _currentGameState;
    }

    private void OnDisable()
    {
        if (HealthManager.Instance != null)
        {
            HealthManager.Instance.OnPlayerDeath -= HealthManager_OnPlayerDeath;
        }

        if (_catController != null)
        {
            _catController.OnCatCatched -= CatController_OnCatCatched;
        }
    }
}