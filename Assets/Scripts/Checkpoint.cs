using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Conexión con el Gestor")]
    public GestorPersecucion gestor;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || (other.attachedRigidbody != null && other.attachedRigidbody.CompareTag("Player")))
        {
            gestor.PuntoAlcanzado(this.gameObject);
        }
    }
}