using TMPro;
using UnityEngine;
using DG.Tweening;

public class TimerUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private RectTransform _timerRotateableTransform;
    [SerializeField] private TMP_Text _timerText;

    [Header("Settings")]
    [SerializeField] private float _rotationDuration;
    [SerializeField] private Ease _rotationEase;

    private float _elapsedTime;

    private bool _isTimerRunning;
    private Tween _rotationTween;
    private string _finalTime;

    private void Start()
    {
        PlayRotationAnimation();
        Starttimer();

        GameManager.Instance.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void Update()
    {
        if (!_isTimerRunning) return;

        _elapsedTime += Time.deltaTime;

        int minutes = Mathf.FloorToInt(_elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(_elapsedTime % 60f);

        _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    private void GameManager_OnGameStateChanged(GameState gamestate)
    {
        switch (gamestate)
        {
            case GameState.Pause:
                StopTimer();
                break;
            case GameState.Resume:
                ResumeTimer();
                break;
            case GameState.GameOver:
                FinshTimer();
                break;
        }
    }

    private void PlayRotationAnimation()
    {
        _rotationTween = _timerRotateableTransform.DORotate(new Vector3(0f, 0f, -360f), _rotationDuration, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(_rotationEase);
    }

    private void Starttimer()
    {
        _isTimerRunning = true;
        _elapsedTime = 0f;
    }

    private void StopTimer()
    {
        _isTimerRunning = false;
        _rotationTween.Pause();
    }

    private void ResumeTimer()
    {
        _isTimerRunning = true;
        _rotationTween.Play();
    }

    private void FinshTimer()
    {
        StopTimer();
        _finalTime = GetFormattedElapsedTime();
    }

    private string GetFormattedElapsedTime()
    {
        int minutes = Mathf.FloorToInt(_elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(_elapsedTime % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    public string GetFinalTime()
    {
        return _finalTime;
    }
}