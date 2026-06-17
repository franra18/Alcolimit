using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCMovimiento : MonoBehaviour
{
    [Header("Ajustes de Movimiento")]
    public float radioDePaseo = 10f;
    public float tiempoEntrePaseos = 4f;

    private NavMeshAgent agente;
    private Animator animator;
    private float temporizador;

    private void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        temporizador = tiempoEntrePaseos; 
    }

    private void Update()
    {
        temporizador += Time.deltaTime;

        if (temporizador >= tiempoEntrePaseos)
        {
            Vector3 nuevoDestino = ObtenerPuntoAleatorio(transform.position, radioDePaseo, NavMesh.AllAreas);
            agente.SetDestination(nuevoDestino);
            temporizador = 0f;
        }

        SincronizarAnimacion();
    }

    private void SincronizarAnimacion()
    {
        if (animator != null)
        {
            // Si el agente se está moviendo (velocidad mayor a 0.1), el objetivo es 1. Si está quieto, es 0.
            float targetVert = agente.velocity.magnitude > 0.1f ? 1f : 0f;
            
            // Interpola el valor para que la transición de la animación sea suave y no brusca
            float smoothVert = Mathf.Lerp(animator.GetFloat("Vert"), targetVert, Time.deltaTime * 10f);
            
            animator.SetFloat("Vert", smoothVert);
        }
    }

    private Vector3 ObtenerPuntoAleatorio(Vector3 origen, float distancia, int layerMask)
    {
        Vector3 direccionAleatoria = Random.insideUnitSphere * distancia;
        direccionAleatoria += origen;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(direccionAleatoria, out hit, distancia, layerMask))
        {
            return hit.position;
        }
        
        return origen; 
    }
}