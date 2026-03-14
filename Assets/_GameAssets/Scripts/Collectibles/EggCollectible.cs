using UnityEngine;

public class EggCollectible : MonoBehaviour, ICollectible
{
    [Header("Efekt Ayarları")]
    [SerializeField] private GameObject _birthdaySparkPrefab; // Artık ismi BirthdaySpark

    public void Collect()
    {
        // 1. Efekti Oluştur ve Kontrol Et
        if (_birthdaySparkPrefab != null)
        {
            // Yumurtanın biraz üzerinden (0.5 birim yukarıda) oluştur
            GameObject sparkInstance = Instantiate(_birthdaySparkPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
            
            // Hiyerarşide bulabilmen için isim veriyoruz
            sparkInstance.name = "DEBUG_BirthdaySpark";

            // Eğer içinde Particle System varsa manuel olarak 'Oynat' emri ver (Garantici yöntem)
            ParticleSystem ps = sparkInstance.GetComponentInChildren<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }

            // Sahne kalabalıklaşmasın diye 3 saniye sonra objeyi siliyoruz
            Destroy(sparkInstance, 3f);
        }
        else
        {
            Debug.LogWarning("DİKKAT: EggCollectible içindeki BirthdaySparkPrefab alanı boş!");
        }

        // 2. Diğer Sistemleri Tetikle
        if (GameManager.Instance != null) GameManager.Instance.OnEggCollected();
        if (CameraShake.Instance != null) CameraShake.Instance.ShakeCamera(1f, 1f);
        if (AudioManager.Instance != null) AudioManager.Instance.Play(SoundType.PickupGoodSound);

        // 3. Yumurtayı Yok Et
        Destroy(gameObject);
    }
}