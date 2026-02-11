using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    public event Action OnPlayerJumped;

    [Header("References")]
    [SerializeField] private Transform _orientationTransform;

    [Header("Movement Settings ")]

    [SerializeField] private KeyCode _movementkey;
    [SerializeField] private float _movementSpeed;

    [Header("Jump Settings ")]

    [SerializeField] private KeyCode _jumpkey;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _jumpcooldown;
    [SerializeField] private float _airmultiplier;
    [SerializeField] private float _airDrag;
    [SerializeField] private bool _canJump;

    [Header("Slide Settings ")]

    [SerializeField] private KeyCode _slidekey;
    [SerializeField] private float _slidemultiplier;
    [SerializeField] private float _slideDrag;
    [SerializeField] private float _slidecooldown;
    [SerializeField] private bool _canSlide;


    [Header("Ground Check Settings ")]
    [SerializeField] private float _playerHeight;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundDrag;

    private StateController _stateController;
    private Rigidbody _playerRigidbody;

    private float _startingMovementSpeed, _startingJumpForce;

    private float _horizontalInput, _verticalInput;
    private Vector3 _movementdirection;
    private bool _isSliding;


    private void Awake()
    {
        _stateController = GetComponent<StateController>();
        _playerRigidbody = GetComponent<Rigidbody>();
        _playerRigidbody.freezeRotation = true;

        _startingMovementSpeed = _movementSpeed;
        _startingJumpForce = _jumpForce;

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
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(_slidekey))
        {
            _isSliding = true;
        }

        else if (Input.GetKeyDown(_movementkey))
        {
            _isSliding = false;
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
        var isSliding = IsSliding();
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

        if (newState != currentState)
        {
            _stateController.ChangeState(newState);
        }
    }

    private void setplayerMovement()
    {
        _movementdirection = _orientationTransform.forward * _verticalInput
        + _orientationTransform.right * _horizontalInput;

        float forceMultiplier = _stateController.GetCurrentState() switch
        {
            PlayerState.Move => 1f,
            PlayerState.Slide => _slidemultiplier,
            PlayerState.Jump => _airmultiplier,
            _ => 1f
        };

        _playerRigidbody.AddForce(_movementdirection.normalized * _movementSpeed * forceMultiplier, ForceMode.Force);
    }


    private void SetPlayerDrag()
    {

        _playerRigidbody.linearDamping = _stateController.GetCurrentState() switch
        {
            PlayerState.Move => _groundDrag,
            PlayerState.Slide => _slideDrag,
            PlayerState.Jump => _airDrag,
            _ => _playerRigidbody.linearDamping
        };
    }
    private void LimitPlayerSpeed()
    {
        Vector3 flatVelocity = new Vector3(_playerRigidbody.linearVelocity.x, 0f, _playerRigidbody.linearVelocity.z);
        if (flatVelocity.magnitude > _movementSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * _movementSpeed;
            _playerRigidbody.linearVelocity = new Vector3(limitedVelocity.x, _playerRigidbody.linearVelocity.y, limitedVelocity.z);
        }
    }

    private void SetPlayerJumping()
    {
        OnPlayerJumped?.Invoke();
        _playerRigidbody.linearVelocity = new Vector3(_playerRigidbody.linearVelocity.x, 0f, _playerRigidbody.linearVelocity.z);
        _playerRigidbody.AddForce(transform.up * _jumpForce, ForceMode.Impulse);
    }
    private void ResetJumping()
    {
        _canJump = true;
    }

    #region Helper Functions

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, _playerHeight * 0.5f + 0.22f, _groundLayer);
    }

    private Vector3 GetMovementDirection()
    {
        return _movementdirection.normalized;
    }

    private bool IsSliding()
    {
        return _isSliding;
    }

public void SetMovementSpeed(float speed, float duration)
    {
        _movementSpeed += speed;
        Invoke(nameof(ResetMovementSpeed), duration);
    }

    private void ResetMovementSpeed()
    {
        _movementSpeed = _startingMovementSpeed;
    }

public void SetJumpForce(float force, float duration)
    {
        _jumpForce += force;
        Invoke(nameof(ResetJumpForce), duration);
    }

    private void ResetJumpForce()
    {
        _jumpForce = _startingJumpForce;
    }

    #endregion

}