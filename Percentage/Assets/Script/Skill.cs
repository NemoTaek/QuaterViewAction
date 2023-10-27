using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Skill : MonoBehaviour
{
    Player player;

    public int id;  // 스킬 아이디
    public string skillName; // 스킬 이름
    public float damage;    // 스킬 공격력
    public float[] upgradeDamage;    // 스킬 업그레이드 시 상승하는 공격력
    public float skillCoolTime;     // 스킬 쿨타임
    public float skillDuringTime;   // 스킬 지속시간
    public int level;   // 스킬 강화 레벨
    public int maxLevel;    // 스킬 최고 레벨
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
        skillName = data.skillName;
        damage = data.baseDamage;
        upgradeDamage = data.upgradeDamage;
        skillCoolTime = data.skillCoolTime;
        skillDuringTime = data.skillDuringTime;
        level = data.level;
        maxLevel = data.maxLevel;
        desc = data.skillDesc;
        icon = data.skillIcon;

        transform.parent = GameManager.instance.player.hand[(int)data.usableRoleType].transform;
    }

    void Start()
    {

    }

    void Update()
    {
        if (id == player.role * 5)
        {
            skillCoolTime = player.attackSpeed;
        }
        if (id == 11)
        {
            skillDuringTime = damage + upgradeDamage[level];
        }
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
                player.powerUp += (damage + upgradeDamage[level]);

                yield return new WaitForSeconds(skillDuringTime);

                player.buffSprite.sprite = null;
                player.powerUp -= (damage + upgradeDamage[level]);
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
                        SkillBullet shockWave = GameManager.instance.bulletPool.Get(0, 4).GetComponent<SkillBullet>();
                        shockWave.transform.position = player.transform.position;

                        yield return new WaitForSeconds(skillDuringTime);

                        player.buffSprite.sprite = null;
                        shockWave.gameObject.SetActive(false);
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
                GameManager.instance.weapon[player.currentWeaponIndex].Shot(7, dirVec, GameManager.instance.weapon[player.currentWeaponIndex].transform.position, 10);
                break;

            // 마법사 파이어 블로우
            case 6:
                GameManager.instance.weapon[player.currentWeaponIndex].Shot(2, dirVec, GameManager.instance.weapon[player.currentWeaponIndex].transform.position, 5);
                GameManager.instance.weapon[player.currentWeaponIndex].Shot(2, -dirVec, GameManager.instance.weapon[player.currentWeaponIndex].transform.position, 5);
                break;
            // 마법사 명상
            case 7:
                player.buffSprite.sprite = icon;
                player.powerUp += (damage + upgradeDamage[level]);

                yield return new WaitForSeconds(skillDuringTime);

                player.buffSprite.sprite = null;
                player.powerUp -= (damage + upgradeDamage[level]);
                break;
            // 마법사 메테오
            case 8:
                // 메테오 랜덤 위치에 배치
                SkillBullet[] meteors = new SkillBullet[10];
                for(int i=0; i<10; i++)
                {
                    SkillBullet meteor = GameManager.instance.bulletPool.Get(0, 5).GetComponent<SkillBullet>();
                    float randomX = Random.Range(-6.5f, 7.5f);
                    float randomY = Random.Range(-2.5f, 3.5f);

                    meteor.transform.position = new Vector2(randomX, randomY);
                    meteors[i] = meteor;
                }

                yield return new WaitForSeconds(0.5f);

                // 0.5초 후 하강
                for (int i = 0; i < 10; i++)
                {
                    Rigidbody2D meteoRigid = meteors[i].GetComponent<Rigidbody2D>();
                    //meteoRigid.AddForce(new Vector2(-1, -1) * 2f, ForceMode2D.Impulse);
                    meteoRigid.velocity = new Vector2(-1, -1) * 2f;
                }

                yield return new WaitForSeconds(0.5f);

                // 0.5초 후 사라짐
                for (int i = 0; i < 10; i++)
                {
                    meteors[i].gameObject.SetActive(false);
                }

                break;
            // 마법사 인페르노라이즈
            case 9:
                SkillBullet infernorize = GameManager.instance.bulletPool.Get(0, 8).GetComponent<SkillBullet>();
                infernorize.transform.position = player.transform.position + new Vector3(dirVec.x * 3 + dirVec.y * 2, 1);

                yield return new WaitForSeconds(2f);

                infernorize.gameObject.SetActive(false);
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
                player.speed += (damage + upgradeDamage[level]);

                yield return new WaitForSeconds(skillDuringTime);

                player.buffSprite.sprite = null;
                player.speed -= (damage + upgradeDamage[level]);
                break;
            // 도적 지뢰
            case 13:
                // 지뢰 오브젝트 생성
                SkillBullet[] mines = new SkillBullet[8];
                Vector3[] minePosition = new Vector3[8];

                // 플레이어 주변 8방향 설정
                int index = 0;
                for (float dx = -1f; dx < 2; dx++)
                {
                    for (float dy = -1f; dy < 2; dy++)
                    {
                        if (dx == 0 && dy == 0) continue;
                        minePosition[index] = new Vector3(player.transform.position.x + dx, player.transform.position.y + dy, 1);
                        index++;
                    }
                }

                // 설정한 위치에 지뢰 설치
                for (int i = 0; i < 8; i++)
                {
                    mines[i] = GameManager.instance.bulletPool.Get(0, 6).GetComponent<SkillBullet>();
                    mines[i].transform.position = minePosition[i];
                }
                break;
            // 도적 암살
            case 14:
                // 1타
                SkillBullet assassination1 = GameManager.instance.bulletPool.Get(0, 9).GetComponent<SkillBullet>();
                assassination1.transform.position = player.transform.position + new Vector3(dirVec.x * 2, dirVec.y * 2, 1);
                yield return new WaitForSeconds(0.5f);

                // 2타
                SkillBullet assassination2 = GameManager.instance.bulletPool.Get(0, 9).GetComponent<SkillBullet>();
                assassination2.transform.position = player.transform.position + new Vector3(dirVec.x * 2, dirVec.y * 2, 1);
                assassination2.transform.rotation = Quaternion.Euler(0, 0, 90);
                yield return new WaitForSeconds(1f);

                assassination1.gameObject.SetActive(false);
                assassination2.gameObject.SetActive(false);
                break;

            // 거너 백스텝샷
            case 16:
                GameManager.instance.weapon[player.currentWeaponIndex].Shot(3, dirVec, GameManager.instance.weapon[player.currentWeaponIndex].transform.position, 5);
                player.rigid.AddForce(dirVec * (-0.3f));
                break;
            // 거너 다이스
            case 17:
                int randomStatus = Random.Range(0, 4);

                player.buffSprite.sprite = icon;

                switch (randomStatus)
                {
                    case 0:
                        break;
                    case 1:
                        player.attackSpeedUp += (damage + upgradeDamage[level]);
                        yield return new WaitForSeconds(skillDuringTime);
                        player.attackSpeedUp -= (damage + upgradeDamage[level]);
                        break;
                    case 2:
                        // 이동속도의 경우에는 조금만 높아도 휙휙 변해서 반만 적용
                        player.speed += ((damage + upgradeDamage[level]) / 2);
                        yield return new WaitForSeconds(skillDuringTime);
                        player.speed -= ((damage + upgradeDamage[level]) / 2);
                        break;
                    case 3:
                        player.powerUp += (damage + upgradeDamage[level]);
                        yield return new WaitForSeconds(skillDuringTime);
                        player.powerUp -= (damage + upgradeDamage[level]);
                        break;
                }

                player.buffSprite.sprite = null;

                break;
            // 거너 불릿파티
            case 18:
                // 플레이어 위치 기준으로 랜덤 방향으로 지속시간동안 스킬불렛 발사
                float timer = skillDuringTime;

                while(timer > 0)
                {
                    // 발사 방향 랜덤 세팅
                    float shotDirX = Random.Range(-1.0f, 1.0f);
                    float shotDirY = Random.Range(-1.0f, 1.0f);
                    Vector2 shotDir = new Vector2(shotDirX, shotDirY).normalized;

                    // 랜덤 방향 발사
                    GameManager.instance.weapon[player.currentWeaponIndex].Shot(1, shotDir, player.transform.position + new Vector3(shotDir.x, shotDir.y, 1), 3);
                    yield return new WaitForSeconds(0.1f);

                    timer -= 0.1f;
                }
                break;
            // 거너 헤드샷
            case 19:
                GameManager.instance.weapon[player.currentWeaponIndex].Shot(3, dirVec, GameManager.instance.weapon[player.currentWeaponIndex].transform.position, 10);
                break;
        }

        yield return null;
    }
}
