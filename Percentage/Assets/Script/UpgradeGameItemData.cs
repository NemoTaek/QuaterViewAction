using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UpgradeItemData", menuName = "Scriptable Object/UpgradeItemData")]
public class UpgradeGameItemData : ScriptableObject
{
    public int id;
    public string itemName;
    public Sprite sprite;
    public int count;
}
