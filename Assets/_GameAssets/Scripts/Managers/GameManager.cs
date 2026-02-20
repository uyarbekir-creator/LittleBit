using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action<GameState> OnGameStateChanged;

    [Header("References")] 
    [SerializeField] private EggCounterUI _eggCounterUI;
    [SerializeField] private WinLoseUI _winLoseUI;

    [Header("Settings")]
    [SerializeField] private int _maxEggcount = 5;

    private GameState _currentGameState;

    private int _currentEggCount;
    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable() 
    {
        ChangeGameState(GameState.Play);
    }

    public void ChangeGameState(GameState gamestate)
    {
        OnGameStateChanged?.Invoke(gamestate);
        _currentGameState = gamestate;
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

    public GameState GetCurrentGameState()
    {
        return _currentGameState;
    }

}