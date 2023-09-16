using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor.Rendering.LookDev;
using UnityEngine;

public class Skill : MonoBehaviour
{
    public int id;  // 스킬 아이디
    public string skillNname; // 스킬 이름
    public float damage;    // 스킬 공격력
    public float[] upgradeDamage;    // 스킬 업그레이드 시 상승하는 공격력
    public float skillCoolTime;     // 스킬 쿨타임
    public float skillDuringTime;   // 스킬 지속시간
    public float skillRange;    // 스킬 범위
    public int level;   // 스킬 강화 레벨
    public string desc; // 스킬 설명
    public Sprite icon; // 스킬 아이콘

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
