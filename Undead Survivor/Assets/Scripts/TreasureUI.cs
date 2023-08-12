using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TreasureUI : MonoBehaviour
{
    RectTransform rect;
    public Text itemText;
    public Image itemImage;
    Item[] items;
    GameObject currentTreasureBox;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void Start()
    {
        
    }

    public void Show(GameObject treasureBox)
    {
        // Ž����� UI
        rect.localScale = Vector3.one;
        GameManager.instance.Pause();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
        AudioManager.instance.HighPassBgm(true);

        // ���� Ž����� ����
        currentTreasureBox = treasureBox;

        // ȸ������ ������ ������ �� �������� ������
        items = GameManager.instance.gameItems;
        int random = 0;

        // �ر��� �������̸� ȹ�� ����
        while(true)
        {
            random = Random.Range(0, items.Length);
            if (items[random].itemData.isActive) break;
        }

        if(items[random].level < items[random].itemData.damages.Length)
        {
            items[random].OnClick();
            itemText.text = string.Format("{0} ������!", items[random].itemData.itemName);
            itemImage.sprite = items[random].itemData.itemIcon;
        }
        else
        {
            itemText.text = string.Format("{0}��(��) �̹�\n�ְ��� �Դϴ�!", items[random].itemData.itemName);
            itemImage.sprite = items[random].itemData.itemIcon;
        }
    }

    public void Hide()
    {
        rect.localScale = Vector3.zero;
        GameManager.instance.Resume();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        AudioManager.instance.HighPassBgm(false);

        // ���� ���� �� ����
        currentTreasureBox.SetActive(false);
    }

    void Update()
    {
    }
}
