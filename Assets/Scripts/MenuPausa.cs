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
    [SerializeField] private AudioMixer audioMixer;

    [Header("Referencias del Jugador (Interior)")]
    [SerializeField] private FirstPersonController jugadorController;
    [SerializeField] private PlayerInteraction jugadorInteraccion;

    [Header("Referencias del Jugador (Exterior/Coche)")]
    [SerializeField] private CockpitLook cockpitController; // <-- ¡NUEVO! Para que funcione en la escena 2

    private bool estaPausado = false;

    private void Start()
    {
        OcultarTodoElMenu();
        if (canvasHUDJuego != null) canvasHUDJuego.SetActive(true);

        // --- 1. CARGAR SENSIBILIDAD ---
        if (PlayerPrefs.HasKey("Sensibilidad"))
        {
            float sensGuardada = PlayerPrefs.GetFloat("Sensibilidad");
            
            // Se lo aplicamos al que esté activo en la escena actual
            if (jugadorController != null) jugadorController.mouseSensitivity = sensGuardada;
            if (cockpitController != null) cockpitController.mouseSensitivity = sensGuardada;
            
            if (sliderSensibilidad != null) sliderSensibilidad.value = sensGuardada;
        }

        // --- 2. CARGAR AUDIO ---
        if (PlayerPrefs.HasKey("Musica") && audioMixer != null)
        {
            AjustarVolumenMusica(PlayerPrefs.GetFloat("Musica"));
        }

        if (PlayerPrefs.HasKey("Efectos") && audioMixer != null)
        {
            AjustarVolumenEfectos(PlayerPrefs.GetFloat("Efectos"));
        }
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

        // Desactivamos los controles para que no te muevas estando en el menú
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

        // Volvemos a activar los controles
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

    // --- GUARDADO DE AJUSTES ---

    public void AjustarVolumenMusica(float volumen)
    {
        if (audioMixer != null)
        {
            float volumenCalculado = volumen > 0 ? Mathf.Log10(volumen) * 20 : -80f;
            audioMixer.SetFloat("Musica", volumenCalculado);
        }
        PlayerPrefs.SetFloat("Musica", volumen); // <-- GUARDAR
        PlayerPrefs.Save();
    }

    public void AjustarVolumenEfectos(float volumen)
    {
        if (audioMixer != null)
        {
            float volumenCalculado = volumen > 0 ? Mathf.Log10(volumen) * 20 : -80f;
            audioMixer.SetFloat("Efectos", volumenCalculado);
        }
        PlayerPrefs.SetFloat("Efectos", volumen); // <-- GUARDAR
        PlayerPrefs.Save();
    }

    public void AjustarSensibilidad(float sensibilidad)
    {
        // Se lo aplicamos a ambos scripts (aunque solo uno existirá en la escena a la vez)
        if (jugadorController != null) jugadorController.mouseSensitivity = sensibilidad;
        if (cockpitController != null) cockpitController.mouseSensitivity = sensibilidad;

        PlayerPrefs.SetFloat("Sensibilidad", sensibilidad); // <-- GUARDAR
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