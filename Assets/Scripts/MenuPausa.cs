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

    [Header("Referencias del Jugador")]
    [SerializeField] private FirstPersonController jugadorController;
    [SerializeField] private PlayerInteraction jugadorInteraccion;

    private bool estaPausado = false;

    private void Start()
    {
        // Al empezar la partida todo el menú debe estar completamente oculto
        OcultarTodoElMenu();
        if (canvasHUDJuego != null) canvasHUDJuego.SetActive(true);

        // Inicializa la sensibilidad en el slider si existe el jugador
        if (PlayerPrefs.HasKey("Sensibilidad"))
        {
            float sensGuardada = PlayerPrefs.GetFloat("Sensibilidad");
            if (jugadorController != null) jugadorController.mouseSensitivity = sensGuardada;
            if (sliderSensibilidad != null) sliderSensibilidad.value = sensGuardada;
        }
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (estaPausado)
            {
                // Si está en el menú de ajustes y pulsa Escape, vuelve al menú de pausa principal
                if (menuAjustesPanel.activeSelf)
                {
                    MostrarMenuPrincipalPausa();
                }
                else
                {
                    Reanudar();
                }
            }
            else
            {
                Pausar();
            }
        }
    }

    // --- Gestión de Navegación de Menús ---

    public void Pausar()
    {
        estaPausado = true;
        Time.timeScale = 0f;

        MostrarMenuPrincipalPausa();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (canvasHUDJuego != null) canvasHUDJuego.SetActive(false);

        if (jugadorController != null) jugadorController.enabled = false;

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

    // --- Funciones de Botones y Configuración ---

    public void SalirAlMenuPrincipal()
    {
        Time.timeScale = 1f; // Asegúrate de restablecer el tiempo antes de cambiar de escena
        SceneManager.LoadScene(0); // Cambia el 0 por el índice o nombre de tu menú de inicio
    }

    public void AjustarVolumenMusica(float volumen)
    {
        if (audioMixer != null)
        {
            // Evita log10(0) que daría error usando un número mínimo muy bajo
            float volumenCalculado = volumen > 0 ? Mathf.Log10(volumen) * 20 : -80f;
            audioMixer.SetFloat("Musica", volumenCalculado);
        }
    }

    public void AjustarVolumenEfectos(float volumen)
    {
        if (audioMixer != null)
        {
            float volumenCalculado = volumen > 0 ? Mathf.Log10(volumen) * 20 : -80f;
            audioMixer.SetFloat("Efectos", volumenCalculado);
        }
    }

    public void AjustarSensibilidad(float sensibilidad)
    {
        if (jugadorController != null)
        {
            jugadorController.mouseSensitivity = sensibilidad;
        }
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
}