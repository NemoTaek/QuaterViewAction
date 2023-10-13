using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Weapon", menuName = "Scriptable Object/WeaponData")]
public class WeaponData : ScriptableObject
{
    public enum WeaponType { Sword, Staff, Dagger, Gun }

    [Header("----- Main Info -----")]
    public WeaponType weaponType;
    public int weaponId;
    public string weaponName;
    public string weaponDesc;
    public Sprite weaponIcon;
    public bool isPenetrate;
    public bool isSlow;

    [Header("----- Level Data -----")]
    public float baseDamage;
    public float[] upgradeDamage;
    public int level;
    public int maxLevel;
}
