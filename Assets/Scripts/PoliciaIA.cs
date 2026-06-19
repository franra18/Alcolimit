using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(Rigidbody))]
public class PoliciaIA : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador; 

    [Header("Configuración de Conducción")]
    public float velocidadMaxima = 18f;
    public float velocidadGiro = 4f;
    public float distanciaFrenado = 4f;

    [Header("Instinto de Supervivencia (Anti-Atasco)")]
    [Tooltip("Tiempo en segundos que debe estar bloqueado para activar la marcha atrás")]
    public float tiempoLimiteAtascado = 1.5f; 
    [Tooltip("Tiempo en segundos que pasará reculando")]
    public float duracionMarchaAtras = 1.2f;  

    private NavMeshAgent agent;
    private Rigidbody rb;

    // Variables internas del sistema de escape
    private float cronometroAtascado = 0f;
    private float cronometroMarchaAtras = 0f;
    private bool marchandoAtras = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        agent.updatePosition = false;
        agent.updateRotation = false;

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.linearDamping = 1f; 
    }

    void FixedUpdate()
    {
        if (jugador == null) return;

        // --- ESTADO DE ESCAPE (MARCHA ATRÁS) ---
        if (marchandoAtras)
        {
            EjecutarManiobraDeEscape();
            return; // Saltamos el resto del código para que no intente avanzar a la vez
        }

        // --- ESTADO NORMAL (PERSECUCIÓN) ---
        agent.SetDestination(jugador.position);
        agent.nextPosition = transform.position;

        Vector3 siguienteEsquina = agent.steeringTarget;
        Vector3 direccionHaciaEsquina = siguienteEsquina - transform.position;
        direccionHaciaEsquina.y = 0; 

        float distanciaAlJugador = Vector3.Distance(transform.position, jugador.position);

        if (distanciaAlJugador > distanciaFrenado)
        {
            GirarYConducir(direccionHaciaEsquina);
            ComprobarSiNosHemosAtascado();
        }
        else
        {
            Frenar();
            cronometroAtascado = 0f; // Si frena a propósito, no cuenta como atascado
        }
    }

    void GirarYConducir(Vector3 direccion)
    {
        if (direccion.magnitude < 0.2f) return;

        Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo, velocidadGiro * Time.fixedDeltaTime);

        float alineacion = Vector3.Dot(transform.forward, direccion.normalized);

        if (alineacion > 0.4f)
        {
            if (rb.linearVelocity.magnitude < velocidadMaxima)
            {
                Vector3 fuerzaAvance = transform.forward * velocidadMaxima * 2f;
                rb.AddForce(fuerzaAvance, ForceMode.Acceleration);
            }
        }
    }

    void ComprobarSiNosHemosAtascado()
    {
        // Si el coche se mueve a menos de 0.5 m/s (está casi parado) pero el código le exige avanzar...
        if (rb.linearVelocity.magnitude < 0.5f)
        {
            cronometroAtascado += Time.fixedDeltaTime;

            if (cronometroAtascado >= tiempoLimiteAtascado)
            {
                // Salta la alarma: Activamos la marcha atrás
                marchandoAtras = true;
                cronometroMarchaAtras = duracionMarchaAtras;
                cronometroAtascado = 0f;
                Debug.Log("🚨 POLICÍA: ¡Me he quedado enganchado! Iniciando marcha atrás de emergencia.");
            }
        }
        else
        {
            // Si el coche fluye y se mueve bien, el cronómetro se limpia
            cronometroAtascado = 0f;
        }
    }

    void EjecutarManiobraDeEscape()
    {
        cronometroMarchaAtras -= Time.fixedDeltaTime;

        if (cronometroMarchaAtras <= 0f)
        {
            // Ha terminado el tiempo de recular, volvemos a la normalidad
            marchandoAtras = false;
            rb.linearVelocity = Vector3.zero; // Frenazo para quitar inercias viejas
            Debug.Log("🔄 POLICÍA: Desatascado con éxito. Reanudando persecución.");
            return;
        }

        // 1. Aplicamos fuerza física hacia atrás (-transform.forward)
        Vector3 fuerzaRetroceso = -transform.forward * velocidadMaxima * 1.5f;
        rb.AddForce(fuerzaRetroceso, ForceMode.Acceleration);

        // 2. Giramos el volante mientras vamos marcha atrás para desenganchar el parachoques de la pared
        // Girar sobre el eje Y un poco en cada frame simula el giro de las ruedas
        transform.Rotate(Vector3.up * 50f * Time.fixedDeltaTime);
    }

    void Frenar()
    {
        rb.linearVelocity = Vector3.Lerp(rb.linearVelocity, Vector3.zero, 4f * Time.fixedDeltaTime);
    }
}