using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("References")] 
    [SerializeField] private EggCounterUI _eggCounterUI;

    [Header("Settings")]
    [SerializeField] private int _maxEggcount = 5;

    private int _currentEggCount;
    private void Awake()
    {
        Instance = this;
    }

    public void OnEggCollected()
    {
        _currentEggCount++;
        _eggCounterUI.SetEggCounterText(_currentEggCount, _maxEggcount);

        if (_currentEggCount == _maxEggcount)
        {
            Debug.Log("gamewin");
            _eggCounterUI.SetEggCompleted();
        }
        
    }

}