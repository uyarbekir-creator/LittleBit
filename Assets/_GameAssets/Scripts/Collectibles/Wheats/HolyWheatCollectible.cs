using UnityEngine;

public class HolyWheatCollectible : MonoBehaviour, ICollectible
{
    [SerializeField] private WheatDesignSO _wheatDesignSO;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private PlayerStateUI _playerStateUI;
    private BoosterManager _boosterManager;

    public void Initialize(PlayerController controller, PlayerStateUI ui)
    {
        _playerController = controller;
        _playerStateUI = ui;
        _boosterManager = Object.FindAnyObjectByType<BoosterManager>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) Collect();
    }

    public void Collect()
    {
        _playerController.SetJumpForce(_wheatDesignSO.IncreaseDecreaseMultiplier, _wheatDesignSO.ResetBoostDuration);
        
        // Yeni basit UI tetikleyici
        if (_playerStateUI != null)
        {
            _playerStateUI.ActivateBoosterUI("Jump", _wheatDesignSO.ResetBoostDuration);
        }

        CameraShake.Instance.ShakeCamera(1f, 1f);
        AudioManager.Instance.Play(SoundType.PickupGoodSound);

        if (_boosterManager != null) _boosterManager.OnBoosterCollected(gameObject);
        Destroy(gameObject);
    }
}