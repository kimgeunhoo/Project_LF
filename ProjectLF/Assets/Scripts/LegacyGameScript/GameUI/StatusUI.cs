using TMPro;
using UnityEngine;

public class StatusUI : MonoBehaviour
{
    [SerializeField]
    private GameObject hpPanel;

    [SerializeField]
    private TextMeshProUGUI hpText;

    [SerializeField]
    private Player player;

    private int currentHp;

    private void Awake()
    {
        //player = GameManager.Instance.Player;

        //currentHp = player.Hp;
        //hpText.text = ($"Hp : {player.Hp}/{currentHp}");
    }



}
