using UnityEngine;

[CreateAssetMenu(menuName = "Character/PlayerData")]
public class PlayerData : Character
{
    [SerializeField]
    private int dashGage;

    public int DashGage => dashGage;
}
