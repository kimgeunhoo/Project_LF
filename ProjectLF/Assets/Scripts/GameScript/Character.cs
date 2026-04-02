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

    public int Hp { get { return hp; } }
    public int Mana {  get { return mana; } }
    public int Atk { get { return atk; } }
    public int Def {  get { return def; } }
    public int Speed {  get { return speed; } }

    public int DashGage {  get { return dashGage; } }


}
