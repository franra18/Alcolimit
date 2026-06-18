using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [Header("Conexión con el Gestor")]
    public GestorPersecucion gestor;

    [Tooltip("El número de orden de este punto (0 para el primero, 1 para el segundo...)")]
    public int miIndice;

    void OnTriggerEnter(Collider other)
    {
        // Buscamos el Tag "Player" en el objeto que choca O en el objeto raíz (el padre de todo)
        if (other.CompareTag("Player") || other.transform.root.CompareTag("Player"))
        {
            // Le mandamos la señal al Gestor
            gestor.PuntoAlcanzado(miIndice);
        }
    }
}