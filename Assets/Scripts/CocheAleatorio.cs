using UnityEngine;

public class Coche : MonoBehaviour
{
    [Header("Navegación")]
    public WaypointNode nodoActual;
    public float velocidad = 8f;
    public float velocidadGiro = 5f;

    [Header("Sensor Ceda el Paso")]
    public float distanciaFrenado = 4f;
    public float radioEsferaSensor = 0.8f; 
    public LayerMask capaCoches; 

    private bool detenidoPorSemaforo = false;

    void Update()
    {
        if (nodoActual == null) return;

        if (detenidoPorSemaforo) return; 

        // 1. EL OJO DEL COCHE (Láser)
        Vector3 direccionAlNodo = (nodoActual.transform.position - transform.position).normalized;

        if (Physics.SphereCast(transform.position, radioEsferaSensor, direccionAlNodo, out RaycastHit hit, distanciaFrenado, capaCoches))
        {
            Debug.DrawRay(transform.position, direccionAlNodo * distanciaFrenado, Color.red);
            return; 
        }
        
        Debug.DrawRay(transform.position, direccionAlNodo * distanciaFrenado, Color.green);

        // 2. MOVER Y GIRAR
        transform.position = Vector3.MoveTowards(transform.position, nodoActual.transform.position, velocidad * Time.deltaTime);

        Vector3 direccion = nodoActual.transform.position - transform.position;
        if (direccion != Vector3.zero)
        {
            Quaternion rotacionDeseada = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, velocidadGiro * Time.deltaTime);
        }

        // 3. LLEGADA AL NODO
        if (Vector3.Distance(transform.position, nodoActual.transform.position) < 1.0f)
        {
            nodoActual = nodoActual.ObtenerSiguientePunto();
        }
    }

    // --- NUEVO: SENTIR LOS MUROS INVISIBLES ---
    
    void OnTriggerStay(Collider other)
    {
        SemaforoCritico semaforo = other.GetComponent<SemaforoCritico>();
        if (semaforo != null)
        {
           detenidoPorSemaforo = !semaforo.luzVerde;
        }
    }

    void OnTriggerExit(Collider other)
    {
        SemaforoCritico semaforo = other.GetComponent<SemaforoCritico>();
        if (semaforo != null)
        {
            detenidoPorSemaforo = false;
        }
    }
}