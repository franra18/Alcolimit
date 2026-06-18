using UnityEngine;

public class CamaraMinimapa : MonoBehaviour
{
    [Header("Objetivo a Seguir")]
    [Tooltip("Arrastra aquí tu coche principal desde la Jerarquía")]
    public Transform jugador; 

    [Header("Ajustes de Cámara")]
    [Tooltip("A qué altura debe volar el dron del minimapa")]
    public float altura = 50f; 
    
    [Tooltip("Si se desmarca, el minimapa siempre mirará al Norte")]
    public bool rotarConJugador = true;

    // Usamos LateUpdate en lugar de Update para que la cámara se mueva
    // justo después de que el jugador haya terminado de moverse este frame.
    void LateUpdate()
    {
        if (jugador == null) return;

        // 1. SEGUIR LA POSICIÓN
        // Copiamos la posición X y Z del coche, pero le forzamos nuestra propia altura (Y)
        Vector3 nuevaPosicion = jugador.position;
        nuevaPosicion.y = altura;
        transform.position = nuevaPosicion;

        // 2. ROTAR CON EL COCHE
        if (rotarConJugador)
        {
            // Mantenemos la cámara mirando hacia abajo (X = 90)
            // Copiamos la rotación del volante del jugador (Y)
            // Congelamos el giro lateral para no marear (Z = 0)
            transform.rotation = Quaternion.Euler(90f, jugador.eulerAngles.y, 0f);
        }
    }
}