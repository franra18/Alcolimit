using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ZonaAlivio : MonoBehaviour, IInteractable
{
    public enum TipoAlivio { Orinar, Vomitar }
    
    [Header("Configuración de la Zona")]
    public TipoAlivio accionAlPulsarE;
    private string promptText;

    [Header("Requisitos y Efectos")]
    [Range(0f, 100f)]
    public float nivelAlcoholRequerido = 50f;
    
    [Range(0f, 100f)]
    public float reduccionAlcohol = 20f;

    private void Start()
    {
        // El collider no debe ser trigger para que el raycast pueda chocar contra él
        GetComponent<Collider>().isTrigger = false;

        if (accionAlPulsarE == TipoAlivio.Orinar)
            promptText = "Presiona [E] para Orinar";
        else
            promptText = "Presiona [E] para Vomitar";
    }

    public string GetPromptText()
    {
        return promptText;
    }

    public bool CanInteract()
    {
        // Busca al jugador para comprobar si cumple los requisitos antes de mostrar el texto
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            ControladorFluidos fluidos = player.GetComponent<ControladorFluidos>();
            SistemaAlcohol alcohol = player.GetComponent<SistemaAlcohol>();

            if (fluidos != null && alcohol != null)
            {
                // Solo se puede interactuar si no está ya ocupado y tiene suficiente alcohol
                return !fluidos.EstaOcupado() && alcohol.nivelDeAlcohol >= nivelAlcoholRequerido;
            }
        }
        return false;
    }

    public void Interact(Transform cameraTransform, SistemaAlcohol sistemaAlcohol)
    {
        if (!CanInteract()) return;

        // Ejecuta la acción usando las referencias que envía el sistema de interacción
        ControladorFluidos fluidosJugador = sistemaAlcohol.GetComponent<ControladorFluidos>();

        if (fluidosJugador != null)
        {
            fluidosJugador.IniciarAlivio(accionAlPulsarE);
            sistemaAlcohol.nivelDeAlcohol = Mathf.Clamp(sistemaAlcohol.nivelDeAlcohol - reduccionAlcohol, 0f, 100f);
        }
    }
}