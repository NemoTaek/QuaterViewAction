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
        // ���� ���� ����
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
        // ü�� ����
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
                // ü���� 0.5������ ���� ��Ʈ ��ĭ���� ä���
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

        // ��Ƽ�� �������� �ִٸ� ����ϰų� �� Ŭ���� ���� �� ������ ����
        if (activeItem.activeSelf)
        {
            Image[] guage = activeItemGuage.GetComponentsInChildren<Image>();

            // ��Ƽ�� �������� -1�̸� ���� ��� ������ �ǹ��ϹǷ� ������ �ٸ� �Ⱥ��̰� �ϰ�
            // �� �ܸ� ������ �� ����
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
            // ��� ���� �ٽ� �Ⱥ� ���·� ����
            Map.instance.mapSquare[i].GetComponent<Image>().color = new Color(1, 1, 1, 0);

            // ���� ������������ �������� ������ ���� ������ �ʱ�ȭ
            Image[] roomIcon = Map.instance.mapSquare[i].GetComponentsInChildren<Image>(true);
            if (roomIcon[1].gameObject.activeSelf)
            {
                roomIcon[1].sprite = null;
                roomIcon[1].gameObject.SetActive(false);
            }
        }
    }
}
