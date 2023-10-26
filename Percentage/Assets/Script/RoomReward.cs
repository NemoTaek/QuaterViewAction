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

    public void AcquireOrUpgrade(int random, WeaponData randomWeaponData)
    {
        // 랜덤 장비 획득
        // 이미 가지고 있으면 강화, 없으면 획득
        bool isAcquire = false;

        for (int i = 0; i < 5; i++)
        {
            if (i < GameManager.instance.player.getWeaponCount && GameManager.instance.weapon[i].id == random)
            {
                isAcquire = true;
                Weapon randomWeapon = GameManager.instance.weapon[i];

                // 최고 레벨인 경우 그의 공로를 인정하여 더는 강화하지 않도록 설정
                if(randomWeapon.level == randomWeapon.maxLevel)
                {
                    AcquireOrUpgrade(random, randomWeaponData);
                    break;
                }

                // 강화로직 구현
                int upgradeRandom = Random.Range(0, 100);
                if (upgradeRandom >= 0 && upgradeRandom < 60)
                {
                    upgradeText[0].gameObject.SetActive(true);
                    upgradeText[1].gameObject.SetActive(true);

                    upgradeText[0].text = "장비 " + randomWeapon.name + " 강화에 성공하셨습니다.";
                    upgradeText[1].text = "강화에 성공하여 장비 공격력이 " + randomWeapon.upgradeDamage[randomWeapon.level] + " 상승하였습니다.";

                    randomWeapon.level++;
                    randomWeapon.damage += randomWeapon.upgradeDamage[randomWeapon.level];

                    // 현재 착용중인 무기가 강화에 성공했다면 무기 교체 시 공격력 변화에 오류가 없도록 공격력 증가에 바로 반영
                    if (i == GameManager.instance.player.currentWeaponIndex) GameManager.instance.player.powerUp += randomWeapon.upgradeDamage[randomWeapon.level];
                }
                else if (upgradeRandom < 95)
                {
                    failedText.gameObject.SetActive(true);
                    if(randomWeapon.level > 0)
                    {
                        failedText.text = "장비 " + randomWeaponData.weaponName + " 강화에 실패하여 단계가 하락합니다.";

                        // 현재 착용중인 무기의 단계가 하락했다면 무기 교체 시 공격력 변화에 오류가 없도록 공격력 감소에 바로 반영
                        randomWeapon.damage -= randomWeapon.upgradeDamage[randomWeapon.level];
                        if (i == GameManager.instance.player.currentWeaponIndex) GameManager.instance.player.powerUp -= randomWeapon.upgradeDamage[randomWeapon.level];
                        randomWeapon.level--;
                    }
                    else
                    {
                        failedText.text = "장비 " + randomWeaponData.weaponName + " 강화에 실패하였습니다.";
                    }
                }
                else
                {
                    destroyedText.gameObject.SetActive(true);
                    if (random != GameManager.instance.player.role * 5)
                    {
                        destroyedText.text = "장비 " + randomWeaponData.weaponName + " 강화에 실패하여 장비가 파괴되었습니다.";

                        // 파괴된 무기 공격력만큼 감소
                        GameManager.instance.player.powerUp -= randomWeapon.damage;

                        // 무기 개수 감소, 장착 무기를 기본무기로 변경 후 공격력 증가
                        GameManager.instance.player.getWeaponCount--;
                        GameManager.instance.player.currentWeaponIndex = 0;
                        //GameManager.instance.player.powerUp += GameManager.instance.player.hand[GameManager.instance.player.role].haveWeapons[0].damage;

                        // 오브젝트 파괴, 무기 리스트에서 삭제
                        GameObject destoryWeapon = GameManager.instance.player.hand[GameManager.instance.player.role].haveWeapons[i].gameObject;
                        Destroy(destoryWeapon);

                        // 손에 든 무기 업데이트, UI 적용
                        GameManager.instance.player.hand[GameManager.instance.player.role].isWeaponChanged = true;
                        GameManager.instance.ui.isChanged = true;
                    }

                    // 기본무기면 파괴 안되도록. 다시 로직 돌리자
                    else
                    {
                        destroyedText.gameObject.SetActive(false);
                        AcquireOrUpgrade(random, randomWeaponData);
                        break;
                    }
                }

                break;
            }
        }
        if (!isAcquire)
        {
            // 획득 텍스트 노출
            acquireText[0].gameObject.SetActive(true);
            acquireText[0].text = "장비 " + randomWeaponData.weaponName + "을(를) 획득하셨습니다.";

            // 뽑은 무기가 내 직업이 낄 수 있다면 넣고, 아니면 반환
            if(GameManager.instance.player.role == (int)randomWeaponData.weaponType)
            {
                // 우선 현재 보유한 무기 비활성화 후 해당 무기의 공격력만큼 감소
                GameManager.instance.player.hand[GameManager.instance.player.role].haveWeapons[GameManager.instance.player.currentWeaponIndex].gameObject.SetActive(false);
                GameManager.instance.player.powerUp -= GameManager.instance.player.hand[GameManager.instance.player.role].haveWeapons[GameManager.instance.player.currentWeaponIndex].damage;

                // 무기 생성
                GameObject newWeapon = GameManager.instance.GenerateWeapon();
                GameManager.instance.weapon[GameManager.instance.player.getWeaponCount] = newWeapon.AddComponent<Weapon>();
                GameManager.instance.weapon[GameManager.instance.player.getWeaponCount].Init(randomWeaponData);
                GameManager.instance.weapon[GameManager.instance.player.getWeaponCount].name = GameManager.instance.weapon[GameManager.instance.player.getWeaponCount].weaponName;

                // 획득한 무기로 스위칭 후 공격력 증가
                GameManager.instance.player.getWeaponCount++;
                GameManager.instance.player.currentWeaponIndex = GameManager.instance.player.getWeaponCount - 1;
                GameManager.instance.player.hand[GameManager.instance.player.role].isWeaponChanged = true;

                // UI에 적용
                GameManager.instance.ui.isChanged = true;
            }
            else
            {
                acquireText[1].gameObject.SetActive(true);
                acquireText[1].text = "하지만 이 장비는 직업과 맞지 않아 버려졌습니다.";
            }
        }

        weaponSkillImage.gameObject.SetActive(true);
        weaponSkillImage.sprite = randomWeaponData.weaponIcon;
    }

    public void AcquireOrUpgrade(int random, SkillData randomSkillData)
    {
        // 랜덤 스킬 획득
        // 이미 가지고 있으면 강화, 없으면 획득
        bool isAcquire = false;

        for (int i = 0; i < 5; i++)
        {
            if (i < GameManager.instance.player.getSkillCount && GameManager.instance.skill[i].id == random)
            {
                isAcquire = true;
                Skill randomSkill = GameManager.instance.skill[i];

                // 최고 레벨인 경우 그의 공로를 인정하여 더는 강화하지 않도록 설정
                if (randomSkill.level == randomSkill.maxLevel)
                {
                    AcquireOrUpgrade(random, randomSkillData);
                    break;
                }

                // 강화로직 구현
                int upgradeRandom = Random.Range(0, 100);
                if (upgradeRandom >= 0 && upgradeRandom < 60)
                {
                    upgradeText[0].gameObject.SetActive(true);
                    upgradeText[1].gameObject.SetActive(true);

                    // 레벨 증가
                    randomSkill.level++;
                    upgradeText[0].text = "스킬 " + randomSkill.name + " 강화에 성공하셨습니다.";
                    upgradeText[1].text = "강화에 성공하여 스킬 공격력이 " + randomSkill.upgradeDamage[randomSkill.level] + " 상승하였습니다.";
                    randomSkill.damage += randomSkill.upgradeDamage[randomSkill.level];
                }
                else if (upgradeRandom < 95)
                {
                    failedText.gameObject.SetActive(true);
                    if (randomSkill.level > 0)
                    {
                        failedText.text = "스킬 " + randomSkillData.skillName + " 강화에 실패하여 단계가 하락합니다.";
                        randomSkill.damage -= randomSkill.upgradeDamage[randomSkill.level];
                        randomSkill.level--;
                    }
                    else
                    {
                        failedText.text = "스킬 " + randomSkillData.skillName + " 강화에 실패하였습니다.";
                    }

                }
                else
                {
                    destroyedText.gameObject.SetActive(true);
                    if (random != GameManager.instance.player.role * 5)
                    {
                        destroyedText.text = "스킬 " + randomSkillData.skillName + " 강화에 실패하여 스킬이 파괴되었습니다.";

                        // 무기 개수 감소, 장착 무기를 기본무기로 변경
                        GameManager.instance.player.getSkillCount--;
                        GameManager.instance.player.currentSkillIndex = 0;

                        // 오브젝트 파괴, 무기 리스트에서 삭제
                        GameObject destorySkill = GameManager.instance.player.hand[GameManager.instance.player.role].haveSkills[i].gameObject;
                        Destroy(destorySkill);

                        // 손에 든 무기 업데이트, UI 적용
                        GameManager.instance.player.hand[GameManager.instance.player.role].isSkillChanged = true;
                        GameManager.instance.ui.isChanged = true;
                    }

                    // 기본스킬이면 파괴 안되도록. 다시 로직 돌리자
                    else
                    {
                        destroyedText.gameObject.SetActive(false);
                        AcquireOrUpgrade(random, randomSkillData);
                        break;
                    }
                }

                break;
            }
        }
        if (!isAcquire)
        {
            // 획득 텍스트 노출
            acquireText[0].gameObject.SetActive(true);
            acquireText[0].text = "스킬 " + randomSkillData.skillName + "을(를) 획득하셨습니다.";

            // 뽑은 무기가 내 직업이 낄 수 있다면 넣고, 아니면 반환
            if (GameManager.instance.player.role == (int)randomSkillData.usableRoleType)
            {
                // 무기 생성 후 획득한 무기로 스위칭
                GameObject newSkill = GameManager.instance.GenerateSkill();
                GameManager.instance.skill[GameManager.instance.player.getSkillCount] = newSkill.AddComponent<Skill>();
                GameManager.instance.skill[GameManager.instance.player.getSkillCount].Init(randomSkillData);
                GameManager.instance.skill[GameManager.instance.player.getSkillCount].name = GameManager.instance.skill[GameManager.instance.player.getSkillCount].skillName;
                GameManager.instance.player.getSkillCount++;
                //GameManager.instance.player.currentSkillIndex = GameManager.instance.player.getSkillCount - 1;
                GameManager.instance.player.hand[GameManager.instance.player.role].isSkillChanged = true;

                // UI에 적용
                GameManager.instance.ui.isChanged = true;
            }
            else
            {
                acquireText[1].gameObject.SetActive(true);
                acquireText[1].text = "하지만 이 스킬은 직업과 맞지 않아 버려졌습니다.";
            }
        }

        weaponSkillImage.gameObject.SetActive(true);
        weaponSkillImage.sprite = randomSkillData.skillIcon;
    }
}
