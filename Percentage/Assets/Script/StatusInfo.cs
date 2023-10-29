using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusInfo : MonoBehaviour
{
    [Header("----- Component -----")]
    public Player player;

    public Text statusHeart;
    public Text statusSpeed;
    public Text statusAttackSpeed;
    public Text statusPower;

    public GameObject statusWeaponArea;
    public GameObject statusSkillArea;

    public GameObject getItemsArea;

    public Sprite blankImage;

    void OnEnable()
    {
        // 스탯창 정보 출력
        statusHeart.text = player.health.ToString();
        statusSpeed.text = player.speed.ToString();
        statusAttackSpeed.text = player.attackSpeed.ToString("F3");
        statusPower.text = player.power.ToString("F3");

        SetStatus();
    }

    void Start()
    {
        
    }

    void Update()
    {

    }

    public void SetStatus()
    {
        // 무기와 스킬 이미지 세팅
        for (int i = 0; i < 5; i++)
        {
            // 이미지 0: 배경, 이미지 1: 무기 칸 배경, 텍스트 0: 타이틀
            Image[] weaponImage = statusWeaponArea.GetComponentsInChildren<Image>();
            Text[] weaponText = statusWeaponArea.GetComponentsInChildren<Text>();

            weaponImage[2 * (i + 1)].sprite = i < GameManager.instance.player.getWeaponCount ? GameManager.instance.weapon[i].icon : blankImage;
            weaponText[1 + (i * 2)].text = i < GameManager.instance.player.getWeaponCount ? 
                GameManager.instance.weapon[i].name + "(+" + GameManager.instance.weapon[i].level + ")": "";
            weaponText[2 + (i * 2)].text = i < GameManager.instance.player.getWeaponCount ? "공격력: " + GameManager.instance.weapon[i].damage.ToString() : "";

            Image[] skillImage = statusSkillArea.GetComponentsInChildren<Image>();
            Text[] skillText = statusSkillArea.GetComponentsInChildren<Text>();

            skillImage[2 * (i + 1)].sprite = i < GameManager.instance.player.getSkillCount ? GameManager.instance.skill[i].icon : blankImage;
            skillText[1 + (i * 4)].text = i < GameManager.instance.player.getSkillCount ?
                GameManager.instance.skill[i].name + "(+" + GameManager.instance.skill[i].level + ")" : "";
            skillText[2 + (i * 4)].text = i < GameManager.instance.player.getSkillCount ? "쿨타임: " + GameManager.instance.skill[i].skillCoolTime.ToString("F3") : "";
            skillText[3 + (i * 4)].text = i < GameManager.instance.player.getSkillCount ? "지속시간: " + GameManager.instance.skill[i].skillDuringTime.ToString() : "";
            skillText[4 + (i * 4)].text = i < GameManager.instance.player.getSkillCount ? 
                string.Format(GameManager.instance.skill[i].desc.ToString(), GameManager.instance.skill[i].damage) : "";
        }

        // 획득한 아이템 이미지 세팅
        //foreach(Item items in GameManager.instance.getItemList)
        //{

        //}
    }
}
