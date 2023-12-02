using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeGame : MonoBehaviour
{
    public int level;
    public long money;
    public Sprite[] swordImageSprite;
    int[] successPercentage;
    int[] destroyPercentage;
    long upgradeCost;    
    long sellCost;
    Dictionary<string, long> haveMaterialDictionary;
    Dictionary<string, long> needMaterialDictionary;
    int[] useProtect;
    int selectBuyItemId;

    public Image swordImage;
    public Text levelText;
    public Text successText;
    public Text failureText;
    public Text destroyText;
    public Text needProtectText;
    public Text sellCostText;
    public Text moneyText;
    public Text textPrefab;
    public GameObject upgradeMaterialArea;
    public GameObject haveMaterialArea;
    public Image upgradeResultPanel;
    public Image warningPanel;
    public Image confirmBuyPanel;
    public Button upgradeButton;
    public Button sellButton;
    public Button keepButton;
    Text warningText;
    Text confirmBuyText;

    void Awake()
    {
        successPercentage = new int[20] { 100, 95, 90, 85, 80, 75, 70, 65, 60, 55, 50, 45, 40, 35, 30, 30, 30, 30, 20, 20 };
        destroyPercentage = new int[20] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 3, 4, 5, 6, 8, 10, 15, 20 };
        useProtect = new int[20] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 3, 4, 5, 6, 8, 10, 15, 20 };
        haveMaterialDictionary = new Dictionary<string, long>();
        needMaterialDictionary = new Dictionary<string, long>();
        warningText = warningPanel.GetComponentInChildren<Text>();
        confirmBuyText = confirmBuyPanel.GetComponentInChildren<Text>();

        // �ʱ� �ں��� 100����
        //money = 100000000;
        money = 100000000000;
        haveMaterialDictionary.Add("��ȭ��", 10000);
        haveMaterialDictionary.Add("14�� ��", 10000);
        haveMaterialDictionary.Add("15�� ��", 10000);
        haveMaterialDictionary.Add("16�� ��", 10000);
        haveMaterialDictionary.Add("17�� ��", 10000);
        haveMaterialDictionary.Add("18�� ��", 10000);
        haveMaterialDictionary.Add("19�� ��", 10000);
    }

    void Start()
    {
        AudioManager.instance.BGMPlay(0);
        SetUpgradeInfo();
    }

    void Update()
    {
        if (upgradeResultPanel.gameObject.activeSelf || warningPanel.gameObject.activeSelf)
        {
            upgradeButton.interactable = false;
            sellButton.interactable = false;
            keepButton.interactable = false;
        }   
        else
        {
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
        sellCost = upgradeCost / 5 * level;

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
                Instantiate(textPrefab, upgradeMaterialArea.transform);
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
        if (haveMaterialDictionary.Count >= haveMaterialTexts.Length)
        {
            int haveTextCount = haveMaterialDictionary.Count - haveMaterialTexts.Length + 1;
            for (int i=0; i<haveTextCount; i++)
            {
                Instantiate(textPrefab, haveMaterialArea.transform);
            }
            haveMaterialTexts = haveMaterialArea.GetComponentsInChildren<Text>(true);
        }

        int index = 1;
        foreach (KeyValuePair<string, long> have in haveMaterialDictionary)
        {
            haveMaterialTexts[index].gameObject.SetActive(true);
            haveMaterialTexts[index].text = $"{have.Key} x {have.Value}";
            index++;
        }

        if (haveMaterialDictionary.Count + 1 < haveMaterialTexts.Length)
        {
            for (int i = haveMaterialDictionary.Count + 1; i < haveMaterialTexts.Length; i++)
            {
                haveMaterialTexts[i].gameObject.SetActive(false);
            }
        }

        moneyText.text = money == 0 ? "0" : string.Format("{0:#,###}", money);
    }

    void AddMaterialDictionary(string item, int count)
    {
        if (haveMaterialDictionary.ContainsKey(item))
        {
            haveMaterialDictionary[item] += count;
        }
        else
        {
            haveMaterialDictionary.Add(item, count);
        }
    }

    bool CheckUpgradeCondition()
    {
        bool isAvailableUpgrade = false;
        long upgradeStone = 0;
        long materialSword = 0;
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
            foreach (KeyValuePair<string, long> have in needMaterialDictionary)
            {
                if (have.Key.Equals("��"))
                {
                    money -= have.Value;
                }
                else
                {
                    haveMaterialDictionary[have.Key] -= have.Value;
                    if (haveMaterialDictionary[have.Key] <= 0)
                    {
                        haveMaterialDictionary.Remove(have.Key);
                    }
                }
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
                    haveMaterialDictionary["�ı� ������"] -= useProtect[level];
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
    }

    public void Sell()
    {
        money += sellCost;
        level = 0;
        SetUpgradeInfo();
        AudioManager.instance.EffectPlay(AudioManager.Effect.ButtonClick);
    }

    public void Keep()
    {
        // ���� ������ ����
        AddMaterialDictionary($"{level}�� ��", 1);

        level = 0;
        SetUpgradeInfo();
        AudioManager.instance.EffectPlay(AudioManager.Effect.ButtonClick);
    }

    public void ConfirmBuyItem(int itemId)
    {
        selectBuyItemId = itemId;
        string buyItem = "";
        switch (itemId)
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

        confirmBuyPanel.gameObject.SetActive(true);
        confirmBuyText.text = $"{buyItem}��(��)\n�����Ͻðڽ��ϱ�?";

        AudioManager.instance.EffectPlay(AudioManager.Effect.ButtonClick);
    }

    public void BuyItem()
    {
        bool isBuyComplete = false;
        bool isHaveSwordFragment = haveMaterialDictionary.ContainsKey("���� ����");

        // �Ǹ� ������ id ���� ���� ���(�ݾ�)�� �����ϰ�, ��ǰ�� ��´�.
        switch (selectBuyItemId)
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
                    haveMaterialDictionary["���� ����"] -= 5;
                    AddMaterialDictionary("12�� ��ȭ��", 1);
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
                    haveMaterialDictionary["���� ����"] -= 7;
                    AddMaterialDictionary("13�� ��ȭ��", 1);
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
                    haveMaterialDictionary["���� ����"] -= 10;
                    AddMaterialDictionary("14�� ��ȭ��", 1);
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
                    haveMaterialDictionary["���� ����"] -= 15;
                    AddMaterialDictionary("15�� ��ȭ��", 1);
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
                    haveMaterialDictionary["���� ����"] -= 5;
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
                    haveMaterialDictionary["���� ����"] -= 22;
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
                    haveMaterialDictionary["���� ����"] -= 40;
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
    }
}
