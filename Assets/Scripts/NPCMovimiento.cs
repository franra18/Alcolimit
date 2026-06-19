using UnityEngine;
using UnityEngine.AI;
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

    private NavMeshAgent agente;
    private Animator animator;
    
    private float temporizador;
    private float tiempoEsperaActual;
    private bool esperando = true;
    private bool estoyHablando = false;

    private ZonaNPC zonaActual;
    private ZonaNPC[] todasLasZonas;
    private NavMeshTriangulation navMeshData;

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

                if (zonaActual != null && zonaActual.puntoCamarero != null)
                {
                    zonaActual.clienteEsperando = true;
                    zonaActual.clienteServido = false;
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
                // Gestionar el audio si está en una zona con más gente
                if (zonaActual != null && zonaActual.ocupantesActuales > 1)
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
        DejarDeHablar();
        
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
            if (zona.HayEspacio() && zona != zonaActual)
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

            Vector3 puntoSeparado = zonaActual.transform.position + (Random.insideUnitSphere * 1.5f);
            puntoSeparado.y = zonaActual.transform.position.y;
            
            agente.SetDestination(puntoSeparado);
            tiempoEsperaActual = Random.Range(tiempoZonaMin, tiempoZonaMax);
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

        int indiceAleatorio = Random.Range(0, navMeshData.vertices.Length);
        Vector3 puntoAleatorio = navMeshData.vertices[indiceAleatorio];

        agente.SetDestination(puntoAleatorio);
        tiempoEsperaActual = Random.Range(tiempoLibreMin, tiempoLibreMax);
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