using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [Header("----- Component -----")]
    Animator animator;
    public Weapon[] haveWeapons;

    [Header("----- Common -----")]
    public bool isChanged;

    //[Header("----- role -----")]

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

    public void Attack(string direction, Vector2 dirVec, int skillIndex)
    {
        switch (skillIndex)
        {
            case 0:
                animator.SetTrigger(direction + "Attack");

                // 지팡이와 총은 각각 마법과 총알을 발사
                if (GameManager.instance.player.role == 1 || GameManager.instance.player.role == 3)
                {
                    GameManager.instance.weapon[GameManager.instance.player.currentWeaponIndex].Shot(dirVec, transform.position);
                }
                break;

            case 1:
                // 전사는 돌진, 법사는 넉백, 도적은 은신, 거너는 백스텝샷
                if (GameManager.instance.player.role == 0) StartCoroutine(Rush(dirVec));
                else if (GameManager.instance.player.role == 1) StartCoroutine(BackStepShot(-dirVec));
                else if (GameManager.instance.player.role == 2) StartCoroutine(DarkSight());
                else if (GameManager.instance.player.role == 3) StartCoroutine(BackStepShot(-dirVec));
                animator.SetTrigger(direction + "Attack");
                break;
        }
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

    IEnumerator Rush(Vector2 dirVec)
    {
        GameManager.instance.player.rigid.AddForce(dirVec * 0.5f);
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator FireBlow(Vector2 dirVec)
    {
        GameManager.instance.weapon[GameManager.instance.player.currentWeaponIndex].SkillFire(dirVec, transform.position);
        GameManager.instance.weapon[GameManager.instance.player.currentWeaponIndex].SkillFire(-dirVec, transform.position);

        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator DarkSight()
    {
        Color color = GameManager.instance.player.spriteRenderer.color;
        color.a = 0.5f;
        GameManager.instance.player.spriteRenderer.color = color;
        GameManager.instance.player.col.isTrigger = true;

        yield return new WaitForSeconds(3f);
        GameManager.instance.player.col.isTrigger = false;
        color.a = 1f;
        GameManager.instance.player.spriteRenderer.color = color;
    }

    IEnumerator BackStepShot(Vector2 dirVec)
    {
        GameManager.instance.player.rigid.AddForce(dirVec * 0.3f);
        yield return new WaitForSeconds(0.5f);
    }
}
