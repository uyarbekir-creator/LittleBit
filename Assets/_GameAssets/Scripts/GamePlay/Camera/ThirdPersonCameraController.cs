using UnityEngine;

public class ThirdPersonCameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private Transform _orientationTransform;
    [SerializeField] private Transform _playerVisualTransform;

    [Header("Settings")]
    [SerializeField] private float _mouseSensitivity = 1.5f;
    [SerializeField] private float _rotationSpeed = 8f;

    [Header("Smoothness")]
    [Range(0, 0.2f)][SerializeField] private float _cameraSmoothTime = 0.05f;

    private float _yRotation;
    private float _xRotation;
    private float _currentYRotation;
    private float _yRotationVelocity;
    private Rigidbody _playerRigidbody;

    private void Start()
    {
        _playerRigidbody = _playerTransform.GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (GameManager.Instance.GetCurrentGameState() != GameState.Play && GameManager.Instance.GetCurrentGameState() != GameState.Resume) return;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _yRotation += Input.GetAxisRaw("Mouse X") * _mouseSensitivity;
        _xRotation -= Input.GetAxisRaw("Mouse Y") * _mouseSensitivity;
        _xRotation = Mathf.Clamp(_xRotation, -30f, 50f);

        _currentYRotation = Mathf.SmoothDampAngle(_currentYRotation, _yRotation, ref _yRotationVelocity, _cameraSmoothTime);

        _playerRigidbody.MoveRotation(Quaternion.Euler(0, _currentYRotation, 0));
        _orientationTransform.rotation = Quaternion.Euler(0, _currentYRotation, 0);

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 inputDir = _orientationTransform.forward * v + _orientationTransform.right * h;

        if (inputDir != Vector3.zero)
        {
            _playerVisualTransform.forward = Vector3.Slerp(_playerVisualTransform.forward, inputDir.normalized, Time.deltaTime * _rotationSpeed);
        }
    }
}