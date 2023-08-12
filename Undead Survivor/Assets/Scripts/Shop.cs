using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public enum ShopItemType { Item, Character };
    Item purchaseItem;
    Character purchaseCharacter;
    int itemPrice;
    public Text totalCoinText;

    public GameObject purchasePanel;
    public Text purchasePanelTitle;

    public GameObject purchaseResultPanel;
    public Text purchaseResultTitle;

    public Button[] shopItemButton;
    Item[] items;

    public Button[] shopCharacterButton;
    Character[] characters;

    void OnEnable()
    {
        totalCoinText.text = PlayerPrefs.GetInt("Coin").ToString();
        items = GameManager.instance.gameItems;
        characters = GameManager.instance.gameCharacters;

        UpdateItem();

        // Closure Problem: 람다식이 실제 실행되기 전에는 내부 변수를 참조형태로 가지고 있다.
        // 그래서 같은 변수인 i를 계속 주었기 때문에 마지막 값으로 통일되어버리는 문제가 발생.
        // 해결 방법은 반복문 내에 변수를 추가하여 이 변수를 전달함으로써 해결
        for (int i=0; i<shopItemButton.Length; i++)
        {
            int buttonIndex = i;
            shopItemButton[buttonIndex].onClick.AddListener(() => OpenPurchasePanel(ShopItemType.Item, buttonIndex + 4));
            //shopItemButton[buttonIndex].onClick.AddListener(delegate { OpenPurchasePanel(ShopItemType.Item, buttonIndex + 4); });
        }

        for (int i = 0; i < shopCharacterButton.Length; i++)
        {
            int buttonIndex = i;
            shopCharacterButton[buttonIndex].onClick.AddListener(() => OpenPurchasePanel(ShopItemType.Character, buttonIndex + 4));
        }
    }

    void Start()
    {

    }

    void Update()
    {
        
    }

    // 상점 열고, 해당 아이템 정보 세팅
    public void OpenPurchasePanel(ShopItemType type, int itemId)
    {
        purchasePanel.SetActive(true);
        Button purchaseConfirmButton = purchasePanel.GetComponentsInChildren<Button>(true)[0];

        if (type == ShopItemType.Item)
        {
            switch (itemId)
            {
                case 4:
                    itemPrice = 3000;
                    break;
                case 5:
                    itemPrice = 5000;
                    break;
                case 6:
                    itemPrice = 5000;
                    break;
                case 7:
                    itemPrice = 7000;
                    break;
                case 8:
                    itemPrice = 10000;
                    break;
            }

            foreach (Item item in items)
            {
                if (item.itemData.itemId == itemId)
                {
                    purchaseItem = item;
                    purchasePanelTitle.text = item.itemData.itemName + "을(를) \n구매하시겠습니까?";
                }
            }
        }
        else if (type == ShopItemType.Character)
        {
            switch (itemId)
            {
                case 4:
                    itemPrice = 50000;
                    break;
            }

            foreach (Character character in characters)
            {
                if (character.characterData.characterId == itemId)
                {
                    purchaseCharacter = character;
                    purchasePanelTitle.text = character.characterData.characterName + "캐릭터를 \n구매하시겠습니까?";
                }
            }
        }

        purchaseConfirmButton.onClick.AddListener(() => PurchaseItem(type));
    }

    // 구매 여부에 따라 구매 완료 하거나, 돈이 부족하다는 창 출력
    public void PurchaseItem(ShopItemType type)
    {
        int remainCoin = PlayerPrefs.GetInt("Coin");

        if (remainCoin >= itemPrice)
        {
            if (type == ShopItemType.Item)
            {
                purchaseItem.itemData.isActive = true;
                purchaseResultTitle.text = purchaseItem.itemData.itemName + "을(를) \n구매 완료하였습니다.";
            }
            else if (type == ShopItemType.Character)
            {
                purchaseCharacter.characterData.isActive = true;
                purchaseResultTitle.text = purchaseCharacter.characterData.characterName + "캐릭터를 \n구매 완료하였습니다.";
            }

            remainCoin -= itemPrice;
            totalCoinText.text = remainCoin.ToString();

            purchasePanel.SetActive(false);
            purchaseResultPanel.SetActive(true);
        }
        else
        {
            purchasePanel.SetActive(false);
            purchaseResultPanel.SetActive(true);
            purchaseResultTitle.text = "골드가 부족하여 구매에 실패하였습니다.";
        }

        itemPrice = 0;
        PlayerPrefs.SetInt("Coin", remainCoin);
        purchasePanel.SetActive(false);
        UpdateItem();
    }

    void UpdateItem()
    {
        // 아이템 업데이트
        foreach (Item item in items)
        {
            if (item.itemData.itemId >= 4 && item.itemData.isActive)
            {
                // 0: 버튼 자기 자신, 1: 아이콘, 2: 코인 이미지, 3: sold out 판넬, 4: 흐릿한 아이콘
                Image[] shopItemImages = shopItemButton[item.itemData.itemId - 4].GetComponentsInChildren<Image>(true);
                // 0: 이름, 1: 설명, 2: 가격, 3: sold out
                Text[] shopItemTexts = shopItemButton[item.itemData.itemId - 4].GetComponentsInChildren<Text>(true);

                shopItemImages[1].gameObject.SetActive(false);
                shopItemImages[3].gameObject.SetActive(true);
                shopItemTexts[0].gameObject.SetActive(false);
                shopItemTexts[1].gameObject.SetActive(false);
                shopItemButton[item.itemData.itemId - 4].GetComponentInChildren<HorizontalLayoutGroup>(true).gameObject.SetActive(false);

                shopItemButton[item.itemData.itemId - 4].interactable = false;
            }
        }

        // 캐릭터 업데이트
        foreach (Character character in characters)
        {
            if (character.characterData.characterId >= 4 && character.characterData.isActive)
            {
                // 0: 버튼 자기 자신, 1: 아이콘, 2: 코인 이미지, 3: sold out
                Image[] shopCharacterImages = shopCharacterButton[character.characterData.characterId - 4].GetComponentsInChildren<Image>(true);
                // 0: 이름, 1: 설명
                Text[] shopCharacterTexts = shopCharacterButton[character.characterData.characterId - 4].GetComponentsInChildren<Text>(true);

                shopCharacterImages[1].gameObject.SetActive(false);
                shopCharacterImages[3].gameObject.SetActive(true);
                shopCharacterTexts[0].gameObject.SetActive(false);
                shopCharacterTexts[1].gameObject.SetActive(false);
                shopCharacterButton[character.characterData.characterId - 4].GetComponentInChildren<HorizontalLayoutGroup>(true).gameObject.SetActive(false);

                shopCharacterButton[character.characterData.characterId - 4].interactable = false;
            }
        }
    }
}
