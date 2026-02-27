using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    // ... Mevcut event ve değişkenlerin (Değişiklik yok) ...
    public event Action OnPlayerJumped;
    public event Action<PlayerState> OnPlayerStateChanged;

    [Header("References")]
    [SerializeField] private Transform _orientationTransform;

    [Header("Movement Settings")]
    [SerializeField] private float _movementSpeed;

    [Header("Jump Settings")]
    [SerializeField] private KeyCode _jumpkey;
    [SerializeField] private float _jumpForce;
    [SerializeField] private float _jumpcooldown;
    [SerializeField] private float _airmultiplier;
    [SerializeField] private float _airDrag;
    [SerializeField] private bool _canJump;
    [SerializeField] private float _maxFallSpeed = 25f;

    [Header("Slide (Stamina) Settings")]
    [SerializeField] private KeyCode _slidekey = KeyCode.LeftShift;
    [SerializeField] private float _slidemultiplier;
    [SerializeField] private float _slideDrag;
    [SerializeField] private float _maxSlideStamina = 5f;
    [SerializeField] private float _staminaRegenMultiplier = 1.5f;

    [Header("Ground Check Settings")]
    [SerializeField] private float _playerHeight;
    [SerializeField] private LayerMask _groundLayer;
    [SerializeField] private float _groundDrag;

    private StateController _stateController;
    private Rigidbody _playerRigidbody;

    private float _startingMovementSpeed, _startingJumpForce;
    private float _horizontalInput, _verticalInput;
    private Vector3 _movementdirection;
    private bool _isSliding;
    private bool _isInWater;
    private float _currentSlideStamina;

    private void Awake()
    {
        _stateController = GetComponent<StateController>();
        _playerRigidbody = GetComponent<Rigidbody>();
        _playerRigidbody.freezeRotation = true;

        _startingMovementSpeed = _movementSpeed;
        _startingJumpForce = _jumpForce;
        _currentSlideStamina = _maxSlideStamina;
    }

    private void Update()
    {
        if (GameManager.Instance.GetCurrentGameState() != GameState.Play
            && GameManager.Instance.GetCurrentGameState() != GameState.Resume)
        {
            return;
        }

        SetInputs();
        SetStates();
        SetPlayerDrag();
        HandleStamina();
    }

    private void FixedUpdate()
    {
        if (GameManager.Instance.GetCurrentGameState() != GameState.Play
            && GameManager.Instance.GetCurrentGameState() != GameState.Resume)
        {
            _playerRigidbody.linearVelocity = Vector3.zero;
            return;
        }

        setplayerMovement();
        LimitPlayerSpeed();
        ApplyTerminalVelocity();
    }

    private void HandleStamina()
    {
        if (_isSliding)
        {
            _currentSlideStamina -= Time.deltaTime;
            if (_currentSlideStamina <= 0)
            {
                _currentSlideStamina = 0;
                _isSliding = false;
            }
        }
        else
        {
            if (_currentSlideStamina < _maxSlideStamina)
            {
                _currentSlideStamina += Time.deltaTime * _staminaRegenMultiplier;
                _currentSlideStamina = Mathf.Min(_currentSlideStamina, _maxSlideStamina);
            }
        }
    }

    private void ApplyTerminalVelocity()
    {
        if (_playerRigidbody.linearVelocity.y < -_maxFallSpeed)
        {
            _playerRigidbody.linearVelocity = new Vector3(_playerRigidbody.linearVelocity.x, -_maxFallSpeed, _playerRigidbody.linearVelocity.z);
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
            PlayerState.SlideIdle => _slidemultiplier,
            PlayerState.Jump => _airmultiplier,
            _ => 1f
        };

        float waterResistance = _isInWater ? 0.4f : 1f;

        _playerRigidbody.AddForce(_movementdirection.normalized * _movementSpeed * forceMultiplier * waterResistance, ForceMode.Force);
    }

    private void SetPlayerDrag()
    {
        if (_isInWater)
        {
            _playerRigidbody.linearDamping = 2f;
            return;
        }

        _playerRigidbody.linearDamping = _stateController.GetCurrentState() switch
        {
            PlayerState.Move => _groundDrag,
            PlayerState.Slide => _slideDrag,
            PlayerState.SlideIdle => _slideDrag,
            PlayerState.Jump => _airDrag,
            _ => _playerRigidbody.linearDamping
        };
    }

    private void LimitPlayerSpeed()
    {
        Vector3 flatVelocity = new Vector3(_playerRigidbody.linearVelocity.x, 0f, _playerRigidbody.linearVelocity.z);
        float currentMaxSpeed = _isInWater ? _movementSpeed * 0.4f : _movementSpeed;

        if (flatVelocity.magnitude > currentMaxSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * currentMaxSpeed;
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

    private void SetInputs()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal");
        _verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(_slidekey) && _currentSlideStamina > 0 && IsGrounded())
        {
            _isSliding = true;
        }
        else
        {
            _isSliding = false;
        }

        if (Input.GetKey(_jumpkey) && _canJump && IsGrounded())
        {
            _canJump = false;
            SetPlayerJumping();
            Invoke(nameof(ResetJumping), _jumpcooldown);
            AudioManager.Instance.Play(SoundType.JumpSound);
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
            OnPlayerStateChanged?.Invoke(newState);

            // --- MUZIK TUNE AYARI ---
            // Sliding basladiginda muzigi hizlandiriyoruz, bitince normale donduruyoruz
            if (BackgroundMusic.Instance != null)
            {
                if (newState == PlayerState.Slide || newState == PlayerState.SlideIdle)
                {
                    BackgroundMusic.Instance.SetPitch(1.25f, 0.4f); // %25 hizlanma
                }
                else if (currentState == PlayerState.Slide || currentState == PlayerState.SlideIdle)
                {
                    BackgroundMusic.Instance.SetPitch(1.0f, 0.6f); // Normale donus
                }
            }
        }
    }

    public float GetStaminaNormalized()
    {
        return _currentSlideStamina / _maxSlideStamina;
    }

    #region Helper Functions

    private bool IsGrounded()
    {
        Vector3 spherePosition = transform.position + Vector3.down * (_playerHeight * 0.5f);
        return Physics.CheckSphere(spherePosition, 0.2f, _groundLayer, QueryTriggerInteraction.Collide);
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

    public Rigidbody GetPlayerRigidbody()
    {
        return _playerRigidbody;
    }

    public bool CanCatChase()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit,
          _playerHeight * 0.5f + 0.2f, _groundLayer))
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer(Consts.Layers.FLOOR_LAYER))
            {
                return true;
            }
            else if (hit.collider.gameObject.layer == LayerMask.NameToLayer(Consts.Layers.GROUND_LAYER))
            {
                return false;
            }
        }
        return false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & _groundLayer) != 0)
        {
            _isInWater = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & _groundLayer) != 0)
        {
            _isInWater = false;
        }
    }

    #endregion
}