using Arduino;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Sources:
/// https://stackoverflow.com/questions/69653914/clamp-a-quaternion-rotation-in-unity
/// </summary>


public class HorseMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private ArduinoInput arduino;

    [Header("Rotation")]
    public float xRotationClamp = 30f;
    public float zRotationClamp = 15f;

    public float sensitivity;

    [Header("Speed")]
    public float shakeSpeed = 0f;
    private float previousXRotation = 0f; 
    private float lastUpdateTime = 0f;

    private void Update()
    {
        if (arduino.calibratePressed) { ResetRotation(); }
        if (arduino.gyroscopeRotation != null) 
        {
            CalculateSpeed();
            Hobbeling();
        }
    }

    private void CalculateSpeed()
    {
        float currentTime = Time.time;
        float deltaTime = currentTime - lastUpdateTime;

        float currentXRotation = NormalizeAngle(arduino.gyroscopeRotation.eulerAngles.x);

        if (deltaTime > 0f)
        {
            float deltaX = Mathf.Abs(currentXRotation - previousXRotation);

            if (deltaX > 180f)
            {
                deltaX = 360f - deltaX;
            }

            shakeSpeed = deltaX / deltaTime;
        }

        previousXRotation = currentXRotation;
        lastUpdateTime = currentTime;

        Vector3 moveVector = new Vector3(0, 0, shakeSpeed * Time.deltaTime / 100f);
        transform.position = transform.position += moveVector;
    }

    private void Move()
    {

    }

    private void Hobbeling()
    {
        Vector3 gyroEuler = arduino.gyroscopeRotation.eulerAngles;

        gyroEuler.x = NormalizeAngle(gyroEuler.x);
        gyroEuler.z = NormalizeAngle(gyroEuler.z);

        float clampedX = Mathf.Clamp(gyroEuler.x, -xRotationClamp, xRotationClamp);
        float clampedZ = Mathf.Clamp(gyroEuler.z, -zRotationClamp, zRotationClamp);

        Quaternion targetRotation = Quaternion.Euler(clampedX, 0, clampedZ);

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * sensitivity); 
    }

    // Normalizes angles to [-180, 180]
    private float NormalizeAngle(float angle)
    {
        if (angle > 180f) angle -= 360f;
        if (angle < -180f) angle += 360f;
        return angle;
    }

    private void ResetRotation()
    {
        Debug.Log("Button pressed: Resetting rotation and recalibrating gyroscope.");
        // Reset the object's rotation
        transform.rotation = Quaternion.identity;
    }
}
