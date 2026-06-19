using UnityEngine;
using System.Collections;

public class ControladorFluidos : MonoBehaviour
{
    [Header("Sistemas de Partículas")]
    public ParticleSystem particulasOrina;
    public ParticleSystem particulasVomito;

    [Header("Ajustes de Alivio")]
    [Tooltip("Tiempo en segundos que durará la animación/partículas")]
    public float tiempoDuracion = 2.5f;

    private bool estaOcupado = false;

    public bool EstaOcupado()
    {
        return estaOcupado;
    }

    public void IniciarAlivio(ZonaAlivio.TipoAlivio tipo)
    {
        if (estaOcupado) return;
        StartCoroutine(RutinaAlivio(tipo));
    }

    private IEnumerator RutinaAlivio(ZonaAlivio.TipoAlivio tipo)
    {
        estaOcupado = true;

        // 1. Avisa a tu script de movimiento que bloquee las piernas
        SendMessage("PermitirMovimiento", false, SendMessageOptions.DontRequireReceiver);

        // 2. Elegir y activar el chorro
        ParticleSystem particulasActuales = (tipo == ZonaAlivio.TipoAlivio.Orinar) ? particulasOrina : particulasVomito;
        if (particulasActuales != null) particulasActuales.Play();

        // 3. Esperar los segundos definidos
        yield return new WaitForSeconds(tiempoDuracion);

        // 4. Detener el chorro
        if (particulasActuales != null) particulasActuales.Stop();

        // 5. Avisa a tu script de movimiento que devuelva el control
        SendMessage("PermitirMovimiento", true, SendMessageOptions.DontRequireReceiver);

        estaOcupado = false;
    }
}