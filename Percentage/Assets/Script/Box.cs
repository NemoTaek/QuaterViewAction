using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    void Start()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.instance.isOpenBox = true;
            GameManager.instance.rewardBoxPanel.SetActive(true);

            // ���Ⱝȭ, ��ų��ȭ �� ���� ����
            int weaponOrSkill = Random.Range(0, 2);

            // �����̸�
            if (weaponOrSkill == 0)
            {
                AcquireOrUpgrade(GameManager.instance.player.acquireWeapons, "����");
            }

            // �����̸�
            else if (weaponOrSkill == 1)
            {
                AcquireOrUpgrade(GameManager.instance.player.acquireSkills, "��ų");
            }

            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        
    }

    void AcquireOrUpgrade(List<int> list, string type)
    {
        // ���� ����(��ų) ȹ��
        // �̹� ������ ������ ��ȭ, ������ ȹ��
        int random = Random.Range(0, 20);
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
                    Debug.Log("��ȭ ����");
                    int upgradeValue = Random.Range(-5, 5);
                    GameManager.instance.ui.upgradeText[0].gameObject.SetActive(true);
                    GameManager.instance.ui.upgradeText[1].gameObject.SetActive(true);
                    GameManager.instance.ui.upgradeText[0].text = type + "OOO" + "��ȭ�� �����ϼ̽��ϴ�.";
                    GameManager.instance.ui.upgradeText[1].text = "��ȭ�� �����Ͽ� ���� ���ݷ��� " + upgradeValue + "����Ͽ����ϴ�.";
                }
                else if(upgradeRandom < 95)
                {
                    Debug.Log("��ȭ ����");
                    GameManager.instance.ui.failedText.gameObject.SetActive(true);
                    GameManager.instance.ui.failedText.text = type + "OOO" + "��ȭ�� �����ϼ̽��ϴ�.";
                }
                else
                {
                    Debug.Log("�ı�");
                    GameManager.instance.ui.destroyedText.gameObject.SetActive(true);
                    GameManager.instance.ui.destroyedText.text = type + "OOO" + "��ȭ�� �����Ͽ�" + type + "��(��) �ı��Ǿ����ϴ�.";
                }

                break;
            }
        }
        if (!isAcquire)
        {
            Debug.Log("ȹ��");
            // ȹ�� �ؽ�Ʈ ����
            GameManager.instance.ui.acquireText[0].gameObject.SetActive(true);
            //GameManager.instance.ui.acquireText[1].gameObject.SetActive(true);
            GameManager.instance.ui.acquireText[0].text = type + "OOO" + "��/�� ȹ���ϼ̽��ϴ�.";
            list.Add(random);
        }
    }
}
