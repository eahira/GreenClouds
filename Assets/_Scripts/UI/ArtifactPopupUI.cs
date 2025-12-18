using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ArtifactPopupUI : MonoBehaviour
{
    [Header("UI refs")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Image iconImage;
    [SerializeField] private Button continueButton;

    private void Awake()
    {
        if (continueButton != null)
            continueButton.onClick.AddListener(Hide);

        gameObject.SetActive(false);
    }

    public void Show(ArtifactData data)
{
    if (data == null) return;

    if (titleText != null)
        titleText.text = "Вы получили новый артефакт";

    if (descriptionText != null)
        descriptionText.text =
            $"<size=120%><b>{data.title}</b></size>\n\n{data.description}";

    if (iconImage != null)
    {
        iconImage.sprite = data.icon;
        iconImage.enabled = (data.icon != null);
    }

    gameObject.SetActive(true);
    Time.timeScale = 0f;
}


    public void Hide()
    {
        Time.timeScale = 1f;
        gameObject.SetActive(false);
    }
}
