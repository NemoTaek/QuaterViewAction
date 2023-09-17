using System.Collections;
using System.Collections.Generic;
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
            // 전사 돌진
            case 1:
                GameManager.instance.player.rigid.AddForce(dirVec * 0.5f);
                break;
            // 전사 집중
            case 2:
                break;
            // 전사 가드어택
            case 3:
                break;
            // 전사 검기
            case 4:
                break;

            // 마법사 파이어 블로우
            case 6:
                GameManager.instance.weapon[GameManager.instance.player.currentWeaponIndex].SkillFire(dirVec, transform.parent.transform.position);
                GameManager.instance.weapon[GameManager.instance.player.currentWeaponIndex].SkillFire(-dirVec, transform.parent.transform.position);
                break;
            // 마법사 명상
            case 7:
                break;
            // 마법사 메테오
            case 8:
                break;
            // 마법사 인페르노라이즈
            case 9:
                break;

            // 도적 은신
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
            // 도적 헤이스트
            case 12:
                break;
            // 도적 지뢰
            case 13:
                break;
            // 도적 암살
            case 14:
                break;

            // 거너 백스텝샷
            case 16:
                GameManager.instance.player.rigid.AddForce(dirVec * 0.3f);
                break;
            // 거너 다이스
            case 17:
                break;
            // 거너 불릿파티
            case 18:
                break;
            // 거너 헤드샷
            case 19:
                break;
        }

        yield return null;
    }
}
