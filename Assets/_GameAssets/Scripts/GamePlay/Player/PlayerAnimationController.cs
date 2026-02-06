using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField] private Animator _playerAnimator;

    private PlayerController _playerController;
    private StateController _stateController;
    
    private void Awake()
    {
        _playerController = GetComponent<PlayerController>();
        _stateController = GetComponent<StateController>();
    }

    private void SetPlayerAnimations()
    {
        var currentState = _stateController.GetCurrentState();
        
        switch (currentState)
        {
            case PlayerState.Idle:
                _playerAnimator.SetBool("isSliding", false);
                _playerAnimator.SetBool("isMoving", false);
                break;
        }
    }
}
