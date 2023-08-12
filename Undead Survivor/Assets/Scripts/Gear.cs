using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
{
    public ItemData.ItemType type;
    public float rate;

    public void Init(ItemData itemData)
    {
        name = "Gear " + itemData.itemId;
        transform.parent = GameManager.instance.player.transform;
        transform.localPosition = Vector3.zero;

        type = itemData.itemType;
        rate = itemData.damages[0];

        ApplyGear();
    }

    public void LevelUp(float rate)
    {
        this.rate = rate;
        ApplyGear();
    }

    // 현재 가지고 있는 모든 무기의 공속 상승
    void RateUp()
    {
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();
        foreach(Weapon weapon in weapons)
        {
            float baseWeaponSpeed = weapon.baseSpeed;

            switch (weapon.id)
            {
                case 0:
                    weapon.speed = baseWeaponSpeed + (baseWeaponSpeed * rate);
                    break;
                default:
                    weapon.speed = baseWeaponSpeed * (1f - rate);
                    break;
            }
        }
    }

    void SpeedUp()
    {
        float speed = 5 * GameManager.instance.gameCharacters[GameManager.instance.playerId].characterData.baseSpeed;
        GameManager.instance.player.speed = speed + speed * rate;
    }

    void DamageUp()
    {
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();
        foreach (Weapon weapon in weapons)
        {
            float baseWeaponDamage = weapon.baseDamage;
            weapon.damage += (baseWeaponDamage * rate);
        }
    }

    void ApplyGear()
    {
        switch(type)
        {
            case ItemData.ItemType.Glove:
                RateUp();
                break;
            case ItemData.ItemType.Shoes:
                SpeedUp();
                break;
            case ItemData.ItemType.Missile:
                DamageUp();
                break;
        }
    }
}
