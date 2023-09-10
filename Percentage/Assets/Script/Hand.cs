using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class Hand : MonoBehaviour
{
    Animator animator;
    Weapon weapon;

    void Awake()
    {
        animator = GetComponent<Animator>();
        weapon = GetComponentInChildren<Weapon>(true);
    }

    void OnEnable()
    {

    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Attack(string direction)
    {
        animator.SetTrigger(direction + "Attack");
    }
}
