using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomReward : MonoBehaviour
{
    public Text[] acquireText;
    public Text[] upgradeText;
    public Text failedText;
    public Text destroyedText;
    public Image weaponSkillImage;

    void OnEnable()
    {
        // ���� ȹ�� �� ��Ÿ�� �ؽ�Ʈ �� �̹��� �ʱ�ȭ
        acquireText[0].gameObject.SetActive(false);
        acquireText[1].gameObject.SetActive(false);
        upgradeText[0].gameObject.SetActive(false);
        upgradeText[1].gameObject.SetActive(false);
        failedText.gameObject.SetActive(false);
        destroyedText.gameObject.SetActive(false);
        weaponSkillImage.gameObject.SetActive(false);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AcquireOrUpgrade(int random, WeaponData randomWeaponData)
    {
        // ���� ��� ȹ��
        // �̹� ������ ������ ��ȭ, ������ ȹ��
        bool isAcquire = false;

        for (int i = 0; i < 5; i++)
        {
            if (i < GameManager.instance.player.getWeaponCount && GameManager.instance.weapon[i].id == random)
            {
                isAcquire = true;
                Weapon randomWeapon = GameManager.instance.weapon[i];
                int weaponLevel = randomWeapon.level;

                // ��ȭ���� ����
                int upgradeRandom = Random.Range(0, 100);
                if (upgradeRandom >= 0 && upgradeRandom < 60)
                {
                    upgradeText[0].gameObject.SetActive(true);
                    upgradeText[1].gameObject.SetActive(true);
                    upgradeText[0].text = "��� " + randomWeapon.name + " ��ȭ�� �����ϼ̽��ϴ�.";
                    upgradeText[1].text = "��ȭ�� �����Ͽ� ��� ���ݷ��� " + randomWeapon.upgradeDamage[weaponLevel] + " ����Ͽ����ϴ�.";
                    randomWeapon.damage += randomWeapon.upgradeDamage[weaponLevel];
                    randomWeapon.level++;
                }
                else if (upgradeRandom < 95)
                {
                    failedText.gameObject.SetActive(true);
                    if(weaponLevel > 0)
                    {
                        failedText.text = "��� " + randomWeaponData.weaponName + " ��ȭ�� �����Ͽ� �ܰ谡 �϶��մϴ�.";
                        randomWeapon.damage -= randomWeapon.upgradeDamage[weaponLevel];
                        randomWeapon.level--;
                    }
                    else
                    {
                        failedText.text = "��� " + randomWeaponData.weaponName + " ��ȭ�� �����Ͽ����ϴ�.";
                    }

                }
                else
                {
                    destroyedText.gameObject.SetActive(true);
                    if (random != GameManager.instance.player.role * 5)
                    {
                        destroyedText.text = "��� " + randomWeaponData.weaponName + " ��ȭ�� �����Ͽ� ��� �ı��Ǿ����ϴ�.";

                        // ���� ���� ����, ���� ���⸦ �⺻����� ����
                        GameManager.instance.player.getWeaponCount--;
                        GameManager.instance.player.currentWeaponIndex = 0;

                        // ������Ʈ �ı�, ���� ����Ʈ���� ����
                        GameObject destoryWeapon = GameManager.instance.player.hand[GameManager.instance.player.role].haveWeapons[i].gameObject;
                        Destroy(destoryWeapon);

                        // �տ� �� ���� ������Ʈ, UI ����
                        GameManager.instance.player.hand[GameManager.instance.player.role].isChanged = true;
                        GameManager.instance.ui.isChanged = true;
                    }

                    // �⺻����� �ı� �ȵǵ���. �ٽ� ���� ������
                    else
                    {
                        destroyedText.gameObject.SetActive(false);
                        AcquireOrUpgrade(random, randomWeaponData);
                    }
                }

                break;
            }
        }
        if (!isAcquire)
        {
            // ȹ�� �ؽ�Ʈ ����
            acquireText[0].gameObject.SetActive(true);
            acquireText[0].text = "��� " + randomWeaponData.weaponName + "��(��) ȹ���ϼ̽��ϴ�.";

            // ���� ���Ⱑ �� ������ �� �� �ִٸ� �ְ�, �ƴϸ� ��ȯ
            if(GameManager.instance.player.role == (int)randomWeaponData.weaponType)
            {

                // ���� ���� �� ȹ���� ����� ����Ī
                GameObject newWeapon = GameManager.instance.GenerateWeapon();
                GameManager.instance.weapon[GameManager.instance.player.getWeaponCount] = newWeapon.AddComponent<Weapon>();
                GameManager.instance.weapon[GameManager.instance.player.getWeaponCount].Init(randomWeaponData);
                GameManager.instance.weapon[GameManager.instance.player.getWeaponCount].name = GameManager.instance.weapon[GameManager.instance.player.getWeaponCount].weaponName;
                GameManager.instance.player.getWeaponCount++;
                //GameManager.instance.player.currentWeaponIndex = GameManager.instance.player.getWeaponCount - 1;
                GameManager.instance.player.hand[GameManager.instance.player.role].isChanged = true;

                // UI�� ����
                GameManager.instance.ui.isChanged = true;
            }
            else
            {
                acquireText[1].gameObject.SetActive(true);
                acquireText[1].text = "������ �� ���� ������ ���� �ʾ� ���������ϴ�.";
            }
        }

        weaponSkillImage.gameObject.SetActive(true);
        weaponSkillImage.sprite = randomWeaponData.weaponIcon;
    }

    public void AcquireOrUpgrade(int random, SkillData randomSkillData)
    {
        // ���� ��ų ȹ��
        // �̹� ������ ������ ��ȭ, ������ ȹ��
        bool isAcquire = false;

        for (int i = 0; i < 5; i++)
        {
            if (i < GameManager.instance.player.getSkillCount && GameManager.instance.skill[i].id == random)
            {
                isAcquire = true;
                Skill randomSkill = GameManager.instance.skill[i];
                int weaponLevel = randomSkill.level;

                // ��ȭ���� ����
                int upgradeRandom = Random.Range(0, 100);
                if (upgradeRandom >= 0 && upgradeRandom < 60)
                {
                    upgradeText[0].gameObject.SetActive(true);
                    upgradeText[1].gameObject.SetActive(true);
                    upgradeText[0].text = "��ų " + randomSkill.name + " ��ȭ�� �����ϼ̽��ϴ�.";
                    upgradeText[1].text = "��ȭ�� �����Ͽ� ��ų ���ݷ��� " + randomSkill.upgradeDamage[weaponLevel] + " ����Ͽ����ϴ�.";
                    randomSkill.damage += randomSkill.upgradeDamage[weaponLevel];
                    randomSkill.level++;
                }
                else if (upgradeRandom < 95)
                {
                    failedText.gameObject.SetActive(true);
                    if (weaponLevel > 0)
                    {
                        failedText.text = "��ų " + randomSkillData.skillName + " ��ȭ�� �����Ͽ� �ܰ谡 �϶��մϴ�.";
                        randomSkill.damage -= randomSkill.upgradeDamage[weaponLevel];
                        randomSkill.level--;
                    }
                    else
                    {
                        failedText.text = "��ų " + randomSkillData.skillName + " ��ȭ�� �����Ͽ����ϴ�.";
                    }

                }
                else
                {
                    destroyedText.gameObject.SetActive(true);
                    if (random != GameManager.instance.player.role * 5)
                    {
                        destroyedText.text = "��ų " + randomSkillData.skillName + " ��ȭ�� �����Ͽ� ��ų�� �ı��Ǿ����ϴ�.";

                        // ���� ���� ����, ���� ���⸦ �⺻����� ����
                        GameManager.instance.player.getSkillCount--;
                        GameManager.instance.player.currentSkillIndex = 0;

                        // ������Ʈ �ı�, ���� ����Ʈ���� ����
                        GameObject destorySkill = GameManager.instance.player.hand[GameManager.instance.player.role].haveSkills[i].gameObject;
                        Destroy(destorySkill);

                        // �տ� �� ���� ������Ʈ, UI ����
                        GameManager.instance.player.hand[GameManager.instance.player.role].isChanged = true;
                        GameManager.instance.ui.isChanged = true;
                    }

                    // �⺻��ų�̸� �ı� �ȵǵ���. �ٽ� ���� ������
                    else
                    {
                        destroyedText.gameObject.SetActive(false);
                        AcquireOrUpgrade(random, randomSkillData);
                    }
                }

                break;
            }
        }
        if (!isAcquire)
        {
            // ȹ�� �ؽ�Ʈ ����
            acquireText[0].gameObject.SetActive(true);
            acquireText[0].text = "��ų " + randomSkillData.skillName + "��(��) ȹ���ϼ̽��ϴ�.";

            // ���� ���Ⱑ �� ������ �� �� �ִٸ� �ְ�, �ƴϸ� ��ȯ
            if (GameManager.instance.player.role == (int)randomSkillData.usableRoleType)
            {
                // ���� ���� �� ȹ���� ����� ����Ī
                GameObject newSkill = GameManager.instance.GenerateSkill();
                GameManager.instance.skill[GameManager.instance.player.getSkillCount] = newSkill.AddComponent<Skill>();
                GameManager.instance.skill[GameManager.instance.player.getSkillCount].Init(randomSkillData);
                GameManager.instance.skill[GameManager.instance.player.getSkillCount].name = GameManager.instance.skill[GameManager.instance.player.getSkillCount].skillName;
                GameManager.instance.player.getSkillCount++;
                //GameManager.instance.player.currentSkillIndex = GameManager.instance.player.getSkillCount - 1;
                GameManager.instance.player.hand[GameManager.instance.player.role].isChanged = true;

                // UI�� ����
                GameManager.instance.ui.isChanged = true;
            }
            else
            {
                acquireText[1].gameObject.SetActive(true);
                acquireText[1].text = "������ �� ��ų�� ������ ���� �ʾ� ���������ϴ�.";
            }
        }

        weaponSkillImage.gameObject.SetActive(true);
        weaponSkillImage.sprite = randomSkillData.skillIcon;
    }
}
