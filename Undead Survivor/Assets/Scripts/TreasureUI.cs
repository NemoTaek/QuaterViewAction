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
        // 탐험상자 UI
        rect.localScale = Vector3.one;
        GameManager.instance.Pause();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.LevelUp);
        AudioManager.instance.HighPassBgm(true);

        // 현재 탐험상자 저장
        currentTreasureBox = treasureBox;

        // 회복약을 제외한 아이템 중 랜덤으로 레벨업
        items = GameManager.instance.gameItems;
        int random = 0;

        // 해금한 아이템이면 획득 가능
        while(true)
        {
            random = Random.Range(0, items.Length);
            if (items[random].itemData.isActive) break;
        }

        if(items[random].level < items[random].itemData.damages.Length)
        {
            items[random].OnClick();
            itemText.text = string.Format("{0} 레벨업!", items[random].itemData.itemName);
            itemImage.sprite = items[random].itemData.itemIcon;
        }
        else
        {
            itemText.text = string.Format("{0}이(가) 이미\n최고레벨 입니다!", items[random].itemData.itemName);
            itemImage.sprite = items[random].itemData.itemIcon;
        }
    }

    public void Hide()
    {
        rect.localScale = Vector3.zero;
        GameManager.instance.Resume();
        AudioManager.instance.PlaySfx(AudioManager.Sfx.Select);
        AudioManager.instance.HighPassBgm(false);

        // 상자 먹은 후 제거
        currentTreasureBox.SetActive(false);
    }

    void Update()
    {
    }
}
