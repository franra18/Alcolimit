using UnityEngine;

public class GestorPuntoCritico : MonoBehaviour
{
    [Header("Los dos muros invisibles")]
    public SemaforoCritico muroArriba;
    public SemaforoCritico muroAbajo;

    [Header("Configuración")]
    public float tiempoPaso = 4f; // Segundos que tiene cada lado para pasar

    private float timer;
    private bool pasaArriba = true;

    void Start()
    {
        ActualizarLuces();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= tiempoPaso)
        {
            timer = 0f;
            pasaArriba = !pasaArriba; // Cambiamos el turno
            ActualizarLuces();
        }
    }

    void ActualizarLuces()
    {
        // Si le toca a los de arriba, su muro se pone verde y el de abajo en rojo (y viceversa)
        if (muroArriba != null) muroArriba.luzVerde = pasaArriba;
        if (muroAbajo != null) muroAbajo.luzVerde = !pasaArriba;
    }
}