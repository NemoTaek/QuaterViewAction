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

            // 무기강화, 스킬강화 중 랜덤 선택
            int weaponOrSkill = Random.Range(0, 2);

            // 무기이면
            if (weaponOrSkill == 0)
            {
                AcquireOrUpgrade(GameManager.instance.player.acquireWeapons, "무기");
            }

            // 무기이면
            else if (weaponOrSkill == 1)
            {
                AcquireOrUpgrade(GameManager.instance.player.acquireSkills, "스킬");
            }

            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        
    }

    void AcquireOrUpgrade(List<int> list, string type)
    {
        // 랜덤 무기(스킬) 획득
        // 이미 가지고 있으면 강화, 없으면 획득
        int random = Random.Range(0, 20);
        bool isAcquire = false;
        foreach (int typeId in list)
        {
            if (typeId == random)
            {
                isAcquire = true;

                // 강화로직 구현
                int upgradeRandom = Random.Range(0, 100);
                if (upgradeRandom >= 0 && upgradeRandom < 60)
                {
                    Debug.Log("강화 성공");
                    int upgradeValue = Random.Range(-5, 5);
                    GameManager.instance.ui.upgradeText[0].gameObject.SetActive(true);
                    GameManager.instance.ui.upgradeText[1].gameObject.SetActive(true);
                    GameManager.instance.ui.upgradeText[0].text = type + "OOO" + "강화에 성공하셨습니다.";
                    GameManager.instance.ui.upgradeText[1].text = "강화에 성공하여 무기 공격력이 " + upgradeValue + "상승하였습니다.";
                }
                else if(upgradeRandom < 95)
                {
                    Debug.Log("강화 실패");
                    GameManager.instance.ui.failedText.gameObject.SetActive(true);
                    GameManager.instance.ui.failedText.text = type + "OOO" + "강화에 실패하셨습니다.";
                }
                else
                {
                    Debug.Log("파괴");
                    GameManager.instance.ui.destroyedText.gameObject.SetActive(true);
                    GameManager.instance.ui.destroyedText.text = type + "OOO" + "강화에 실패하여" + type + "이(가) 파괴되었습니다.";
                }

                break;
            }
        }
        if (!isAcquire)
        {
            Debug.Log("획득");
            // 획득 텍스트 노출
            GameManager.instance.ui.acquireText[0].gameObject.SetActive(true);
            //GameManager.instance.ui.acquireText[1].gameObject.SetActive(true);
            GameManager.instance.ui.acquireText[0].text = type + "OOO" + "을/를 획득하셨습니다.";
            list.Add(random);
        }
    }
}
