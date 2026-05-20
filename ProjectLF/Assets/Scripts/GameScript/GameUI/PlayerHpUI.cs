using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpUI : MonoBehaviour
{
    [SerializeField]
    private Image hpFillImage;
    [SerializeField]
    private TextMeshProUGUI hpText;

    public void SetHp(int currentHp, int maxHp)
    {
        float ratio = 0f;

        if(maxHp > 0)
            ratio = (float) currentHp / maxHp;

        ratio = Mathf.Clamp01(ratio);

        if (hpFillImage != null)
            hpFillImage.fillAmount = ratio;

        if (hpText != null)
            hpText.text = $"{currentHp} / {maxHp}";

    }
}
