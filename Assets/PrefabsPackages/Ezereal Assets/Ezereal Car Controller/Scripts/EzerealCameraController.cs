using UnityEngine;
using UnityEngine.InputSystem;

namespace Ezereal
{
    public class EzerealCameraController : MonoBehaviour
    {
        [Header("Control de Cámara")]
        [Tooltip("Si está marcado, ignora el botón de cambiar cámara y la deja fija.")]
        public bool bloquearCamara = true;

        [Tooltip("La cámara con la que empezará el coche (ponla en 'wheel').")]
        [SerializeField] CameraViews currentCameraView = CameraViews.wheel;

        [SerializeField] private GameObject[] cameras; // Assume cameras are in order: cockpit, close, far, locked, wheel

        private void Awake()
        {
            // Al arrancar, encendemos únicamente la cámara seleccionada (la del volante)
            SetCameraView(currentCameraView);
        }

        void OnSwitchCamera()
        {
            // 🔥 EL ESCUDO: Si el bloqueo está activo, salimos de la función sin hacer nada
            if (bloquearCamara) return;

            // --- LÓGICA ORIGINAL RECUPERADA ---
            currentCameraView = (CameraViews)(((int)currentCameraView + 1) % cameras.Length);
            SetCameraView(currentCameraView);
        }

        private void SetCameraView(CameraViews view)
        {
            // --- LÓGICA ORIGINAL RECUPERADA ---
            for (int i = 0; i < cameras.Length; i++)
            {
                if (cameras[i] != null) // Pequeña protección de seguridad
                {
                    cameras[i].SetActive(i == (int)view);
                }
            }
        }
    }
}