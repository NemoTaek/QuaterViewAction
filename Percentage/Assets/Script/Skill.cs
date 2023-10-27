using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Skill : MonoBehaviour
{
    Player player;

    public int id;  // ��ų ���̵�
    public string skillName; // ��ų �̸�
    public float damage;    // ��ų ���ݷ�
    public float[] upgradeDamage;    // ��ų ���׷��̵� �� ����ϴ� ���ݷ�
    public float skillCoolTime;     // ��ų ��Ÿ��
    public float skillDuringTime;   // ��ų ���ӽð�
    public int level;   // ��ų ��ȭ ����
    public int maxLevel;    // ��ų �ְ� ����
    public string desc; // ��ų ����
    public Sprite icon; // ��ų ������

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
            // ���� ����
            case 1:
                player.hand[player.role].animator.SetTrigger(GameManager.instance.SetAttackAnimation(dirVec));
                player.hand[player.role].animator.speed = 5;

                player.rigid.AddForce(dirVec * 0.5f);
                break;
            // ���� ����
            case 2:
                player.buffSprite.sprite = icon;
                player.powerUp += (damage + upgradeDamage[level]);

                yield return new WaitForSeconds(skillDuringTime);

                player.buffSprite.sprite = null;
                player.powerUp -= (damage + upgradeDamage[level]);
                break;
            // ���� �������
            case 3:
                // ��ų�� ����ߴٴ� ���� �˸���
                player.buffSprite.sprite = icon;

                float guardTimer = skillDuringTime;
                while (guardTimer > 0)
                {
                    guardTimer -= Time.deltaTime;

                    // ���ӽð� �ȿ� �÷��̾ �ǰݴ������� 1�ʰ� ����� �߻�
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

                // ���ӽð� ���� �ߵ����� �ʾ����� ������ off
                player.buffSprite.sprite = null;
                break;
            // ���� �˱�
            case 4:
                player.hand[player.role].animator.SetTrigger(GameManager.instance.SetAttackAnimation(dirVec));
                GameManager.instance.weapon[player.currentWeaponIndex].Shot(7, dirVec, GameManager.instance.weapon[player.currentWeaponIndex].transform.position, 10);
                break;

            // ������ ���̾� ��ο�
            case 6:
                GameManager.instance.weapon[player.currentWeaponIndex].Shot(2, dirVec, GameManager.instance.weapon[player.currentWeaponIndex].transform.position, 5);
                GameManager.instance.weapon[player.currentWeaponIndex].Shot(2, -dirVec, GameManager.instance.weapon[player.currentWeaponIndex].transform.position, 5);
                break;
            // ������ ���
            case 7:
                player.buffSprite.sprite = icon;
                player.powerUp += (damage + upgradeDamage[level]);

                yield return new WaitForSeconds(skillDuringTime);

                player.buffSprite.sprite = null;
                player.powerUp -= (damage + upgradeDamage[level]);
                break;
            // ������ ���׿�
            case 8:
                // ���׿� ���� ��ġ�� ��ġ
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

                // 0.5�� �� �ϰ�
                for (int i = 0; i < 10; i++)
                {
                    Rigidbody2D meteoRigid = meteors[i].GetComponent<Rigidbody2D>();
                    //meteoRigid.AddForce(new Vector2(-1, -1) * 2f, ForceMode2D.Impulse);
                    meteoRigid.velocity = new Vector2(-1, -1) * 2f;
                }

                yield return new WaitForSeconds(0.5f);

                // 0.5�� �� �����
                for (int i = 0; i < 10; i++)
                {
                    meteors[i].gameObject.SetActive(false);
                }

                break;
            // ������ ���丣�������
            case 9:
                SkillBullet infernorize = GameManager.instance.bulletPool.Get(0, 8).GetComponent<SkillBullet>();
                infernorize.transform.position = player.transform.position + new Vector3(dirVec.x * 3 + dirVec.y * 2, 1);

                yield return new WaitForSeconds(2f);

                infernorize.gameObject.SetActive(false);
                break;

            // ���� ����
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
            // ���� ���̽�Ʈ
            case 12:
                player.buffSprite.sprite = icon;
                player.speed += (damage + upgradeDamage[level]);

                yield return new WaitForSeconds(skillDuringTime);

                player.buffSprite.sprite = null;
                player.speed -= (damage + upgradeDamage[level]);
                break;
            // ���� ����
            case 13:
                // ���� ������Ʈ ����
                SkillBullet[] mines = new SkillBullet[8];
                Vector3[] minePosition = new Vector3[8];

                // �÷��̾� �ֺ� 8���� ����
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

                // ������ ��ġ�� ���� ��ġ
                for (int i = 0; i < 8; i++)
                {
                    mines[i] = GameManager.instance.bulletPool.Get(0, 6).GetComponent<SkillBullet>();
                    mines[i].transform.position = minePosition[i];
                }
                break;
            // ���� �ϻ�
            case 14:
                // 1Ÿ
                SkillBullet assassination1 = GameManager.instance.bulletPool.Get(0, 9).GetComponent<SkillBullet>();
                assassination1.transform.position = player.transform.position + new Vector3(dirVec.x * 2, dirVec.y * 2, 1);
                yield return new WaitForSeconds(0.5f);

                // 2Ÿ
                SkillBullet assassination2 = GameManager.instance.bulletPool.Get(0, 9).GetComponent<SkillBullet>();
                assassination2.transform.position = player.transform.position + new Vector3(dirVec.x * 2, dirVec.y * 2, 1);
                assassination2.transform.rotation = Quaternion.Euler(0, 0, 90);
                yield return new WaitForSeconds(1f);

                assassination1.gameObject.SetActive(false);
                assassination2.gameObject.SetActive(false);
                break;

            // �ų� �齺�ܼ�
            case 16:
                GameManager.instance.weapon[player.currentWeaponIndex].Shot(3, dirVec, GameManager.instance.weapon[player.currentWeaponIndex].transform.position, 5);
                player.rigid.AddForce(dirVec * (-0.3f));
                break;
            // �ų� ���̽�
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
                        // �̵��ӵ��� ��쿡�� ���ݸ� ���Ƶ� ���� ���ؼ� �ݸ� ����
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
            // �ų� �Ҹ���Ƽ
            case 18:
                // �÷��̾� ��ġ �������� ���� �������� ���ӽð����� ��ų�ҷ� �߻�
                float timer = skillDuringTime;

                while(timer > 0)
                {
                    // �߻� ���� ���� ����
                    float shotDirX = Random.Range(-1.0f, 1.0f);
                    float shotDirY = Random.Range(-1.0f, 1.0f);
                    Vector2 shotDir = new Vector2(shotDirX, shotDirY).normalized;

                    // ���� ���� �߻�
                    GameManager.instance.weapon[player.currentWeaponIndex].Shot(1, shotDir, player.transform.position + new Vector3(shotDir.x, shotDir.y, 1), 3);
                    yield return new WaitForSeconds(0.1f);

                    timer -= 0.1f;
                }
                break;
            // �ų� ��弦
            case 19:
                GameManager.instance.weapon[player.currentWeaponIndex].Shot(3, dirVec, GameManager.instance.weapon[player.currentWeaponIndex].transform.position, 10);
                break;
        }

        yield return null;
    }
}
