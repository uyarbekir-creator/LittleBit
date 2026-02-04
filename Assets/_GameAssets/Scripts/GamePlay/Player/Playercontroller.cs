using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform orientationTransform;

    [Header("Movement Settings ")]

    [SerializeField] private KeyCode _movementkey;
    [SerializeField] private float _movementspeed;

    [Header("Jump Settings ")]

    [SerializeField] private KeyCode _jumpkey;
    [SerializeField] private float _jumpforce;
    [SerializeField] private float _jumpcooldown;
    [SerializeField] private bool _canJump;

    [Header("Slide Settings ")]

    [SerializeField] private KeyCode _slidekey;
    [SerializeField] private float _slidemultiplier;
    [SerializeField] private float _slideDrag;
    [SerializeField] private float _slidecooldown;
    [SerializeField] private bool _canSlide;


    [Header("Ground Settings ")]
    [SerializeField] private float _playerHeight;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundDrag;

    private StateController _stateController;
    private Rigidbody _playerRigidbody;

    private float horizontalInput, _verticalInput;
    private Vector3 _movementdirection;
    private bool _isSliding;


    private void Awake()
    {
        _stateController = GetComponent<StateController>();
        _playerRigidbody = GetComponent<Rigidbody>();
        _playerRigidbody.freezeRotation = true;

    }

    private void Update()
    {
        setinputs();
        SetStates();
        SetPlayerDrag();
        LimitPlayerSpeed();
    }

    private void FixedUpdate()
    {
        setplayerMovement();
    }
    private void setinputs()

    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(_slidekey))
        {
            _isSliding = true;
        }

        else if (Input.GetKeyDown(_movementkey))
        {
            _isSliding = false;
            Debug.Log("Not Sliding");
        }

        else if (Input.GetKey(_jumpkey) && _canJump && IsGrounded())
        {
            _canJump = false;
            SetPlayerJumping();
            Invoke(nameof(ResetJumping), _jumpcooldown);
        }
    }

    private void SetStates()
    {
        var movementDirection = GetMovementDirection();
        var isGrounded = IsGrounded();
        var _isSliding = _IsSliding();
        var currentState = _stateController.GetCurrentState();

        var newState = currentState switch
        {
            _ when movementDirection == Vector3.zero && isGrounded && !isSliding => PlayerState.Idle,
            _ when movementDirection != Vector3.zero && isGrounded && !isSliding => PlayerState.Move,
            _ when movementDirection != Vector3.zero && isGrounded && isSliding => PlayerState.Slide,
            _ when movementDirection == Vector3.zero && isGrounded && isSliding => PlayerState.SlideIdle,
            _ when !_canJump && !isGrounded => PlayerState.Jump,
            _ => currentState
        };

        if(newState != currentState)
        {
            _stateController.ChangeState(newState);
        }

        Debug.Log(newState);
    }

    private void setplayerMovement()
    {
        _movementdirection = orientationTransform.forward * verticalInput + orientationTransform.right * horizontalInput;

        if (_isSliding)
        {

            _playerRigidbody.AddForce(_movementdirection.normalized * _movementspeed * _slidemultiplier, ForceMode.Force);

        }
        else
        {
            _playerRigidbody.AddForce(_movementdirection.normalized * _movementspeed, ForceMode.Force);

        }



    }

    private void SetPlayerDrag()
    {
        if (_isSliding)
        {
            _playerRigidbody.linearDamping = _slideDrag;
        }
        else
        {
            _playerRigidbody.linearDamping = _groundDrag;
        }
    }
    private void LimitPlayerSpeed()
    {
        Vector3 flatVelocity = new Vector3(_playerRigidbody.linearVelocity.x, 0f, _playerRigidbody.linearVelocity.z);
        if (flatVelocity.magnitude > _movementspeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * _movementspeed;
            _playerRigidbody.linearVelocity = new Vector3(limitedVelocity.x, _playerRigidbody.linearVelocity.y, limitedVelocity.z);
        }
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

    private Vector3 GetMovementDirection()
    {
        return _movementdirection.normalized;
    }
    
    private bool _IsSliding()
    {
        return _isSliding;
    }

}