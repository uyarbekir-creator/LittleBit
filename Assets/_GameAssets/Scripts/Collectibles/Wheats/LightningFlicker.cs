using UnityEngine;

public class LightningFlicker : MonoBehaviour
{
    private Material _material;

    [Header("Dönme Ayarları")]
    [Tooltip("Yıldırımın kendi ekseninde dönme hızı")]
    public float rotationSpeed = 150f;

    [Header("Parlaklik Ayarlari")]
    [Tooltip("Minimum parlaklik degeri (Normal gorunum)")]
    public float minIntensity = 1f;

    [Tooltip("Maksimum parlaklik degeri (Cakma ani)")]
    public float maxIntensity = 10f;

    [Header("Hiz Ayari")]
    [Tooltip("Degisimin ne kadar hizli olacagi")]
    public float flickerSpeed = 0.05f;

    private float _nextActionTime = 0f;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    private void Start()
    {
        Renderer renderer = GetComponent<Renderer>();

        if (renderer != null)
        {
            _material = renderer.material;
            _material.EnableKeyword("_EMISSION");
        }
        else
        {
            Debug.LogError("LightningFlicker: Bu objede MeshRenderer bulunamadi!");
        }
    }

    private void Update()
    {
        // 1. DÖNME EFEKTİ
        // Yıldırımın dik durduğunu varsayarak Y ekseninde (up) döndürüyoruz
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.Self);

        // 2. ÇAKMA/TİTREME (FLICKER) EFEKTİ
        if (_material != null && Time.time > _nextActionTime)
        {
            _nextActionTime = Time.time + flickerSpeed;

            float intensity = Random.Range(minIntensity, maxIntensity);
            Color finalColor = Color.yellow * intensity;

            _material.SetColor(EmissionColor, finalColor);
        }
    }
}