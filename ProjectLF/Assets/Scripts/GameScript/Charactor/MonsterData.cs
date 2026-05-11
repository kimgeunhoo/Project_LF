using UnityEngine;

[CreateAssetMenu(menuName = "Character/MonsterData")]
public class MonsterData : Character
{
    [Header("Monster")]
    public string Name { get; set; }

    public GameObject monsterPF;

    public int gold;

}
