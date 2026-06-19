using UnityEngine;

public class ZonaNPC : MonoBehaviour
{
    public int capacidadMaxima = 2;
    
    [Header("Tipo de Zona")]
    public bool esPistaDeBaile = false;

    [HideInInspector] public int ocupantesActuales = 0;
    [HideInInspector] public bool alguienHablando = false;

    [Header("Ajustes de Barra (Punto Camarero)")]
    public Transform puntoCamarero;
    
    [Header("Ajustes de Área Amplia (Pista, VIP...)")]
    public BoxCollider areaAleatoria;
    
    [HideInInspector] public bool clienteEsperando = false;
    [HideInInspector] public bool clienteServido = false;

    public bool HayEspacio()
    {
        return ocupantesActuales < capacidadMaxima;
    }

    public Vector3 ObtenerPuntoDestino()
    {
        if (areaAleatoria != null)
        {
            Bounds limites = areaAleatoria.bounds;
            float xAleatorio = Random.Range(limites.min.x, limites.max.x);
            float zAleatorio = Random.Range(limites.min.z, limites.max.z);
            
            return new Vector3(xAleatorio, transform.position.y, zAleatorio);
        }
        else
        {
            Vector3 puntoCercano = transform.position + (Random.insideUnitSphere * 0.5f);
            puntoCercano.y = transform.position.y;
            return puntoCercano;
        }
    }
}