using UnityEngine;

public class WaterPhysics : MonoBehaviour
{
    [SerializeField] private float _waterDrag = 3f;      // Su içindeki sürtünme
    [SerializeField] private float _buoyancyForce = 15f; // Suyun kaldırma kuvveti

    private void OnTriggerStay(Collider other)
    {
        // Sadece Rigidbody'si olan nesneler etkilenir
        Rigidbody rb = other.GetComponent<Rigidbody>();

        if (rb != null)
        {
            // Suyun kaldırma kuvvetini uygula (Yukarı itme)
            rb.AddForce(Vector3.up * _buoyancyForce, ForceMode.Acceleration);

            // Su direncini simüle et (Yavaşlatma)
            rb.linearDamping = _waterDrag; 
            rb.angularDamping = _waterDrag;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Sudan çıkınca sürtünmeyi normale döndür (Varsayılan genelde 0'dır)
            rb.linearDamping = 0.05f;
            rb.angularDamping = 0.05f;
        }
    }
}