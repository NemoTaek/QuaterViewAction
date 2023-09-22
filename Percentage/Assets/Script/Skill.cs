using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill : MonoBehaviour
{
    Player player;

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
            // ���� ����
            case 1:
                player.hand[player.role].animator.SetTrigger(GameManager.instance.SetAttackAnimation(dirVec));
                player.hand[player.role].animator.speed = 5;

                player.rigid.AddForce(dirVec * 0.5f);
                break;
            // ���� ����
            case 2:
                player.buffSprite.sprite = icon;
                player.power += upgradeDamage[level];

                yield return new WaitForSeconds(skillDuringTime);

                player.buffSprite.sprite = null;
                player.power -= upgradeDamage[level];
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
                        GameObject shockWave = GameManager.instance.objectPool.Get(6);
                        shockWave.transform.position = player.transform.position;

                        yield return new WaitForSeconds(1f);

                        player.buffSprite.sprite = null;
                        shockWave.SetActive(false);
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
                GameManager.instance.weapon[player.currentWeaponIndex].Shot(9, dirVec, GameManager.instance.weapon[player.currentWeaponIndex].transform.position, 10);
                break;

            // ������ ���̾� ��ο�
            case 6:
                GameManager.instance.weapon[player.currentWeaponIndex].Shot(4, dirVec, GameManager.instance.weapon[player.currentWeaponIndex].transform.position, 5);
                GameManager.instance.weapon[player.currentWeaponIndex].Shot(4, -dirVec, GameManager.instance.weapon[player.currentWeaponIndex].transform.position, 5);
                break;
            // ������ ���
            case 7:
                player.buffSprite.sprite = icon;
                player.power += upgradeDamage[level];

                yield return new WaitForSeconds(skillDuringTime);

                player.buffSprite.sprite = null;
                player.power -= upgradeDamage[level];
                break;
            // ������ ���׿�
            case 8:
                // ���׿� ���� ��ġ�� ��ġ
                GameObject[] meteors = new GameObject[10];
                for(int i=0; i<10; i++)
                {
                    GameObject meteor = GameManager.instance.objectPool.Get(7);
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
                GameObject infernorize = GameManager.instance.objectPool.Get(10);
                infernorize.transform.position = player.transform.position + new Vector3(dirVec.x * 3 + dirVec.y * 2, 1);

                yield return new WaitForSeconds(2f);

                infernorize.SetActive(false);
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
                player.speed += upgradeDamage[level];

                yield return new WaitForSeconds(skillDuringTime);

                player.buffSprite.sprite = null;
                player.speed -= upgradeDamage[level];
                break;
            // ���� ����
            case 13:
                // ���� ������Ʈ ����
                GameObject[] mines = new GameObject[8];
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
                    mines[i] = GameManager.instance.objectPool.Get(8);
                    mines[i].transform.position = minePosition[i];
                }
                break;
            // ���� �ϻ�
            case 14:
                // 1Ÿ
                GameObject assassination1 = GameManager.instance.objectPool.Get(11);
                assassination1.transform.position = player.transform.position + new Vector3(dirVec.x * 2, dirVec.y * 2, 1);
                yield return new WaitForSeconds(0.5f);

                // 2Ÿ
                GameObject assassination2 = GameManager.instance.objectPool.Get(11);
                assassination2.transform.position = player.transform.position + new Vector3(dirVec.x * 2, dirVec.y * 2, 1);
                assassination2.transform.rotation = Quaternion.Euler(0, 0, 90);
                yield return new WaitForSeconds(1f);

                assassination1.SetActive(false);
                assassination2.SetActive(false);
                break;

            // �ų� �齺�ܼ�
            case 16:
                GameManager.instance.weapon[player.currentWeaponIndex].Shot(5, dirVec, GameManager.instance.weapon[player.currentWeaponIndex].transform.position, 5);
                player.rigid.AddForce(dirVec * (-0.3f));
                break;
            // �ų� ���̽�
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

                        player.buffSprite.sprite = null;
                        player.attackSpeed -= upgradeDamage[level];
                        break;
                    case 2:
                        player.buffSprite.sprite = icon;
                        player.speed += upgradeDamage[level];

                        yield return new WaitForSeconds(skillDuringTime);

                        player.buffSprite.sprite = null;
                        player.speed -= upgradeDamage[level];
                        break;
                    case 3:
                        player.buffSprite.sprite = icon;
                        player.power += upgradeDamage[level];

                        yield return new WaitForSeconds(skillDuringTime);

                        player.buffSprite.sprite = null;
                        player.power -= upgradeDamage[level];
                        break;
                }
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
                    GameManager.instance.weapon[player.currentWeaponIndex].Shot(3, shotDir, player.transform.position + new Vector3(shotDir.x, shotDir.y, 1), 3);
                    yield return new WaitForSeconds(0.1f);

                    timer -= 0.1f;
                }
                break;
            // �ų� ��弦
            case 19:
                GameManager.instance.weapon[player.currentWeaponIndex].Shot(5, dirVec, GameManager.instance.weapon[player.currentWeaponIndex].transform.position, 10);
                break;
        }

        yield return null;
    }
}
