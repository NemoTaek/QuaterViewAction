using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponName; // ���� �̸�
    public int[] wearableRole;    // ���� ������ ����
    public float damage;    // ���� ������
    
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
