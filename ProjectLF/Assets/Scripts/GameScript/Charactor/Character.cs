using UnityEngine;


[CreateAssetMenu(menuName = "Character/Data")]
public class Character : ScriptableObject
{
    [SerializeField]
    protected int hp;
    [SerializeField] 
    protected int maxHp;
    [SerializeField]
    protected int mana;
    [SerializeField]
    protected int atk;
    [SerializeField]
    protected int def;
    [SerializeField]
    protected int speed;
    [SerializeField]
    protected int dashGage;

    public int Hp { get { return hp; } }
    public int MaxHp { get { return maxHp; } }
    public int Mana {  get { return mana; } }
    public int Atk { get { return atk; } }
    public int Def {  get { return def; } }
    public int Speed {  get { return speed; } }

    public int DashGage {  get { return dashGage; } }


}
