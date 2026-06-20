using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class GeneradorRutas : MonoBehaviour
{
    [Header("Modo de Juego")]
    [Tooltip("Si está marcado, solo se mostrará la baliza final (Modo Libre). Si no, será una ruta guiada.")]
    public bool modoLibre = false; // 🔥 NUESTRO INTERRUPTOR

    [Header("Configuración de la Ruta Guiada")]
    public int nodoInicio = 1;
    public int nodoFin = 19;
    public int longitudMinima = 5;
    public int longitudMaxima = 10;

    [Header("Gestor de Victoria (UI)")]
    public GameObject panelVictoria;
    
    private bool nivelCompletado = false; 

    private Dictionary<int, List<int>> grafo = new Dictionary<int, List<int>>()
    {
        {1, new List<int>{2, 6}},
        {2, new List<int>{1, 3, 7}},
        {3, new List<int>{2, 4, 8}},
        {4, new List<int>{3, 5, 9}},
        {5, new List<int>{4, 10}},
        {6, new List<int>{1, 7, 11}},
        {7, new List<int>{2, 6, 8, 12}},
        {8, new List<int>{3, 9, 13}},
        {9, new List<int>{8, 4, 14, 10}},
        {10, new List<int>{9, 5, 15}},
        {11, new List<int>{6, 16}},
        {12, new List<int>{11, 7, 13}},
        {13, new List<int>{12, 8, 14, 17}},
        {14, new List<int>{9, 13, 15}},
        {15, new List<int>{10, 14, 18}},
        {16, new List<int>{11, 17}},
        {17, new List<int>{19, 13, 16}},
        {18, new List<int>{15}},
        {19, new List<int>()} 
    };

    private List<List<int>> rutasValidas = new List<List<int>>();

    public List<int> ObtenerRutaAleatoria()
    {
        // ==========================================
        // 🔥 LÓGICA DEL MODO LIBRE 🔥
        // ==========================================
    if (modoLibre)
        {
            // Le pasamos ÚNICAMENTE el nodo final para que ignore el punto de salida
            Debug.Log("🗺️ Modo Libre activado: Mostrando únicamente la meta final.");
            return new List<int> { nodoFin };
        }

        // ==========================================
        // LÓGICA ORIGINAL (MODO GUIADO)
        // ==========================================
        rutasValidas.Clear();
        List<int> rutaActual = new List<int> { nodoInicio };
        
        BuscarRutasDFS(nodoInicio, rutaActual);

        if (rutasValidas.Count > 0)
        {
            int indice = Random.Range(0, rutasValidas.Count);
            return rutasValidas[indice];
        }
        else
        {
            return null;
        }
    }

    private void BuscarRutasDFS(int nodoActual, List<int> rutaActual)
    {
        if (rutaActual.Count > longitudMaxima) return;

        if (nodoActual == nodoFin)
        {
            if (rutaActual.Count >= longitudMinima)
            {
                rutasValidas.Add(new List<int>(rutaActual));
            }
            return;
        }

        foreach (int vecino in grafo[nodoActual])
        {
            if (!rutaActual.Contains(vecino))
            {
                rutaActual.Add(vecino);
                BuscarRutasDFS(vecino, rutaActual);
                rutaActual.RemoveAt(rutaActual.Count - 1);
            }
        }
    }

    public void CompletarNivel()
    {
        if (nivelCompletado) return;

        nivelCompletado = true;
        Debug.Log("🏆 ¡VICTORIA! Has llegado al nodo 19 y escapado de la policía.");

        Time.timeScale = 0f;

        if (panelVictoria != null)
        {
            panelVictoria.SetActive(true);
        }
        else
        {
            Debug.LogWarning("⚠️ GeneradorRutas: No has asignado el PanelVictoria en el Inspector.");
        }
    }

    public void ReiniciarOSiguienteNivel()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }

    // Función para el botón del Menú Principal en la pantalla de Victoria
    public void VolverAlMenuPrincipal()
    {
        // Descongelamos el tiempo por si el jugador ganó y el juego se paró
        Time.timeScale = 1f; 
        
        // PON AQUÍ EL NOMBRE EXACTO DE TU ESCENA DEL MENÚ
        SceneManager.LoadScene("MenuInicio"); 
    }
}