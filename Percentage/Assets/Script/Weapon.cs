using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Animator animator;
    public string weaponName; // 무기 이름
    public float damage;    // 무기 데미지

    void Awake()
    {
        animator = GetComponentInParent<Animator>();
    }

    public void Init(string weaponName, float damage)
    {
        this.weaponName = weaponName;
        this.damage = damage;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
