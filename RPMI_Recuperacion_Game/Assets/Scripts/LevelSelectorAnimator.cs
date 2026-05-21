using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectorAnimator : MonoBehaviour
{
    [Header("Elementos UI")]
    public TextMeshProUGUI titleText;
    public GameObject[] levelNodes;   // Arrastra los 3 nodos en orden
    public Button backButton;

    [Header("Animación")]
    public float titleDuration = 0.6f;
    public float nodeDuration = 0.4f;
    public float delayEntreNodos = 0.15f;

    private Vector3[] originalNodeScales;
    void Start()
    {
        // Guardar scales originales ANTES de ponerlos a cero
        originalNodeScales = new Vector3[levelNodes.Length];
        for (int i = 0; i < levelNodes.Length; i++)
            originalNodeScales[i] = levelNodes[i].transform.localScale;

        // Ocultar todo al inicio
        titleText.transform.localScale = Vector3.zero;
        foreach (GameObject node in levelNodes)
            node.transform.localScale = Vector3.zero;
        backButton.transform.localScale = Vector3.zero;

        StartCoroutine(AnimarEntrada());
    }

    private IEnumerator AnimarEntrada()
    {
        yield return StartCoroutine(ScaleUpBounce(titleText.transform, titleDuration, Vector3.one));

        for (int i = 0; i < levelNodes.Length; i++)
        {
            StartCoroutine(ScaleUpBounce(levelNodes[i].transform, nodeDuration, originalNodeScales[i]));
            yield return new WaitForSeconds(delayEntreNodos);
        }

        yield return new WaitForSeconds(0.1f);
        yield return StartCoroutine(ScaleUpBounce(backButton.transform, nodeDuration, Vector3.one));
    }
    private IEnumerator ScaleUpBounce(Transform target, float duration, Vector3 targetScale)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / duration);
            float scale = progress < 0.7f
                ? Mathf.Lerp(0f, 1.2f, progress / 0.7f)
                : Mathf.Lerp(1.2f, 1f, (progress - 0.7f) / 0.3f);
            target.localScale = targetScale * scale;
            yield return null;
        }
        target.localScale = targetScale;
    }
}