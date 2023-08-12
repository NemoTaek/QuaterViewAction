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

        // Closure Problem: ���ٽ��� ���� ����Ǳ� ������ ���� ������ �������·� ������ �ִ�.
        // �׷��� ���� ������ i�� ��� �־��� ������ ������ ������ ���ϵǾ������ ������ �߻�.
        // �ذ� ����� �ݺ��� ���� ������ �߰��Ͽ� �� ������ ���������ν� �ذ�
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

    // ���� ����, �ش� ������ ���� ����
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
                    purchasePanelTitle.text = item.itemData.itemName + "��(��) \n�����Ͻðڽ��ϱ�?";
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
                    purchasePanelTitle.text = character.characterData.characterName + "ĳ���͸� \n�����Ͻðڽ��ϱ�?";
                }
            }
        }

        purchaseConfirmButton.onClick.AddListener(() => PurchaseItem(type));
    }

    // ���� ���ο� ���� ���� �Ϸ� �ϰų�, ���� �����ϴٴ� â ���
    public void PurchaseItem(ShopItemType type)
    {
        int remainCoin = PlayerPrefs.GetInt("Coin");

        if (remainCoin >= itemPrice)
        {
            if (type == ShopItemType.Item)
            {
                purchaseItem.itemData.isActive = true;
                purchaseResultTitle.text = purchaseItem.itemData.itemName + "��(��) \n���� �Ϸ��Ͽ����ϴ�.";
            }
            else if (type == ShopItemType.Character)
            {
                purchaseCharacter.characterData.isActive = true;
                purchaseResultTitle.text = purchaseCharacter.characterData.characterName + "ĳ���͸� \n���� �Ϸ��Ͽ����ϴ�.";
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
            purchaseResultTitle.text = "��尡 �����Ͽ� ���ſ� �����Ͽ����ϴ�.";
        }

        itemPrice = 0;
        PlayerPrefs.SetInt("Coin", remainCoin);
        purchasePanel.SetActive(false);
        UpdateItem();
    }

    void UpdateItem()
    {
        // ������ ������Ʈ
        foreach (Item item in items)
        {
            if (item.itemData.itemId >= 4 && item.itemData.isActive)
            {
                // 0: ��ư �ڱ� �ڽ�, 1: ������, 2: ���� �̹���, 3: sold out �ǳ�, 4: �帴�� ������
                Image[] shopItemImages = shopItemButton[item.itemData.itemId - 4].GetComponentsInChildren<Image>(true);
                // 0: �̸�, 1: ����, 2: ����, 3: sold out
                Text[] shopItemTexts = shopItemButton[item.itemData.itemId - 4].GetComponentsInChildren<Text>(true);

                shopItemImages[1].gameObject.SetActive(false);
                shopItemImages[3].gameObject.SetActive(true);
                shopItemTexts[0].gameObject.SetActive(false);
                shopItemTexts[1].gameObject.SetActive(false);
                shopItemButton[item.itemData.itemId - 4].GetComponentInChildren<HorizontalLayoutGroup>(true).gameObject.SetActive(false);

                shopItemButton[item.itemData.itemId - 4].interactable = false;
            }
        }

        // ĳ���� ������Ʈ
        foreach (Character character in characters)
        {
            if (character.characterData.characterId >= 4 && character.characterData.isActive)
            {
                // 0: ��ư �ڱ� �ڽ�, 1: ������, 2: ���� �̹���, 3: sold out
                Image[] shopCharacterImages = shopCharacterButton[character.characterData.characterId - 4].GetComponentsInChildren<Image>(true);
                // 0: �̸�, 1: ����
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
