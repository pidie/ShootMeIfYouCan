using UnityEngine;

public class FloorSensor : MonoBehaviour
{
    public bool IsGrounded { get; private set; }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Floor"))
            IsGrounded = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Floor"))
            IsGrounded = false;
    }
}