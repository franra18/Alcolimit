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

    private Transform objetivoActual;

    public void SetObjetivo(Transform nuevoObjetivo)
    {
        objetivoActual = nuevoObjetivo;
        
        iconoFlecha.gameObject.SetActive(true); 
        }

    void Update()
    {
        if (objetivoActual == null || jugador == null || !objetivoActual.gameObject.activeInHierarchy)
        {
            iconoFlecha.gameObject.SetActive(false);
            return;
        }

        Vector3 viewportPos = camaraMinimapa.WorldToViewportPoint(objetivoActual.position);

        bool estaDentroDelMinimapa = viewportPos.x >= 0f && viewportPos.x <= 1f && 
                                     viewportPos.y >= 0f && viewportPos.y <= 1f && 
                                     viewportPos.z > 0f; 

        if (estaDentroDelMinimapa)
        {
            if (iconoFlecha.gameObject.activeSelf)
            {
                iconoFlecha.gameObject.SetActive(false);
            }
        }
        else
        {
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