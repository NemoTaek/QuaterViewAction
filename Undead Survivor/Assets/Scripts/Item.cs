using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Item : MonoBehaviour
{
    public ItemData itemData;
    public int level;
    public Weapon weapon;
    Text textLevel;
    Text textName;
    Text textDesc;
    Image icon;
    public Gear gear;

    void Awake()
    {
        icon = GetComponentsInChildren<Image>()[1]; // 0번 인덱스는 자기 자신이기 때문에, 1번 인덱스로 가져와야 함
        icon.sprite = itemData.itemIcon;

        // GetComponents의 순서는 Hierarchy 순서와 일치
        Text[] texts = GetComponentsInChildren<Text>();
        textLevel = texts[0];
        textName = texts[1];
        textDesc = texts[2];
    }

    void OnEnable()
    {
        textLevel.text = "Lv." + (level + 1);
        textName.text = itemData.itemName;
        switch(itemData.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
            case ItemData.ItemType.Spear:
            case ItemData.ItemType.Boomerang:
            case ItemData.ItemType.ShotGun:
                textDesc.text = string.Format(itemData.itemDesc, (int)(itemData.damages[level] * 100 - 100), itemData.counts[level]);
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoes:
            case ItemData.ItemType.Missile:
                textDesc.text = string.Format(itemData.itemDesc, itemData.damages[level] * 100);
                break;
            case ItemData.ItemType.Heal:
                textDesc.text = string.Format(itemData.itemDesc);
                break;
        }
        
    }

    public void OnClick()
    {
        switch(itemData.itemType)
        {
            case ItemData.ItemType.Melee:
            case ItemData.ItemType.Range:
            case ItemData.ItemType.Spear:
            case ItemData.ItemType.Boomerang:
            case ItemData.ItemType.ShotGun:
                if (level == 0)
                {
                    GameObject newWeapon = new GameObject();
                    weapon = newWeapon.AddComponent<Weapon>();
                    weapon.Init(itemData);
                }
                else
                {
                    float nextDamage = weapon.damage;
                    int nextCount = weapon.count;
                    nextDamage *= itemData.damages[level];
                    nextCount += itemData.counts[level];

                    weapon.LevelUp(nextDamage, nextCount);
                }
                level++;
                break;
            case ItemData.ItemType.Glove:
            case ItemData.ItemType.Shoes:
            case ItemData.ItemType.Missile:
                if (level == 0)
                {
                    GameObject newGear = new GameObject();
                    gear = newGear.AddComponent<Gear>();
                    gear.Init(itemData);
                }
                else
                {
                    float nextRate = itemData.damages[level];
                    gear.LevelUp(nextRate);
                }
                level++;
                break;
            case ItemData.ItemType.Heal:
                GameManager.instance.health += 50;
                if(GameManager.instance.health > GameManager.instance.maxHealth)
                {
                    GameManager.instance.health = GameManager.instance.maxHealth;
                }
                break;
        }

        if(level == itemData.damages.Length)
        {
            GetComponent<Button>().interactable = false;
        }
    }
}
