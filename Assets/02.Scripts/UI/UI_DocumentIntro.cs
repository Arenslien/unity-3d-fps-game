using System.Collections;
using UnityEngine;

// 로그인 서류가 화면 밖에서 미끄러져 들어와 제자리에 자리 잡는 등장 연출.
// 연출이 끝나기 전에는 입력을 막는다.
[RequireComponent(typeof(CanvasGroup))]
public class UI_DocumentIntro : MonoBehaviour
{
    [Header("연출")]
    [SerializeField] private Vector2 _slideOffset = new Vector2(1100f, -90f);  // 시작 위치(제자리 기준)
    [SerializeField] private float _startRotationOffset = 9f;                  // 시작할 때 더 기울어진 각도
    [SerializeField] private float _delay = 0.6f;                              // 배경을 먼저 보여 주는 시간
    [SerializeField] private float _duration = 1.0f;

    private RectTransform _rect;
    private CanvasGroup _group;
    private Vector2 _finalPosition;
    private float _finalRotation;

    private void Awake()
    {
        _rect = (RectTransform)transform;
        _group = GetComponent<CanvasGroup>();

        _finalPosition = _rect.anchoredPosition;
        _finalRotation = _rect.localEulerAngles.z;

        // 연출 시작 전에 서류가 제자리에 보이지 않도록 먼저 밖에 둔다.
        _rect.anchoredPosition = _finalPosition + _slideOffset;
        _rect.localRotation = Quaternion.Euler(0f, 0f, _finalRotation + _startRotationOffset);
        _group.alpha = 0f;
        _group.interactable = false;
        _group.blocksRaycasts = false;
    }

    private void OnEnable()
    {
        StartCoroutine(Intro_Coroutine());
    }

    private IEnumerator Intro_Coroutine()
    {
        yield return new WaitForSecondsRealtime(_delay);

        float time = 0f;
        while (time < _duration)
        {
            time += Time.unscaledDeltaTime;
            float k = EaseOutBack(Mathf.Clamp01(time / _duration));
            _rect.anchoredPosition = Vector2.LerpUnclamped(_finalPosition + _slideOffset, _finalPosition, k);
            _rect.localRotation = Quaternion.Euler(0f, 0f, Mathf.LerpUnclamped(_finalRotation + _startRotationOffset, _finalRotation, k));
            _group.alpha = Mathf.Clamp01(time / (_duration * 0.35f));
            yield return null;
        }

        _rect.anchoredPosition = _finalPosition;
        _rect.localRotation = Quaternion.Euler(0f, 0f, _finalRotation);
        _group.alpha = 1f;
        _group.interactable = true;
        _group.blocksRaycasts = true;
    }

    // 목표를 살짝 지나쳤다가 돌아오는 감속 곡선.
    private static float EaseOutBack(float x)
    {
        const float c1 = 1.2f;
        const float c3 = c1 + 1f;
        return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
    }
}
