using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuPausa : MonoBehaviour
{
    [Header("Paneles de la UI")]
    [SerializeField] private GameObject menuPausaPanel;
    [SerializeField] private GameObject menuAjustesPanel;
    [SerializeField] private GameObject canvasHUDJuego;

    [Header("Sliders / Ajustes")]
    [SerializeField] private Slider sliderSensibilidad;
    [SerializeField] private Slider sliderMusica;  // <-- NUEVO
    [SerializeField] private Slider sliderEfectos; // <-- NUEVO
    [SerializeField] private AudioMixer audioMixer;

    [Header("Referencias del Jugador (Interior)")]
    [SerializeField] private FirstPersonController jugadorController;
    [SerializeField] private PlayerInteraction jugadorInteraccion;

    [Header("Referencias del Jugador (Exterior/Coche)")]
    [SerializeField] private CockpitLook cockpitController;

    private bool estaPausado = false;

    private void Start()
    {
        OcultarTodoElMenu();
        if (canvasHUDJuego != null) canvasHUDJuego.SetActive(true);

        CargarAjustes();
    }

    private void CargarAjustes()
    {
        // Cargamos los valores guardados o sus valores por defecto si es la primera vez
        float sensGuardada = PlayerPrefs.GetFloat("Sensibilidad", 15f);
        float volMusica = PlayerPrefs.GetFloat("Musica", 1f);
        float volEfectos = PlayerPrefs.GetFloat("Efectos", 1f);

        // --- 1. APLICAR SENSIBILIDAD ---
        if (jugadorController != null) jugadorController.mouseSensitivity = sensGuardada;
        if (cockpitController != null) cockpitController.mouseSensitivity = sensGuardada;
        if (sliderSensibilidad != null) sliderSensibilidad.value = sensGuardada;

        // --- 2. APLICAR AUDIO ---
        if (sliderMusica != null) sliderMusica.value = volMusica;
        if (sliderEfectos != null) sliderEfectos.value = volEfectos;

        AjustarVolumenMusica(volMusica);
        AjustarVolumenEfectos(volEfectos);
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (estaPausado)
            {
                if (menuAjustesPanel.activeSelf) MostrarMenuPrincipalPausa();
                else Reanudar();
            }
            else Pausar();
        }
    }

    public void Pausar()
    {
        estaPausado = true;
        Time.timeScale = 0f;
        MostrarMenuPrincipalPausa();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (canvasHUDJuego != null) canvasHUDJuego.SetActive(false);

        if (jugadorController != null) jugadorController.enabled = false;
        if (cockpitController != null) cockpitController.enabled = false;
        if (jugadorInteraccion != null) jugadorInteraccion.enabled = false;
    }

    public void Reanudar()
    {
        estaPausado = false;
        Time.timeScale = 1f;
        OcultarTodoElMenu();

        if (canvasHUDJuego != null) canvasHUDJuego.SetActive(true);

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (jugadorController != null) jugadorController.enabled = true;
        if (cockpitController != null) cockpitController.enabled = true;
        if (jugadorInteraccion != null) jugadorInteraccion.enabled = true;
    }

    public void AbrirAjustes()
    {
        if (menuPausaPanel != null) menuPausaPanel.SetActive(false);
        if (menuAjustesPanel != null) menuAjustesPanel.SetActive(true);
    }

    public void MostrarMenuPrincipalPausa()
    {
        if (menuPausaPanel != null) menuPausaPanel.SetActive(true);
        if (menuAjustesPanel != null) menuAjustesPanel.SetActive(false);
    }

    private void OcultarTodoElMenu()
    {
        if (menuPausaPanel != null) menuPausaPanel.SetActive(false);
        if (menuAjustesPanel != null) menuAjustesPanel.SetActive(false);
    }

    public void SalirAlMenuPrincipal()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(0); 
    }

    public void AjustarVolumenMusica(float volumen)
    {
        if (audioMixer != null)
        {
            float volumenCalculado = volumen > 0 ? Mathf.Log10(volumen) * 20 : -80f;
            audioMixer.SetFloat("Musica", volumenCalculado);
        }
        PlayerPrefs.SetFloat("Musica", volumen); 
        PlayerPrefs.Save();
    }

    public void AjustarVolumenEfectos(float volumen)
    {
        if (audioMixer != null)
        {
            float volumenCalculado = volumen > 0 ? Mathf.Log10(volumen) * 20 : -80f;
            audioMixer.SetFloat("Efectos", volumenCalculado);
        }
        PlayerPrefs.SetFloat("Efectos", volumen); 
        PlayerPrefs.Save();
    }

    public void AjustarSensibilidad(float sensibilidad)
    {
        if (jugadorController != null) jugadorController.mouseSensitivity = sensibilidad;
        if (cockpitController != null) cockpitController.mouseSensitivity = sensibilidad;

        PlayerPrefs.SetFloat("Sensibilidad", sensibilidad); 
        PlayerPrefs.Save();
    }

    public void CambiarModoPantalla(int indiceModo)
    {
        switch (indiceModo)
        {
            case 0: Screen.fullScreenMode = FullScreenMode.ExclusiveFullScreen; break;
            case 1: Screen.fullScreenMode = FullScreenMode.FullScreenWindow; break;
            case 2: Screen.fullScreenMode = FullScreenMode.Windowed; break;
        }
    }
}