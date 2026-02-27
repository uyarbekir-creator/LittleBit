using UnityEngine;

public class BounceObject : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _bounceForce = 15f; // Firlatma gucu
    [SerializeField] private SoundType _bounceSound = SoundType.JumpSound; // Calacak ses

    private void OnCollisionEnter(Collision collision)
    {
        // Degen objenin Rigidbody'si var mi kontrol et
        Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // Carpma anindaki dikey hizi sifirla (daha tutarli bir ziplama icin)
            Vector3 velocity = rb.linearVelocity;
            velocity.y = 0;
            rb.linearVelocity = velocity;

            // Yukari dogru firlat
            rb.AddForce(Vector3.up * _bounceForce, ForceMode.Impulse);

            // Ses efekti cal
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.Play(_bounceSound);
            }
        }
    }
}