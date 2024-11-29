using UnityEngine;
using UnityEngine.Splines;

public class SplineMoveTest : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1f;

    [SerializeField]
    private SplineContainer splineContainer;

    private Spline splineTrack;
    private float trackLength;

    private float timer = 0f;

    private float trackOffset;

    private void Start()
    {
        splineTrack = splineContainer.Spline;
        trackLength = splineTrack.GetLength();
    }

    void Update()
    {
        timer += moveSpeed * Time.deltaTime / trackLength;
        if (timer > 1f) timer = 0f;

        Vector3 trackPosition = splineTrack.EvaluatePosition(timer);

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            trackOffset -= Time.deltaTime * moveSpeed;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            trackOffset += Time.deltaTime * moveSpeed;
        }

        UpdateRotation();

        transform.position = trackPosition;
        Vector3 posOffset = transform.right * trackOffset;
        transform.localPosition += posOffset;
    }

    private void UpdateRotation()
    {
        Vector3 forward = Vector3.forward;
        Vector3 up = Vector3.up;

        forward = splineTrack.EvaluateTangent(timer);
        if (Vector3.Magnitude(forward) <= Mathf.Epsilon)
        {
            if (timer < 1f)
                forward = splineTrack.EvaluateTangent(Mathf.Min(1f, timer + 0.01f));
            else
                forward = splineTrack.EvaluateTangent(timer - 0.01f);
        }
        forward.Normalize();
        up = splineTrack.EvaluateUpVector(timer);

        transform.rotation = Quaternion.LookRotation(forward, up);
    }
}
