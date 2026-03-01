using UnityEngine;

public class LightningFlicker : MonoBehaviour
{
    private Material _material;

    [Header("Parlaklik Ayarlari")]
    [Tooltip("Minimum parlaklik degeri (Normal gorunum)")]
    public float minIntensity = 1f;

    [Tooltip("Maksimum parlaklik degeri (Cakma ani)")]
    public float maxIntensity = 10f;

    [Header("Hiz Ayari")]
    [Tooltip("Degisimin ne kadar hizli olacagi")]
    public float flickerSpeed = 0.05f;

    private float _nextActionTime = 0f;

    private void Start()
    {
        // Kodun calismasi icin bu objede MeshRenderer olmalidir
        Renderer renderer = GetComponent<Renderer>();

        if (renderer != null)
        {
            // .material kullanmak bu objeye ozel bir kopya olusturur
            _material = renderer.material;

            // URP materyallerinde emisyonu kodla kontrol etmek icin bu anahtar sarttir
            _material.EnableKeyword("_EMISSION");
        }
        else
        {
            Debug.LogError("LightningFlicker: Bu objede MeshRenderer bulunamadi! Lutfen scripti gorsel modele (Visual) atin.");
        }
    }

    private void Update()
    {
        if (_material != null && Time.time > _nextActionTime)
        {
            // Bir sonraki degisim zamanini belirle
            _nextActionTime = Time.time + flickerSpeed;

            // Rastgele bir parlaklik sec
            float intensity = Random.Range(minIntensity, maxIntensity);

            // Rengi parlaklik degeriyle carp (HDR etkisi)
            Color finalColor = Color.yellow * intensity;

            // URP Lit Shader icindeki emisyon kanalina rengi gonder
            _material.SetColor("_EmissionColor", finalColor);
        }
    }
}