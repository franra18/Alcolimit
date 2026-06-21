using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.Audio; 
using UnityEngine.UI;    

public class MenuInicio : MonoBehaviour
{
    [Header("Paneles del Menú")]
    [SerializeField] private GameObject menuPrincipal;
    [SerializeField] private GameObject menuAjustes;

    [Header("Referencias de Ajustes")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider sliderMusica;
    [SerializeField] private Slider sliderEfectos;
    [SerializeField] private Slider sliderSensibilidad;

    private void Start()
    {
        if (menuPrincipal != null) menuPrincipal.SetActive(true);
        if (menuAjustes != null) menuAjustes.SetActive(false);

        CargarAjustesGuardados();
    }

    private void CargarAjustesGuardados()
    {
        // Utilizamos EXACTAMENTE las mismas claves que en MenuPausa
        float volMusica = PlayerPrefs.GetFloat("Musica", 1f);
        float volEfectos = PlayerPrefs.GetFloat("Efectos", 1f);
        float sensibilidad = PlayerPrefs.GetFloat("Sensibilidad", 15f);

        // Inicializa visualmente los sliders
        if (sliderMusica != null) sliderMusica.value = volMusica;
        if (sliderEfectos != null) sliderEfectos.value = volEfectos;
        if (sliderSensibilidad != null) sliderSensibilidad.value = sensibilidad;

        // Aplica directamente el volumen al AudioMixer
        AplicarVolumenMixer("Musica", volMusica);
        AplicarVolumenMixer("Efectos", volEfectos);
    }

    // --- Métodos de Interfaz ---

    public void Jugar()
    {
        PlayerPrefs.SetFloat("NivelAlcohol", 0f);
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
        PlayerPrefs.SetFloat("Musica", volumen); // Clave unificada
        PlayerPrefs.Save(); // Forzar guardado
    }

    public void AjustarVolumenEfectos(float volumen)
    {
        AplicarVolumenMixer("Efectos", volumen);
        PlayerPrefs.SetFloat("Efectos", volumen); // Clave unificada
        PlayerPrefs.Save(); // Forzar guardado
    }

    public void AjustarSensibilidad(float sensibilidad)
    {
        PlayerPrefs.SetFloat("Sensibilidad", sensibilidad); 
        PlayerPrefs.Save(); // Forzar guardado
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