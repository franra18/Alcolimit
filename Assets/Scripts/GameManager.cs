using UnityEngine;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Arrastra aquí a tu jugador (debe tener el script SistemaAlcohol)")]
    public SistemaAlcohol sistemaAlcoholJugador;

    [Header("Ajustes de Tiempo")]
    [Tooltip("Tiempo en segundos que debe pasar para restar 1 unidad de alcohol")]
    public float segundosParaRestar = 10f;

    private void Start()
    {
        if (sistemaAlcoholJugador != null)
        {
            StartCoroutine(RutinaBajarAlcohol());
        }
        else
        {
            Debug.LogWarning("GameManager: No se ha asignado el script SistemaAlcohol.");
        }
    }

    private IEnumerator RutinaBajarAlcohol()
    {
        while (true)
        {
            yield return new WaitForSeconds(segundosParaRestar);

            if (sistemaAlcoholJugador != null && sistemaAlcoholJugador.nivelDeAlcohol > 0f)
            {
                sistemaAlcoholJugador.nivelDeAlcohol = Mathf.Clamp(sistemaAlcoholJugador.nivelDeAlcohol - 1f, 0f, 100f);
            }
        }
    }
}