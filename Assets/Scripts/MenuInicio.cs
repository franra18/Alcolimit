using UnityEngine;
using UnityEngine.SceneManagement; 
using UnityEngine.Audio; // Obligatorio para controlar el AudioMixer
using UnityEngine.UI;    // Obligatorio para usar componentes Slider y UI

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

    // Start se ejecuta una sola vez al iniciar la escena
    private void Start()
    {
        // Nos aseguramos de que solo el menú principal empiece visible
        if (menuPrincipal != null) menuPrincipal.SetActive(true);
        if (menuAjustes != null) menuAjustes.SetActive(false);

        CargarAjustesGuardados();
    }

    private void CargarAjustesGuardados()
    {
        // Carga los valores guardados previamente, o usa valores por defecto
        float volMusica = PlayerPrefs.GetFloat("VolMusica", 1f);
        float volEfectos = PlayerPrefs.GetFloat("VolEfectos", 1f);
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
        PlayerPrefs.SetFloat("VolMusica", volumen); 
    }

    public void AjustarVolumenEfectos(float volumen)
    {
        AplicarVolumenMixer("Efectos", volumen);
        PlayerPrefs.SetFloat("VolEfectos", volumen); 
    }

    public void AjustarSensibilidad(float sensibilidad)
    {
        PlayerPrefs.SetFloat("Sensibilidad", sensibilidad); 
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

    // Método auxiliar para calcular la escala logarítmica del AudioMixer
    private void AplicarVolumenMixer(string parametro, float volumen)
    {
        if (audioMixer != null)
        {
            float volumenCalculado = volumen > 0 ? Mathf.Log10(volumen) * 20 : -80f;
            audioMixer.SetFloat(parametro, volumenCalculado);
        }
    }
}