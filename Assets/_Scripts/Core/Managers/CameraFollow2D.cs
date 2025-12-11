using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Кого следим")]
    public Transform target;          // сюда перетянем Player

    [Header("Настройки движения")]
    public float smoothSpeed = 5f;    // чем больше, тем быстрее камера догоняет
    public Vector3 offset = new Vector3(0f, 0f, -10f); // смещение от игрока

    private void LateUpdate()
    {
        if (target == null) return;

        // желаемая позиция = позиция игрока + смещение
        Vector3 desiredPosition = target.position + offset;

        // на всякий случай фиксируем z-координату
        desiredPosition.z = offset.z;

        // плавно двигаем камеру
        transform.position = Vector3.Lerp(
            transform.position,
            desiredPosition,
            smoothSpeed * Time.deltaTime
        );
    }
}
