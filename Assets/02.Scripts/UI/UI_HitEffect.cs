using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class UI_HitEffect : MonoBehaviour
{
    [SerializeField] private float _flashTime = 0.2f;
    [SerializeField] private float _fadeTime = 0.3f;
    private const int AlphaValue = 60;
    private Image _image;

    private void Awake()
    {
        _image = GetComponent<Image>();
        _image.enabled = false;
    }

    private void Start()
    {
        Player player = GameObject.FindAnyObjectByType<Player>();

        if (player != null)
        {
            player.OnDamaged += OnPlayerDamaged;
        }
    }

    private void OnPlayerDamaged()
    {
        Fade(_fadeTime);
    }

    public void Flash()
    {
        StartCoroutine(Flash_Coroutine(_flashTime));
    }

    private IEnumerator Flash_Coroutine(float duration)
    {
        _image.enabled = true;

        float elapsed = 0f;

        // 60 --> 0
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            Color color = _image.color;
            color.a = Mathf.Lerp(AlphaValue, 0f, elapsed / duration);
            _image.color = color;

            yield return null;
        }

        _image.enabled = false;
    }

    public void Fade(float alpha)
    {
        SetAlpha(alpha);

        StartCoroutine(Fade_Coroutine());
    }

    private IEnumerator Fade_Coroutine()
    {
        _image.enabled = true;

        float elapsedTime = 0f;

        float startAlpha = _image.color.a;

        while (elapsedTime < _fadeTime)
        {
            float alpha = Mathf.Lerp(startAlpha, 0f, elapsedTime / _fadeTime);

            SetAlpha(alpha);

            elapsedTime += Time.deltaTime;
            yield return null; // 한 프레임 씩 쉼
        }

        SetAlpha(0f);
        _image.enabled = false;
    }

    private void SetAlpha(float alpha)
    {
        Color color = _image.color;
        color.a = alpha;
        _image.color = color;
    }
}