using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuPausa : MonoBehaviour
{
    [Header("UI")]
    public GameObject panelMenuPausa;
    public Slider sliderSensibilidad; // Para igualar el valor inicial

    [Header("Referencias")]
    public FirstPersonController jugadorController;
    public AudioMixer audioMixer;

    private bool estaPausado = false;

    private void Start()
    {
        panelMenuPausa.SetActive(false);

        // Iguala el Slider a la sensibilidad actual del personaje al iniciar
        if (jugadorController != null && sliderSensibilidad != null)
        {
            sliderSensibilidad.value = jugadorController.mouseSensitivity;
        }
    }

    private void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (estaPausado) Reanudar();
            else Pausar();
        }
    }

    public void Reanudar()
    {
        panelMenuPausa.SetActive(false);
        Time.timeScale = 1f;
        estaPausado = false;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (jugadorController != null) jugadorController.enabled = true;
    }

    public void Pausar()
    {
        panelMenuPausa.SetActive(true);
        Time.timeScale = 0f;
        estaPausado = true;
        
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (jugadorController != null) jugadorController.enabled = false;
    }

    public void ReiniciarPartida()
    {
        Reanudar();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void AjustarVolumenMusica(float volumen)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat("Musica", Mathf.Log10(volumen) * 20);
        }
    }

    public void AjustarVolumenEfectos(float volumen)
    {
        if (audioMixer != null)
        {
            audioMixer.SetFloat("Efectos", Mathf.Log10(volumen) * 20);
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