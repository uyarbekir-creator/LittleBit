using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public event Action<GameState> OnGameStateChanged;

    [Header("References")] 
    [SerializeField] private EggCounterUI _eggCounterUI;

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
        Debug.Log($"Game State: " + gamestate);
    }

    public void OnEggCollected()
    {
        _currentEggCount++;
        _eggCounterUI.SetEggCounterText(_currentEggCount, _maxEggcount);

        if (_currentEggCount == _maxEggcount)
        {
            Debug.Log("gamewin");
            _eggCounterUI.SetEggCompleted();
            ChangeGameState(GameState.GameOver);
        }
        
    }

    public GameState GetCurrentGameState()
    {
        return _currentGameState;
    }

}