using UnityEngine;
using UnityEngine.UI;

public class RottenWheatCollectible : MonoBehaviour, ICollectible
{
    [SerializeField] private WheatDesignSO _wheatDesignSO;
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private PlayerStateUI _playerStateUI;


    private RectTransform _playerBoosterTransform;
    private Image _playerboosterImage;
    
    private void Awake()
    {
        _playerBoosterTransform = _playerStateUI.GetBoosterSlowTransform;
        _playerboosterImage = _playerBoosterTransform.GetComponent<Image>();
    }

    public void Collect()
    {
        _playerController.SetMovementSpeed(_wheatDesignSO.IncreaseDecreaseMultiplier, _wheatDesignSO.ResetBoostDuration);
        
        _playerStateUI.PlayBoosterUIAnimations(_playerBoosterTransform, _playerboosterImage,
        _playerStateUI.GetRottenBoosterWheatImage, _wheatDesignSO.ActiveWheatSprite,
        _wheatDesignSO.PassiveWheatSprite, _wheatDesignSO.ActiveWheatSprite,
        _wheatDesignSO.PassiveWheatSprite, _wheatDesignSO.ResetBoostDuration);
        
        CameraShake.Instance.ShakeCamera(1f, 1f);
        
        Destroy(gameObject);
    }
}
