using UnityEngine;

public class LucesPolicia : MonoBehaviour
{
    [Header("Materiales del Coche")]
    public Material materialAzul;
    public Material materialRojo;

    [Header("Control de Brillo")]
    [Tooltip("Sube este número para que la sirena deslumbre más (ej. 8)")]
    public float intensidadMaxima = 8f; 
    public float velocidadParpadeo = 0.2f;

    [Header("Luces Reales (Point Lights)")]
    public Light luzAzul;
    public Light luzRoja;

    // Colores puros (puedes cambiarlos aquí si quieres un azul más claro o más oscuro)
    private Color azulPuro = new Color(0f, 0.5f, 1f); 
    private Color rojoPuro = Color.red;
    private Color colorApagado = Color.black; 

    private float temporizador;
    private bool estadoAzul = true;

    void Start()
    {
        if (luzAzul != null) luzAzul.enabled = true;
        if (luzRoja != null) luzRoja.enabled = false;
    }

    void Update()
    {
        temporizador += Time.deltaTime;

        if (temporizador >= velocidadParpadeo)
        {
            temporizador = 0f;
            estadoAzul = !estadoAzul;

            // 🔥 LA FÓRMULA MÁGICA: Multiplicamos el color por tu variable de intensidad
            Color colorAzulBrillante = azulPuro * intensidadMaxima;
            Color colorRojoBrillante = rojoPuro * intensidadMaxima;

            if (estadoAzul)
            {
                // Enciende Azul
                if (materialAzul != null) materialAzul.SetColor("_EmissionColor", colorAzulBrillante);
                if (materialRojo != null) materialRojo.SetColor("_EmissionColor", colorApagado);
                
                if (luzAzul != null) luzAzul.enabled = true;
                if (luzRoja != null) luzRoja.enabled = false;
            }
            else
            {
                // Enciende Rojo
                if (materialAzul != null) materialAzul.SetColor("_EmissionColor", colorApagado);
                if (materialRojo != null) materialRojo.SetColor("_EmissionColor", colorRojoBrillante);

                if (luzAzul != null) luzAzul.enabled = false;
                if (luzRoja != null) luzRoja.enabled = true;
            }
        }
    }

    void OnDisable()
    {
        // Al parar el juego, dejamos las sirenas apagadas por defecto
        if (materialAzul != null) materialAzul.SetColor("_EmissionColor", colorApagado);
        if (materialRojo != null) materialRojo.SetColor("_EmissionColor", colorApagado);
    }
}