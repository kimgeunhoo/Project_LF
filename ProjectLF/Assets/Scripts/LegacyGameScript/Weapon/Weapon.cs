using UnityEngine;

[System.Serializable]
public class Weapon : ScriptableObject
{
    public string weaponName;
    public Sprite weaponIcon;
    public GameObject weaponPrefab;
    public float damage;
    public float range;
    public float fireRate;
    public float firespeed;
}
