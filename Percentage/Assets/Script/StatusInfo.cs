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
        // ����â ���� ���
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
        // ����� ��ų �̹��� ����
        for (int i = 0; i < 5; i++)
        {
            // �̹��� 0: ���, �̹��� 1: ���� ĭ ���, �ؽ�Ʈ 0: Ÿ��Ʋ
            Image[] weaponImage = statusWeaponArea.GetComponentsInChildren<Image>();
            Text[] weaponText = statusWeaponArea.GetComponentsInChildren<Text>();

            weaponImage[2 * (i + 1)].sprite = i < GameManager.instance.player.getWeaponCount ? GameManager.instance.weapon[i].icon : blankImage;
            weaponText[1 + (i * 2)].text = i < GameManager.instance.player.getWeaponCount ? 
                GameManager.instance.weapon[i].name + "(+" + GameManager.instance.weapon[i].level + ")": "";
            weaponText[2 + (i * 2)].text = i < GameManager.instance.player.getWeaponCount ? "���ݷ�: " + GameManager.instance.weapon[i].damage.ToString() : "";

            Image[] skillImage = statusSkillArea.GetComponentsInChildren<Image>();
            Text[] skillText = statusSkillArea.GetComponentsInChildren<Text>();

            skillImage[2 * (i + 1)].sprite = i < GameManager.instance.player.getSkillCount ? GameManager.instance.skill[i].icon : blankImage;
            skillText[1 + (i * 4)].text = i < GameManager.instance.player.getSkillCount ?
                GameManager.instance.skill[i].name + "(+" + GameManager.instance.skill[i].level + ")" : "";
            skillText[2 + (i * 4)].text = i < GameManager.instance.player.getSkillCount ? "��Ÿ��: " + GameManager.instance.skill[i].skillCoolTime.ToString("F3") : "";
            skillText[3 + (i * 4)].text = i < GameManager.instance.player.getSkillCount ? "���ӽð�: " + GameManager.instance.skill[i].skillDuringTime.ToString() : "";
            skillText[4 + (i * 4)].text = i < GameManager.instance.player.getSkillCount ? 
                string.Format(GameManager.instance.skill[i].desc.ToString(), GameManager.instance.skill[i].damage) : "";
        }

        // ȹ���� ������ �̹��� ����
        //foreach(Item items in GameManager.instance.getItemList)
        //{

        //}
    }
}
