using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    public Player player;

    public Image[] heartArea;
    public Sprite heart;
    public Sprite heartEmpty;

    public Image coin;
    public Text coinText;

    public Image[] gameWeaponArea;
    public Image[] statusWeaponArea;
    public Sprite[] weaponImage;
    public Text[] weaponName;
    public Text[] weaponPower;

    public Image[] gameSkillArea;
    public Image[] statusSkillArea;
    public Sprite[] skillImage;
    public Text[] skillName;
    public Text[] skillDesc;

    public Text statusHeart;
    public Text statusSpeed;
    public Text statusAttackSpeed;
    public Text statusPower;

    void OnEnable()
    {
        int uiIndex = player.role * 5;

        // ���� ���� ����
        coinText.text = "0";

        // ����� ��ų �̹��� ����
        for(int i=0; i<5; i++)
        {
            gameWeaponArea[i].sprite = weaponImage[uiIndex + i];
            statusWeaponArea[i].sprite = weaponImage[uiIndex + i];
            gameSkillArea[i].sprite = skillImage[uiIndex + i];
            statusSkillArea[i].sprite = skillImage[uiIndex + i];
        }
        
        // ü�� ����
        for(int i=0; i<heartArea.Length; i++)
        {
            Color color = heartArea[i].color;

            if(i < player.health)
            {
                heartArea[i].sprite = heart;
            }
            else
            {
                color.a = 0;
                heartArea[i].color = color;
            }
        }

        // ����â ���� ���
        statusHeart.text = player.health.ToString();
        statusSpeed.text = player.speed.ToString();
        statusAttackSpeed.text = player.attackSpeed.ToString();
        statusPower.text = player.power.ToString();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
