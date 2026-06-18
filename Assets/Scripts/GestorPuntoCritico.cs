using UnityEngine;

public class GestorPuntoCritico : MonoBehaviour
{
    [Header("Los dos muros invisibles")]
    public SemaforoCritico muroArriba;
    public SemaforoCritico muroAbajo;

    [Header("Tiempos")]
    public float tiempoPaso = 4f; // Segundos que tiene cada lado para pasar (luz verde)
    public float tiempoDespeje = 1.5f; // Segundos de margen donde AMBOS están en rojo

    private float timer;
    private bool pasaArriba = true;
    private bool enFaseDespeje = false; // Nos indica si estamos en el momento de pausa

    void Start()
    {
        ActualizarLuces();
    }

    void Update()
    {
        timer += Time.deltaTime;

        // 1. Si estamos en la fase de seguridad (todos en rojo esperando a que se vacíe el cruce)
        if (enFaseDespeje)
        {
            if (timer >= tiempoDespeje)
            {
                enFaseDespeje = false; // Terminamos el despeje
                timer = 0f;
                pasaArriba = !pasaArriba; // Cambiamos el turno al otro carril
                ActualizarLuces(); // Encendemos la luz verde del que le toca
            }
        }
        // 2. Si estamos en la fase normal (alguien tiene luz verde)
        else
        {
            if (timer >= tiempoPaso)
            {
                enFaseDespeje = true; // Entramos en fase de seguridad
                timer = 0f;
                PonerTodoEnRojo(); // Cortamos el tráfico de golpe para ambos
            }
        }
    }

    // Función que fuerza a los dos muros a ponerse en rojo
    void PonerTodoEnRojo()
    {
        if (muroArriba != null) muroArriba.luzVerde = false;
        if (muroAbajo != null) muroAbajo.luzVerde = false;
    }

    // Función que le da verde a uno y rojo al otro según de quién sea el turno
    void ActualizarLuces()
    {
        if (muroArriba != null) muroArriba.luzVerde = pasaArriba;
        if (muroAbajo != null) muroAbajo.luzVerde = !pasaArriba;
    }
}