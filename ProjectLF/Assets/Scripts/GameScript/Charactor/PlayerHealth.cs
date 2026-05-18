using TMPro;
using UnityEngine;


public class PlayerHealth : MonoBehaviour
{
    [SerializeField]
    private PlayerData playerData;
    private int maxHp => playerData.Hp;
    [SerializeField]
    private StatusUI hpUI;


    public void SetHp(int currentHp, int maxHp)
    {

    }
}
