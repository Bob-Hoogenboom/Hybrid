using UnityEngine;
using UnityEngine.Splines;

public class SplineMoveTest : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1f;
    [SerializeField]
    private float maxOffset = 1f;

    [SerializeField]
    private AnimationCurve curver;

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

        //calculates and sets the transform offset from the center of the track
        Vector3 posOffset = transform.right * trackOffset;
        transform.localPosition = trackPosition + posOffset;

        //creates a forward and back transform
        Pose forwardTransform = new Pose(splineTrack.EvaluatePosition(trackProgress + 0.05f), UpdateRotation(trackProgress + 0.05f));
        Pose backTransform = new Pose(splineTrack.EvaluatePosition(trackProgress - 0.05f), UpdateRotation(trackProgress - 0.05f));

        //calculates whether the left or right side of the track is the shortest corner
        float leftDist = Vector3.Distance(forwardTransform.position - forwardTransform.right, backTransform.position - backTransform.right);
        float rightDist = Vector3.Distance(forwardTransform.position + forwardTransform.right, backTransform.position + backTransform.right);
        string shortestCorner = leftDist < rightDist ? "left" : "right";

        //checks if the player is in the shortest corner
        bool isInInnerCorner = false;
        if (trackOffset < 0 && shortestCorner == "left" || trackOffset > 0 && shortestCorner == "right")
        {
            isInInnerCorner = true;
        }

        //alignment is used to get the intensity of the curve
        float curveIntensity = Vector3.Dot(forwardTransform.right, backTransform.right);
        curveIntensity = Mathf.Abs(curveIntensity - 1);

        float remappedOffset = Utility.Remap(trackOffset, -0.5f, 0.5f, -1, 1);
        float baseMultiplier = curver.Evaluate(curveIntensity * Mathf.Abs(remappedOffset));

        baseMultiplier = isInInnerCorner ? baseMultiplier : -baseMultiplier;

        var minCvalue = curver.keys[0].value;
        var maxCvalue = curver.keys[curver.length - 1].value;

        float THEMULTIPLIER = Utility.Remap(baseMultiplier, -maxCvalue, maxCvalue, minCvalue, maxCvalue);

        //adds progress on the track with a multiplier and resets to zero at start position
        trackProgress += (moveSpeed * THEMULTIPLIER) * Time.deltaTime / trackLength;
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
