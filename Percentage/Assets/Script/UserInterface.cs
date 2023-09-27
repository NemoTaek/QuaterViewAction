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
    public Sprite heartHalf;
    public Sprite heartEmpty;

    public Image coin;
    public Text coinText;

    public Image[] gameWeaponArea;
    public Image[] gameSkillArea;

    public GameObject bossUI;
    public RectTransform bossHealth;

    public GameObject mapBoard;
    public GameObject roomSquare;

    void OnEnable()
    {
        // 코인 개수 세팅
        coinText.text = "0";
    }

    void Start()
    {
        for(int i = 10; i < 500; i += 60)
        {
            for (int j = 10; j < 500; j += 60)
            {
                GameObject room = Instantiate(roomSquare, mapBoard.transform);
                room.GetComponent<RectTransform>().anchoredPosition = new Vector3(-i, -j, 0);
            }
        }
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
        // 체력 세팅
        for (int i = 0; i < heartArea.Length; i++)
        {
            Color color = heartArea[i].color;
            color.a = 1;

            if (i <= GameManager.instance.player.currentHealth - 1)
            {
                heartArea[i].sprite = heart;
            }
            else if (i <= GameManager.instance.player.health - 1)
            {
                // 체력이 0.5단위인 경우는 하트 반칸으로 채우기
                if (GameManager.instance.player.currentHealth - i < 1 && GameManager.instance.player.currentHealth - i > 0)
                {
                    heartArea[i].sprite = heartHalf;
                }
                else
                {
                    heartArea[i].sprite = heartEmpty;
                }
            }
            else
            {
                color.a = 0;
            }

            heartArea[i].color = color;
        }

        // 무기와 스킬 이미지 세팅
        for (int i = 0; i < 5; i++)
        {
            gameWeaponArea[i].sprite = i < GameManager.instance.player.getWeaponCount ? GameManager.instance.weapon[i].icon : blankImage;
            gameSkillArea[i].sprite = i < GameManager.instance.player.getSkillCount ? GameManager.instance.skill[i].icon : blankImage;

            // 현재 사용중인 무기와 스킬은 노란색 테두리
            if (GameManager.instance.player.currentWeaponIndex == i) gameWeaponArea[i].GetComponentsInParent<Image>()[1].color = new Color32(255, 201, 87, 255);
            else gameWeaponArea[i].GetComponentsInParent<Image>()[1].color = Color.white;
            if (GameManager.instance.player.currentSkillIndex == i) gameSkillArea[i].GetComponentsInParent<Image>()[1].color = new Color32(255, 201, 87, 255);
            else gameSkillArea[i].GetComponentsInParent<Image>()[1].color = Color.white;
        }
        isChanged = false;
    }
}
