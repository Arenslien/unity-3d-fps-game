using System.Collections;
using TMPro;
using UnityEngine;

// 켜질 때마다 글자를 왼쪽부터 한 글자씩 타이핑하듯 보여 주는 텍스트.
[RequireComponent(typeof(TMP_Text))]
public class UI_TypewriterText : MonoBehaviour
{
    [Header("타이밍")]
    [SerializeField] private CanvasGroup _waitFor;        // 이 그룹이 조작 가능해질 때까지 기다린다(서류 등장 연출이 끝나길 기다림).
    [SerializeField] private float _startDelay = 0.25f;  // 시작하기 전 잠깐 쉬는 시간
    [SerializeField] private float _interval = 0.015f;   // 글자 하나가 나오는 간격

    private TMP_Text _text;

    private void Awake()
    {
        _text = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        StartCoroutine(Type_Coroutine());
    }

    private IEnumerator Type_Coroutine()
    {
        _text.maxVisibleCharacters = 0;

        // 다른 컴포넌트의 Awake가 끝난 뒤 상태를 확인하도록 한 프레임 쉰다.
        yield return null;
        while (_waitFor != null && !_waitFor.interactable) yield return null;

        yield return new WaitForSecondsRealtime(_startDelay);

        _text.ForceMeshUpdate();
        int count = _text.textInfo.characterCount;

        for (int i = 1; i <= count; i++)
        {
            _text.maxVisibleCharacters = i;
            yield return new WaitForSecondsRealtime(_interval);
        }

        _text.maxVisibleCharacters = int.MaxValue;
    }
}
