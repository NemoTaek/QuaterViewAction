using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combo : MonoBehaviour
{
    public Animator comboAnimator;

    void Awake()
    {
        comboAnimator = GetComponent<Animator>();
    }
    void Start()
    {

    }

    void Update()
    {

    }

    public void SetComboAnimation()
    {
        comboAnimator.SetTrigger("isCombo");
    }
}
