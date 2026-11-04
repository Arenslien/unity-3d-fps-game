using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// 입력칸 오류 연출. 좌우로 떨리고 밑줄이 빨갛게 켜져서 "여기를 꼭 채워야 한다"는 걸 알려 준다.
// 빨간 밑줄은 사용자가 다시 입력하기 시작하면 사라진다.
public class UI_InputFieldFeedback : MonoBehaviour
{
    [Header("구성 요소")]
    [SerializeField] private TMP_InputField _input;
    [SerializeField] private RectTransform _visual;  // 흔들 대상(입력칸의 겉모습)
    [SerializeField] private Graphic _errorUnderline;  // 오류일 때 켜지는 빨간 밑줄
    [SerializeField] private Graphic _flash;           // 오류 순간에 번쩍이는 붉은 막
    [SerializeField] private TMP_Text _placeholder;    // 오류일 때 붉게 바뀌는 안내 글자

    [Header("연출")]
    [SerializeField] private Color _errorColor = new Color(0.78f, 0.1f, 0.08f, 1f);
    [SerializeField] private float _shakeAmount = 14f;
    [SerializeField] private float _shakeDuration = 0.45f;
    [SerializeField] private float _shakeFrequency = 42f;  // 클수록 빨리 떨린다.
    [SerializeField] private float _flashAlpha = 0.3f;

    private Coroutine _routine;
    private Color _placeholderColor;

    private void Awake()
    {
        _placeholderColor = _placeholder.color;
        _input.onValueChanged.AddListener(_ => ClearError());
        ClearError();
    }

    // 떨림과 빨간 밑줄을 보여 준다.
    public void PlayError()
    {
        if (_routine != null) StopCoroutine(_routine);
        _routine = StartCoroutine(Error_Coroutine());
    }

    // 오류 표시를 모두 지운다.
    public void ClearError()
    {
        if (_routine != null) StopCoroutine(_routine);
        _routine = null;

        _visual.anchoredPosition = Vector2.zero;
        SetAlpha(_errorUnderline, 0f);
        SetAlpha(_flash, 0f);
        _placeholder.color = _placeholderColor;
    }

    private IEnumerator Error_Coroutine()
    {
        _errorUnderline.color = _errorColor;
        _flash.color = _errorColor;
        _placeholder.color = new Color(_errorColor.r, _errorColor.g, _errorColor.b, 0.85f);
        SetAlpha(_errorUnderline, 1f);

        float time = 0f;
        while (time < _shakeDuration)
        {
            time += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(time / _shakeDuration);
            float fade = (1f - k) * (1f - k);

            _visual.anchoredPosition = new Vector2(Mathf.Sin(time * _shakeFrequency) * _shakeAmount * fade, 0f);
            SetAlpha(_flash, _flashAlpha * fade);
            yield return null;
        }

        // 떨림은 멈추지만 빨간 밑줄과 안내 글자는 다시 입력할 때까지 남긴다.
        _visual.anchoredPosition = Vector2.zero;
        SetAlpha(_flash, 0f);
        _routine = null;
    }

    private static void SetAlpha(Graphic graphic, float alpha)
    {
        Color color = graphic.color;
        color.a = alpha;
        graphic.color = color;
    }
}
