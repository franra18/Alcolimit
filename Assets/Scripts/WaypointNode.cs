using System.Collections.Generic;
using UnityEngine;

public class WaypointNode : MonoBehaviour
{
    [Header("Conexiones")]
    [Tooltip("El siguiente punto (o puntos) de tu ruta")]
    public List<WaypointNode> siguientesPuntos;

    public WaypointNode ObtenerSiguientePunto()
    {
        if (siguientesPuntos == null || siguientesPuntos.Count == 0) return null;
        
        // Si hay varios caminos (bifurcación), elige uno al azar
        int indiceAleatorio = Random.Range(0, siguientesPuntos.Count);
        return siguientesPuntos[indiceAleatorio];
    }
}