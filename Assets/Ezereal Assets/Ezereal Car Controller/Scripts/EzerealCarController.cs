using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using UnityEngine.UI;

namespace Ezereal
{
    public class EzerealCarController : MonoBehaviour // This is the main system resposible for car control.
    {
        [Header("Ezereal References")]

        [SerializeField] EzerealLightController ezerealLightController;
        [SerializeField] EzerealSoundController ezerealSoundController;
        [SerializeField] EzerealWheelFrictionController ezerealWheelFrictionController;

        [Header("References")]

        public Rigidbody vehicleRB;
        public WheelCollider frontLeftWheelCollider;
        public WheelCollider frontRightWheelCollider;
        public WheelCollider rearLeftWheelCollider;
        public WheelCollider rearRightWheelCollider;
        WheelCollider[] wheels;

        [SerializeField] Transform frontLeftWheelMesh;
        [SerializeField] Transform frontRightWheelMesh;
        [SerializeField] Transform rearLeftWheelMesh;
        [SerializeField] Transform rearRightWheelMesh;

        [SerializeField] Transform steeringWheel;

        [SerializeField] TMP_Text currentGearTMP_UI;
        [SerializeField] TMP_Text currentGearTMP_Dashboard;

        [SerializeField] TMP_Text currentSpeedTMP_UI;
        [SerializeField] TMP_Text currentSpeedTMP_Dashboard;
        [SerializeField] Slider accelerationSlider;

        [Header("Settings")]
        public bool isStarted = true;

        public float maxForwardSpeed = 100f; // 100f default
        public float maxReverseSpeed = 30f; // 30f default
        public float horsePower = 1000f; // 100f0 default
        public float brakePower = 2000f; // 2000f default
        public float handbrakeForce = 3000f; // 3000f default
        public float maxSteerAngle = 30f; // 30f default
        public float steeringSpeed = 5f; // 0.5f default
        public float stopThreshold = 1f; // 1f default. At what speed car will make a full stop
        public float decelerationSpeed = 0.5f; // 0.5f default
        public float maxSteeringWheelRotation = 360f; // 360 for real steering wheel. 120 would be more suitable for racing.

        [Header("Drive Type")]
        public DriveTypes driveType = DriveTypes.RWD;

        [Header("Gearbox")]
        public AutomaticGears currentGear = AutomaticGears.Drive;

        [Header("Debug Info")]
        public bool stationary = true;
        [SerializeField] float currentSpeed = 0f;
        [SerializeField] float currentAccelerationValue = 0f;
        [SerializeField] float currentBrakeValue = 0f;
        [SerializeField] float currentHandbrakeValue = 0f;
        [SerializeField] float currentSteerAngle = 0f;
        [SerializeField] float targetSteerAngle = 0f;
        [SerializeField] float FrontLeftWheelRPM = 0f;
        [SerializeField] float FrontRightWheelRPM = 0f;
        [SerializeField] float RearLeftWheelRPM = 0f;
        [SerializeField] float RearRightWheelRPM = 0f;

        [SerializeField] float speedFactor = 0f; // Leave at zero. Responsible for smooth acceleration and near-top-speed slowdown.

        // Variables de respaldo para procesar la entrada WASD de forma nativa e independiente
        private float rawInputAcceleration = 0f;
        private float rawInputBrake = 0f;

        private void Awake()
        {
            wheels = new WheelCollider[]
            {
            frontLeftWheelCollider,
            frontRightWheelCollider,
            rearLeftWheelCollider,
            rearRightWheelCollider,
            };

            if (ezerealLightController == null)
            {
                Debug.LogWarning("EzerealLightController reference is missing. Ignore or attach one if you want to have light controls.");
            }

            if (ezerealSoundController == null)
            {
                Debug.LogWarning("EzerealSoundController reference is missing. Ignore or attach one if you want to have engine sounds.");
            }

            if (ezerealWheelFrictionController == null)
            {
                Debug.LogWarning("EzerealWheelFrictionController reference is missing. Ignore or attach one if you want to have friction controls.");
            }

            if (vehicleRB == null)
            {
                Debug.LogError("VehicleRB reference is missing for EzerealCarController!");
            }

            if (isStarted)
            {
                Debug.Log("Car is started.");

                if (ezerealLightController != null)
                {
                    ezerealLightController.MiscLightsOn();
                }

                if (ezerealSoundController != null)
                {
                    ezerealSoundController.TurnOnEngineSound();
                }
            }
        }

