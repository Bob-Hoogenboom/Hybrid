using UnityEngine;
using UnityEngine.Splines;

public class SplineMoveTest : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1f;
    [SerializeField]
    private float maxOffset = 1f;

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

        Pose nextTransform = new Pose();
        nextTransform.position = splineTrack.EvaluatePosition(trackProgress + Time.deltaTime);
        nextTransform.rotation = UpdateRotation(trackProgress + Time.deltaTime);

        //FIX multiplier applying on straight sections

        float dist = Vector3.Distance(trackPosition + posOffset, nextTransform.position + nextTransform.right * trackOffset);
        float outerDist = Vector3.Distance(trackPosition - posOffset, nextTransform.position - nextTransform.right * trackOffset);
        bool isInsideCorner = dist < outerDist;
        float cornerMultiplier = isInsideCorner ? 2 * Mathf.Abs(trackOffset) : 0.5f * Mathf.Abs(trackOffset);

        //adds progress on the track and reset to zero at start position
        trackProgress += (moveSpeed + (moveSpeed * cornerMultiplier)) * Time.deltaTime / trackLength;
        lapTime += Time.deltaTime;
        if (trackProgress > 1f)
        {
            trackProgress = 0f;
            Debug.Log(lapTime);
            lapTime = 0f;
        }
    }

    /// <summary>
    /// Gets correct rotation on the spline given the progress
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
