using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class ButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("Hover")]
    public float hoverScale = 1.15f;
    public float hoverSpeed = 8f;

    [Header("Click")]
    public float clickScale = 0.85f;
    public float clickDuration = 0.1f;

    [Header("Wave")]
    public bool waveOnHover = true;
    public float waveHeight = 5f;
    public float waveSpeed = 3f;
    public float waveOffset = 0.3f;

    [Header("Color")]
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;
    public float colorSpeed = 8f;

    [Header("Sonidos")]
    public AudioClip hoverSound;
    public AudioClip clickSound;

    private RectTransform rectTransform;
    private TextMeshProUGUI tmp;
    private Vector3 originalScale;
    private bool isHovered = false;
    private bool isAnimatingClick = false;
    private Coroutine scaleCoroutine;
    private Coroutine waveCoroutine;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        tmp = GetComponentInChildren<TextMeshProUGUI>();
        originalScale = rectTransform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        isHovered = true;
        MusicManager.instance?.PlaySFX(hoverSound); // 🔊 Sonido hover

        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        scaleCoroutine = StartCoroutine(ScaleTo(originalScale * hoverScale));

        if (waveOnHover && waveCoroutine == null)
            waveCoroutine = StartCoroutine(WaveText());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isHovered = false;

        if (scaleCoroutine != null) StopCoroutine(scaleCoroutine);
        scaleCoroutine = StartCoroutine(ScaleTo(originalScale));

        if (waveCoroutine != null)
        {
            StopCoroutine(waveCoroutine);
            waveCoroutine = null;
            ResetTextVertices();
        }

        if (tmp != null) tmp.color = normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!isAnimatingClick)
        {
            MusicManager.instance?.PlaySFX(clickSound); // 🔊 Sonido click
            StartCoroutine(ClickAnimation());
        }
    }

    private IEnumerator ClickAnimation()
    {
        isAnimatingClick = true;
        yield return StartCoroutine(ScaleTo(originalScale * clickScale, clickDuration));
        yield return StartCoroutine(ScaleTo(originalScale * hoverScale, clickDuration));
        isAnimatingClick = false;
    }

    private IEnumerator ScaleTo(Vector3 targetScale, float duration = -1f)
    {
        if (duration < 0) duration = 1f / hoverSpeed;

        Vector3 startScale = rectTransform.localScale;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
            rectTransform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }

        rectTransform.localScale = targetScale;
    }

    private IEnumerator WaveText()
    {
        while (isHovered)
        {
            if (tmp == null) yield break;

            tmp.ForceMeshUpdate();
            TMP_TextInfo textInfo = tmp.textInfo;

            float colorT = (Mathf.Sin(Time.unscaledTime * colorSpeed) + 1f) / 2f;
            tmp.color = Color.Lerp(normalColor, hoverColor, colorT);

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                int vertexIndex = charInfo.vertexIndex;
                int materialIndex = charInfo.materialReferenceIndex;
                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                float wave = Mathf.Sin(Time.unscaledTime * waveSpeed + i * waveOffset) * waveHeight;
                Vector3 offset = new Vector3(0, wave, 0);

                vertices[vertexIndex + 0] += offset;
                vertices[vertexIndex + 1] += offset;
                vertices[vertexIndex + 2] += offset;
                vertices[vertexIndex + 3] += offset;
            }

            for (int i = 0; i < textInfo.meshInfo.Length; i++)
                tmp.UpdateGeometry(textInfo.meshInfo[i].mesh, i);

            yield return null;
        }
    }

    private void ResetTextVertices()
    {
        if (tmp == null) return;
        tmp.ForceMeshUpdate();
        TMP_TextInfo textInfo = tmp.textInfo;

        for (int i = 0; i < textInfo.meshInfo.Length; i++)
            tmp.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
    }
}