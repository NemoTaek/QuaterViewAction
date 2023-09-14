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
    public Sprite[] weaponImage;

    public Image[] gameSkillArea;
    public Sprite[] skillImage;

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
            SetWeaponInventory();
        }
    }

    public void SetWeaponInventory()
    {
        // ����� ��ų �̹��� ����
        for (int i = 0; i < 5; i++)
        {
            gameWeaponArea[i].sprite = i < GameManager.instance.player.getWeaponCount ? GameManager.instance.weapon[i].icon : blankImage;
            //gameSkillArea[i].sprite = skillImage[uiIndex + i];
        }
        isChanged = false;
    }
}
