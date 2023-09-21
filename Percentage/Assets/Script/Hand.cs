using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [Header("----- Component -----")]
    public Animator animator;
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

    public void Attack(Vector2 dirVec, int skillIndex)
    {
        // 스킬 사용 가능한지 체크
        bool isUsableSkill = GameManager.instance.skill[skillIndex].isUsableSkill;

        // 사용 불가능하면 탈출
        if (!isUsableSkill) return;

        // 애니메이터 속도 기본값. 전사 돌진에서 속도를 5로 사용하기 때문에 기본값이 필요
        if (GameManager.instance.player.role != 3) animator.speed = 1;

        if (skillIndex == 0)
        {
            // 스킬 사용
            GameManager.instance.skill[skillIndex].UseSkillSetting();

            // 총은 무기 움직이는 모션이 없으므로 제외하고 애니메이션 실행
            if (GameManager.instance.player.role != 3) animator.SetTrigger(GameManager.instance.SetAttackAnimation(dirVec));

            // 지팡이와 총은 각각 마법과 총알을 발사
            if (GameManager.instance.player.role == 1)
            {
                // 불릿 id, 발사 방향, 발사 위치, 발사 속도
                GameManager.instance.weapon[GameManager.instance.player.currentWeaponIndex].Shot(2, dirVec, transform.position, 5);
            }
            else if (GameManager.instance.player.role == 3)
            {
                // 불릿 id, 발사 방향, 발사 위치, 발사 속도
                GameManager.instance.weapon[GameManager.instance.player.currentWeaponIndex].Shot(3, dirVec, transform.position, 5);
            }
        }
        else
        {
            StartCoroutine(GameManager.instance.skill[skillIndex].UseSkill(dirVec));
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
