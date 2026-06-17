using UnityEngine;

public class RotateBehavior : MonoBehaviour
{
    [SerializeField] private Vector3 rotationSpeed = new Vector3(0f, 30f, 0f);

    public Vector3 RotationSpeed
    {
        get => rotationSpeed;
        set => rotationSpeed = value;
    }

    private void Update()
    {
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}