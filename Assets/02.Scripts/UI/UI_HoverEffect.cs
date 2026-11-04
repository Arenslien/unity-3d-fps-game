using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// 마우스 호버·이탈·클릭·선택 상태에 따라 여러 그래픽의 색과 크기를 부드럽게 바꾼다.
// 로그인 서류의 입력칸과 버튼에 붙여 쓴다.
public class UI_HoverEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler, ISelectHandler, IDeselectHandler
{
    [Serializable]
    private struct ColorTarget
    {
        public Graphic Graphic;
        public Color Normal;    // 평소
        public Color Hover;     // 마우스를 올렸을 때
        public Color Selected;  // 선택(입력 중)일 때
    }

    [Header("색상 대상")]
    [SerializeField] private ColorTarget[] _targets;

    [Header("크기")]
    [SerializeField] private float _hoverScale = 1.03f;
    [SerializeField] private float _pressedScale = 0.97f;

    [Header("속도")]
    [SerializeField] private float _speed = 14f;

    private bool _hovered;
    private bool _pressed;
    private bool _selected;

    private void OnEnable()
    {
        // 비활성화됐다 켜질 때 이전 상태가 남지 않도록 처음 상태로 맞춘다.
        _hovered = false;
        _pressed = false;
        _selected = false;
        transform.localScale = Vector3.one;

        foreach (ColorTarget target in _targets)
        {
            if (target.Graphic != null) target.Graphic.color = target.Normal;
        }
    }

    private void Update()
    {
        float t = 1f - Mathf.Exp(-_speed * Time.unscaledDeltaTime);

        foreach (ColorTarget target in _targets)
        {
            if (target.Graphic == null) continue;

            Color goal = _selected ? target.Selected : (_hovered ? target.Hover : target.Normal);
            target.Graphic.color = Color.Lerp(target.Graphic.color, goal, t);
        }

        float goalScale = _pressed ? _pressedScale : (_hovered ? _hoverScale : 1f);
        transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * goalScale, t);
    }

    public void OnPointerEnter(PointerEventData eventData) { _hovered = true; }

    public void OnPointerExit(PointerEventData eventData)
    {
        _hovered = false;
        _pressed = false;
    }

    public void OnPointerDown(PointerEventData eventData) { _pressed = true; }

    public void OnPointerUp(PointerEventData eventData) { _pressed = false; }

    public void OnSelect(BaseEventData eventData) { _selected = true; }

    public void OnDeselect(BaseEventData eventData) { _selected = false; }
}
