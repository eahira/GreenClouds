using UnityEngine;
using TMPro;

public class FloatingDamageText : MonoBehaviour
{
    private static FloatingDamageText prefab;

    [SerializeField] private TextMeshPro text;
    [SerializeField] private float lifeTime = 0.7f;
    [SerializeField] private float moveSpeed = 1.5f;

    /// <summary>
    /// Спавн текста урона в мире.
    /// </summary>
    public static void Spawn(Vector3 worldPos, int damage)
    {
        // грузим префаб из Resources/FloatingDamageText один раз
        if (prefab == null)
        {
            prefab = Resources.Load<FloatingDamageText>("FloatingDamageText");
            if (prefab == null)
            {
                Debug.LogWarning("FloatingDamageText prefab not found in Resources/FloatingDamageText");
                return;
            }
        }

        FloatingDamageText instance = Instantiate(prefab, worldPos, Quaternion.identity);
        instance.Setup(damage);
    }

    public void Setup(int damage)
    {
        if (text == null)
            text = GetComponent<TextMeshPro>();

        text.text = "-" + damage.ToString();
    }

    private void Update()
    {
        // поднимаем текст вверх
        transform.position += Vector3.up * moveSpeed * Time.deltaTime;

        lifeTime -= Time.deltaTime;
        if (lifeTime <= 0f)
            Destroy(gameObject);
    }
}
