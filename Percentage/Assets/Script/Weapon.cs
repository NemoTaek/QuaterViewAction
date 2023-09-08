using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName; // 무기 이름
    public int[] wearableRole;    // 착용 가능한 직업
    public float damage;    // 무기 데미지
    
    public void Init(string weaponName, int[] wearableRole, float damage)
    {
        this.weaponName = weaponName;
        this.wearableRole = wearableRole;
        this.damage = damage;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
