using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UpgradeGame : MonoBehaviour
{
    [Header("----- Game Property -----")]
    public int level;
    public long money;
    public Sprite[] swordImageSprite;
    public Sprite[] itemImageSprite;
    int[] successPercentage;
    int[] destroyPercentage;
    long upgradeCost;    
    long sellCost;
    Dictionary<string, long> haveMaterialDictionary;
    Dictionary<string, long> needMaterialDictionary;
    int[] useProtect;
    public int selectItem;
    string whatAreYouDoing;

    [Header("----- UI Component -----")]
    public Image swordImage;
    public Text levelText;
    public Text successText;
    public Text failureText;
    public Text destroyText;
    public Text needProtectText;
    public Text sellCostText;
    public Text moneyText;
    public Text materialTextPrefab;
    public GameObject upgradeMaterialArea;
    public GameObject haveMaterialArea;
    public GameObject useItemArea;
    public Image upgradeResultPanel;
    public Image warningPanel;
    public Image twoButtonPanel;
    public Button upgradeButton;
    public Button sellButton;
    public Button keepButton;
    public UpgradeUseItemButton useItemButtonPrefab;
    public Button twoButtonOK;
    Text warningText;
    Text twoButtonText;

    [Header("----- Items Property -----")]
    public UpgradeGameItemData[] upgradeGameItemDatas;
    public UpgradeUseItemButton[] upgradeUseItemButtons;
    long upgradeStone;
    long materialSword;

    void Awake()
    {
        successPercentage = new int[20] { 100, 95, 90, 85, 80, 75, 70, 65, 60, 55, 50, 45, 40, 35, 30, 30, 30, 30, 20, 20 };
        destroyPercentage = new int[20] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 3, 4, 5, 6, 8, 10, 15, 20 };
        useProtect = new int[20] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 3, 4, 5, 6, 8, 10, 15, 20 };
        haveMaterialDictionary = new Dictionary<string, long>();
        needMaterialDictionary = new Dictionary<string, long>();
        warningText = warningPanel.GetComponentInChildren<Text>();
        twoButtonText = twoButtonPanel.GetComponentInChildren<Text>();
        upgradeUseItemButtons = new UpgradeUseItemButton[upgradeGameItemDatas.Length];

        // ����� �����Ͱ� �ִٸ� �ε�
        LoadData();
    }

    void Start()
    {
        // BGM ���
        AudioManager.instance.BGMPlay(0);

        // ��� ���� ������ �⺻ ����
        for (int i=0; i<upgradeGameItemDatas.Length; i++)
        {
            useItemButtonPrefab.Init(upgradeGameItemDatas[i]);
            UpgradeUseItemButton item = Instantiate(useItemButtonPrefab, useItemArea.transform);
            upgradeUseItemButtons[i] = item;
            item.gameObject.SetActive(false);
        }

        // ���� ���� �� �ʿ��� ���� ����
        SetUpgradeInfo();
    }

    void Update()
    {
        // â�� �������� ��ư ��Ȱ��ȭ
        if (upgradeResultPanel.gameObject.activeSelf || warningPanel.gameObject.activeSelf || twoButtonPanel.gameObject.activeSelf)
        {
            upgradeButton.interactable = false;
            sellButton.interactable = false;
            keepButton.interactable = false;
        }   
        else
        {
            // ���� ��ȭ �ÿ��� ��ȭ�� �� �� ����
            upgradeButton.interactable = level == 20 ? false : true;
            sellButton.interactable = true;
            keepButton.interactable = true;
        }

        // ���� â �ݱ� �Է�
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            upgradeResultPanel.gameObject.SetActive(false);
            warningPanel.gameObject.SetActive(false);
        }
    }

    void SetUpgradeInfo()
    {
        // ��ȭ ��� ���
        upgradeCost = Mathf.CeilToInt(1000 + (Mathf.Pow(100, 3) * Mathf.Pow(level, 2.7f) / 400));

        // �Ǹ� ��� ���
        sellCost = (upgradeCost / 5) * (5 * (level / 5) * 2 + (level % 5));

        // ���� �ؽ�Ʈ ����
        // ���� ������ �� �� ������ �и�
        if (level == 20)
        {
            upgradeCost = 0;

            needProtectText.gameObject.SetActive(false);
            swordImage.sprite = swordImageSprite[level];
            levelText.text = "�ְ� ��ȭ���� �޼�";
            successText.text = "����Ȯ�� : 0%";
            failureText.text = "����Ȯ�� : 0%";
            destroyText.text = "�ı�Ȯ�� : 0%";
        }
        else
        {
            // 10������ �ı� Ȯ�� ����
            if (level >= 10) needProtectText.gameObject.SetActive(true);
            else needProtectText.gameObject.SetActive(false);

            levelText.text = $"{level}�� > {level + 1}��";
            successText.text = $"����Ȯ�� : {successPercentage[level]}%";
            failureText.text = $"����Ȯ�� : {100 - successPercentage[level] - destroyPercentage[level]}%";
            destroyText.text = $"�ı�Ȯ�� : {destroyPercentage[level]}%";
            needProtectText.text = $"�ʿ� �ı� ������ ���� : {useProtect[level]}";
        }

        // ���� ��� �ؽ�Ʈ
        swordImage.sprite = swordImageSprite[level];
        sellCostText.text = sellCost == 0 ? "0" : string.Format("{0:#,###}", sellCost);
        moneyText.text = money == 0 ? "0" : string.Format("{0:#,###}", money);

        // ��ȭ ��� ����
        SetUpgradeCondition();

        // ���� ���� ��� ����
        SetHaveMaterial();
    }

    void SetUpgradeCondition()
    {
        // ��ȭ ���� �⺻������ ���� ���õǰ�, �߰������� ��ȭ���̳� �����ܰ� ���� ����
        needMaterialDictionary.Clear();
        needMaterialDictionary.Add("��", upgradeCost);

        switch (level)
        {
            case 10:
            case 11:
            case 12:
            case 13:
            case 14:
                needMaterialDictionary.Add("��ȭ��", 1);
                break;
            case 15:
                needMaterialDictionary.Add("��ȭ��", 2);
                needMaterialDictionary.Add($"{level - 1}�� ��", 1);
                break;
            case 16:
                needMaterialDictionary.Add("��ȭ��", 3);
                needMaterialDictionary.Add($"{level - 1}�� ��", 1);
                break;
            case 17:
                needMaterialDictionary.Add("��ȭ��", 4);
                needMaterialDictionary.Add($"{level - 1}�� ��", 1);
                break;
            case 18:
                needMaterialDictionary.Add("��ȭ��", 5);
                needMaterialDictionary.Add($"{level - 1}�� ��", 1);
                break;
            case 19:
                needMaterialDictionary.Add("��ȭ��", 6);
                needMaterialDictionary.Add($"{level - 1}�� ��", 2);
                break;
        }

        // 1���� ��¥�� ��ȭ ������� �־�� �Ǵϱ� �׳� �ְ�, �������� �ʿ��ϸ� Ȱ��ȭ �� ����, ���ʿ��ϸ� ��Ȱ��ȭ
        // ������ ���� �ʵ��� ������ ��ŭ �̸� �ؽ�Ʈ ����
        Text[] upgradeMaterialTexts = upgradeMaterialArea.GetComponentsInChildren<Text>(true);
        if (needMaterialDictionary.Count >= upgradeMaterialTexts.Length)
        {
            int needTextCount = needMaterialDictionary.Count - upgradeMaterialTexts.Length + 1;
            for (int i = 0; i < needTextCount; i++)
            {
                Instantiate(materialTextPrefab, upgradeMaterialArea.transform);
            }
            upgradeMaterialTexts = upgradeMaterialArea.GetComponentsInChildren<Text>(true);
        }

        int index = 1;
        foreach (KeyValuePair<string, long> have in needMaterialDictionary)
        {
            upgradeMaterialTexts[index].gameObject.SetActive(true);
            if (have.Key.Equals("��"))
            {
                upgradeMaterialTexts[index].text = level == 20 ? "" : string.Format("{0:#,###}��", have.Value);
            }
            else
            {
                upgradeMaterialTexts[index].text = $"{have.Key} x {have.Value}";
            }
            index++;
        }

        // ���� �� ��ġ�� �ؽ�Ʈ ��Ȱ��ȭ
        if (needMaterialDictionary.Count + 1 < upgradeMaterialTexts.Length)
        {
            for (int i = needMaterialDictionary.Count + 1; i < upgradeMaterialTexts.Length; i++)
            {
                upgradeMaterialTexts[i].gameObject.SetActive(false);
            }
        }
    }

    void SetHaveMaterial()
    {
        Text[] haveMaterialTexts = haveMaterialArea.GetComponentsInChildren<Text>(true);

        // ���� ��� �ؽ�Ʈ�� �����Ѹ�ŭ ���� ä���
        if (haveMaterialDictionary.Count >= haveMaterialTexts.Length)
        {
            int haveTextCount = haveMaterialDictionary.Count - haveMaterialTexts.Length + 1;
            for (int i=0; i<haveTextCount; i++)
            {
                Instantiate(materialTextPrefab, haveMaterialArea.transform);
            }
            haveMaterialTexts = haveMaterialArea.GetComponentsInChildren<Text>(true);
        }

        // ��� ���� ������ ��ư ���� ���� üũ
        foreach (UpgradeUseItemButton itemButton in upgradeUseItemButtons)
        {
            if (itemButton.count > 0) itemButton.gameObject.SetActive(true);
            else itemButton.gameObject.SetActive(false);
        }

        int index = 1;
        foreach (KeyValuePair<string, long> have in haveMaterialDictionary)
        {
            // ���� ��� ��� �ؽ�Ʈ ����
            haveMaterialTexts[index].gameObject.SetActive(true);
            haveMaterialTexts[index].text = $"{have.Key} x {have.Value}";
            index++;
        }

        // ��Ḧ �Ҹ��ؼ� ���� ��� ��Ϻ��� �ؽ�Ʈ ���� �� ������ ������ ��Ȱ��ȭ
        if (haveMaterialDictionary.Count + 1 < haveMaterialTexts.Length)
        {
            for (int i = haveMaterialDictionary.Count + 1; i < haveMaterialTexts.Length; i++)
            {
                haveMaterialTexts[i].gameObject.SetActive(false);
            }
        }

        // �� ����
        moneyText.text = money == 0 ? "0" : string.Format("{0:#,###}", money);
    }

    void AddMaterialDictionary(string item, long count)
    {
        // ��ᰡ ������ ������ŭ �� �߰�, ������ Ű�� �� �߰�
        // �߰� ������ ������ �� �� �ִ�. ���� ��� ����ؼ� 0 ���ϰ� �ȴٸ� ����
        if (haveMaterialDictionary.ContainsKey(item))
        {
            haveMaterialDictionary[item] += count;

            if (haveMaterialDictionary[item] <= 0)
            {
                haveMaterialDictionary.Remove(item);
            }
        }
        else
        {
            haveMaterialDictionary.Add(item, count);
        }
    }

    bool CheckUpgradeCondition()
    {
        bool isAvailableUpgrade = false;
        upgradeStone = 0;
        materialSword = 0;
        
        if (haveMaterialDictionary.ContainsKey("��ȭ��")) {
            upgradeStone = haveMaterialDictionary["��ȭ��"];
        }
        if (haveMaterialDictionary.ContainsKey($"{level - 1}�� ��"))
        {
            materialSword = haveMaterialDictionary[$"{level - 1}�� ��"];
        }

        switch (level)
        {
            // 10�����ʹ� ��ȭ���� ���ܰ��� �� �ϳ��� ��ȭ ���
            case 10:
            case 11:
            case 12:
            case 13:
            case 14:
                if (upgradeStone >= 1) isAvailableUpgrade = true;
                break;
            case 15:
                if (upgradeStone >= 2 && materialSword >= 1) isAvailableUpgrade = true;
                break;
            case 16:
                if (upgradeStone >= 3 && materialSword >= 1) isAvailableUpgrade = true;
                break;
            case 17:
                if (upgradeStone >= 4 && materialSword >= 1) isAvailableUpgrade = true;
                break;
            case 18:
                if (upgradeStone >= 5 && materialSword >= 1) isAvailableUpgrade = true;
                break;
            case 19:
                if (upgradeStone >= 10 && materialSword >= 2) isAvailableUpgrade = true;
                break;
            default:
                isAvailableUpgrade = true;
                break;
        }

        if (money < upgradeCost) isAvailableUpgrade = false;

        return isAvailableUpgrade;
    }

    public void Upgrade()
    {
        bool isAvailableUpgrade = CheckUpgradeCondition();

        if (isAvailableUpgrade)
        {
            // ��ȭ ��� �Ҹ�
            // �տ��� ��ȭ ��ᰡ �ִ��� üũ�� �����Ƿ� �Ʒ� ������ ��ȭ�� �����ϴٴ� ������ ����
            foreach (KeyValuePair<string, long> have in needMaterialDictionary)
            {
                if (have.Key.Equals("��")) money -= have.Value;
                else AddMaterialDictionary(have.Key, have.Value * -1);
            }

            // ��ȭ ���â ����
            upgradeResultPanel.gameObject.SetActive(true);
            Text[] result = upgradeResultPanel.GetComponentsInChildren<Text>();

            int random = Random.Range(0, 100);
            // ����
            if (random < successPercentage[level])
            {
                level++;
                result[0].text = "SUCCESS";
                result[0].color = new Color(235 / 255f, 192 / 255f, 78 / 255f);
                result[1].text = "��ȭ�� �����Ͽ� �Ѵܰ� ����մϴ�.";
                AudioManager.instance.EffectPlay(AudioManager.Effect.Success);
            }
            // �ı�
            else if (random < successPercentage[level] + destroyPercentage[level])
            {
                // �ı� �������� �ִٸ� �ı� �������� ����ϰ� ���� ���� ����
                bool isHaveProtect = haveMaterialDictionary.ContainsKey("�ı� ������");
                if (isHaveProtect && haveMaterialDictionary["�ı� ������"] >= useProtect[level])
                {
                    AddMaterialDictionary("�ı� ������", useProtect[level] * -1);
                    result[0].text = "FAILED";
                    result[0].color = new Color(200 / 255f, 198 / 255f, 196 / 255f);
                    result[1].text = "�ı� �������� ����Ͽ� ���� �ı����� �ʽ��ϴ�.";
                    AudioManager.instance.EffectPlay(AudioManager.Effect.Fail);
                }
                // �ı� �������� ���ų� ������ �����ϸ� �ı�
                else
                {
                    // �ı� �� ������ ���� ���� ���� ȹ��
                    AddMaterialDictionary("���� ����", destroyPercentage[level]);
                    level = 0;
                    result[0].text = "DESTROYED";
                    result[0].color = new Color(200 / 255f, 198 / 255f, 196 / 255f);
                    result[1].text = "��ȭ�� �����Ͽ� ��� �ı��˴ϴ�.";
                    AudioManager.instance.EffectPlay(AudioManager.Effect.Destroyed);
                }
            }
            // �������� ����
            else
            {
                result[0].text = "FAILED";
                result[0].color = new Color(227 / 255f, 78 / 255f, 70 / 255f);

                // 5���� ��ȭ������ �����ص� �ܰ谡 �������� ����
                if (level % 5 != 0)
                {
                    level--;
                    result[1].text = "��ȭ�� �����Ͽ� �Ѵܰ� �϶��մϴ�.";
                }
                else
                {
                    result[1].text = "��ȭ�� �����Ͽ����ϴ�.";
                }
                
                AudioManager.instance.EffectPlay(AudioManager.Effect.Fail);
            }

            // ��ȭ �� ���� ����
            SetUpgradeInfo();
        }

        // ��ȭ ��ᰡ ������ ��� ��ᰡ �����մϴ� ���
        else
        {
            warningPanel.gameObject.SetActive(true);
            warningText.text = "��ȭ ��ᰡ �����մϴ�.";
        }

        // ��ȭ �� ����
        SaveData();
    }

    public void Sell()
    {
        money += sellCost;
        level = 0;
        SetUpgradeInfo();
        AudioManager.instance.EffectPlay(AudioManager.Effect.ButtonClick);

        // �Ǹ� �� ����
        SaveData();
    }

    public void Keep()
    {
        // ���� ������ ����
        AddMaterialDictionary($"{level}�� ��", 1);

        level = 0;
        SetUpgradeInfo();
        AudioManager.instance.EffectPlay(AudioManager.Effect.ButtonClick);

        // ŵ�� �� ����
        SaveData();
    }

    public void ClickOKButton()
    {
        // �ڳ� ���� ���ϴ°ǰ�?
        // �ൿ�� �� �ѹ� �� Ȯ���ϴ� �ൿ�� '����', '������ ���' �� �����Ѵ�.
        switch (whatAreYouDoing)
        {
            case "buy":
                BuyItem();
                break;
            case "use":
                UseItem();
                break;
        }

        twoButtonPanel.gameObject.SetActive(false);
    }

    public void ConfirmBuyItem(int itemId)
    {
        whatAreYouDoing = "buy";
        selectItem = itemId;
        string buyItem = "";
        switch (selectItem)
        {
            case 0:
                buyItem = "��ȭ��";
                break;
            case 1:
                buyItem = "��ȭ�� x 10";
                break;
            case 2:
                buyItem = "��ȭ�� x 50";
                break;
            case 3:
                buyItem = "12�� ��ȭ��";
                break;
            case 4:
                buyItem = "13�� ��ȭ��";
                break;
            case 5:
                buyItem = "14�� ��ȭ��";
                break;
            case 6:
                buyItem = "15�� ��ȭ��";
                break;
            case 7:
            case 8:
                buyItem = "�ı� ������";
                break;
            case 9:
            case 10:
                buyItem = "�ı� ������ x 5";
                break;
            case 11:
            case 12:
                buyItem = "�ı� ������ x 10";
                break;
        }

        twoButtonPanel.gameObject.SetActive(true);
        twoButtonText.text = $"{buyItem}��(��)\n�����Ͻðڽ��ϱ�?";

        AudioManager.instance.EffectPlay(AudioManager.Effect.ButtonClick);
    }

    public void BuyItem()
    {
        bool isBuyComplete = false;
        bool isHaveSwordFragment = haveMaterialDictionary.ContainsKey("���� ����");

        // �Ǹ� ������ id ���� ���� ���(�ݾ�)�� �����ϰ�, ��ǰ�� ��´�.
        switch (selectItem)
        {
            case 0:
                // ��ᰡ �ִٸ�
                if (money >= 1000000)
                {
                    // ��� ����
                    money -= 1000000;

                    // �̹� �ش� ��ǰ�� ������ ���, ������ �߰�
                    AddMaterialDictionary("��ȭ��", 1);

                    // ���� �Ϸ�
                    isBuyComplete = true;
                }
                // ��ᰡ ������ ����� â ����
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = $"{1000000 - money}���� �����մϴ�.";
                }
                break;
            case 1:
                if (money >= 9000000)
                {
                    money -= 9000000;
                    AddMaterialDictionary("��ȭ��", 10);
                    isBuyComplete = true;
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = $"{9000000 - money}���� �����մϴ�.";
                }
                break;
            case 2:
                if (money >= 40000000)
                {
                    money -= 40000000;
                    AddMaterialDictionary("��ȭ��", 50);
                    isBuyComplete = true;
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = $"{40000000 - money}���� �����մϴ�.";
                }
                break;
            case 3:
                if (isHaveSwordFragment && haveMaterialDictionary["���� ����"] >= 5)
                {
                    AddMaterialDictionary("���� ����", -5);
                    AddMaterialDictionary("12�� ��ȭ��", 1);
                    upgradeUseItemButtons[0].SetCountText(upgradeUseItemButtons[0].count + 1);
                    isBuyComplete = true;
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = isHaveSwordFragment ? $"���� ���� {5 - haveMaterialDictionary["���� ����"]}���� �����մϴ�." : "���� ���� 5�� �����մϴ�.";
                }
                break;
            case 4:
                if (isHaveSwordFragment && haveMaterialDictionary["���� ����"] >= 7)
                {
                    AddMaterialDictionary("���� ����", -7);
                    AddMaterialDictionary("13�� ��ȭ��", 1);
                    upgradeUseItemButtons[1].SetCountText(upgradeUseItemButtons[1].count + 1);
                    isBuyComplete = true;
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = isHaveSwordFragment ? $"���� ���� {7 - haveMaterialDictionary["���� ����"]}���� �����մϴ�." : "���� ���� 7���� �����մϴ�.";
                }
                break;
            case 5:
                if (isHaveSwordFragment && haveMaterialDictionary["���� ����"] >= 10)
                {
                    AddMaterialDictionary("���� ����", -10);
                    AddMaterialDictionary("14�� ��ȭ��", 1);
                    upgradeUseItemButtons[2].SetCountText(upgradeUseItemButtons[2].count + 1);
                    isBuyComplete = true;
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = isHaveSwordFragment ? $"���� ���� {10 - haveMaterialDictionary["���� ����"]}���� �����մϴ�." : "���� ���� 10���� �����մϴ�.";
                }
                break;
            case 6:
                if (isHaveSwordFragment && haveMaterialDictionary["���� ����"] >= 15) {
                    AddMaterialDictionary("���� ����", -15);
                    AddMaterialDictionary("15�� ��ȭ��", 1);
                    upgradeUseItemButtons[3].SetCountText(upgradeUseItemButtons[3].count + 1);
                    isBuyComplete = true;
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = isHaveSwordFragment ? $"���� ���� {15 - haveMaterialDictionary["���� ����"]}���� �����մϴ�." : "���� ���� 15���� �����մϴ�.";
                }
                break;
            case 7:
                if (money >= 10000000)
                {
                    money -= 10000000;
                    AddMaterialDictionary("�ı� ������", 1);
                    isBuyComplete = true;
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = $"{10000000 - money}���� �����մϴ�.";
                }
                break;
            case 8:
                if (isHaveSwordFragment && haveMaterialDictionary["���� ����"] >= 5)
                {
                    AddMaterialDictionary("���� ����", -5);
                    AddMaterialDictionary("�ı� ������", 1);
                    isBuyComplete = true;
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = isHaveSwordFragment ? $"���� ���� {5 - haveMaterialDictionary["���� ����"]}���� �����մϴ�." : "���� ���� 5���� �����մϴ�.";
                }
                break;
            case 9:
                if (money >= 45000000)
                {
                    money -= 45000000;
                    AddMaterialDictionary("�ı� ������", 5);
                    isBuyComplete = true;
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = $"{45000000 - money}���� �����մϴ�.";
                }
                break;
            case 10:
                if (isHaveSwordFragment && haveMaterialDictionary["���� ����"] >= 22) {
                    AddMaterialDictionary("���� ����", -22);
                    AddMaterialDictionary("�ı� ������", 5);
                    isBuyComplete = true;
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = isHaveSwordFragment ? $"���� ���� {22 - haveMaterialDictionary["���� ����"]}���� �����մϴ�." : "���� ���� 22���� �����մϴ�.";
                }
                break;
            case 11:
                if (money >= 80000000)
                {
                    money -= 80000000;
                    AddMaterialDictionary("�ı� ������", 10);
                    isBuyComplete = true;
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = $"{80000000 - money}���� �����մϴ�.";
                }
                break;
            case 12:
                if (isHaveSwordFragment && haveMaterialDictionary["���� ����"] >= 40) {
                    AddMaterialDictionary("���� ����", -40);
                    AddMaterialDictionary("�ı� ������", 10);
                    isBuyComplete = true;
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = isHaveSwordFragment ? $"���� ���� {40 - haveMaterialDictionary["���� ����"]}���� �����մϴ�." : "���� ���� 40���� �����մϴ�.";
                }
                break;
        }

        // ���� ������ ����
        SetHaveMaterial();

        if (isBuyComplete) AudioManager.instance.EffectPlay(AudioManager.Effect.BuyItem);
        else AudioManager.instance.EffectPlay(AudioManager.Effect.Fail);

        // ������ ���� �� ����
        SaveData();
    }

    public void ConfirmUseItem(UpgradeUseItemButton itemButton)
    {
        whatAreYouDoing = "use";
        selectItem = itemButton.id;
        string buyItem = "";
        switch (selectItem)
        {
            case 3:
                buyItem = "12�� ��ȭ��";
                break;
            case 4:
                buyItem = "13�� ��ȭ��";
                break;
            case 5:
                buyItem = "14�� ��ȭ��";
                break;
            case 6:
                buyItem = "15�� ��ȭ��";
                break;
        }

        twoButtonPanel.gameObject.SetActive(true);
        twoButtonText.text = $"{buyItem}��(��)\n����Ͻðڽ��ϱ�?";

        AudioManager.instance.EffectPlay(AudioManager.Effect.ButtonClick);
    }

    public void UseItem()
    {
        switch (selectItem)
        {
            case 3:
                if (haveMaterialDictionary.ContainsKey("12�� ��ȭ��"))
                {
                    AddMaterialDictionary("12�� ��ȭ��", -1);
                    upgradeUseItemButtons[0].SetCountText(upgradeUseItemButtons[0].count - 1);
                    level = 12;
                    SetUpgradeInfo();
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = "12�� ��ȭ���� �����մϴ�.";
                }
                break;
            case 4:
                if (haveMaterialDictionary.ContainsKey("13�� ��ȭ��"))
                {
                    AddMaterialDictionary("13�� ��ȭ��", -1);
                    upgradeUseItemButtons[1].SetCountText(upgradeUseItemButtons[1].count - 1);
                    level = 13;
                    SetUpgradeInfo();
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = "13�� ��ȭ���� �����մϴ�.";
                }
                break;
            case 5:
                if (haveMaterialDictionary.ContainsKey("14�� ��ȭ��"))
                {
                    AddMaterialDictionary("14�� ��ȭ��", -1);
                    upgradeUseItemButtons[2].SetCountText(upgradeUseItemButtons[2].count - 1);
                    level = 14;
                    SetUpgradeInfo();
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = "14�� ��ȭ���� �����մϴ�.";
                }
                break;
            case 6:
                if (haveMaterialDictionary.ContainsKey("15�� ��ȭ��"))
                {
                    AddMaterialDictionary("15�� ��ȭ��", -1);
                    upgradeUseItemButtons[3].SetCountText(upgradeUseItemButtons[3].count - 1);
                    level = 15;
                    SetUpgradeInfo();
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = "15�� ��ȭ���� �����մϴ�.";
                }
                break;
        }

        // ���� ������ ����
        SetHaveMaterial();

        AudioManager.instance.EffectPlay(AudioManager.Effect.ButtonClick);

        // ������ ��� �� ����
        SaveData();
    }

    void SaveData()
    {
        // �����ؾ� �� �����͸� Dictionary�� �߰� (���� ���, ��, ����)
        Dictionary<string, long> saveData = new Dictionary<string, long>(haveMaterialDictionary);
        saveData.Add("��", money);
        saveData.Add("����", level);
        string saveDataJson = DataJson.DictionaryToJson(saveData);

        // ���� ��ο� ���̺����� ����
        // ��ΰ� ���ٸ� ����
        string path = Application.dataPath + "/Data";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        // ���� ���� ���� ��ȣȭ�� �ѹ� �غ���?
        // System.Text.Encoding.UTF8.GetBytes(string): �Ű����� ���ڿ��� byte �迭�� ���ڵ��ϴ� �޼ҵ�
        // System.Convert.ToBase64String(byte[]): �Ű����� ����Ʈ �迭�� ���ڵ��� base64���ڿ��� ��ȯ�ϴ� �޼ҵ�
        byte[] byteData = System.Text.Encoding.UTF8.GetBytes(saveDataJson);
        string encodedJson = System.Convert.ToBase64String(byteData);

        // File.WriteAllText(path, text): �� ������ �����, ������ �� �� ������ ����. �̹� ������ �����ϸ� ���
        File.WriteAllText(path + "/saveData.json", encodedJson);
    }

    void LoadData()
    {
        // ���̺����� �ε�
        string path = Application.dataPath + "/Data";

        // File.Exists(path): �ش� ��ο� ������ �ִ��� Ȯ��
        bool isExistsDataFile = File.Exists(path + "/saveData.json");
        if (isExistsDataFile)
        {
            // File.ReadAllText(path): �ؽ�Ʈ������ ����, ��� �ؽ�Ʈ�� ���� �� ������ ����
            string loadDataJson = File.ReadAllText(path + "/saveData.json");

            // ���������� ��ȣȭ�� ������ �ص��غ���?
            // System.Convert.FromBase64String(string): ���ڵ��� base64���ڿ��κ��� byte �迭�� ��ȯ�ϴ� �޼ҵ�
            // System.Text.Encoding.UTF8.GetString(byte[]): �Ű����� byte �迭���� ���ڿ��� ���ڵ��ϴ� �޼ҵ�
            byte[] byteData = System.Convert.FromBase64String(loadDataJson);
            string encodedJson = System.Text.Encoding.UTF8.GetString(byteData);

            // �ε��� ������ ������ �ε�
            if (encodedJson != null && encodedJson.Length > 0)
            {
                Dictionary<string, long> loadData = DataJson.DictionaryFromJson<string, long>(encodedJson);

                // �Ľ��� Dictionary�� ��ȸ�ϸ� money�� level ���� ���� �� ����
                foreach (KeyValuePair<string, long> data in loadData)
                {
                    if (data.Key == "��")
                    {
                        money = data.Value;
                    }
                    else if (data.Key == "����")
                    {
                        level = (int)data.Value;
                    }
                }
                loadData.Remove("��");
                loadData.Remove("����");

                // �������� ���� ��� Dictionary�� ����
                haveMaterialDictionary = loadData;
            }
        }
        else
        {
            // �⺻ �ڻ� 10��
            money = 1000000000;
            SaveData();
        }
    }

    public void ExitUpgradeGame()
    {
        // ������ �� ������ ����
        SaveData();

        AudioManager.instance.BGMStop();
        SceneManager.LoadScene("Intro");
    }
}
