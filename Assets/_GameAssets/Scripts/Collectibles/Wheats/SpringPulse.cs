using UnityEngine;

public class SpringPulse : MonoBehaviour
{
    private Material _material;
    private static readonly int EmissionColor = Shader.PropertyToID("_EmissionColor");

    [Header("Renk ve Parlaklık")]
    public Color pulseColor = Color.green; // Zıplama için yeşil idealdir
    public float minIntensity = 0.5f;
    public float maxIntensity = 5f;
    public float pulseSpeed = 2f;

    private void Start()
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            _material = renderer.material;
            _material.EnableKeyword("_EMISSION");
        }
    }

    private void Update()
    {
        if (_material != null)
        {
            // Sinüs dalgası kullanarak 0 ile 1 arasında yumuşak bir değer alıyoruz
            float lerp = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f;

            // Bu değeri min ve max parlaklık arasında dağıtıyoruz
            float intensity = Mathf.Lerp(minIntensity, maxIntensity, lerp);

            _material.SetColor(EmissionColor, pulseColor * intensity);
        }
    }
}