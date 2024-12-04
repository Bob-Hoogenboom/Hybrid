using Arduino;
using UnityEngine;
using UnityEngine.Splines;

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

    public float sensitivity = 10f;

    [Header("Speed")]
    [SerializeField] private float speedModifier = 200f; 
    public float shakeSpeed = 0f;
    private float previousXRotation = 0f;

    [SerializeField]
    [Tooltip("The maximum offset from the main line of the spline track")]
    private float maxOffset = 1f;

    [SerializeField]
    [Tooltip("The multiplier applied in corners. Time value should always range from 0 to 1. Where 0 is the negative and 1 is the positive modifier")]
    private AnimationCurve cornerMultiplier;

    [SerializeField]
    private SplineContainer splineContainer;

    private Spline _splineTrack;
    private float _trackLength;
    private float _trackProgress = 0f;
    private float _trackOffset;

    private void Start()
    {
        _splineTrack = splineContainer.Spline;
        _trackLength = _splineTrack.GetLength();
    }

    private void Update()
    {
        if (arduino.calibratePressed) { ResetRotation(); }
        if (arduino.gyroscopeRotation != null) 
        {
            CalculateSpeed();
            Move();
            Hobbeling();
        }
    }

    private void CalculateSpeed()
    {
        float currentXRotation = NormalizeAngle(arduino.gyroscopeRotation.eulerAngles.x);
        float deltaX = Mathf.Abs(currentXRotation - previousXRotation);

        if (deltaX > 180f)
        {
            deltaX = 360f - deltaX;
        }

        shakeSpeed = deltaX / Time.deltaTime /speedModifier;
        previousXRotation = currentXRotation;
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

    private void Move()
    {
        Vector3 trackPosition = _splineTrack.EvaluatePosition(_trackProgress);

        //controls for offsetting the player on the track and clamps between the max
        if (transform.rotation.z < 0)
        {
            _trackOffset += transform.rotation.z * Time.deltaTime * shakeSpeed;
        }
        else if (transform.rotation.z > 0)
        {
            _trackOffset += transform.rotation.z * Time.deltaTime * shakeSpeed;
        }
        _trackOffset = Mathf.Clamp(_trackOffset, -maxOffset, maxOffset);

        transform.rotation = UpdateRotation(_trackProgress);

        //calculates and sets the transform offset from the center of the track
        Vector3 posOffset = transform.right * _trackOffset;
        transform.localPosition = trackPosition + posOffset;

        //creates a forward and back transform
        Pose forwardTransform = new Pose(_splineTrack.EvaluatePosition(_trackProgress + 0.05f), UpdateRotation(_trackProgress + 0.05f));
        Pose backTransform = new Pose(_splineTrack.EvaluatePosition(_trackProgress - 0.05f), UpdateRotation(_trackProgress - 0.05f));

        //calculates whether the left or right side of the track is the shortest corner
        float leftDist = Vector3.Distance(forwardTransform.position - forwardTransform.right, backTransform.position - backTransform.right);
        float rightDist = Vector3.Distance(forwardTransform.position + forwardTransform.right, backTransform.position + backTransform.right);
        string shortestCorner = leftDist < rightDist ? "left" : "right";

        //checks if the player is in the shortest corner
        bool isInInnerCorner = false;
        if (_trackOffset < 0 && shortestCorner == "left" || _trackOffset > 0 && shortestCorner == "right")
        {
            isInInnerCorner = true;
        }

        //gets the intensity of a curve. ranging from 0(not curved) to 1(extremely curved)
        float curveIntensity = Vector3.Dot(forwardTransform.right, backTransform.right);
        curveIntensity = Mathf.Abs(curveIntensity - 1);

        //calculates the base multiplier with the intensity of the curve applied
        float remappedOffset = Utility.Remap(_trackOffset, -maxOffset, maxOffset, -1, 1);
        float baseMultiplier = cornerMultiplier.Evaluate(Mathf.Abs(remappedOffset));
        baseMultiplier = (isInInnerCorner ? baseMultiplier : -baseMultiplier) * curveIntensity;

        //calculates the multiplier based on the cornerMultiplier curve
        var minCurveValue = cornerMultiplier.keys[0].value;
        var maxCurveValue = cornerMultiplier.keys[cornerMultiplier.length - 1].value;
        float THEMULTIPLIER = Utility.Remap(baseMultiplier, -maxCurveValue, maxCurveValue, minCurveValue, maxCurveValue);

        //adds progress on the track with a multiplier and resets to zero at start position
        _trackProgress += (shakeSpeed * THEMULTIPLIER) * Time.deltaTime / _trackLength;
        if (_trackProgress > 1f)
        {
            _trackProgress = 0f;
        }
    }

    /// <summary>
    /// Returns quaternion rotation on the spline given the progress on the track
    /// </summary>
    /// <param name="trackProgress"></param>
    /// <returns></returns>
    private Quaternion UpdateRotation(float trackProgress)
    {
        Vector3 forward = Vector3.forward;
        Vector3 up = Vector3.up;

        forward = _splineTrack.EvaluateTangent(trackProgress);
        if (Vector3.Magnitude(forward) <= Mathf.Epsilon)
        {
            if (trackProgress < 1f)
                forward = _splineTrack.EvaluateTangent(Mathf.Min(1f, trackProgress + 0.01f));
            else
                forward = _splineTrack.EvaluateTangent(trackProgress - 0.01f);
        }
        forward.Normalize();
        up = _splineTrack.EvaluateUpVector(trackProgress);

        return Quaternion.LookRotation(forward, up);
    }

    private void ResetRotation()
    {
        Debug.Log("Button pressed: Resetting rotation and recalibrating gyroscope.");
        // Reset the object's rotation
        transform.rotation = Quaternion.identity;
    }
}
