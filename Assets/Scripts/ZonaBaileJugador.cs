using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(Collider))]
public class ZonaBaileJugador : MonoBehaviour
{
    [Header("Configuración de Interfaz")]
    public string textoInteraccion = "Presiona [E] para Bailar";
    public string textoCooldown = "Descansando...";

    [Header("Efectos")]
    [Range(0f, 100f)]
    [Tooltip("Cantidad de alcohol que se restará cada vez que bailes")]
    public float reduccionAlcohol = 15f;

    [Header("Animación y Tiempos")]
    [Tooltip("Tiempo en segundos que dura la animación de baile")]
    public float duracionBaile = 3f;
    [Tooltip("Segundos de enfriamiento (cooldown) antes de poder volver a bailar")]
    public float tiempoCooldown = 5f;

    private bool jugadorEnZona = false;
    private SistemaAlcohol alcoholJugador;
    private Animator animadorJugador;
    
    private float proximoBaileDisponible = 0f;
    private bool estaBailando = false;

    private void Start()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnZona = true;
            alcoholJugador = other.GetComponent<SistemaAlcohol>();
            animadorJugador = other.GetComponent<Animator>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorEnZona = false;
            alcoholJugador = null;
            animadorJugador = null;
        }
    }

    private void Update()
    {
        if (jugadorEnZona && alcoholJugador != null)
        {
            if (Keyboard.current.eKey.wasPressedThisFrame)
            {
                IntentarBailar();
            }
        }
    }

    private void IntentarBailar()
    {
        if (!estaBailando && Time.time >= proximoBaileDisponible)
        {
            StartCoroutine(RutinaBailar());
        }
    }

    private IEnumerator RutinaBailar()
    {
        estaBailando = true;
        
        if (alcoholJugador != null)
        {
            alcoholJugador.SendMessage("PermitirMovimiento", false, SendMessageOptions.DontRequireReceiver);
            alcoholJugador.nivelDeAlcohol = Mathf.Clamp(alcoholJugador.nivelDeAlcohol - reduccionAlcohol, 0f, 100f);
        }

        if (animadorJugador != null)
        {
            animadorJugador.SetBool("Dancing", true);
        }

        yield return new WaitForSeconds(duracionBaile);

        if (animadorJugador != null)
        {
            animadorJugador.SetBool("Dancing", false);
        }

        if (alcoholJugador != null)
        {
            alcoholJugador.SendMessage("PermitirMovimiento", true, SendMessageOptions.DontRequireReceiver);
        }

        estaBailando = false;
        proximoBaileDisponible = Time.time + tiempoCooldown; 
    }

    private void OnGUI()
    {
        if (jugadorEnZona && !estaBailando)
        {
            GUIStyle estilo = new GUIStyle();
            estilo.fontSize = 24;
            estilo.normal.textColor = Color.white;
            estilo.alignment = TextAnchor.MiddleCenter;

            string textoAMostrar = (Time.time < proximoBaileDisponible) ? textoCooldown : textoInteraccion;

            // Dibuja el texto en el centro inferior de la pantalla
            GUI.Label(new Rect(Screen.width / 2 - 150, Screen.height / 2 + 50, 300, 50), textoAMostrar, estilo);
        }
    }
}