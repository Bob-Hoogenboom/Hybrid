using UnityEngine;
using UnityEngine.Splines;

public class SplineMoveTest : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1f;
    [SerializeField]
    private float maxOffset = 1f;

    [SerializeField]
    private float cornerMultiplierBoost = 2f;

    [SerializeField]
    private SplineContainer splineContainer;

    private Spline splineTrack;
    private float trackLength;

    private float trackProgress = 0f;

    private float trackOffset;

    private float lapTime = 0f;

    private void Start()
    {
        splineTrack = splineContainer.Spline;
        trackLength = splineTrack.GetLength();
    }

    void Update()
    {
        Vector3 trackPosition = splineTrack.EvaluatePosition(trackProgress);

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            trackOffset -= Time.deltaTime * moveSpeed;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            trackOffset += Time.deltaTime * moveSpeed;
        }
        trackOffset = Mathf.Clamp(trackOffset, -maxOffset, maxOffset);

        transform.rotation = UpdateRotation(trackProgress);

        Vector3 posOffset = transform.right * trackOffset;
        transform.localPosition = trackPosition + posOffset;

        //creates a forward and back transform
        Pose forwardTransform = new Pose(splineTrack.EvaluatePosition(trackProgress + 0.05f), UpdateRotation(trackProgress + 0.05f));
        Pose backTransform = new Pose(splineTrack.EvaluatePosition(trackProgress - 0.05f), UpdateRotation(trackProgress - 0.05f));

        float leftDist = Vector3.Distance(forwardTransform.position - forwardTransform.right, backTransform.position - backTransform.right);
        float rightDist = Vector3.Distance(forwardTransform.position + forwardTransform.right, backTransform.position + backTransform.right);

        string shortestCorner = leftDist < rightDist ? "left" : "right";

        bool isInInnerCorner = false;
        float shortCornerDistance = Mathf.Min(leftDist, rightDist);
        float longCornerDistance = Mathf.Max(leftDist, rightDist);
        if (trackOffset < 0 && shortestCorner == "left" || trackOffset > 0 && shortestCorner == "right")
        {
            isInInnerCorner = true;
        }

        float multiplier = isInInnerCorner ? cornerMultiplierBoost * Mathf.Abs(trackOffset) : (1/cornerMultiplierBoost) * Mathf.Abs(trackOffset);
        float curvyness = longCornerDistance - shortCornerDistance;

        curvyness = isInInnerCorner ? longCornerDistance - shortCornerDistance : shortCornerDistance - longCornerDistance;

        multiplier *= curvyness;

        //adds progress on the track with a multiplier and resets to zero at start position
        trackProgress += (moveSpeed * (1+multiplier)) * Time.deltaTime / trackLength;
        lapTime += Time.deltaTime;
        if (trackProgress > 1f)
        {
            trackProgress = 0f;
            Debug.Log(lapTime);
            lapTime = 0f;
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

        forward = splineTrack.EvaluateTangent(trackProgress);
        if (Vector3.Magnitude(forward) <= Mathf.Epsilon)
        {
            if (trackProgress < 1f)
                forward = splineTrack.EvaluateTangent(Mathf.Min(1f, trackProgress + 0.01f));
            else
                forward = splineTrack.EvaluateTangent(trackProgress - 0.01f);
        }
        forward.Normalize();
        up = splineTrack.EvaluateUpVector(trackProgress);

        return Quaternion.LookRotation(forward, up);
    }
}
