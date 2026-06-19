using UnityEngine;

public class ZonaNPC : MonoBehaviour
{
    public int capacidadMaxima = 2;
    
    [HideInInspector] public int ocupantesActuales = 0;
    [HideInInspector] public bool alguienHablando = false;

    [Header("Ajustes de Barra (Dejar vacío si no es barra)")]
    public Transform puntoCamarero;
    
    [HideInInspector] public bool clienteEsperando = false;
    [HideInInspector] public bool clienteServido = false;

    public bool HayEspacio()
    {
        return ocupantesActuales < capacidadMaxima;
    }
}