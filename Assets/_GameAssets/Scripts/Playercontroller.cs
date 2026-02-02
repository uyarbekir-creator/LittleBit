using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform orientationTransform;

    [Header("Movement Settings ")]

    [SerializeField] private float _movementspeed;
    
    [Header("Jump Settings ")]

    [SerializeField] private KeyCode _jumpkey;
    [SerializeField] private float _jumpforce;
    [SerializeField] private float _jumpcooldown;
    [SerializeField] private bool _canJump;

    [Header("Ground Settings ")]
    [SerializeField] private float _playerHeight;
    [SerializeField] private LayerMask _groundLayer;

    private Rigidbody _playerRigidbody;

    private float horizontalInput, verticalInput;
    private Vector3 _movementdirection;
    

    private void Awake()
    {
        // Fixing the casing for GetComponent
        _playerRigidbody = GetComponent<Rigidbody>();
        _playerRigidbody.freezeRotation = true;
    }

    private void Update()
    {
        setinputs();
    }
    
    private void FixedUpdate()
    {
        setplayerMovement();
    }
     private void setinputs()

        {
            horizontalInput = Input.GetAxisRaw("Horizontal");
            verticalInput = Input.GetAxisRaw("Vertical");

            if (Input.GetKey(_jumpkey) && _canJump && IsGrounded())
            {
                _canJump = false;
                SetPlayerJumping();
                Invoke(nameof(ResetJumping), _jumpcooldown);
            }
        }
        // Ready for movement implementation
        private void setplayerMovement()


    {
        _movementdirection = orientationTransform.forward * verticalInput + orientationTransform.right * horizontalInput;
        
        _playerRigidbody.AddForce(_movementdirection.normalized * _movementspeed, ForceMode.Force);
        }

        private void SetPlayerJumping()
        {
           _playerRigidbody.linearVelocity = new Vector3(_playerRigidbody.linearVelocity.x, 0f, _playerRigidbody.linearVelocity.z);
            _playerRigidbody.AddForce(transform.up * _jumpforce, ForceMode.Impulse);
        }
        private void ResetJumping()
        {
            _canJump = true;
        }    
        
        private bool IsGrounded()
        {
            return Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f + 2f, _groundLayer);
        }
}