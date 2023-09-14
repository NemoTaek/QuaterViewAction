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

    public void AcquireOrUpgrade(List<int> list, string type)
    {
        // ���� ����(��ų) ȹ��
        // �̹� ������ ������ ��ȭ, ������ ȹ��
        int random = Random.Range(0, 5);
        bool isAcquire = false;
        WeaponData randomWeaponData = GameManager.instance.weaponData[random];

        for (int i=0; i<list.Count; i++)
        {
            if (list[i] == random)
            {
                isAcquire = true;
                Weapon randomWeapon = GameManager.instance.weapon[random];
                int weaponLevel = randomWeapon.level;

                // ��ȭ���� ����
                int upgradeRandom = Random.Range(0, 100);
                if (upgradeRandom >= 0 && upgradeRandom < 60)
                {
                    upgradeText[0].gameObject.SetActive(true);
                    upgradeText[1].gameObject.SetActive(true);
                    upgradeText[0].text = type + " " + randomWeapon.name + "��ȭ�� �����ϼ̽��ϴ�.";
                    upgradeText[1].text = "��ȭ�� �����Ͽ� ���� ���ݷ��� " + randomWeapon.upgradeDamage[weaponLevel] + "����Ͽ����ϴ�.";
                    randomWeapon.damage += randomWeapon.upgradeDamage[weaponLevel];
                    randomWeapon.level++;
                }
                else if (upgradeRandom < 70)
                {
                    failedText.gameObject.SetActive(true);
                    if(weaponLevel > 0)
                    {
                        failedText.text = type + " " + randomWeaponData.weaponName + "��ȭ�� �����Ͽ� �ܰ谡 �϶��մϴ�.";
                        randomWeapon.damage -= randomWeapon.upgradeDamage[weaponLevel];
                        randomWeapon.level--;
                    }
                    else
                    {
                        failedText.text = type + " " + randomWeaponData.weaponName + "��ȭ�� �����Ͽ����ϴ�.";
                    }

                }
                else
                {
                    destroyedText.gameObject.SetActive(true);
                    if (random != GameManager.instance.player.role * 5)
                    {
                        destroyedText.text = type + " " + randomWeaponData.weaponName + "��ȭ�� �����Ͽ� " + type + "��(��) �ı��Ǿ����ϴ�.";

                        // ���� ���� ����, ���� ���⸦ �⺻����� ����
                        GameManager.instance.player.getWeaponCount--;
                        GameManager.instance.player.currentWeaponIndex = 0;

                        // ������Ʈ �ı�, ���� ����Ʈ���� ����
                        GameObject destoryWeapon = GameManager.instance.player.hand[GameManager.instance.player.role].haveWeapons[i].gameObject;
                        Destroy(destoryWeapon);
                        list.RemoveAt(i);

                        // �տ� �� ���� ������Ʈ, UI ����
                        GameManager.instance.player.hand[GameManager.instance.player.role].isChanged = true;
                        GameManager.instance.ui.isChanged = true;
                    }

                    // �⺻����� �ı� �ȵǵ���. �ٽ� ���� ������
                    else
                    {
                        destroyedText.gameObject.SetActive(false);
                        AcquireOrUpgrade(list, type);
                    }
                }

                break;
            }
        }
        if (!isAcquire)
        {
            // ȹ�� �ؽ�Ʈ ����
            acquireText[0].gameObject.SetActive(true);
            acquireText[0].text = type + " " + randomWeaponData.weaponName + "��(��) ȹ���ϼ̽��ϴ�.";

            // ���� ���Ⱑ �� ������ �� �� �ִٸ� �ְ�, �ƴϸ� ��ȯ
            if(GameManager.instance.player.role == (int)randomWeaponData.weaponType)
            {
                list.Add(random);

                // ���� ���� �� ȹ���� ����� ����Ī
                GameObject newWeapon = GameManager.instance.GenerateWeapon();
                GameManager.instance.weapon[GameManager.instance.player.getWeaponCount] = newWeapon.AddComponent<Weapon>();
                GameManager.instance.weapon[GameManager.instance.player.getWeaponCount].Init(randomWeaponData);
                GameManager.instance.player.getWeaponCount++;
                GameManager.instance.player.currentWeaponIndex = GameManager.instance.player.getWeaponCount - 1;
                GameManager.instance.player.hand[GameManager.instance.player.role].isChanged = true;

                // UI�� ����
                GameManager.instance.ui.isChanged = true;
            }
            else
            {
                acquireText[1].gameObject.SetActive(true);
                acquireText[1].text = "������ �� ����� ������ ���� �ʾ� ���������ϴ�.";
            }
        }

        weaponSkillImage.gameObject.SetActive(true);
        if (type == "����")
        {
            weaponSkillImage.sprite = randomWeaponData.weaponIcon;
        }
        else
        {
            //weaponImage.gameObject.SetActive(true);
            //weaponImage.sprite = randomWeaponData.weaponIcon;
        }
    }
}
