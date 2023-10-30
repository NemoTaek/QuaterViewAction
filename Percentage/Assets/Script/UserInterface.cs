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

    public GameObject activeItem;
    public Image activeItemImage;
    public Image activeItemGuage;

    public Image[] gameWeaponArea;
    public Image[] gameSkillArea;

    public GameObject bossUI;
    public RectTransform bossHealth;

    public GameObject mapBoard;
    public GameObject roomSquare;
    public GameObject[] roomIcons;

    void Awake()
    {
        roomIcons = new GameObject[81];
    }

    void OnEnable()
    {
        // 코인 개수 세팅
        coinText.text = "0";
    }

    void Start()
    {
        MakeMapBoard();
        //ClearMapBoard();
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

        // 액티브 아이템이 있다면 사용하거나 방 클리어 했을 때 게이지 세팅
        if (activeItem.activeSelf)
        {
            Image[] guage = activeItemGuage.GetComponentsInChildren<Image>();

            // 액티브 게이지가 -1이면 무한 사용 가능을 의미하므로 게이지 바를 안보이게 하고
            // 그 외면 게이지 바 세팅
            if (GameManager.instance.player.activeItem.activeGuage == -1) guage[0].gameObject.SetActive(false);
            else
            {
                guage[0].gameObject.SetActive(true);
                guage[1].fillAmount = (float)GameManager.instance.player.activeItem.currentGuage / GameManager.instance.player.activeItem.activeGuage;
            }
        }

        isChanged = false;
    }

    void MakeMapBoard()
    {
        int index = 0;
        for (int i = 10; i < 500; i += 60)
        {
            for (int j = 10; j < 500; j += 60)
            {
                GameObject roomIcon = Instantiate(roomSquare, mapBoard.transform);
                roomIcon.GetComponent<RectTransform>().anchoredPosition = new Vector3(-i, -j, 0);
                roomIcons[index] = roomIcon;
                index++;
            }
        }
    }

    public void ClearMapBoard()
    {
        for (int i = 1; i < 82; i++)
        {
            // 모든 방을 다시 안본 상태로 설정
            Map.instance.mapSquare[i].GetComponent<Image>().color = new Color(1, 1, 1, 0);

            // 이전 스테이지에서 아이콘을 설정한 것이 있으면 초기화
            Image[] roomIcon = Map.instance.mapSquare[i].GetComponentsInChildren<Image>(true);
            if (roomIcon[1].gameObject.activeSelf)
            {
                roomIcon[1].sprite = null;
                roomIcon[1].gameObject.SetActive(false);
            }
        }
    }
}