        void OnStartCar()
        {
            isStarted = !isStarted;

            if (isStarted)
            {
                Debug.Log("Car started.");

                if (ezerealLightController != null)
                {
                    ezerealLightController.MiscLightsOn();
                }

                if (ezerealSoundController != null)
                {
                    ezerealSoundController.TurnOnEngineSound();
                }

            }
            else if (!isStarted)
            {
                Debug.Log("Car turned off");

                if (ezerealLightController != null)
                {
                    ezerealLightController.AllLightsOff();
                }

                if (ezerealSoundController != null)
                {
                    ezerealSoundController.TurnOffEngineSound();
                }

                frontLeftWheelCollider.motorTorque = 0;
                frontRightWheelCollider.motorTorque = 0;
                rearLeftWheelCollider.motorTorque = 0;
                rearRightWheelCollider.motorTorque = 0;
            }
        }

        void OnAccelerate(InputValue accelerationValue)
        {
            // Guardamos la presión física de la tecla 'W' (Acelerador estándar)
            rawInputAcceleration = accelerationValue.Get<float>();
        }

        void Acceleration()
        {
            if (isStarted)
            {
                if (currentGear == AutomaticGears.Drive)
                {
                    // Calculate how close the car is to top speed
                    // as a number from zero to one
                    speedFactor = Mathf.InverseLerp(0, maxForwardSpeed, currentSpeed);

                    // Use that to calculate how much torque is available 
                    // (zero torque at top speed)
                    float currentMotorTorque = Mathf.Lerp(horsePower, 0, speedFactor);

                    if (currentAccelerationValue > 0f && currentSpeed < maxForwardSpeed)
                    {
                        if (driveType == DriveTypes.RWD)
                        {
                            rearLeftWheelCollider.motorTorque = currentMotorTorque * currentAccelerationValue;
                            rearRightWheelCollider.motorTorque = currentMotorTorque * currentAccelerationValue;
                        }
                        else if (driveType == DriveTypes.FWD)
                        {
                            frontLeftWheelCollider.motorTorque = currentMotorTorque * currentAccelerationValue;
                            frontRightWheelCollider.motorTorque = currentMotorTorque * currentAccelerationValue;
                        }
                        else if (driveType == DriveTypes.AWD)
                        {
                            frontLeftWheelCollider.motorTorque = currentMotorTorque * currentAccelerationValue;
                            frontRightWheelCollider.motorTorque = currentMotorTorque * currentAccelerationValue;
                            rearLeftWheelCollider.motorTorque = currentMotorTorque * currentAccelerationValue;
                            rearRightWheelCollider.motorTorque = currentMotorTorque * currentAccelerationValue;
                        }
                    }
                    else
                    {
                        frontLeftWheelCollider.motorTorque = 0;
                        frontRightWheelCollider.motorTorque = 0;
                        rearLeftWheelCollider.motorTorque = 0;
                        rearRightWheelCollider.motorTorque = 0;
                    }
                }

                if (currentGear == AutomaticGears.Reverse)
                {
                    if (currentAccelerationValue > 0f && currentSpeed > -maxReverseSpeed)
                    {
                        // Mantenemos la inversión nativa que calcula el asset para sus marchas traseras
                        float internalAcceleration = 1f * currentAccelerationValue; 

                        if (driveType == DriveTypes.RWD)
                        {
                            rearLeftWheelCollider.motorTorque = -internalAcceleration * horsePower;
                            rearRightWheelCollider.motorTorque = -internalAcceleration * horsePower;
                        }
                        else if (driveType == DriveTypes.FWD)
                        {
                            frontLeftWheelCollider.motorTorque = -internalAcceleration * horsePower;
                            frontRightWheelCollider.motorTorque = -internalAcceleration * horsePower;
                        }
                        else if (driveType == DriveTypes.AWD)
                        {
                            frontLeftWheelCollider.motorTorque = -internalAcceleration * horsePower;
                            frontRightWheelCollider.motorTorque = -internalAcceleration * horsePower;
                            rearLeftWheelCollider.motorTorque = -internalAcceleration * horsePower;
                            rearRightWheelCollider.motorTorque = -internalAcceleration * horsePower;
                        }

                    }
                    else
                    {
                        frontLeftWheelCollider.motorTorque = 0;
                        frontRightWheelCollider.motorTorque = 0;
                        rearLeftWheelCollider.motorTorque = 0;
                        rearRightWheelCollider.motorTorque = 0;
                    }
                }

                UpdateAccelerationSlider();
            }
        }

        void OnBrake(InputValue brakeValue)
        {
            // Guardamos la presión física de la tecla 'S' (Freno estándar)
            rawInputBrake = brakeValue.Get<float>();
        }

