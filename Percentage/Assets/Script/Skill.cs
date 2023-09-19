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
                        GameObject shockWave = GameManager.instance.ObjectPool.Get(6);
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
                GameManager.instance.weapon[player.currentWeaponIndex].Shot(9, dirVec, GameManager.instance.weapon[player.role].transform.position, 10);
                break;

            // ������ ���̾� ��ο�
            case 6:
                GameManager.instance.weapon[player.currentWeaponIndex].Shot(4, dirVec, GameManager.instance.weapon[player.role].transform.position, 5);
                GameManager.instance.weapon[player.currentWeaponIndex].Shot(4, -dirVec, GameManager.instance.weapon[player.role].transform.position, 5);
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
                // �� ������ �˾Ƴ��� �ű⿡ ���� �ڽ� ��ġ�Ѱ� ó�� �������� 10������ ��������
                break;
            // ������ ���丣�������
            case 9:
                // Ű�ٿ� �ؼ� 3�� �̻� �����ٰ� ���� �ǵ���
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
                // �÷��̾� �����¿�� 1���� ���ܷ� �ؼ� �������� 3�� ��ġ
                // ���� �ǰݴ��ص� ü�� �ȴ⵵�� ���� (���� �⵵��)
                break;
            // ���� �ϻ�
            case 14:
                // Ű�ٿ� �ؼ� 3�� �̻� �����ٰ� ���� �ǵ���
                break;

            // �ų� �齺�ܼ�
            case 16:
                GameManager.instance.weapon[player.currentWeaponIndex].Shot(5, dirVec, transform.position, 5);
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
            // �ų� �Ҹ���Ƽ
            case 18:
                // �÷��̾� ��ġ �������� ���� �������� ���ӽð����� ��ų�ҷ� �߻�
                break;
            // �ų� ��弦
            case 19:
                // Ű�ٿ� �ؼ� 3�� �̻� �����ٰ� ���� �ǵ���
                break;
        }

        yield return null;
    }
}
