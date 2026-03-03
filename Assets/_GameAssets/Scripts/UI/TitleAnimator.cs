using UnityEngine;
using TMPro;
using DG.Tweening;

public class TitleAnimator : MonoBehaviour
{
    [Header("Punch Animation")]
    [SerializeField] private float _punchAmount = 0.1f; // Artık bu kullanılacak
    [SerializeField] private float _animationDuration = 1.5f;

    [Header("Floating Animation")]
    [SerializeField] private float _moveDistance = 20f;
    [SerializeField] private float _rotateAmount = 5f;

    private RectTransform _rectTransform;
    private TextMeshProUGUI _textMesh;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _textMesh = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        if (_rectTransform == null) return;

        // Yüzme animasyonu
        _rectTransform.DOAnchorPosY(_rectTransform.anchoredPosition.y + _moveDistance, _animationDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        // Sallanma animasyonu
        _rectTransform.DORotate(new Vector3(0, 0, _rotateAmount), _animationDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        // Ölçeklendirme (Büyüyüp Küçülme) animasyonu
        // 1.1f yerine 1f + _punchAmount kullanarak değişkeni aktif ettik!
        _rectTransform.DOScale(1f + _punchAmount, _animationDuration * 0.8f)
            .SetEase(Ease.InOutFlash)
            .SetLoops(-1, LoopType.Yoyo);

        // Renk değiştirme döngüsü
        if (_textMesh != null)
        {
            DOTween.Sequence()
                .Append(_textMesh.DOColor(Color.red, 1f))
                .Append(_textMesh.DOColor(Color.yellow, 1f))
                .Append(_textMesh.DOColor(Color.green, 1f))
                .Append(_textMesh.DOColor(Color.cyan, 1f))
                .SetLoops(-1, LoopType.Restart)
                .SetLink(gameObject); // Obje yok olursa sequence da durur
        }
    }

    private void OnDestroy()
    {
        // DOTween.Kill(this) daha genel bir temizlik sağlar
        transform.DOKill();
        if (_textMesh != null) _textMesh.DOKill();
    }
}