        void Braking()
        {
            if (currentBrakeValue > 0f)
            {
                frontLeftWheelCollider.brakeTorque = currentBrakeValue * brakePower;
                frontRightWheelCollider.brakeTorque = currentBrakeValue * brakePower;
            }
            else
            {
                frontLeftWheelCollider.brakeTorque = 0;
                frontRightWheelCollider.brakeTorque = 0;
            }
        }

        void OnHandbrake(InputValue handbrakeValue)
        {
            currentHandbrakeValue = handbrakeValue.Get<float>();

            if (isStarted)
            {
                if (currentHandbrakeValue > 0)
                {
                    if (ezerealWheelFrictionController != null)
                    {
                        ezerealWheelFrictionController.StartDrifting(currentHandbrakeValue);
                    }

                    if (ezerealLightController != null)
                    {
                        ezerealLightController.HandbrakeLightOn();
                    }
                }
                else
                {
                    if (ezerealWheelFrictionController != null)
                    {
                        ezerealWheelFrictionController.StopDrifting();
                    }

                    if (ezerealLightController != null)
                    {
                        ezerealLightController.HandbrakeLightOff();
                    }
                }
            }
        }

        void Handbraking()
        {
            if (currentHandbrakeValue > 0f)
            {
                rearLeftWheelCollider.motorTorque = 0;
                rearRightWheelCollider.motorTorque = 0;
                rearLeftWheelCollider.brakeTorque = currentHandbrakeValue * handbrakeForce;
                rearRightWheelCollider.brakeTorque = currentHandbrakeValue * handbrakeForce;
            }
            else
            {
                rearLeftWheelCollider.brakeTorque = 0;
                rearRightWheelCollider.brakeTorque = 0;
            }
        }

        void OnSteer(InputValue turnValue)
        {
            targetSteerAngle = turnValue.Get<float>() * maxSteerAngle;
        }

        void Steering()
        {
            float adjustedspeedFactor = Mathf.InverseLerp(20, maxForwardSpeed, currentSpeed); //minimum speed affecting steerAngle is 20
            float adjustedTurnAngle = targetSteerAngle * (1 - adjustedspeedFactor); //based on current speed.
            currentSteerAngle = Mathf.Lerp(currentSteerAngle, adjustedTurnAngle, Time.deltaTime * steeringSpeed);

            frontLeftWheelCollider.steerAngle = currentSteerAngle;
            frontRightWheelCollider.steerAngle = currentSteerAngle;

            UpdateWheel(frontLeftWheelCollider, frontLeftWheelMesh);
            UpdateWheel(frontRightWheelCollider, frontRightWheelMesh);
            UpdateWheel(rearLeftWheelCollider, rearLeftWheelMesh);
            UpdateWheel(rearRightWheelCollider, rearRightWheelMesh);
        }

        void Slowdown()
        {
            if (vehicleRB != null)
            {
                if (currentAccelerationValue == 0 && currentBrakeValue == 0 && currentHandbrakeValue == 0)
                {
#if UNITY_6000_0_OR_NEWER
                    vehicleRB.linearVelocity = Vector3.Lerp(vehicleRB.linearVelocity, Vector3.zero, Time.deltaTime * decelerationSpeed);
#else
                    vehicleRB.velocity = Vector3.Lerp(vehicleRB.velocity, Vector3.zero, Time.deltaTime * decelerationSpeed);
#endif
                }
            }
        }

        // Dejamos las funciones manuales intactas para evitar errores de compilación con las llamadas de Unity Action Maps
        void OnDownShift() {}
        void OnUpShift() {}

