using UnityEngine;

public class RadiationGlow : MonoBehaviour
{
    [Header("Parlama Ayarları")]
    public Color hazardRed = Color.red; 
    public float pulseSpeed = 3f; 
    public float minIntensity = 2f;
    public float maxIntensity = 8f;

    private Material _material;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    void Start()
    {
        // Mesh Renderer'a ulaş ve materyali kontrol et
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            // Materyalin bir kopyasını oluştur (diğer objeleri etkilememesi için)
            _material = renderer.material;
            // Emission kanalını aktif et
            _material.EnableKeyword("_EMISSION");
        }
        else
        {
            Debug.LogError("Hata: " + gameObject.name + " üzerinde Renderer bulunamadı!");
        }
    }

    void Update()
    {
        if (_material != null)
        {
            // PingPong ile min ve max arasında yumuşak bir değer oluşturuyoruz
            float lerp = Mathf.PingPong(Time.time * pulseSpeed, 1f);
            float intensity = Mathf.Lerp(minIntensity, maxIntensity, lerp);
            
            // Materyale rengi ve parlaklığı uygula
            _material.SetColor(EmissionColor, hazardRed * intensity);
        }
    }
}