using UnityEngine;
using UnityEngine.UI;

public class GoldWheatCollectible : MonoBehaviour, ICollectible
{
    [SerializeField] private WheatDesignSO _wheatDesignSO;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private PlayerStateUI _playerStateUI;

    private RectTransform _playerBoosterTransform;
    private Image _playerboosterImage;
    private void Awake()
    {
        _playerBoosterTransform = _playerStateUI.GetBoosterSpeedTransform;
        _playerboosterImage = _playerBoosterTransform.GetComponent<Image>();
    }

    public void Collect()
    {
        _playerController.SetMovementSpeed(_wheatDesignSO.IncreaseDecreaseMultiplier, _wheatDesignSO.ResetBoostDuration);

        _playerStateUI.PlayBoosterUIAnimations(_playerBoosterTransform, _playerboosterImage,
        _playerStateUI.GetGoldBoosterWheatImage, _wheatDesignSO.ActiveWheatSprite,
        _wheatDesignSO.PassiveWheatSprite, _wheatDesignSO.ActiveWheatSprite,
        _wheatDesignSO.PassiveWheatSprite, _wheatDesignSO.ResetBoostDuration);

        CameraShake.Instance.ShakeCamera(1f, 1f);
        AudioManager.Instance.Play(SoundType.PickupGoodSound);

        Destroy(gameObject);
    }
}
