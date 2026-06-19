using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class CamareroMovimiento : MonoBehaviour
{
    [Header("Referencias")]
    public Transform puntoCajaRegistradora;
    public AudioSource audioFuente;
    
    [Header("Ajustes")]
    public float tiempoSirviendo = 1f;
    public float velocidadRotacion = 300f;

    private NavMeshAgent agente;
    private Animator animator;
    private ZonaNPC[] todasLasZonas;
    private ZonaNPC zonaAtendiendoActual;
    
    private float temporizadorServicio;
    
    private enum Estado { EsperandoCaja, YendoABarra, Sirviendo }
    private Estado estadoActual = Estado.EsperandoCaja;

    private void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        todasLasZonas = Object.FindObjectsByType<ZonaNPC>(FindObjectsSortMode.None);
    }

    private void Update()
    {
        switch (estadoActual)
        {
            case Estado.EsperandoCaja:
                if (puntoCajaRegistradora != null && !agente.pathPending && agente.remainingDistance <= 0.2f)
                {
                    EncararHacia(puntoCajaRegistradora.rotation);
                }
                RevisarClientes();
                break;

            case Estado.YendoABarra:
                if (!agente.pathPending && agente.remainingDistance <= 0.2f && agente.hasPath)
                {
                    estadoActual = Estado.Sirviendo;
                    temporizadorServicio = 0f;
                    
                    if (audioFuente != null) audioFuente.Play();
                }
                break;

            case Estado.Sirviendo:
                if (zonaAtendiendoActual != null && zonaAtendiendoActual.puntoCamarero != null)
                {
                    EncararHacia(zonaAtendiendoActual.puntoCamarero.rotation);
                }

                temporizadorServicio += Time.deltaTime;
                if (temporizadorServicio >= tiempoSirviendo)
                {
                    if (audioFuente != null) audioFuente.Stop();

                    if (zonaAtendiendoActual != null)
                    {
                        zonaAtendiendoActual.clienteServido = true;
                        zonaAtendiendoActual.clienteEsperando = false;
                        zonaAtendiendoActual = null;
                    }
                    estadoActual = Estado.EsperandoCaja; 
                }
                break;
        }

        SincronizarAnimacion();
    }

    private void RevisarClientes()
    {
        ZonaNPC zonaNecesitaAtencion = null;

        foreach (ZonaNPC zona in todasLasZonas)
        {
            if (zona.puntoCamarero != null && zona.clienteEsperando && !zona.clienteServido)
            {
                zonaNecesitaAtencion = zona;
                break;
            }
        }

        if (zonaNecesitaAtencion != null)
        {
            zonaAtendiendoActual = zonaNecesitaAtencion;
            agente.SetDestination(zonaAtendiendoActual.puntoCamarero.position);
            estadoActual = Estado.YendoABarra;
        }
        else if (puntoCajaRegistradora != null && agente.destination != puntoCajaRegistradora.position)
        {
            agente.SetDestination(puntoCajaRegistradora.position);
        }
    }

    private void EncararHacia(Quaternion rotacionDestino)
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotacionDestino, velocidadRotacion * Time.deltaTime);
    }

    private void SincronizarAnimacion()
    {
        if (animator != null)
        {
            float targetVert = agente.velocity.magnitude > 0.1f ? 1f : 0f;
            float smoothVert = Mathf.Lerp(animator.GetFloat("Vert"), targetVert, Time.deltaTime * 10f);
            animator.SetFloat("Vert", smoothVert);
        }
    }
}