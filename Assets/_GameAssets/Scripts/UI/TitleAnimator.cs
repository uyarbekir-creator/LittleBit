using UnityEngine;
using TMPro;
using DG.Tweening;

public class TitleAnimator : MonoBehaviour
{
    [Header("Punch Animation")]
    [SerializeField] private float _punchAmount = 0.1f;
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

        _rectTransform.DOAnchorPosY(_rectTransform.anchoredPosition.y + _moveDistance, _animationDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        _rectTransform.DORotate(new Vector3(0, 0, _rotateAmount), _animationDuration)
            .SetEase(Ease.InOutSine)
            .SetLoops(-1, LoopType.Yoyo);

        _rectTransform.DOScale(1.1f, _animationDuration * 0.8f)
            .SetEase(Ease.InOutFlash)
            .SetLoops(-1, LoopType.Yoyo);

        if (_textMesh != null)
        {
            Sequence colorSequence = DOTween.Sequence();
            colorSequence.Append(_textMesh.DOColor(Color.red, 1f));
            colorSequence.Append(_textMesh.DOColor(Color.yellow, 1f));
            colorSequence.Append(_textMesh.DOColor(Color.green, 1f));
            colorSequence.Append(_textMesh.DOColor(Color.cyan, 1f));
            colorSequence.SetLoops(-1, LoopType.Restart);
        }
    }

    private void OnDestroy()
    {
        _rectTransform?.DOKill();
        _textMesh?.DOKill();
    }
}