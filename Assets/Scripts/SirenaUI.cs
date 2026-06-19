using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SirenaUI : MonoBehaviour
{
    [Header("Colores de la Sirena")]
    public Color colorA = Color.red;
    public Color colorB = Color.blue;

    [Header("Configuración")]
    [Tooltip("Tiempo en segundos que tarda en cambiar de un color a otro")]
    public float velocidadCambio = 0.2f;

    private Image imagenUI;
    private float cronometro;
    private bool mostrandoColorA = true;

    void Start()
    {
        // Cogemos el componente Image del propio icono automáticamente
        imagenUI = GetComponent<Image>();
        imagenUI.color = colorA;
    }

    void Update()
    {
        cronometro += Time.deltaTime;

        // Cuando pasa el tiempo configurado, flasheamos al otro color
        if (cronometro >= velocidadCambio)
        {
            cronometro = 0f;
            mostrandoColorA = !mostrandoColorA;

            // Cambiamos el color de la imagen
            imagenUI.color = mostrandoColorA ? colorA : colorB;
        }
    }
}