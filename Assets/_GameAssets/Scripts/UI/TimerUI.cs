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

    private void Start() 
    {
        PlayRotationAnimation();
        Starttimer();
    }

    private void PlayRotationAnimation()
    {
        _timerRotateableTransform.DORotate(new Vector3(0f, 0f, -360f), _rotationDuration, RotateMode.FastBeyond360)
            .SetLoops(-1, LoopType.Restart)
            .SetEase(_rotationEase);
    }

    private void Starttimer()
    {
        _elapsedTime = 0f;
        InvokeRepeating(nameof(UpdatetimerUI), 0f, 1f);
    }

    private void UpdatetimerUI()
    {
        _elapsedTime += 1f;
        
        int minutes = Mathf.FloorToInt(_elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(_elapsedTime % 60f);

        _timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    }