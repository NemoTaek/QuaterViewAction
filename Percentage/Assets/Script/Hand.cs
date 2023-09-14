using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Hand : MonoBehaviour
{
    Animator animator;
    public Weapon[] haveWeapons;

    public bool isChanged;

    void Awake()
    {
        animator = GetComponent<Animator>();
        haveWeapons = GetComponentsInChildren<Weapon>(true);
    }

    void OnEnable()
    {

    }

    void Start()
    {
        
    }

    void Update()
    {
        if (isChanged)
        {
            haveWeapons = GetComponentsInChildren<Weapon>(true);
            SwapWeapon(GameManager.instance.player.currentWeaponIndex);
        }
    }

    public void Attack(string direction)
    {
        animator.SetTrigger(direction + "Attack");
    }

    public void SwapWeapon(int index)
    {
        for(int i=0; i< haveWeapons.Length; i++)
        {
            if (haveWeapons[i].gameObject.activeSelf)
            {
                haveWeapons[i].gameObject.SetActive(false);
                break;
            }
        }
        haveWeapons[index].gameObject.SetActive(true);
        isChanged = false;
    }
}
