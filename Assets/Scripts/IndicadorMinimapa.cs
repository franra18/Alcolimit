using UnityEngine;

public class IndicadorMinimapa : MonoBehaviour
{
    [Header("Referencias")]
    public Transform jugador; 
    public RectTransform minimapaUI; 
    public RectTransform iconoFlecha; 
    public Camera camaraMinimapa; 

    [Header("Ajustes Visuales")]
    public float margenBorde = 0f;

    [Header("Comportamiento")]
    [Tooltip("Si está marcado, el icono se apaga al entrar en el mapa (Checkpoints). Si está desmarcado, se queda visible dentro (Policía).")]
    public bool ocultarAlEntrarEnMinimapa = true;

    private Transform objetivoActual;

    public void SetObjetivo(Transform nuevoObjetivo)
    {
        objetivoActual = nuevoObjetivo;
        iconoFlecha.gameObject.SetActive(true); 
        Debug.Log("📡 RADAR: Registrado objetivo -> " + nuevoObjetivo.name);
    }

    void Update()
    {
        if (objetivoActual == null || jugador == null || !objetivoActual.gameObject.activeInHierarchy)
        {
            iconoFlecha.gameObject.SetActive(false);
            return;
        }

        // Convertimos la posición 3D del mundo a coordenadas 2D del visor de la cámara (valores entre 0 y 1)
        Vector3 viewportPos = camaraMinimapa.WorldToViewportPoint(objetivoActual.position);

        // Comprobamos si matemáticamente está dentro del cuadrado del minimapa
        bool estaDentroDelMinimapa = viewportPos.x >= 0f && viewportPos.x <= 1f && 
                                     viewportPos.y >= 0f && viewportPos.y <= 1f && 
                                     viewportPos.z > 0f; 

        if (estaDentroDelMinimapa)
        {
            if (ocultarAlEntrarEnMinimapa)
            {
                // COMPORTAMIENTO CHECKPOINT: Nos ocultamos
                if (iconoFlecha.gameObject.activeSelf)
                {
                    iconoFlecha.gameObject.SetActive(false);
                }
            }
            else
            {
                // COMPORTAMIENTO POLICÍA: Nos mantenemos encendidos y nos posicionamos en su calle real
                if (!iconoFlecha.gameObject.activeSelf)
                {
                    iconoFlecha.gameObject.SetActive(true);
                }

                // Convertimos el rango (0 a 1) del Viewport al tamaño real en píxeles de tu interfaz de minimapa
                float posX = (viewportPos.x - 0.5f) * minimapaUI.rect.width;
                float posY = (viewportPos.y - 0.5f) * minimapaUI.rect.height;
                iconoFlecha.anchoredPosition = new Vector2(posX, posY);

                // Hacemos que la flecha mire hacia el "Norte" del mapa de forma neutral cuando circula por dentro
                iconoFlecha.localRotation = Quaternion.identity; 
            }
        }
        else
        {
            // COMPORTAMIENTO CUANDO ESTÁ FUERA (Igual para ambos: se engancha al borde de la pantalla)
            if (!iconoFlecha.gameObject.activeSelf)
            {
                iconoFlecha.gameObject.SetActive(true);
            }

            Vector2 direccion2D = new Vector2(viewportPos.x - 0.5f, viewportPos.y - 0.5f);
            direccion2D.Normalize(); 

            float radioUI = (minimapaUI.rect.height / 2f) + margenBorde;
            iconoFlecha.anchoredPosition = direccion2D * radioUI;

            float angulo = Mathf.Atan2(direccion2D.y, direccion2D.x) * Mathf.Rad2Deg;
            iconoFlecha.localRotation = Quaternion.Euler(0, 0, angulo - 90f);
        }
    }
}