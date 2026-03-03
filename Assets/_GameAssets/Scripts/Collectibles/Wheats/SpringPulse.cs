using UnityEngine;

public class SpringPulse : MonoBehaviour
{
    private Material _material;
    // Renderer'ın hangi objede olduğunu bulmak için değişken ekledik
    private Renderer _targetRenderer; 
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    [Header("Dönme Ayarları")]
    public float rotationSpeed = 100f;

    [Header("Renk ve Parlaklık")]
    public Color pulseColor = Color.green; 
    public float minIntensity = 0.5f;
    public float maxIntensity = 5f;
    public float pulseSpeed = 2f;

    private void Start()
    {
        // Eğer scripti dıştaki Pivot'a atarsan, içteki Mesh'i bulması lazım
        _targetRenderer = GetComponentInChildren<Renderer>();
        
        if (_targetRenderer != null)
        {
            _material = _targetRenderer.material;
            _material.EnableKeyword("_EMISSION");
        }
    }

    private void Update()
    {
        // 1. DÜNYA EKSENİNDE DÖNME (Pivot Dönüşü)
        // Bu komut en dıştaki objeyi döndürür, içteki 15 derecelik yay da onunla tur atar
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime, Space.World);

        // 2. Parlama Efekti
        if (_material != null)
        {
            float lerp = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;
            float intensity = Mathf.Lerp(minIntensity, maxIntensity, lerp);
            _material.SetColor(EmissionColor, pulseColor * intensity);
        }
    }
}