using System.Collections.Generic;
using UnityEngine;

public class AlcoholBottlesManager : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Number of bottles to show randomly at the start of each game")]
    public int bottlesToShow = 15;

    [Header("Bottles List")]
    [Tooltip("List of all placed bottles in the scene. If empty, it will automatically look for all child GameObjects.")]
    public List<GameObject> bottles = new List<GameObject>();

    private void Start()
    {
        // If list is empty, automatically populate with children
        if (bottles == null || bottles.Count == 0)
        {
            bottles = new List<GameObject>();
            foreach (Transform child in transform)
            {
                bottles.Add(child.gameObject);
            }
        }

        // Disable all bottles first
        foreach (var bottle in bottles)
        {
            if (bottle != null)
            {
                bottle.SetActive(false);
            }
        }

        // Guard against requesting more than available
        int actualToShow = Mathf.Min(bottlesToShow, bottles.Count);

        // Copy list to select without replacement
        List<GameObject> candidates = new List<GameObject>(bottles);

        int activatedCount = 0;
        for (int i = 0; i < actualToShow; i++)
        {
            if (candidates.Count == 0) break;

            int randomIndex = Random.Range(0, candidates.Count);
            GameObject chosenBottle = candidates[randomIndex];

            if (chosenBottle != null)
            {
                chosenBottle.SetActive(true);
                activatedCount++;
            }

            candidates.RemoveAt(randomIndex);
        }

        Debug.Log($"[AlcoholBottlesManager] Successfully activated {activatedCount} random bottles out of {bottles.Count} total bottles.");
    }
}
