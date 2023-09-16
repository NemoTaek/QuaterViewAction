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
    public Skill[] haveScills;

    [Header("----- Common -----")]
    public bool isChanged;

    //[Header("----- role -----")]

    void Awake()
    {
        animator = GetComponent<Animator>();
        haveWeapons = GetComponentsInChildren<Weapon>(true);
        haveScills = GetComponentsInChildren<Skill>(true);
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
            haveScills = GetComponentsInChildren<Skill>(true);
            SwapWeapon(GameManager.instance.player.currentWeaponIndex);
        }
    }

    public void Attack(string direction)
    {
        animator.SetTrigger(direction + "Attack");
    }

    public void Attack(string direction, Vector2 dirVec, int skillIndex)
    {
        // 스킬 사용 가능한지 체크
        bool isUsableSkill = GameManager.instance.skill[skillIndex].isUsableSkill;

        // 사용 불가능하면 탈출
        if (!isUsableSkill) return;

        switch (skillIndex)
        {
            case 0:
                // 스킬 사용
                UseSkill(skillIndex);

                if (GameManager.instance.player.role != 3) animator.SetTrigger(direction + "Attack");

                // 지팡이와 총은 각각 마법과 총알을 발사
                if (GameManager.instance.player.role == 1 || GameManager.instance.player.role == 3)
                {
                    GameManager.instance.weapon[GameManager.instance.player.currentWeaponIndex].Shot(dirVec, transform.position);
                }
                break;

            case 1:
                //float skillDamage = GameManager.instance.skillData[GameManager.instance.player.role * 5 + skillIndex].upgradeDamage[];
                // 전사는 돌진, 법사는 넉백, 도적은 은신, 거너는 백스텝샷
                if (GameManager.instance.player.role == 0) Rush(dirVec, skillIndex);
                else if (GameManager.instance.player.role == 1) FireBlow(dirVec, skillIndex);
                else if (GameManager.instance.player.role == 2) StartCoroutine(DarkSight(skillIndex));
                else if (GameManager.instance.player.role == 3) BackStepShot(-dirVec, skillIndex);

                if (GameManager.instance.player.role != 3) animator.SetTrigger(direction + "Attack");
                break;

            case 2:
                // 전사는 집중, 법사는 명상, 도적은 지뢰, 거너는 다이스
                //if (GameManager.instance.player.role == 0) Concentration();
                //else if (GameManager.instance.player.role == 1) Meditation();
                //else if (GameManager.instance.player.role == 2) MineLander();
                //else if (GameManager.instance.player.role == 3) Dice();

                if (GameManager.instance.player.role != 3) animator.SetTrigger(direction + "Attack");
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

    void UseSkill(int skillIndex)
    {
        GameManager.instance.skill[skillIndex].isUseSkill = true;
        GameManager.instance.skill[skillIndex].isUsableSkill = false;
        StartCoroutine(GameManager.instance.skill[skillIndex].CheckSkillCoolTime());
    }

    // 2번스킬들
    void Rush(Vector2 dirVec, int skillIndex)
    {
        // 스킬 사용
        UseSkill(skillIndex);

        //float skillDamage = 
        //GameManager.instance.weapon[GameManager.instance.player.currentWeaponIndex].damage += 
        GameManager.instance.player.rigid.AddForce(dirVec * 0.5f);
    }

    void FireBlow(Vector2 dirVec, int skillIndex)
    {
        // 스킬 사용
        UseSkill(skillIndex);

        GameManager.instance.weapon[GameManager.instance.player.currentWeaponIndex].SkillFire(dirVec, transform.position);
        GameManager.instance.weapon[GameManager.instance.player.currentWeaponIndex].SkillFire(-dirVec, transform.position);
    }

    IEnumerator DarkSight(int skillIndex)
    {
        // 스킬 사용
        UseSkill(skillIndex);

        Color color = GameManager.instance.player.spriteRenderer.color;
        color.a = 0.5f;
        GameManager.instance.player.spriteRenderer.color = color;
        GameManager.instance.player.col.isTrigger = true;

        yield return new WaitForSeconds(GameManager.instance.skillData[11].skillDuringTime);
        GameManager.instance.player.col.isTrigger = false;
        color.a = 1f;
        GameManager.instance.player.spriteRenderer.color = color;
    }

    void BackStepShot(Vector2 dirVec, int skillIndex)
    {
        // 스킬 사용
        UseSkill(skillIndex);

        GameManager.instance.player.rigid.AddForce(dirVec * 0.3f);
    }

    // 3번스킬들
    //void Conenctration()
    //{

    //}

    //void Meditation()
    //{

    //}

    //void MineLander()
    //{

    //}

    //void Dice()
    //{

    //}
}
