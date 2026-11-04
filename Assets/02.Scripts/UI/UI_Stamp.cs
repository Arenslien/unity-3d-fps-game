using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 서류 위에 도장을 쾅 찍는 연출. 승인(초록)과 거부(빨강) 두 가지를 보여 준다.
// 처음에는 보이지 않고, Show를 부를 때만 나타난다.
public class UI_Stamp : MonoBehaviour
{
    [Header("구성 요소")]
    [SerializeField] private CanvasGroup _group;
    [SerializeField] private Image _frame;
    [SerializeField] private TextMeshProUGUI _label;
    [SerializeField] private RectTransform _shakeTarget;  // 도장이 찍힐 때 흔들 서류

    [Header("문구와 색")]
    [SerializeField] private string _approvedText = "APPROVED";
    [SerializeField] private string _deniedText = "ACCESS DENIED";
    [SerializeField] private Color _approvedColor = new Color(0.1f, 0.5f, 0.2f, 1f);
    [SerializeField] private Color _deniedColor = new Color(0.72f, 0.1f, 0.08f, 1f);

    [Header("연출")]
    [SerializeField] private float _startScale = 2.4f;     // 찍히기 전 크기
    [SerializeField] private float _slamDuration = 0.14f;  // 내려찍는 시간
    [SerializeField] private float _finalAlpha = 0.92f;
    [SerializeField] private float _rotation = -12f;
    [SerializeField] private float _approvedShake = 6f;    // 승인: 가볍게 흔들기
    [SerializeField] private float _deniedShake = 18f;     // 거부: 세게 흔들기
    [SerializeField] private float _shakeDuration = 0.35f;

    private Coroutine _stampRoutine;
    private Coroutine _shakeRoutine;
    private Vector2 _shakeOrigin;

    private void Awake()
    {
        Hide();
    }

    // approved가 true면 초록 APPROVED, false면 빨간 ACCESS DENIED 도장을 찍는다.
    public void Show(bool approved)
    {
        if (_stampRoutine != null) StopCoroutine(_stampRoutine);
        _stampRoutine = StartCoroutine(Stamp_Coroutine(approved));
    }

    // 도장을 지운다.
    public void Hide()
    {
        if (_stampRoutine != null) StopCoroutine(_stampRoutine);
        _stampRoutine = null;
        _group.alpha = 0f;
    }

    private IEnumerator Stamp_Coroutine(bool approved)
    {
        Color color = approved ? _approvedColor : _deniedColor;
        _label.text = approved ? _approvedText : _deniedText;
        _label.color = color;
        _frame.color = color;
        transform.localRotation = Quaternion.Euler(0f, 0f, _rotation);

        // 1. 크게 시작해서 가속하며 내려찍는다.
        float time = 0f;
        while (time < _slamDuration)
        {
            time += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(time / _slamDuration);
            transform.localScale = Vector3.one * Mathf.Lerp(_startScale, 1f, k * k);
            _group.alpha = Mathf.Lerp(0f, _finalAlpha, Mathf.Clamp01(k * 2f));
            yield return null;
        }

        transform.localScale = Vector3.one;
        _group.alpha = _finalAlpha;

        // 2. 찍히는 순간 서류가 흔들리고, 도장이 살짝 눌린다.
        Shake(approved ? _approvedShake : _deniedShake);

        float punchTime = 0f;
        const float punchDuration = 0.12f;
        while (punchTime < punchDuration)
        {
            punchTime += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(punchTime / punchDuration);
            transform.localScale = Vector3.one * (1f - 0.06f * Mathf.Sin(k * Mathf.PI));
            yield return null;
        }

        transform.localScale = Vector3.one;
        _stampRoutine = null;
    }

    private void Shake(float amount)
    {
        if (_shakeTarget == null) return;

        // 흔들던 중이면 원래 위치로 되돌린 뒤 다시 시작한다.
        if (_shakeRoutine != null)
        {
            StopCoroutine(_shakeRoutine);
            _shakeTarget.anchoredPosition = _shakeOrigin;
        }

        _shakeOrigin = _shakeTarget.anchoredPosition;
        _shakeRoutine = StartCoroutine(Shake_Coroutine(amount));
    }

    private IEnumerator Shake_Coroutine(float amount)
    {
        float time = 0f;
        while (time < _shakeDuration)
        {
            time += Time.unscaledDeltaTime;
            float strength = amount * (1f - time / _shakeDuration);
            _shakeTarget.anchoredPosition = _shakeOrigin + Random.insideUnitCircle * strength;
            yield return null;
        }

        _shakeTarget.anchoredPosition = _shakeOrigin;
        _shakeRoutine = null;
    }
}
