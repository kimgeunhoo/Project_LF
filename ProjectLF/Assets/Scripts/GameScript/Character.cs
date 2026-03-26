using UnityEngine;


[CreateAssetMenu(menuName = "Character/Data")]
public class Character : ScriptableObject
{
    [SerializeField]
    private int hp;
    [SerializeField]
    private int mana;
    [SerializeField]
    private int atk;
    [SerializeField]
    private int def;
    [SerializeField]
    private int speed;
    [SerializeField]
    private int dashGage;

    public int Speed {  get { return speed; } }

}
