using System.Collections;
using Unity.Cinemachine;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance; 

    private CinemachineBasicMultiChannelPerlin _cinemachineBasicMultiChannelPerlin;
    private bool _isShakeEnabled = true; // Sarsıntıyı tamamen kapatan kilit

    [Header("Kedi Takip Ayarları")]
    public Transform player;
    public Transform cat;
    public float maxDistance = 20f;
    public float maxAmplitude = 1f;
    public float maxFrequency = 1.5f;

    private float _shakeTimer;
    private float _shakeTimerTotal;
    private float _startingIntensity;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        _cinemachineBasicMultiChannelPerlin = GetComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void ResetShake()
    {
        _isShakeEnabled = false; // Kilidi kapat
        StopAllCoroutines();
        _shakeTimer = 0f;
        
        if (_cinemachineBasicMultiChannelPerlin != null)
        {
            _cinemachineBasicMultiChannelPerlin.AmplitudeGain = 0f;
            _cinemachineBasicMultiChannelPerlin.FrequencyGain = 0f;
        }
    }

    private IEnumerator CameraShakeCoroutine(float intensity, float time, float delay)
    {
        if (!_isShakeEnabled) yield break;
        yield return new WaitForSeconds(delay);
        _shakeTimer = time;
        _shakeTimerTotal = time;
        _startingIntensity = intensity;
    }

    public void ShakeCamera(float intensity, float time, float delay = 0f)
    {
        if (!_isShakeEnabled) return;
        StartCoroutine(CameraShakeCoroutine(intensity, time, delay));
    }

    private void Update()
    {
        // Eğer sarsıntı kilitlendiyse Update'i çalıştırma
        if (!_isShakeEnabled || _cinemachineBasicMultiChannelPerlin == null) return;

        float distanceIntensity = 0f;
        float currentFrequency = 0f;

        if (player != null && cat != null)
        {
            float distance = Vector3.Distance(player.position, cat.position);
            if (distance < maxDistance)
            {
                float proximityMultiplier = 1f - (distance / maxDistance);
                distanceIntensity = proximityMultiplier * maxAmplitude;
                currentFrequency = proximityMultiplier * maxFrequency;
            }
        }

        float timerIntensity = 0f;
        if (_shakeTimer > 0)
        {
            _shakeTimer -= Time.deltaTime;
            timerIntensity = Mathf.Lerp(_startingIntensity, 0f, 1f - (_shakeTimer / _shakeTimerTotal));
            if (timerIntensity > 0) currentFrequency = maxFrequency;
        }

        _noiseUpdate(distanceIntensity, timerIntensity, currentFrequency);
    }

    private void _noiseUpdate(float distAmp, float timerAmp, float freq)
    {
        _cinemachineBasicMultiChannelPerlin.AmplitudeGain = Mathf.Max(distAmp, timerAmp);
        _cinemachineBasicMultiChannelPerlin.FrequencyGain = freq;
    }
}