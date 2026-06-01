using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHpUI : MonoBehaviour
{
    [SerializeField]
    private RectTransform hpFillRect;
    [SerializeField] 
    private RectTransform hpBgRect;
    [SerializeField]
    private TextMeshProUGUI hpText;
    [SerializeField]
    private Player player;

    private float maxWidth;

    private void Awake()
    {
        if (player == null)
            player = GetComponentInParent<Player>();
        maxWidth = hpBgRect.sizeDelta.x;
        Debug.Log($"[PlayerHpUI] maxWidth = {maxWidth}, fillWidth = {hpFillRect.rect.width}");
    }


    public void SetHp(int currentHp, int maxHp)
    {
        float bgWidth = hpBgRect.rect.width;
        float hpBarPadding = 12f;
        float ratio = Mathf.Clamp01((float)currentHp / player.MaxHp);
        bgWidth = bgWidth - hpBarPadding;

        if (hpFillRect == null || hpBgRect == null)
        {
            Debug.LogError("[PlayerHpUI] HP Rect ø¨∞·¿Ã æ» µ ");
            return;
        }
        hpFillRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, bgWidth * ratio);

        if (hpText != null)
            hpText.text = $"{currentHp} / {maxHp}";

        Debug.Log($"[PlayerHpUI] current:{currentHp}, max:{maxHp}, ratio:{ratio}, width:{maxWidth * ratio}");

    }
}
