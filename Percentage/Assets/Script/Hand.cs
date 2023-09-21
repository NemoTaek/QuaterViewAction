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
        // ��ų ��� �������� üũ
        bool isUsableSkill = GameManager.instance.skill[skillIndex].isUsableSkill;

        // ��� �Ұ����ϸ� Ż��
        if (!isUsableSkill) return;

        // �ִϸ����� �ӵ� �⺻��. ���� �������� �ӵ��� 5�� ����ϱ� ������ �⺻���� �ʿ�
        if (GameManager.instance.player.role != 3) animator.speed = 1;

        if (skillIndex == 0)
        {
            // ��ų ���
            GameManager.instance.skill[skillIndex].UseSkillSetting();

            // ���� ���� �����̴� ����� �����Ƿ� �����ϰ� �ִϸ��̼� ����
            if (GameManager.instance.player.role != 3) animator.SetTrigger(GameManager.instance.SetAttackAnimation(dirVec));

            // �����̿� ���� ���� ������ �Ѿ��� �߻�
            if (GameManager.instance.player.role == 1)
            {
                // �Ҹ� id, �߻� ����, �߻� ��ġ, �߻� �ӵ�
                GameManager.instance.weapon[GameManager.instance.player.currentWeaponIndex].Shot(2, dirVec, transform.position, 5);
            }
            else if (GameManager.instance.player.role == 3)
            {
                // �Ҹ� id, �߻� ����, �߻� ��ġ, �߻� �ӵ�
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
