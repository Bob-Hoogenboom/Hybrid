using UnityEngine;

public class SpriteLookAtCam : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    private void FixedUpdate()
    {
        if (!spriteRenderer.enabled) return;
        Vector3 cameraDirection = transform.position - Camera.main.transform.position;
        transform.rotation = Quaternion.LookRotation(cameraDirection);
    }
}
