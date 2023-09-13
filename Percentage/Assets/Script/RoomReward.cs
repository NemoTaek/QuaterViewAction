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
        foreach (int typeId in list)
        {
            if (typeId == random)
            {
                isAcquire = true;

                // ��ȭ���� ����
                int upgradeRandom = Random.Range(0, 100);
                if (upgradeRandom >= 0 && upgradeRandom < 60)
                {
                    int upgradeValue = Random.Range(-5, 5);
                    upgradeText[0].gameObject.SetActive(true);
                    upgradeText[1].gameObject.SetActive(true);
                    upgradeText[0].text = type + " " + GameManager.instance.weaponData[random].weaponName + "��ȭ�� �����ϼ̽��ϴ�.";
                    upgradeText[1].text = "��ȭ�� �����Ͽ� ���� ���ݷ��� " + upgradeValue + "����Ͽ����ϴ�.";
                }
                else if (upgradeRandom < 95)
                {
                    failedText.gameObject.SetActive(true);
                    failedText.text = type + " " + GameManager.instance.weaponData[random].weaponName + "��ȭ�� �����ϼ̽��ϴ�.";
                }
                else
                {
                    destroyedText.gameObject.SetActive(true);
                    destroyedText.text = type + " " + GameManager.instance.weaponData[random].weaponName + "��ȭ�� �����Ͽ�" + type + "��(��) �ı��Ǿ����ϴ�.";
                }

                break;
            }
        }
        if (!isAcquire)
        {
            // ȹ�� �ؽ�Ʈ ����
            acquireText[0].gameObject.SetActive(true);
            acquireText[0].text = type + " " + GameManager.instance.weaponData[random].weaponName + "��(��) ȹ���ϼ̽��ϴ�.";

            // ���� ���Ⱑ �� ������ �� �� �ִٸ� �ְ�, �ƴϸ� ��ȯ
            if(GameManager.instance.player.role == (int)GameManager.instance.weaponData[random].weaponType)
            {
                list.Add(random);

                // ���� ����
                GameObject newWeapon = GameManager.instance.GenerateWeapon();
                GameManager.instance.weapon[GameManager.instance.player.getWeaponCount] = newWeapon.AddComponent<Weapon>();
                GameManager.instance.weapon[GameManager.instance.player.getWeaponCount].Init(GameManager.instance.weaponData[random]);
                GameManager.instance.player.getWeaponCount++;

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
            weaponSkillImage.sprite = GameManager.instance.weaponData[random].weaponIcon;
        }
        else
        {
            //weaponImage.gameObject.SetActive(true);
            //weaponImage.sprite = GameManager.instance.weaponData[random].weaponIcon;
        }
    }
}
