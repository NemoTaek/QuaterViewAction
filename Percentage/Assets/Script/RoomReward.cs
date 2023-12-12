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

                // �ְ� ������ ��� ���� ���θ� �����Ͽ� ���� ��ȭ���� �ʵ��� ����
                if(randomWeapon.level == randomWeapon.maxLevel)
                {
                    AudioManager.instance.EffectPlay(AudioManager.Effect.Success);
                    upgradeText[0].gameObject.SetActive(true);
                    upgradeText[0].text = "��� " + randomWeapon.name + "��(��) �̹� �ְ� �����Դϴ�.\n���̻� ��ȭ���� �ʽ��ϴ�.";
                }

                else
                {
                    // ��ȭ���� ����
                    int upgradeRandom = Random.Range(0, 100);
                    if (upgradeRandom >= 0 && upgradeRandom < 60)
                    {
                        AudioManager.instance.EffectPlay(AudioManager.Effect.Success);
                        upgradeText[0].gameObject.SetActive(true);
                        upgradeText[1].gameObject.SetActive(true);

                        randomWeapon.level++;
                        upgradeText[0].text = "��� " + randomWeapon.name + " ��ȭ�� �����ϼ̽��ϴ�.";
                        upgradeText[1].text = "��ȭ�� �����Ͽ� ��� ���ݷ��� " + randomWeapon.upgradeDamage[randomWeapon.level] + " ����Ͽ����ϴ�.";
                        randomWeapon.damage += randomWeapon.upgradeDamage[randomWeapon.level];

                        // ���� �������� ���Ⱑ ��ȭ�� �����ߴٸ� ���� ��ü �� ���ݷ� ��ȭ�� ������ ������ ���ݷ� ������ �ٷ� �ݿ�
                        if (i == GameManager.instance.player.currentWeaponIndex) GameManager.instance.player.powerUp += randomWeapon.upgradeDamage[randomWeapon.level];
                    }
                    else if (upgradeRandom < 95)
                    {
                        AudioManager.instance.EffectPlay(AudioManager.Effect.Fail);
                        failedText.gameObject.SetActive(true);

                        if (randomWeapon.level > 0)
                        {
                            failedText.text = "��� " + randomWeaponData.weaponName + " ��ȭ�� �����Ͽ� �ܰ谡 �϶��մϴ�.";

                            // ���� �������� ������ �ܰ谡 �϶��ߴٸ� ���� ��ü �� ���ݷ� ��ȭ�� ������ ������ ���ݷ� ���ҿ� �ٷ� �ݿ�
                            randomWeapon.damage -= randomWeapon.upgradeDamage[randomWeapon.level];
                            if (i == GameManager.instance.player.currentWeaponIndex) GameManager.instance.player.powerUp -= randomWeapon.upgradeDamage[randomWeapon.level];
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
                            // ���� �ı� �������� �ִٸ� �ش� �������� �����ϰ� �ı��� ���´�.
                            Item isInGetItemList = GameManager.instance.getItemList.Find(x => x.id == 22);
                            if (isInGetItemList)
                            {
                                AudioManager.instance.EffectPlay(AudioManager.Effect.Fail);

                                destroyedText.text = "��� " + randomWeaponData.weaponName + " ��ȭ�� �����Ͽ� ��� �ı�... �� �� ������\n" +
                                    "�ı� ���� �������� �־� �ı����� �ʾҽ��ϴ�.";

                                GameManager.instance.getItemList.Remove(isInGetItemList);
                                Item[] playerItems = GameManager.instance.player.getItems.GetComponentsInChildren<Item>();
                                foreach (Item item in playerItems)
                                {
                                    if (item.id == 22)
                                    {
                                        Destroy(item.gameObject);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                AudioManager.instance.EffectPlay(AudioManager.Effect.Destroyed);

                                destroyedText.text = "��� " + randomWeaponData.weaponName + " ��ȭ�� �����Ͽ� ��� �ı��Ǿ����ϴ�.\n" +
                                    "���� ���� " + ((random % 5) + 1) + "���� ȹ���Ͽ����ϴ�."; ;

                                /* �����غ��� �ı��� ���� ���� �����ϴ�
                                1. �� Ŭ���� �������� ���� �ڽ����� �쿬�� ���� �������� �ʰ� ������ �� �� �ı��Ǿ���.
                                ���� �������� �ƴ� ���� �ı��Ǿ��� ������ ���ݷ� ��ȭ�� �ʿ����� �ʴ�.
                                ������ -> hand�� �ִ� �� ������ ��ĭ�� ������ shift �Ѵ�. -> ����!


                                2. �ڽ����� �쿬�� ���� �����ϰ� �ִ� ���� �ı��ǰų�, ���� Ŭ���� �������� ���� ��ȭ �ֹ������� �ı��Ǿ���.
                                �ı��Ǵ� ������ ���ݷ� ���� -> �����ϴ� ���⸦ �⺻����(0��)�� ���� -> ������ -> hand�� �ִ� �� ������ ��ĭ�� ������ shift �Ѵ�. -> ����!
                                */

                                // ���� �������� ���Ⱑ �ı��Ǿ�����
                                if (i == GameManager.instance.player.currentWeaponIndex)
                                {
                                    // �ı��� ���� ���ݷ¸�ŭ ����
                                    GameManager.instance.player.powerUp -= randomWeapon.damage;

                                    // ���� ���⸦ �⺻����� ����
                                    GameManager.instance.player.currentWeaponIndex = 0;
                                    GameManager.instance.player.hand[GameManager.instance.player.role].isWeaponChanged = true;
                                }
                                // �ı��� ���⺸�� �� ���� ���⸦ �����ϰ� ������
                                else if (i < GameManager.instance.player.currentWeaponIndex)
                                {
                                    // ������ ��ܾ��ϴϱ� �ε��� ����
                                    GameManager.instance.player.currentWeaponIndex--;
                                }
                                // �տ������� ��ȭ ����

                                // ���� ���� ����
                                GameManager.instance.player.getWeaponCount--;

                                // ������Ʈ �ı�, ���� ����Ʈ���� ����
                                GameObject destoryWeapon = GameManager.instance.weapon[i].gameObject;
                                Destroy(destoryWeapon);

                                // �տ� �� ���� ������Ʈ, UI ����
                                GameManager.instance.ui.isChanged = true;

                                // ���� ��ĭ�� ������ shift
                                for (int j = i; j < GameManager.instance.player.getWeaponCount; j++)
                                {
                                    GameManager.instance.weapon[j] = GameManager.instance.weapon[j + 1];
                                }

                                // �������� null ������ ����
                                for (int j = GameManager.instance.player.getWeaponCount; j < GameManager.instance.player.hand[GameManager.instance.player.role].haveWeapons.Length; j++)
                                {
                                    GameManager.instance.weapon[j] = null;
                                }

                                // �ı� �� ���� ���� ������ ȹ��
                                GameManager.instance.GameDataManage("���� ����", (random % 5) + 1);
                            }
                        }

                        // �⺻����� �ı� �ȵǵ���. �ٽ� ���� ������
                        else
                        {
                            destroyedText.gameObject.SetActive(false);
                            AcquireOrUpgrade(random, randomWeaponData);
                            break;
                        }
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
                AudioManager.instance.EffectPlay(AudioManager.Effect.Success);

                // �켱 ���� ������ ���� ��Ȱ��ȭ �� �ش� ������ ���ݷ¸�ŭ ����
                GameManager.instance.player.hand[GameManager.instance.player.role].haveWeapons[GameManager.instance.player.currentWeaponIndex].gameObject.SetActive(false);
                GameManager.instance.player.powerUp -= GameManager.instance.weapon[GameManager.instance.player.currentWeaponIndex].damage;

                // ���� ����
                GameObject newWeapon = GameManager.instance.GenerateWeapon();
                GameManager.instance.weapon[GameManager.instance.player.getWeaponCount] = newWeapon.AddComponent<Weapon>();
                GameManager.instance.weapon[GameManager.instance.player.getWeaponCount].Init(randomWeaponData);
                GameManager.instance.weapon[GameManager.instance.player.getWeaponCount].name = GameManager.instance.weapon[GameManager.instance.player.getWeaponCount].weaponName;

                // ȹ���� ����� ����Ī �� ���ݷ� ����
                GameManager.instance.player.getWeaponCount++;
                GameManager.instance.player.currentWeaponIndex = GameManager.instance.player.getWeaponCount - 1;
                GameManager.instance.player.hand[GameManager.instance.player.role].isWeaponChanged = true;

                // UI�� ����
                GameManager.instance.ui.isChanged = true;
            }
            else
            {
                AudioManager.instance.EffectPlay(AudioManager.Effect.Fail);
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

                // �ְ� ������ ��� ���� ���θ� �����Ͽ� ���� ��ȭ���� �ʵ��� ����
                if (randomSkill.level == randomSkill.maxLevel)
                {
                    AudioManager.instance.EffectPlay(AudioManager.Effect.Success);
                    upgradeText[0].gameObject.SetActive(true);
                    upgradeText[0].text = "��ų " + randomSkill.name + "��(��) �̹� �ְ� �����Դϴ�.\n���̻� ��ȭ���� �ʽ��ϴ�.";
                }
                else
                {
                    // ��ȭ���� ����
                    int upgradeRandom = Random.Range(0, 100);
                    if (upgradeRandom >= 0 && upgradeRandom < 60)
                    {
                        AudioManager.instance.EffectPlay(AudioManager.Effect.Success);
                        upgradeText[0].gameObject.SetActive(true);
                        upgradeText[1].gameObject.SetActive(true);

                        // ���� ����
                        randomSkill.level++;
                        upgradeText[0].text = "��ų " + randomSkill.name + " ��ȭ�� �����ϼ̽��ϴ�.";
                        upgradeText[1].text = "��ȭ�� �����Ͽ� ��ų ���ݷ��� " + randomSkill.upgradeDamage[randomSkill.level] + " ����Ͽ����ϴ�.";
                        randomSkill.damage += randomSkill.upgradeDamage[randomSkill.level];
                    }
                    else if (upgradeRandom < 95)
                    {
                        AudioManager.instance.EffectPlay(AudioManager.Effect.Fail);
                        failedText.gameObject.SetActive(true);

                        if (randomSkill.level > 0)
                        {
                            failedText.text = "��ų " + randomSkillData.skillName + " ��ȭ�� �����Ͽ� �ܰ谡 �϶��մϴ�.";
                            randomSkill.damage -= randomSkill.upgradeDamage[randomSkill.level];
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
                            // ���� �ı� �������� �ִٸ� �ش� �������� �����ϰ� �ı��� ���´�.
                            Item isInGetItemList = GameManager.instance.getItemList.Find(x => x.id == 22);
                            if (isInGetItemList)
                            {
                                AudioManager.instance.EffectPlay(AudioManager.Effect.Fail);
                                destroyedText.text = "��� " + randomSkillData.skillName + " ��ȭ�� �����Ͽ� ��ų�� �ı�... �� �� ������\n" +
                                    "�ı� ���� �������� �־� �ı����� �ʾҽ��ϴ�.";

                                GameManager.instance.getItemList.Remove(isInGetItemList);
                                Item[] playerItems = GameManager.instance.player.getItems.GetComponentsInChildren<Item>();
                                foreach (Item item in playerItems)
                                {
                                    if (item.id == 22)
                                    {
                                        Destroy(item.gameObject);
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                AudioManager.instance.EffectPlay(AudioManager.Effect.Destroyed);
                                destroyedText.text = "��ų " + randomSkillData.skillName + " ��ȭ�� �����Ͽ� ��ų�� �ı��Ǿ����ϴ�.\n" + 
                                    "���� ���� " + ((random % 5) + 1) + "���� ȹ���Ͽ����ϴ�.";

                                // ���� ������� ��ų�� �ı��Ǿ�����
                                if (i == GameManager.instance.player.currentSkillIndex)
                                {
                                    // ��ų�� �⺻��ų�� ����
                                    GameManager.instance.player.currentSkillIndex = 0;
                                }
                                // �ı��� ��ų���� �� ���� ��ų�� ����ϰ� ������
                                else if (i < GameManager.instance.player.currentSkillIndex)
                                {
                                    // ������ ��ܾ��ϴϱ� �ε��� ����
                                    GameManager.instance.player.currentSkillIndex--;
                                }
                                // �տ������� ��ȭ ����

                                // ��ų ���� ����
                                GameManager.instance.player.getSkillCount--;

                                // ������Ʈ �ı�, ��ų ����Ʈ���� ����
                                GameObject destorySkill = GameManager.instance.skill[i].gameObject;
                                Destroy(destorySkill);

                                // �տ� �� ���� ������Ʈ, UI ����
                                GameManager.instance.player.hand[GameManager.instance.player.role].isSkillChanged = true;
                                GameManager.instance.ui.isChanged = true;

                                // ��ų ��ĭ�� ������ shift
                                for (int j = i; j < GameManager.instance.player.getSkillCount; j++)
                                {
                                    GameManager.instance.skill[j] = GameManager.instance.skill[j + 1];
                                }

                                // �������� null ������ ����
                                for (int j = GameManager.instance.player.getSkillCount; j < 5; j++)
                                {
                                    GameManager.instance.skill[j] = null;
                                }

                                // �ı� �� ���� ���� ������ ȹ��
                                GameManager.instance.GameDataManage("���� ����", (random % 5) + 1);
                            }
                        }

                        // �⺻��ų�̸� �ı� �ȵǵ���. �ٽ� ���� ������
                        else
                        {
                            destroyedText.gameObject.SetActive(false);
                            AcquireOrUpgrade(random, randomSkillData);
                            break;
                        }
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
                AudioManager.instance.EffectPlay(AudioManager.Effect.Success);

                // ���� ���� �� ȹ���� ����� ����Ī
                GameObject newSkill = GameManager.instance.GenerateSkill();
                GameManager.instance.skill[GameManager.instance.player.getSkillCount] = newSkill.AddComponent<Skill>();
                GameManager.instance.skill[GameManager.instance.player.getSkillCount].Init(randomSkillData);
                GameManager.instance.skill[GameManager.instance.player.getSkillCount].name = GameManager.instance.skill[GameManager.instance.player.getSkillCount].skillName;
                GameManager.instance.player.getSkillCount++;
                //GameManager.instance.player.currentSkillIndex = GameManager.instance.player.getSkillCount - 1;
                GameManager.instance.player.hand[GameManager.instance.player.role].isSkillChanged = true;

                // UI�� ����
                GameManager.instance.ui.isChanged = true;
            }
            else
            {
                AudioManager.instance.EffectPlay(AudioManager.Effect.Fail);
                acquireText[1].gameObject.SetActive(true);
                acquireText[1].text = "������ �� ��ų�� ������ ���� �ʾ� ���������ϴ�.";
            }
        }

        weaponSkillImage.gameObject.SetActive(true);
        weaponSkillImage.sprite = randomSkillData.skillIcon;
    }
}
