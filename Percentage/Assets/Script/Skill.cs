using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    Player player;

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

    public bool isAttack;
    public bool isUseSkill;
    public bool isUsableSkill;
    public float coolTimeTimer;

    void Awake()
    {
        isUsableSkill = true;
        player = GameManager.instance.player;
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
                isAttack = false;
                yield break;
            }
            yield return new WaitForSeconds(0.01f * Time.deltaTime);
        }
    }

    public void UseSkillSetting()
    {
        isUseSkill = true;
        isUsableSkill = false;
        isAttack = true;
        StartCoroutine(CheckSkillCoolTime());
    }

    public IEnumerator UseSkill(Vector2 dirVec)
    {
        UseSkillSetting();

        switch(id)
        {
            // 전사 돌진
            case 1:
                player.hand[player.role].animator.SetTrigger(GameManager.instance.SetAttackAnimation(dirVec));
                player.hand[player.role].animator.speed = 5;
                player.rigid.AddForce(dirVec * 0.5f);
                break;
            // 전사 집중
            case 2:
                player.buffSprite.sprite = icon;
                player.power += upgradeDamage[level];

                yield return new WaitForSeconds(skillDuringTime);

                player.buffSprite.sprite = null;
                player.power -= upgradeDamage[level];
                break;
            // 전사 가드어택
            case 3:
                // 스킬을 사용했다는 것을 알리고
                player.buffSprite.sprite = icon;

                float guardTimer = skillDuringTime;
                while (guardTimer > 0)
                {
                    guardTimer -= Time.deltaTime;

                    // 지속시간 안에 플레이어가 피격당했으면 1초간 충격파 발생
                    if (player.isDamaged)
                    {
                        GameObject shockWave = GameManager.instance.ObjectPool.Get(6);
                        shockWave.transform.position = player.transform.position;

                        yield return new WaitForSeconds(1f);

                        player.buffSprite.sprite = null;
                        shockWave.SetActive(false);
                        break;
                    }

                    yield return new WaitForSeconds(0.01f * Time.deltaTime);
                }

                // 지속시간 내에 발동되지 않았으면 아이콘 off
                player.buffSprite.sprite = null;
                break;
            // 전사 검기
            case 4:
                player.hand[player.role].animator.SetTrigger(GameManager.instance.SetAttackAnimation(dirVec));
                GameManager.instance.weapon[player.currentWeaponIndex].Shot(9, dirVec, GameManager.instance.weapon[player.role].transform.position, 10);
                break;

            // 마법사 파이어 블로우
            case 6:
                GameManager.instance.weapon[player.currentWeaponIndex].Shot(4, dirVec, GameManager.instance.weapon[player.role].transform.position, 5);
                GameManager.instance.weapon[player.currentWeaponIndex].Shot(4, -dirVec, GameManager.instance.weapon[player.role].transform.position, 5);
                break;
            // 마법사 명상
            case 7:
                player.buffSprite.sprite = icon;
                player.power += upgradeDamage[level];

                yield return new WaitForSeconds(skillDuringTime);

                player.buffSprite.sprite = null;
                player.power -= upgradeDamage[level];
                break;
            // 마법사 메테오
            case 8:
                // 맵 사이즈 알아내서 거기에 지금 박스 설치한것 처럼 랜덤으로 10개정도 나오도록
                break;
            // 마법사 인페르노라이즈
            case 9:
                // 키다운 해서 3초 이상 눌렀다가 떼면 되도록
                break;

            // 도적 은신
            case 11:
                Color color = player.spriteRenderer.color;
                color.a = 0.5f;
                player.spriteRenderer.color = color;
                player.col.isTrigger = true;

                yield return new WaitForSeconds(skillDuringTime);

                player.col.isTrigger = false;
                color.a = 1f;
                player.spriteRenderer.color = color;
                break;
            // 도적 헤이스트
            case 12:
                player.buffSprite.sprite = icon;
                player.speed += upgradeDamage[level];

                yield return new WaitForSeconds(skillDuringTime);

                player.buffSprite.sprite = null;
                player.speed -= upgradeDamage[level];
                break;
            // 도적 지뢰
            case 13:
                // 플레이어 상하좌우로 1정도 내외로 해서 랜덤으로 3개 설치
                // 나는 피격당해도 체력 안닳도록 설정 (적만 닳도록)
                break;
            // 도적 암살
            case 14:
                // 키다운 해서 3초 이상 눌렀다가 떼면 되도록
                break;

            // 거너 백스텝샷
            case 16:
                GameManager.instance.weapon[player.currentWeaponIndex].Shot(5, dirVec, transform.position, 5);
                player.rigid.AddForce(dirVec * (-0.3f));
                break;
            // 거너 다이스
            case 17:
                int randomStatus = Random.Range(0, 4);
                switch(randomStatus)
                {
                    case 0:
                        break;
                    case 1:
                        player.buffSprite.sprite = icon;
                        player.attackSpeed += upgradeDamage[level];

                        yield return new WaitForSeconds(skillDuringTime);

                        player.buffSprite.sprite = icon;
                        player.attackSpeed -= upgradeDamage[level];
                        break;
                    case 2:
                        player.buffSprite.sprite = icon;
                        player.speed += upgradeDamage[level];

                        yield return new WaitForSeconds(skillDuringTime);

                        player.buffSprite.sprite = icon;
                        player.speed -= upgradeDamage[level];
                        break;
                    case 3:
                        player.buffSprite.sprite = icon;
                        player.power += upgradeDamage[level];

                        yield return new WaitForSeconds(skillDuringTime);

                        player.buffSprite.sprite = icon;
                        player.power -= upgradeDamage[level];
                        break;
                }
                break;
            // 거너 불릿파티
            case 18:
                // 플레이어 위치 기준으로 랜덤 방향으로 지속시간동안 스킬불렛 발사
                break;
            // 거너 헤드샷
            case 19:
                // 키다운 해서 3초 이상 눌렀다가 떼면 되도록
                break;
        }

        yield return null;
    }
}
