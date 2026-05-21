using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class LevelNode : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI del nodo")]
    public Button button;
    public TextMeshProUGUI levelNumberText;
    public TextMeshProUGUI lockText;
    public Image nodeImage;

    [Header("Colores")]
    public Color colorDesbloqueado = new Color(0.2f, 0.9f, 0.4f);
    public Color colorBloqueado = new Color(0.4f, 0.4f, 0.4f);

    [Header("Animación hover")]
    public float hoverScale = 1.15f;

    private Vector3 originalScale;
    private int levelNumber;
    private bool isUnlocked;
    private bool wasUnlocked = false; // Para detectar si acaba de desbloquearse

    void Start()
    {
        StartCoroutine(CaptureScaleDelayed());

    }
    private System.Collections.IEnumerator CaptureScaleDelayed()
    {
        //Espera hasta que el scale sea mayor que cero
        yield return new WaitUntil(() => transform.localScale.x > 0.9f);
        originalScale = transform.localScale;       
    }

    public void SetState(bool unlocked, int number)
    {
        bool justUnlocked = unlocked && !wasUnlocked && number != 1;
        levelNumber = number;
        isUnlocked = unlocked;

        // Número del nivel
        if (levelNumberText != null)
            levelNumberText.text = number.ToString();

        // Candado
        if (lockText != null)
            lockText.text = unlocked ? "" : "🔒";

        // Color del nodo
        if (nodeImage != null)
            nodeImage.color = unlocked ? colorDesbloqueado : colorBloqueado;

        // Botón
        if (button != null)
        {
            button.interactable = unlocked;
            button.onClick.RemoveAllListeners();
            if (unlocked)
                button.onClick.AddListener(OnNodeClicked);
        }

        // 🎉 Animación de desbloqueo si acaba de desbloquearse
        if (justUnlocked)
            StartCoroutine(UnlockAnimation());

        wasUnlocked = unlocked;
    }

    void OnNodeClicked()
    {
        if (!isUnlocked) return;
        StartCoroutine(ClickAnimation());
    }

    // ==================== ANIMACIONES ====================

    private IEnumerator ClickAnimation()
    {
        float duracion = 0.1f;
        float elapsed = 0f;

        while (elapsed < duracion)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duracion;
            transform.localScale = Vector3.Lerp(originalScale, originalScale * 0.9f, t);
            yield return null;
        }

        yield return new WaitForSeconds(0.05f);

        if (LevelManager.instance != null)
            LevelManager.instance.LoadLevel(levelNumber);
    }

    private IEnumerator UnlockAnimation()
    {
        // Espera un frame para que todo esté inicializado
        yield return null;

        // 3 bounces al desbloquearse
        for (int i = 0; i < 3; i++)
        {
            yield return StartCoroutine(ScaleTo(originalScale * 1.2f, 0.12f));
            yield return StartCoroutine(ScaleTo(originalScale, 0.12f));
        }

        // Flash de color blanco
        if (nodeImage != null)
        {
            nodeImage.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            nodeImage.color = colorDesbloqueado;
        }
    }

    private IEnumerator ScaleTo(Vector3 target, float duracion = 0.1f)
    {
        float elapsed = 0f;
        Vector3 inicio = transform.localScale;

        while (elapsed < duracion)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / duracion);
            transform.localScale = Vector3.Lerp(inicio, target, t);
            yield return null;
        }

        transform.localScale = target;
    }

    // ==================== HOVER ====================

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isUnlocked) return;
        StartCoroutine(ScaleTo(originalScale * hoverScale));
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StartCoroutine(ScaleTo(originalScale));
    }
}