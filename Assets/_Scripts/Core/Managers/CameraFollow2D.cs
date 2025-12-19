using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Кого следим")]
    public Transform target;

    [Header("Настройки движения")]
    public float smoothSpeed = 5f;
    public Vector3 offset = new Vector3(0f, 0f, -10f);

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPosition = target.position + offset;

        desiredPosition.z = offset.z;

        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}
