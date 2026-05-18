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


    }
}
