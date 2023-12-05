using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeItemData", menuName = "Scriptable Object/UpgradeItemData")]
public class UpgradeGameItemData : ScriptableObject
{
    public int id;
    public string name;
    public Sprite sprite;
    public int count;
}
