using UnityEngine;

public class SpatulaBooster : MonoBehaviour, IBoostable
{
    [SerializeField] private float _jumpForce;
    public void Boost(PlayerController playerController)
    {
        Rigidbody playerRigidbody = playerController.GetPlayerRigidbody();

        playerRigidbody.linearVelocity = new Vector3(playerRigidbody.linearVelocity.x, 0f, playerRigidbody.linearVelocity.z);
        playerRigidbody.AddForce(transform.forward * _jumpForce, ForceMode.Impulse);
    }
        
}
