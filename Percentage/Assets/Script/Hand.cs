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
    public Skill[] haveSkills;

    [Header("----- Common -----")]
    public bool isChanged;

    //[Header("----- role -----")]

    void Awake()
    {
        animator = GetComponent<Animator>();
        haveWeapons = GetComponentsInChildren<Weapon>(true);
        haveSkills = GetComponentsInChildren<Skill>(true);
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
            haveSkills = GetComponentsInChildren<Skill>(true);
            SwapWeapon(GameManager.instance.player.currentWeaponIndex);
        }
    }

    public void Attack(string direction, Vector2 dirVec, int skillIndex)
    {
        // 스킬 사용 가능한지 체크
        bool isUsableSkill = GameManager.instance.skill[skillIndex].isUsableSkill;

        // 사용 불가능하면 탈출
        if (!isUsableSkill) return;

        if(skillIndex == 0)
        {
            // 스킬 사용
            GameManager.instance.skill[skillIndex].UseSkillSetting();

            if (GameManager.instance.player.role != 3) animator.SetTrigger(direction + "Attack");

            // 지팡이와 총은 각각 마법과 총알을 발사
            if (GameManager.instance.player.role == 1 || GameManager.instance.player.role == 3)
            {
                GameManager.instance.weapon[GameManager.instance.player.currentWeaponIndex].Shot(dirVec, transform.position);
            }
        }
        else
        {
            StartCoroutine(GameManager.instance.skill[skillIndex].UseSkill(dirVec, GameManager.instance.skill[skillIndex].id));
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
}
