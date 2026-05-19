using System.Collections;
using UnityEngine;
using TMPro;

public class PauseTextAnimation : MonoBehaviour
{
    [Header("Animación de letras")]
    public float waveHeight = 10f;       // Altura de la ola
    public float waveSpeed = 2f;         // Velocidad de la ola
    public float waveOffset = 0.3f;      // Separación entre letras

    [Header("Color")]
    public bool rainbowMode = false;     // Modo arcoíris
    public Color color1 = Color.white;
    public Color color2 = Color.yellow;
    public float colorSpeed = 2f;

    private TextMeshProUGUI tmp;
    private bool isVisible = false;

    void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        isVisible = true;
        StartCoroutine(AnimateText());
    }

    void OnDisable()
    {
        isVisible = false;
    }

    private IEnumerator AnimateText()
    {
        while (isVisible)
        {
            tmp.ForceMeshUpdate();
            TMP_TextInfo textInfo = tmp.textInfo;

            for (int i = 0; i < textInfo.characterCount; i++)
            {
                TMP_CharacterInfo charInfo = textInfo.characterInfo[i];
                if (!charInfo.isVisible) continue;

                int vertexIndex = charInfo.vertexIndex;
                int materialIndex = charInfo.materialReferenceIndex;
                Vector3[] vertices = textInfo.meshInfo[materialIndex].vertices;

                // 🌊 Efecto wave (cada letra sube y baja con offset)
                float wave = Mathf.Sin(Time.unscaledTime * waveSpeed + i * waveOffset) * waveHeight;
                Vector3 offset = new Vector3(0, wave, 0);

                vertices[vertexIndex + 0] += offset;
                vertices[vertexIndex + 1] += offset;
                vertices[vertexIndex + 2] += offset;
                vertices[vertexIndex + 3] += offset;

                // 🎨 Animación de color
                Color32[] colors = textInfo.meshInfo[materialIndex].colors32;
                Color animatedColor;

                if (rainbowMode)
                {
                    float hue = (Time.unscaledTime * colorSpeed + i * 0.2f) % 1f;
                    animatedColor = Color.HSVToRGB(hue, 1f, 1f);
                }
                else
                {
                    float t = (Mathf.Sin(Time.unscaledTime * colorSpeed + i * waveOffset) + 1f) / 2f;
                    animatedColor = Color.Lerp(color1, color2, t);
                }

                colors[vertexIndex + 0] = (Color32)animatedColor;
                colors[vertexIndex + 1] = (Color32)animatedColor;
                colors[vertexIndex + 2] = (Color32)animatedColor;
                colors[vertexIndex + 3] = (Color32)animatedColor;
            }

            // Aplicar cambios
            for (int i = 0; i < textInfo.meshInfo.Length; i++)
            {
                tmp.UpdateGeometry(textInfo.meshInfo[i].mesh, i);
                tmp.UpdateVertexData(TMP_VertexDataUpdateFlags.Colors32);
            }

            yield return null;
        }
    }
}