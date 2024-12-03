using System;
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

        float nextTrackPosition = (splineTrack.GetLength() * 2f);
        Pose nextTransform = new Pose();
        nextTransform.position = splineTrack.EvaluatePosition(trackProgress + nextTrackPosition);
        nextTransform.rotation = UpdateRotation(trackProgress + nextTrackPosition);

        //checks if player is in inner or outer corner and calculates modifier
        float dist = Vector3.Distance(trackPosition + posOffset, nextTransform.position + nextTransform.right * trackOffset);
        float outerDist = Vector3.Distance(trackPosition - posOffset, nextTransform.position - nextTransform.right * trackOffset);
        bool isInsideCorner = dist < outerDist;
        float cornerMultiplier = isInsideCorner ? cornerMultiplierBoost * Mathf.Abs(trackOffset) : (1/cornerMultiplierBoost) * Mathf.Abs(trackOffset);

        //gets the dot product of the corner to check how strong a multiplier should apply
        Vector3 vec1 = splineTrack.EvaluateTangent(trackProgress);
        Vector3 vec2 = splineTrack.EvaluateTangent(trackProgress + (1f / nextTrackPosition));
        var dot = Vector3.Dot(vec1.normalized, vec2.normalized);
        float curveEffect = 1 + (cornerMultiplier * ((1 - dot) * 100));

        //adds progress on the track with a multiplier and resets to zero at start position
        trackProgress += (moveSpeed * curveEffect) * Time.deltaTime / trackLength;
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
