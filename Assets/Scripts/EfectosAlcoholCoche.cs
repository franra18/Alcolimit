using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EfectosAlcoholCoche : MonoBehaviour
{
    [Header("Referencias")]
    [Tooltip("Arrastra aquí la cámara del jugador (o el pivote de la cámara si usas un script para mirar)")]
    public Transform mainCamera;
    [Tooltip("Arrastra aquí el objeto Global Volume de la escena del coche")]
    public Volume volumenGlobal;

    [Header("Ajustes de Mareo")]
    public float velocidadMareo = 1.2f;
    public float anguloMaximoMareo = 12f;

    private float nivelDeAlcohol = 0f;
    private ColorAdjustments ajustesColor;
    private LensDistortion distorsionLente;

    private void Start()
    {
        // Recuperar el nivel de alcohol con el que el jugador terminó la escena anterior
        nivelDeAlcohol = PlayerPrefs.GetFloat("NivelAlcohol", 0f);

        // Vinculamos los efectos del Volume a las variables del script
        if (volumenGlobal != null && volumenGlobal.profile != null)
        {
            volumenGlobal.profile.TryGet(out ajustesColor);
            volumenGlobal.profile.TryGet(out distorsionLente);
        }
    }

    private void Update()
    {
        float porcentajeAlcohol = nivelDeAlcohol / 100f; 

        AplicarMareoCamara(porcentajeAlcohol);
        AplicarEfectosVisuales(porcentajeAlcohol);
    }

    private void AplicarMareoCamara(float porcentaje)
    {
        if (mainCamera == null) return;

        float ruidoX = (Mathf.PerlinNoise(Time.time * velocidadMareo, 0f) * 2f) - 1f;
        float ruidoY = (Mathf.PerlinNoise(0f, Time.time * velocidadMareo) * 2f) - 1f;
        float ruidoZ = (Mathf.PerlinNoise(Time.time * velocidadMareo, Time.time * velocidadMareo) * 2f) - 1f;

        float rotX = ruidoX * anguloMaximoMareo * porcentaje;
        float rotY = ruidoY * anguloMaximoMareo * porcentaje;
        float rotZ = ruidoZ * (anguloMaximoMareo * 0.5f) * porcentaje; 

        // IMPORTANTE: Si ya tienes un script para mirar alrededor con el ratón en el coche, 
        // aplica este script a un objeto "Padre" de la cámara (Pivot), no directamente a la cámara.
        mainCamera.localRotation = Quaternion.Euler(rotX, rotY, rotZ);
    }

    private void AplicarEfectosVisuales(float porcentaje)
    {
        if (ajustesColor != null)
        {
            ajustesColor.saturation.value = Mathf.Lerp(0f, -80f, porcentaje); 
            ajustesColor.colorFilter.value = Color.Lerp(Color.white, new Color(0.85f, 0.9f, 0.6f), porcentaje); 
        }

        if (distorsionLente != null)
        {
            distorsionLente.intensity.value = Mathf.Lerp(0f, -0.6f, porcentaje);
        }
    }
}