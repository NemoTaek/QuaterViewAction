using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.Rendering.LookDev;
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
}
