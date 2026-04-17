using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Character/PlayerData")]
public class PlayerData : Character
{
    public int[,] inventory;

    public int[] weaponSlot = new int[2];

    public GameObject atkCol;

    public Animator animator;


}
