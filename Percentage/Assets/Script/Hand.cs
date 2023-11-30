using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{
    [Header("----- Component -----")]
    public Animator animator;
    public Weapon[] haveWeapons;
    public Skill[] haveSkills;
    

    [Header("----- Common -----")]
    public bool isWeaponChanged;
    public bool isSkillChanged;

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
        if (isWeaponChanged)
        {
            haveWeapons = GetComponentsInChildren<Weapon>(true);
            SwapWeapon(GameManager.instance.player.currentWeaponIndex);
        }
        else if (isSkillChanged)
        {
            haveSkills = GetComponentsInChildren<Skill>(true);
            isSkillChanged = false;
        }
    }

    public void Attack(Vector2 dirVec, int skillIndex)
    {
        // ��ų ��� �������� üũ
        bool isUsableSkill = GameManager.instance.skill[skillIndex].isUsableSkill;

        // ��� �Ұ����ϸ� Ż��
        if (!isUsableSkill) return;

        // �ִϸ����� �ӵ� �⺻��. ���� �������� �ӵ��� 5�� ����ϱ� ������ �⺻���� �ʿ�
        if (GameManager.instance.player.role != 3) animator.speed = 5;

        if (skillIndex == 0)
        {
            // ��ų ���
            GameManager.instance.skill[skillIndex].UseSkillSetting();

            // ���� ���� �����̴� ����� �����Ƿ� �����ϰ� �ִϸ��̼� ����
            if (GameManager.instance.player.role != 3) animator.SetTrigger(GameManager.instance.SetAttackAnimation(dirVec));

            // �����̿� ���� ���� ������ �Ѿ��� �߻�
            if (GameManager.instance.player.role == 0)
            {
                AudioManager.instance.EffectPlay(AudioManager.Effect.KnightAttack);
            }
            else if (GameManager.instance.player.role == 1)
            {
                // �Ҹ� id, �߻� ����, �߻� ��ġ, �߻� �ӵ�
                GameManager.instance.weapon[GameManager.instance.player.currentWeaponIndex].Shot(0, dirVec, transform.position, 5);
                AudioManager.instance.EffectPlay(AudioManager.Effect.WizardShot);
            }
            else if (GameManager.instance.player.role == 2)
            {
                AudioManager.instance.EffectPlay(AudioManager.Effect.ThiefAttack);
            }
            else if (GameManager.instance.player.role == 3)
            {
                // �Ҹ� id, �߻� ����, �߻� ��ġ, �߻� �ӵ�
                GameManager.instance.weapon[GameManager.instance.player.currentWeaponIndex].Shot(1, dirVec, transform.position, 5);
                AudioManager.instance.EffectPlay(AudioManager.Effect.GunnerShot);
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

        // ��ü������ �׸�ŭ ���ݷ��� �ٲ��� �Ѵ�.
        GameManager.instance.player.powerUp += GameManager.instance.weapon[GameManager.instance.player.currentWeaponIndex].damage;

        isWeaponChanged = false;
    }
}
