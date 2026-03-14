using UnityEngine;
using UnityEngine.UI;

public class CatIndicator : MonoBehaviour
{
    [Header("Referanslar")]
    public Transform player; // Kamera veya Oyuncu
    public Transform cat;    // Kedi
    public Image indicatorImage; // Okun kendisi (Pivot değil, içindeki resim)

    [Header("Ayarlar")]
    public bool hideWhenVisible = true;

    void LateUpdate()
    {
        if (player == null || cat == null) return;

        // 1. Kedinin oyuncuya göre yönünü hesapla
        Vector3 directionToCat = cat.position - player.position;
        directionToCat.y = 0; // Yüksekliği sıfırla ki sadece yatay yönü alalım

        // 2. Kedinin ekranda görünür olup olmadığını kontrol et
        Vector3 screenPos = Camera.main.WorldToScreenPoint(cat.position);
        bool isVisible = screenPos.z > 0 && screenPos.x > 0 && screenPos.x < Screen.width && screenPos.y > 0 && screenPos.y < Screen.height;

        if (hideWhenVisible && isVisible)
        {
            indicatorImage.enabled = false;
            return;
        }
        
        indicatorImage.enabled = true;

        // 3. KRİTİK NOKTA: Oyuncunun baktığı yöne göre kedinin açısını bul
        // Forward (ileri) ve Right (sağ) vektörlerini kullanarak açıyı hesapla
        float angle = Vector3.SignedAngle(player.forward, directionToCat, Vector3.up);

        // 4. Pivotu döndür (Bu işlem oku merkez etrafında döndürür)
        // Unity'de UI rotasyonu için açıyı tersine çevirmemiz gerekebilir (-angle)
        transform.localRotation = Quaternion.Euler(0, 0, -angle);
    }
}