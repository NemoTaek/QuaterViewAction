using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    public bool isChanged = false;
    public Sprite blankImage;

    public Image[] heartArea;
    public Sprite heart;
    public Sprite heartEmpty;

    public Image coin;
    public Text coinText;

    public Image[] gameWeaponArea;
    public Image[] gameSkillArea;

    void OnEnable()
    {
        // ���� ���� ����
        coinText.text = "0";
        
        // ü�� ����
        for(int i=0; i<heartArea.Length; i++)
        {
            Color color = heartArea[i].color;

            if(i < GameManager.instance.player.health)
            {
                heartArea[i].sprite = heart;
            }
            else
            {
                color.a = 0;
                heartArea[i].color = color;
            }
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (isChanged)
        {
            SetInventory();
        }
    }

    public void SetInventory()
    {
        // ����� ��ų �̹��� ����
        for (int i = 0; i < 5; i++)
        {
            gameWeaponArea[i].sprite = i < GameManager.instance.player.getWeaponCount ? GameManager.instance.weapon[i].icon : blankImage;
            gameSkillArea[i].sprite = i < GameManager.instance.player.getSkillCount ? GameManager.instance.skill[i].icon : blankImage;

            // ���� ������� ����� ��ų�� ����� �׵θ�
            if (GameManager.instance.player.currentWeaponIndex == i) gameWeaponArea[i].GetComponentsInParent<Image>()[1].color = new Color32(255, 201, 87, 255);
            else gameWeaponArea[i].GetComponentsInParent<Image>()[1].color = Color.white;
            if (GameManager.instance.player.currentSkillIndex == i) gameSkillArea[i].GetComponentsInParent<Image>()[1].color = new Color32(255, 201, 87, 255);
            else gameSkillArea[i].GetComponentsInParent<Image>()[1].color = Color.white;
        }
        isChanged = false;
    }
}
