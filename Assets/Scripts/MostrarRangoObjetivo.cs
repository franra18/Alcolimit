using UnityEngine;
using TMPro;

public class MostrarRangoObjetivo : MonoBehaviour
{
    [Tooltip("Arrastra aquí el objeto TextMeshPro que mostrará el rango")]
    public TMP_Text textoRango;

    private void Start()
    {
        if (textoRango != null)
        {
            float umbralMin = PlayerPrefs.GetFloat("UmbralMin", 5f);
            float umbralMax = PlayerPrefs.GetFloat("UmbralMax", 6f);

            textoRango.text = $"Rango objetivo: Entre {umbralMin} y {umbralMax}";
        }
        else
        {
            Debug.LogWarning("No has asignado el texto de rango en el inspector.");
        }
    }
}