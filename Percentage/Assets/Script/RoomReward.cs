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
        WeaponData randomWeaponData = GameManager.instance.weaponData[random];

        for (int i=0; i<list.Count; i++)
        {
            if (list[i] == random)
            {
                isAcquire = true;
                Weapon randomWeapon = GameManager.instance.weapon[random];
                int weaponLevel = randomWeapon.level;

                // 강화로직 구현
                int upgradeRandom = Random.Range(0, 100);
                if (upgradeRandom >= 0 && upgradeRandom < 60)
                {
                    upgradeText[0].gameObject.SetActive(true);
                    upgradeText[1].gameObject.SetActive(true);
                    upgradeText[0].text = type + " " + randomWeapon.name + "강화에 성공하셨습니다.";
                    upgradeText[1].text = "강화에 성공하여 무기 공격력이 " + randomWeapon.upgradeDamage[weaponLevel] + "상승하였습니다.";
                    randomWeapon.damage += randomWeapon.upgradeDamage[weaponLevel];
                    randomWeapon.level++;
                }
                else if (upgradeRandom < 70)
                {
                    failedText.gameObject.SetActive(true);
                    if(weaponLevel > 0)
                    {
                        failedText.text = type + " " + randomWeaponData.weaponName + "강화에 실패하여 단계가 하락합니다.";
                        randomWeapon.damage -= randomWeapon.upgradeDamage[weaponLevel];
                        randomWeapon.level--;
                    }
                    else
                    {
                        failedText.text = type + " " + randomWeaponData.weaponName + "강화에 실패하였습니다.";
                    }

                }
                else
                {
                    destroyedText.gameObject.SetActive(true);
                    if (random != GameManager.instance.player.role * 5)
                    {
                        destroyedText.text = type + " " + randomWeaponData.weaponName + "강화에 실패하여 " + type + "이(가) 파괴되었습니다.";

                        // 무기 개수 감소, 장착 무기를 기본무기로 변경
                        GameManager.instance.player.getWeaponCount--;
                        GameManager.instance.player.currentWeaponIndex = 0;

                        // 오브젝트 파괴, 무기 리스트에서 삭제
                        GameObject destoryWeapon = GameManager.instance.player.hand[GameManager.instance.player.role].haveWeapons[i].gameObject;
                        Destroy(destoryWeapon);
                        list.RemoveAt(i);

                        // 손에 든 무기 업데이트, UI 적용
                        GameManager.instance.player.hand[GameManager.instance.player.role].isChanged = true;
                        GameManager.instance.ui.isChanged = true;
                    }

                    // 기본무기면 파괴 안되도록. 다시 로직 돌리자
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
            // 획득 텍스트 노출
            acquireText[0].gameObject.SetActive(true);
            acquireText[0].text = type + " " + randomWeaponData.weaponName + "을(를) 획득하셨습니다.";

            // 뽑은 무기가 내 직업이 낄 수 있다면 넣고, 아니면 반환
            if(GameManager.instance.player.role == (int)randomWeaponData.weaponType)
            {
                list.Add(random);

                // 무기 생성 후 획득한 무기로 스위칭
                GameObject newWeapon = GameManager.instance.GenerateWeapon();
                GameManager.instance.weapon[GameManager.instance.player.getWeaponCount] = newWeapon.AddComponent<Weapon>();
                GameManager.instance.weapon[GameManager.instance.player.getWeaponCount].Init(randomWeaponData);
                GameManager.instance.player.getWeaponCount++;
                GameManager.instance.player.currentWeaponIndex = GameManager.instance.player.getWeaponCount - 1;
                GameManager.instance.player.hand[GameManager.instance.player.role].isChanged = true;

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
            weaponSkillImage.sprite = randomWeaponData.weaponIcon;
        }
        else
        {
            //weaponImage.gameObject.SetActive(true);
            //weaponImage.sprite = randomWeaponData.weaponIcon;
        }
    }
}
