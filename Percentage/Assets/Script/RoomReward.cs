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
        // 보상 획득 시 나타난 텍스트 및 이미지 초기화
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
        // 랜덤 무기(스킬) 획득
        // 이미 가지고 있으면 강화, 없으면 획득
        int random = Random.Range(0, 5);
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
                    int upgradeValue = Random.Range(-5, 5);
                    upgradeText[0].gameObject.SetActive(true);
                    upgradeText[1].gameObject.SetActive(true);
                    upgradeText[0].text = type + " " + GameManager.instance.weaponData[random].weaponName + "강화에 성공하셨습니다.";
                    upgradeText[1].text = "강화에 성공하여 무기 공격력이 " + upgradeValue + "상승하였습니다.";
                }
                else if (upgradeRandom < 95)
                {
                    failedText.gameObject.SetActive(true);
                    failedText.text = type + " " + GameManager.instance.weaponData[random].weaponName + "강화에 실패하셨습니다.";
                }
                else
                {
                    destroyedText.gameObject.SetActive(true);
                    destroyedText.text = type + " " + GameManager.instance.weaponData[random].weaponName + "강화에 실패하여" + type + "이(가) 파괴되었습니다.";
                }

                break;
            }
        }
        if (!isAcquire)
        {
            // 획득 텍스트 노출
            acquireText[0].gameObject.SetActive(true);
            acquireText[0].text = type + " " + GameManager.instance.weaponData[random].weaponName + "을(를) 획득하셨습니다.";

            // 뽑은 무기가 내 직업이 낄 수 있다면 넣고, 아니면 반환
            if(GameManager.instance.player.role == (int)GameManager.instance.weaponData[random].weaponType)
            {
                list.Add(random);

                // 무기 생성
                GameObject newWeapon = GameManager.instance.GenerateWeapon();
                GameManager.instance.weapon[GameManager.instance.player.getWeaponCount] = newWeapon.AddComponent<Weapon>();
                GameManager.instance.weapon[GameManager.instance.player.getWeaponCount].Init(GameManager.instance.weaponData[random]);
                GameManager.instance.player.getWeaponCount++;

                // UI에 적용
                GameManager.instance.ui.isChanged = true;
            }
            else
            {
                acquireText[1].gameObject.SetActive(true);
                acquireText[1].text = "하지만 이 무기는 직업과 맞지 않아 버려졌습니다.";
            }
        }

        weaponSkillImage.gameObject.SetActive(true);
        if (type == "무기")
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
