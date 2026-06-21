using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.Audio; 
using UnityEngine.UI; 
using TMPro;

public class MenuInicio : MonoBehaviour
{
    [Header("Paneles del Menú")]
    [SerializeField] private GameObject menuPrincipal;
    [SerializeField] private GameObject menuAjustes;

    [Header("UI Estadísticas")]
    [SerializeField] private TMP_Text textoVictorias; 

    [Header("Referencias de Ajustes")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider sliderMusica;
    [SerializeField] private Slider sliderEfectos;
    [SerializeField] private Slider sliderSensibilidad;

    private void Start()
    {
        if (menuPrincipal != null) menuPrincipal.SetActive(true);
        if (menuAjustes != null) menuAjustes.SetActive(false);

        if (textoVictorias != null)
        {
            int victorias = PlayerPrefs.GetInt("Victorias", 0);
            textoVictorias.text = $"Número de Victorias: {victorias}";
        }

        CargarAjustesGuardados();
    }

    private void CargarAjustesGuardados()
    {
        float volMusica = PlayerPrefs.GetFloat("Musica", 1f);
        float volEfectos = PlayerPrefs.GetFloat("Efectos", 1f);
        float sensibilidad = PlayerPrefs.GetFloat("Sensibilidad", 15f);

        if (sliderMusica != null) sliderMusica.value = volMusica;
        if (sliderEfectos != null) sliderEfectos.value = volEfectos;
        if (sliderSensibilidad != null) sliderSensibilidad.value = sensibilidad;

        AplicarVolumenMixer("Musica", volMusica);
        AplicarVolumenMixer("Efectos", volEfectos);
    }

    // --- Métodos de Interfaz ---

    public void Jugar()
    {
        PlayerPrefs.SetFloat("NivelAlcohol", 0f);

        // Generar umbrales aleatorios: 5-6, 6-7, 7-8, u 8-9
        int umbralMin = Random.Range(5, 9); // Devuelve 5, 6, 7 u 8
        int umbralMax = umbralMin + 1;      // Devuelve 6, 7, 8 o 9

        // Guardar los valores generados
        PlayerPrefs.SetFloat("UmbralMin", (float)umbralMin);
        PlayerPrefs.SetFloat("UmbralMax", (float)umbralMax);

        PlayerPrefs.Save();
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void AbrirAjustes()
    {
        menuPrincipal.SetActive(false);
        menuAjustes.SetActive(true);
    }

    public void CerrarAjustes()
    {
        menuAjustes.SetActive(false);
        menuPrincipal.SetActive(true);
    }

    public void Salir()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();

        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    // --- Funciones para los Eventos de tus Ajustes ---

    public void AjustarVolumenMusica(float volumen)
    {
        AplicarVolumenMixer("Musica", volumen);
        PlayerPrefs.SetFloat("Musica", volumen); 
        PlayerPrefs.Save(); 
    }

    public void AjustarVolumenEfectos(float volumen)
    {
        AplicarVolumenMixer("Efectos", volumen);
        PlayerPrefs.SetFloat("Efectos", volumen); 
        PlayerPrefs.Save(); 
    }

    public void AjustarSensibilidad(float sensibilidad)
    {
        PlayerPrefs.SetFloat("Sensibilidad", sensibilidad); 
        PlayerPrefs.Save(); 
    }

    public void CambiarModoPantalla(int indiceModo)
    {
        switch (indiceModo)
        {
            case 0:
                Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen;
                break;
            case 1:
                Screen.fullScreenMode = FullScreenMode.FullScreenWindow;
                break;
            case 2:
                Screen.fullScreenMode = FullScreenMode.Windowed;
                break;
        }
    }

    private void AplicarVolumenMixer(string parametro, float volumen)
    {
        if (audioMixer != null)
        {
            float volumenCalculado = volumen > 0 ? Mathf.Log10(volumen) * 20 : -80f;
            audioMixer.SetFloat(parametro, volumenCalculado);
        }
    }
}