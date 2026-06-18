using System.Collections.Generic;
using UnityEngine;

public class GeneradorRutas : MonoBehaviour
{
    [Header("Configuración de la Ruta")]
    public int nodoInicio = 1;
    public int nodoFin = 19;
    public int longitudMinima = 5;
    public int longitudMaxima = 10;

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
        {19, new List<int>()} // Meta final
    };

    private List<List<int>> rutasValidas = new List<List<int>>();

    public List<int> ObtenerRutaAleatoria()
    {
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
}