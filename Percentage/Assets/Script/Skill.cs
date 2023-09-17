using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public int id;  // ��ų ���̵�
    public string skillNname; // ��ų �̸�
    public float damage;    // ��ų ���ݷ�
    public float[] upgradeDamage;    // ��ų ���׷��̵� �� ����ϴ� ���ݷ�
    public float skillCoolTime;     // ��ų ��Ÿ��
    public float skillDuringTime;   // ��ų ���ӽð�
    public float skillRange;    // ��ų ����
    public int level;   // ��ų ��ȭ ����
    public string desc; // ��ų ����
    public Sprite icon; // ��ų ������

    public bool isUseSkill;
    public bool isUsableSkill;
    public float coolTimeTimer;

    void Awake()
    {
        isUsableSkill = true;
    }

    public void Init(SkillData data)
    {
        id = data.skillId;
        skillNname = data.skillName;
        damage = data.baseDamage;
        upgradeDamage = data.upgradeDamage;
        skillCoolTime = data.skillCoolTime;
        skillDuringTime = data.skillDuringTime;
        skillRange = data.skillRange;
        level = data.level;
        desc = data.skillDesc;
        icon = data.skillIcon;

        transform.parent = GameManager.instance.player.hand[(int)data.usableRoleType].transform;
    }

    void Start()
    {

    }

    void Update()
    {

    }

    public IEnumerator CheckSkillCoolTime()
    {
        coolTimeTimer = skillCoolTime;

        while(coolTimeTimer > 0)
        {
            coolTimeTimer -= Time.deltaTime;
            if (coolTimeTimer <= 0)
            {
                isUseSkill = false;
                isUsableSkill = true;
                yield break;
            }
            yield return new WaitForSeconds(0.01f * Time.deltaTime);
        }
    }

    public void UseSkillSetting()
    {
        isUseSkill = true;
        isUsableSkill = false;
        StartCoroutine(CheckSkillCoolTime());
    }

    public IEnumerator UseSkill(Vector2 dirVec, int id)
    {
        UseSkillSetting();

        switch(id)
        {
            // ���� ����
            case 1:
                GameManager.instance.player.rigid.AddForce(dirVec * 0.5f);
                break;
            // ���� ����
            case 2:
                break;
            // ���� �������
            case 3:
                break;
            // ���� �˱�
            case 4:
                break;

            // ������ ���̾� ��ο�
            case 6:
                GameManager.instance.weapon[GameManager.instance.player.currentWeaponIndex].SkillFire(dirVec, transform.parent.transform.position);
                GameManager.instance.weapon[GameManager.instance.player.currentWeaponIndex].SkillFire(-dirVec, transform.parent.transform.position);
                break;
            // ������ ���
            case 7:
                break;
            // ������ ���׿�
            case 8:
                break;
            // ������ ���丣�������
            case 9:
                break;

            // ���� ����
            case 11:
                Color color = GameManager.instance.player.spriteRenderer.color;
                color.a = 0.5f;
                GameManager.instance.player.spriteRenderer.color = color;
                GameManager.instance.player.col.isTrigger = true;

                yield return new WaitForSeconds(skillDuringTime);
                GameManager.instance.player.col.isTrigger = false;
                color.a = 1f;
                GameManager.instance.player.spriteRenderer.color = color;
                break;
            // ���� ���̽�Ʈ
            case 12:
                break;
            // ���� ����
            case 13:
                break;
            // ���� �ϻ�
            case 14:
                break;

            // �ų� �齺�ܼ�
            case 16:
                GameManager.instance.player.rigid.AddForce(dirVec * 0.3f);
                break;
            // �ų� ���̽�
            case 17:
                break;
            // �ų� �Ҹ���Ƽ
            case 18:
                break;
            // �ų� ��弦
            case 19:
                break;
        }

        yield return null;
    }
}