        private void FixedUpdate()
        {
            // ==========================================
            // 🔥 ADAPTADOR DE TRANSMISIÓN AUTOMÁTICA ARCADE (WASD) 🔥
            // ==========================================
            if (currentGear == AutomaticGears.Drive)
            {
                // Comportamiento normal: W acelera, S frena
                currentAccelerationValue = rawInputAcceleration;
                currentBrakeValue = rawInputBrake;

                // Si el camión se detiene completamente y el jugador mantiene pulsado S (Atrás)
                if (stationary && rawInputBrake > 0.1f && rawInputAcceleration == 0f)
                {
                    currentGear = AutomaticGears.Reverse;
                    UpdateGearText("R");
                    if (isStarted && ezerealLightController != null)
                    {
                        ezerealLightController.ReverseLightsOn();
                    }
                }
            }
            else if (currentGear == AutomaticGears.Reverse)
            {
                // Comportamiento invertido: S manda potencia al motor de marcha atrás, W activa el freno mecánico
                currentAccelerationValue = rawInputBrake;
                currentBrakeValue = rawInputAcceleration;

                // Si el camión retrocediendo llega a detenerse del todo y el jugador pulsa W (Adelante)
                if (stationary && rawInputAcceleration > 0.1f && rawInputBrake == 0f)
                {
                    currentGear = AutomaticGears.Drive;
                    UpdateGearText("D");
                    if (isStarted && ezerealLightController != null)
                    {
                        ezerealLightController.ReverseLightsOff();
                    }
                }
            }
            else
            {
                // Si por algún motivo está en Neutral (N), reconducimos la tracción al pulsar W
                if (rawInputAcceleration > 0.1f)
                {
                    currentGear = AutomaticGears.Drive;
                    UpdateGearText("D");
                }
            }

            // Gestión unificada y contextual de las luces de freno traseras
            if (isStarted && ezerealLightController != null)
            {
                if (currentBrakeValue > 0f) ezerealLightController.BrakeLightsOn();
                else ezerealLightController.BrakeLightsOff();
            }
            // ==========================================

            Acceleration();

            Braking();

            Handbraking();

            Steering();

            Slowdown();

            RotateSteeringWheel();

            if
                (
                    Mathf.Abs(frontLeftWheelCollider.rpm) < stopThreshold &&
                    Mathf.Abs(frontRightWheelCollider.rpm) < stopThreshold &&
                    Mathf.Abs(rearLeftWheelCollider.rpm) < stopThreshold &&
                    Mathf.Abs(rearRightWheelCollider.rpm) < stopThreshold
                )
            {
                stationary = true;
            }
            else
            {
                stationary = false;
            }

            if (vehicleRB != null) // Unity uses m/s as for default. So I convert from m/s to km/h. For mph use 2.23694f instead of 3.6f.
            {
#if UNITY_6000_0_OR_NEWER
                currentSpeed = Vector3.Dot(vehicleRB.gameObject.transform.forward, vehicleRB.linearVelocity);
                currentSpeed *= 3.6f;
                UpdateSpeedText(currentSpeed);
#else
                currentSpeed = Vector3.Dot(vehicleRB.gameObject.transform.forward, vehicleRB.velocity);
                currentSpeed *= 3.6f; 
                UpdateSpeedText(currentSpeed);
#endif
            }

            FrontLeftWheelRPM = frontLeftWheelCollider.rpm;
            FrontRightWheelRPM = frontRightWheelCollider.rpm;
            RearLeftWheelRPM = rearLeftWheelCollider.rpm;
            RearRightWheelRPM = rearRightWheelCollider.rpm;
        }

        private void UpdateWheel(WheelCollider col, Transform mesh)
        {
            col.GetWorldPose(out Vector3 position, out Quaternion rotation);
            mesh.SetPositionAndRotation(position, rotation);
        }

        void RotateSteeringWheel()
        {
            if (steeringWheel == null) return;
            float currentXAngle = steeringWheel.transform.localEulerAngles.x; // Maximum steer angle in degrees

            // Calculate the rotation based on the steer angle
            float normalizedSteerAngle = Mathf.Clamp(frontLeftWheelCollider.steerAngle, -maxSteerAngle, maxSteerAngle);
            float rotation = Mathf.Lerp(maxSteeringWheelRotation, -maxSteeringWheelRotation, (normalizedSteerAngle + maxSteerAngle) / (2 * maxSteerAngle));

            // Set the local rotation of the steering wheel
            steeringWheel.localRotation = Quaternion.Euler(currentXAngle, 0, rotation);
        }

        void UpdateGearText(string gear)
        {
            if (currentGearTMP_UI != null) currentGearTMP_UI.text = gear;
            if (currentGearTMP_Dashboard != null) currentGearTMP_Dashboard.text = gear;
        }

        void UpdateSpeedText(float speed)
        {
            speed = Mathf.Abs(speed);

            if (currentSpeedTMP_UI != null) currentSpeedTMP_UI.text = speed.ToString("F0");
            if (currentSpeedTMP_Dashboard != null) currentSpeedTMP_Dashboard.text = speed.ToString("F0");
        }

        void UpdateAccelerationSlider()
        {
            if (accelerationSlider == null) return;
            if (currentGear == AutomaticGears.Drive || currentGear == AutomaticGears.Reverse)
            {
                accelerationSlider.value = Mathf.Lerp(accelerationSlider.value, currentAccelerationValue, Time.deltaTime * 15f);
            }
            else
            {
                accelerationSlider.value = 0;
            }
        }

        public bool InAir()
        {
            foreach (WheelCollider wheel in wheels)
            {
                if (wheel.GetGroundHit(out _))
                {
                    return false;
                }
            }
            return true;
        }
    }
}