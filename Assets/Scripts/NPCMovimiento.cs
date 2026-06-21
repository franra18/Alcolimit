using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCMovimiento : MonoBehaviour
{
    [Header("Referencias")]
    public AudioSource audioFuente;

    [Header("Probabilidades")]
    [Range(0f, 1f)] public float probabilidadIrAZona = 0.5f;

    [Header("Tiempos de Espera (Segundos)")]
    public float tiempoZonaMin = 2f;
    public float tiempoZonaMax = 5f;
    public float tiempoLibreMin = 1f;
    public float tiempoLibreMax = 3f;

    [Header("Ajustes de Baile")]
    public float velocidadGiroBaile = 200f;
    public float tiempoBaile = 10f; // Variable añadida para el tiempo fijo en pista

    private NavMeshAgent agente;
    private Animator animator;
    
    private float temporizador;
    private float tiempoEsperaActual;
    private bool esperando = true;
    private bool estoyHablando = false;

    private ZonaNPC zonaActual;
    private ZonaNPC[] todasLasZonas;
    private NavMeshTriangulation navMeshData;
    private Coroutine rutinaGiro;

    private void Start()
    {
        agente = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        
        todasLasZonas = Object.FindObjectsByType<ZonaNPC>(FindObjectsSortMode.None);
        navMeshData = NavMesh.CalculateTriangulation();
        
        tiempoEsperaActual = Random.Range(tiempoLibreMin, tiempoLibreMax);
        temporizador = 0f;
        esperando = true;
    }

    private void Update()
    {
        if (!esperando)
        {
            if (!agente.pathPending && agente.remainingDistance <= 0.5f)
            {
                esperando = true;
                temporizador = 0f;

                if (zonaActual != null)
                {
                    if (zonaActual.puntoCamarero != null)
                    {
                        zonaActual.clienteEsperando = true;
                        zonaActual.clienteServido = false;
                    }

                    if (zonaActual.esPistaDeBaile)
                    {
                        if (rutinaGiro != null) StopCoroutine(rutinaGiro);
                        rutinaGiro = StartCoroutine(GirarYBailar());
                    }
                }
            }
        }
        else 
        {
            if (zonaActual != null && zonaActual.puntoCamarero != null)
            {
                if (zonaActual.clienteServido)
                {
                    DecidirProximoDestino();
                    esperando = false; 
                }
            }
            else
            {
                if (zonaActual != null && zonaActual.ocupantesActuales > 1 && !zonaActual.esPistaDeBaile)
                {
                    if (!zonaActual.alguienHablando && !estoyHablando)
                    {
                        zonaActual.alguienHablando = true;
                        estoyHablando = true;
                        if (audioFuente != null) audioFuente.Play();
                    }
                }
                else
                {
                    DejarDeHablar();
                }

                temporizador += Time.deltaTime;
                if (temporizador >= tiempoEsperaActual)
                {
                    DecidirProximoDestino();
                    esperando = false; 
                }
            }
        }

        SincronizarAnimacion();
    }

    private IEnumerator GirarYBailar()
    {
        float anguloY = Random.Range(80f, 100f);
        Quaternion rotacionDestino = Quaternion.Euler(0f, anguloY, 0f);

        agente.updateRotation = false;

        while (Quaternion.Angle(transform.rotation, rotacionDestino) > 2f)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotacionDestino, velocidadGiroBaile * Time.deltaTime);
            yield return null;
        }

        agente.updateRotation = true;
        if (animator != null)
        {
            animator.SetBool("Dancing", true);
        }
    }

    private void DejarDeHablar()
    {
        if (estoyHablando)
        {
            if (audioFuente != null) audioFuente.Stop();
            estoyHablando = false;
            if (zonaActual != null) zonaActual.alguienHablando = false;
        }
    }

    private void DecidirProximoDestino()
    {
        if (rutinaGiro != null) StopCoroutine(rutinaGiro);
        
        DejarDeHablar();
        
        if (animator != null) animator.SetBool("Dancing", false);
        
        if (Random.value <= probabilidadIrAZona && todasLasZonas.Length > 0)
        {
            IrAZonaDisponible();
        }
        else
        {
            IrAPuntoAleatorio();
        }
    }

    private void IrAZonaDisponible()
    {
        List<ZonaNPC> zonasDisponibles = new List<ZonaNPC>();

        foreach (ZonaNPC zona in todasLasZonas)
        {
            // NUEVO: Comprueba que la zona esté a la misma altura (margen de 1 metro)
            bool esMismaPlanta = Mathf.Abs(zona.transform.position.y - transform.position.y) < 1f;

            if (zona.HayEspacio() && zona != zonaActual && esMismaPlanta)
            {
                zonasDisponibles.Add(zona);
            }
        }

        if (zonasDisponibles.Count > 0)
        {
            LiberarZonaActual(); 

            int indiceAleatorio = Random.Range(0, zonasDisponibles.Count);
            zonaActual = zonasDisponibles[indiceAleatorio];
            zonaActual.ocupantesActuales++;

            Vector3 destinoDestino = zonaActual.ObtenerPuntoDestino();
            
            agente.SetDestination(destinoDestino);
            
            if (zonaActual.esPistaDeBaile) tiempoEsperaActual = tiempoBaile;
            else tiempoEsperaActual = Random.Range(tiempoZonaMin, tiempoZonaMax);
        }
        else
        {
            IrAPuntoAleatorio();
        }
    }

    private void IrAPuntoAleatorio()
    {
        LiberarZonaActual(); 

        if (navMeshData.vertices.Length == 0) return;

        Vector3 puntoAleatorio = transform.position;
        bool puntoValido = false;

        // NUEVO: Intenta hasta 30 veces buscar un vértice que pertenezca a su misma planta
        for (int i = 0; i < 30; i++)
        {
            int indiceAleatorio = Random.Range(0, navMeshData.vertices.Length);
            Vector3 posiblePunto = navMeshData.vertices[indiceAleatorio];

            if (Mathf.Abs(posiblePunto.y - transform.position.y) < 1f)
            {
                puntoAleatorio = posiblePunto;
                puntoValido = true;
                break;
            }
        }

        if (puntoValido)
        {
            agente.SetDestination(puntoAleatorio);
            tiempoEsperaActual = Random.Range(tiempoLibreMin, tiempoLibreMax);
        }
    }

    private void LiberarZonaActual()
    {
        DejarDeHablar();
        
        if (zonaActual != null)
        {
            zonaActual.ocupantesActuales--;
            zonaActual.clienteEsperando = false;
            zonaActual.clienteServido = false;
            zonaActual = null;
        }
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