using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusInfo : MonoBehaviour
{
    [Header("----- Component -----")]
    public Player player;

    //public Image[] statusSkillArea;
    //public Sprite[] skillImage;
    //public Text[] skillName;
    //public Text[] skillDesc;

    public Text statusHeart;
    public Text statusSpeed;
    public Text statusAttackSpeed;
    public Text statusPower;

    public GameObject statusWeaponArea;
    public GameObject statusSkillArea;

    public Sprite blankImage;

    void OnEnable()
    {
        // ����â ���� ���
        statusHeart.text = player.health.ToString();
        statusSpeed.text = player.speed.ToString();
        statusAttackSpeed.text = player.attackSpeed.ToString();
        statusPower.text = player.power.ToString();

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
            weaponText[1 + (i * 3)].text = i < GameManager.instance.player.getWeaponCount ? GameManager.instance.weapon[i].name : "";
            weaponText[2 + (i * 3)].text = i < GameManager.instance.player.getWeaponCount ? "����: " + GameManager.instance.weapon[i].level.ToString() : "";
            weaponText[3 + (i * 3)].text = i < GameManager.instance.player.getWeaponCount ? "���ݷ�: " + GameManager.instance.weapon[i].damage.ToString() : "";

            Image[] skillImage = statusSkillArea.GetComponentsInChildren<Image>();
            Text[] skillText = statusSkillArea.GetComponentsInChildren<Text>();

            skillImage[2 * (i + 1)].sprite = i < GameManager.instance.player.getSkillCount ? GameManager.instance.skill[i].icon : blankImage;
            skillText[1 + (i * 6)].text = i < GameManager.instance.player.getSkillCount ? GameManager.instance.skill[i].name : "";
            skillText[2 + (i * 6)].text = i < GameManager.instance.player.getSkillCount ? "����: " + GameManager.instance.skill[i].level.ToString() : "";
            skillText[3 + (i * 6)].text = i < GameManager.instance.player.getSkillCount ? "���ݷ�: " + GameManager.instance.skill[i].damage.ToString() : "";
            skillText[4 + (i * 6)].text = i < GameManager.instance.player.getSkillCount ? "��Ÿ��: " + GameManager.instance.skill[i].skillCoolTime.ToString() : "";
            skillText[5 + (i * 6)].text = i < GameManager.instance.player.getSkillCount ? "���ӽð�: " + GameManager.instance.skill[i].skillDuringTime.ToString() : "";
            skillText[6 + (i * 6)].text = i < GameManager.instance.player.getSkillCount ? GameManager.instance.skill[i].desc.ToString() : "";
        }
    }
}
