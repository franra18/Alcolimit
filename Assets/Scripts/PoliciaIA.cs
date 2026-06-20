using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using TMPro;

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

    [Tooltip("Arrastra aquí el objeto IconoPolicia de la UI para rastrear este coche")]
    public IndicadorMinimapa indicadorRadarPolicia;

    [Header("Condición de Derrota (Arresto)")]
    [Tooltip("Distancia (Y) en metros a la que el policía empieza a arrestarte")]
    public float distanciaCaptura = 6f; 

    [Tooltip("Arrastra aquí el TextoAlarmaPolicia del Canvas")]
    public TMP_Text textoAlarmaUI;

    [Tooltip("Tiempo (X) en segundos que debes permanecer cerca para perder")]
    public float tiempoParaCaptura = 3f; 

    [Tooltip("Arrastra aquí el PanelGameOver que creaste en el Canvas")]
    public GameObject panelGameOver;

    private float cronometroCaptura = 0f;
    private bool juegoTerminado = false;

    private NavMeshAgent agent;
    private Rigidbody rb;

    // Variables internas del sistema de escape
    private float cronometroAtascado = 0f;
    private float cronometroMarchaAtras = 0f;
    private bool marchandoAtras = false;

    void Start()
    {
        if (panelGameOver != null)
    {
        panelGameOver.SetActive(false);
    }
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();

        agent.updatePosition = false;
        agent.updateRotation = false;

        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        rb.linearDamping = 1f; 
        // Le pasamos nuestro propio Transform al radar de la policía
        if (indicadorRadarPolicia != null)
        {
            indicadorRadarPolicia.SetObjetivo(transform);
        }

        NavMeshAgent agente = GetComponent<NavMeshAgent>();

    // Obligamos al agente a obedecer las variables de nuestro script
    if (agente != null)
    {
        agente.speed = velocidadMaxima;
        agente.angularSpeed = velocidadGiro;
        agente.stoppingDistance = distanciaFrenado;
    }
    }

void FixedUpdate()
    {
        if (jugador == null || juegoTerminado) return; // Si el juego terminó, la IA deja de pensar

        // 1. Calculamos la distancia SIEMPRE al principio del todo (antes de movernos)
        float distanciaAlJugador = Vector3.Distance(transform.position, jugador.position);
        //: {distanciaAlJugador:F1} metros");

        // 2. Comprobamos la condición de Derrota
        ComprobarArresto(distanciaAlJugador);

        // 3. 🔥 LA REGLA DE ORO: Si está tocando/acorralando al jugador, prohibido ir hacia atrás
        if (distanciaAlJugador <= distanciaCaptura)
        {
            marchandoAtras = false;
            cronometroAtascado = 0f; // Reiniciamos el contador para que no se estrese
        }

        // 4. --- ESTADO DE ESCAPE (MARCHA ATRÁS) ---
        if (marchandoAtras)
        {
            EjecutarManiobraDeEscape();
            MitigarVelocidadLateral();
            return; 
        }

        // 5. --- ESTADO NORMAL (PERSECUCIÓN) ---
        agent.SetDestination(jugador.position);
        agent.nextPosition = transform.position;

        Vector3 siguienteEsquina = agent.steeringTarget;
        Vector3 direccionHaciaEsquina = siguienteEsquina - transform.position;
        direccionHaciaEsquina.y = 0; 

        if (distanciaAlJugador > distanciaFrenado)
        {
            GirarYConducir(direccionHaciaEsquina);
            ComprobarSiNosHemosAtascado();
        }
        else
        {
            Frenar();
            cronometroAtascado = 0f; 
        }

        MitigarVelocidadLateral();
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

    void MitigarVelocidadLateral()
{
    Vector3 velocidadLocal = transform.InverseTransformDirection(rb.linearVelocity);

    velocidadLocal.x *= 0.1f; 

    rb.linearVelocity = transform.TransformDirection(velocidadLocal);
}

void ComprobarArresto(float distanciaAlJugador)
    {
        if (juegoTerminado) return;

        if (distanciaAlJugador <= distanciaCaptura)
        {
            cronometroCaptura += Time.fixedDeltaTime;

            // 🔥 MODO PELIGRO: Texto en rojo con la cuenta atrás
            if (textoAlarmaUI != null)
            {
                textoAlarmaUI.color = Color.red; // Cambiamos el color a rojo alerta
                // Calculamos cuánto tiempo queda restando el máximo menos el actual
                float tiempoRestante = tiempoParaCaptura - cronometroCaptura;
                textoAlarmaUI.text = $"¡PELIGRO! ARRESTO EN: {tiempoRestante:F1}s";
            }

            if (cronometroCaptura >= tiempoParaCaptura)
            {
                juegoTerminado = true;
                Time.timeScale = 0f; 
                
                if (panelGameOver != null)
                {
                    panelGameOver.SetActive(true);
                }
                
                // Ocultamos el texto del radar al perder
                if (textoAlarmaUI != null) textoAlarmaUI.gameObject.SetActive(false); 
            }
        }
        else
        {
            if (cronometroCaptura > 0f) cronometroCaptura = 0f;

            // 🔥 MODO SEGURO: Texto en blanco mostrando la distancia normal
            if (textoAlarmaUI != null)
            {
                textoAlarmaUI.color = Color.white; // Color normal
                textoAlarmaUI.text = $"Policía a: {distanciaAlJugador:F1} m";
            }
        }
    }
}