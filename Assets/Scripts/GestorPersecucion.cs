using System.Collections.Generic;
using UnityEngine;

public class GestorPersecucion : MonoBehaviour
{
    [Header("Ruta de la Misión")]
    [Tooltip("Arrastra aquí los cilindros en el orden exacto del recorrido")]
    public List<GameObject> checkpoints;

    private int checkpointActual = 0;

    void Start()
    {
        // Al arrancar la escena, actualizamos la lista para que solo se vea el primer punto
        ActualizarCheckpoints();
    }

    public void PuntoAlcanzado(int indicePunto)
    {
        // Verificamos que el jugador ha pisado el punto que tocaba, y no uno antiguo
        if (indicePunto == checkpointActual)
        {
            Debug.Log("¡Checkpoint " + checkpointActual + " alcanzado!");
            checkpointActual++; // Sumamos 1 para avanzar al siguiente objetivo

            if (checkpointActual >= checkpoints.Count)
            {
                // Si ya no quedan más puntos en la lista, hemos llegado al final
                Debug.Log("¡Misión Completada! El coche está a salvo en la casa.");
                
                // (En el futuro, aquí pondremos la pantalla de victoria o frenaremos el coche)
            }
            else
            {
                // Si aún quedan puntos en la ruta, encendemos el siguiente
                ActualizarCheckpoints();
            }
        }
    }

    void ActualizarCheckpoints()
    {
        // Recorremos toda la lista de cilindros
        for (int i = 0; i < checkpoints.Count; i++)
        {
            if (checkpoints[i] != null)
            {
                // Esta línea es magia pura: 
                // Activa el objeto SI su posición en la lista (i) coincide con el que toca (checkpointActual).
                // Apaga el objeto automáticamente si NO coincide.
                checkpoints[i].SetActive(i == checkpointActual);
            }
        }
    }
}