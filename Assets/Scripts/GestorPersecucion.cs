using System.Collections.Generic;
using UnityEngine;

public class GestorPersecucion : MonoBehaviour
{
    [Header("Referencias")]
    public GeneradorRutas generador; 
    
    [Tooltip("¡Arrastra aquí tu objeto IconoDestino para que el gestor pueda hablar con él!")]
    public IndicadorMinimapa indicadorRadar; 

    [Tooltip("Mete TODOS los cilindros del 1 al 19 aquí en orden numérico estricto")]
    public List<GameObject> todosLosCheckpointsDelMapa; 

    private List<GameObject> rutaActiva = new List<GameObject>();
    private int checkpointActual = 0;

    void Start()
    {
        foreach (var cp in todosLosCheckpointsDelMapa)
        {
            if (cp != null) cp.SetActive(false);
        }

        List<int> rutaNumerica = generador.ObtenerRutaAleatoria();

        if (rutaNumerica != null)
        {
            foreach (int idNodo in rutaNumerica)
            {
                rutaActiva.Add(todosLosCheckpointsDelMapa[idNodo - 1]);
            }
            
            ActualizarCheckpoints();
        }
    }

public void PuntoAlcanzado(GameObject checkpointTocado)
    {
        if (checkpointTocado == rutaActiva[checkpointActual])
        {
            checkpointActual++; 

            if (checkpointActual >= rutaActiva.Count)
            {
                if (indicadorRadar != null) 
                {
                    indicadorRadar.gameObject.SetActive(false);
                }
            }
            else
            {
               ActualizarCheckpoints();
            }
        }
    }

    void ActualizarCheckpoints()
    {
        for (int i = 0; i < rutaActiva.Count; i++)
        {
            if (rutaActiva[i] != null)
            {
                bool meTocaAparecer = (i == checkpointActual);
                rutaActiva[i].SetActive(meTocaAparecer);

                if (meTocaAparecer)
                {
                    
                    if (indicadorRadar != null)
                    {
                        indicadorRadar.SetObjetivo(rutaActiva[i].transform);
                    }
                }
            }
        }
    }
}