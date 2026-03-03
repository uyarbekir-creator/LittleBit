using UnityEngine;

public class RadiationGlow : MonoBehaviour
{
    [Header("Dönme Ayarları")]
    public float rotationSpeed = 100f; // [cite: 2026-03-01]

    [Header("Parlama Ayarları")]
    public Color hazardRed = Color.red; // [cite: 2026-03-01]
    public float pulseSpeed = 3f; // [cite: 2026-03-01]
    public float minIntensity = 2f; // [cite: 2026-03-01]
    public float maxIntensity = 8f; // [cite: 2026-03-01]

    private Material _material;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // Materyalin bir kopyasını oluştur (diğer objeleri etkilememesi için) [cite: 2026-03-01]
            _material = renderer.material;
            _material.EnableKeyword("_EMISSION"); // [cite: 2026-03-01]
        }
    }

    void Update()
    {
        // 1. Kendi ekseni etrafında (Y ekseni) döndür [cite: 2026-03-01]
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // 2. Parlama efektini uygula [cite: 2026-03-01]
        if (_material != null)
        {
            float lerp = Mathf.PingPong(Time.time * pulseSpeed, 1f);
            float intensity = Mathf.Lerp(minIntensity, maxIntensity, lerp);
            
            _material.SetColor(EmissionColor, hazardRed * intensity);
        }
    }
}