using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeGame : MonoBehaviour
{
    int level;
    public Sprite[] swordImageSprite;
    int[] successPercentage;
    int[] destroyPercentage;
    long upgradeCost;
    long money;
    long sellCost;
    Dictionary<string, int> haveMaterialDictionary;
    List<string> needMaterial;

    public Image swordImage;
    public Text levelText;
    public Text successText;
    public Text failureText;
    public Text destroyText;
    public Text sellCostText;
    public Text moneyText;
    public Text textPrefab;
    public GameObject upgradeMaterialArea;
    public GameObject haveMaterialArea;
    public Image upgradeResultPanel;
    public Image WarningPanel;

    void Awake()
    {
        successPercentage = new int[20] { 100, 95, 90, 85, 80, 75, 70, 65, 60, 55, 50, 45, 40, 35, 30, 25, 20, 15, 10, 5 };
        destroyPercentage = new int[20] { 0, 0, 0, 0, 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 12, 14, 16, 18, 20 };
        haveMaterialDictionary = new Dictionary<string, int>();
        needMaterial = new List<string>();

        // �ʱ� �ں��� 100����
        money = 1000000;
    }

    void Start()
    {
        SetUpgradeInfo();
    }

    void Update()
    {
        
    }

    void SetUpgradeInfo()
    {
        // ��ȭ ��� ���
        upgradeCost = Mathf.CeilToInt(1000 + (Mathf.Pow(100, 3) * Mathf.Pow(level, 2.7f) / 400));

        // �Ǹ� ��� ���
        sellCost = upgradeCost / 8 * level;

        // ���� �ؽ�Ʈ ����
        swordImage.sprite = swordImageSprite[level];
        levelText.text = $"{level}�� > {level + 1}��";
        successText.text = $"����Ȯ�� : {successPercentage[level]}%";
        failureText.text = $"����Ȯ�� : {100 - successPercentage[level] - destroyPercentage[level]}%";
        destroyText.text = $"�ı�Ȯ�� : {destroyPercentage[level]}%";
        sellCostText.text = sellCost == 0 ? "0" : string.Format("{0:#,###}", sellCost);
        moneyText.text = money == 0 ? "0" : string.Format("{0:#,###}", money);

        // ��ȭ ��� ����
        SetUpgradeCondition();

        // 1���� ��¥�� ��ȭ ������� �־�� �Ǵϱ� �׳� �ְ�, �������� �ʿ��ϸ� Ȱ��ȭ �� ����, ���ʿ��ϸ� ��Ȱ��ȭ
        // ������ ���� �ʵ��� ������ ��ŭ �̸� �ؽ�Ʈ ����
        Text[] upgradeMaterialTexts = upgradeMaterialArea.GetComponentsInChildren<Text>(true);
        if (needMaterial.Count >= upgradeMaterialTexts.Length)
        {
            int needTextCount = needMaterial.Count - upgradeMaterialTexts.Length + 1;
            for (int i=0; i<needTextCount; i++)
            {
                Instantiate(textPrefab, upgradeMaterialArea.transform);
            }
            upgradeMaterialTexts = upgradeMaterialArea.GetComponentsInChildren<Text>(true);
        }

        for (int i=1; i<=needMaterial.Count; i++)
        {
            upgradeMaterialTexts[i].gameObject.SetActive(true);
            upgradeMaterialTexts[i].text = needMaterial[i - 1];
        }

        // ���� �� ��ġ�� �ؽ�Ʈ ��Ȱ��ȭ
        if (needMaterial.Count + 1 < upgradeMaterialTexts.Length)
        {
            for (int i = needMaterial.Count + 1; i < upgradeMaterialTexts.Length; i++)
            {
                upgradeMaterialTexts[i].gameObject.SetActive(false);
            }
        }
    }

    void SetUpgradeCondition()
    {
        needMaterial.Clear();
        needMaterial.Add(string.Format("{0:#,###}��", upgradeCost));

        switch (level)
        {
            case 10:
            case 11:
            case 12:
            case 13:
            case 14:
                needMaterial.Add("��ȭ�� x 1");
                break;
            case 15:
                needMaterial.Add("��ȭ�� x 2");
                needMaterial.Add($"{level - 1}�� �� x 1");
                break;
            case 16:
                needMaterial.Add("��ȭ�� x 3");
                needMaterial.Add($"{level - 1}�� �� x 1");
                break;
            case 17:
                needMaterial.Add("��ȭ�� x 4");
                needMaterial.Add($"{level - 1}�� �� x 1");
                break;
            case 18:
                needMaterial.Add("��ȭ�� x 5");
                needMaterial.Add($"{level - 1}�� �� x 1");
                break;
            case 19:
                needMaterial.Add("��ȭ�� x 6");
                needMaterial.Add($"{level - 1}�� �� x 2");
                break;
        }
    }

    bool CheckUpgradeCondition()
    {
        bool isAvailableUpgrade = false;
        int upgradeStone = 0;
        int materialSword = 0;
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
            // ��ȭ �޼� �Ҹ�
            money -= upgradeCost;

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
            }
            // �ı�
            else if (random < successPercentage[level] + destroyPercentage[level])
            {
                level = 0;
                result[0].text = "DESTROYED";
                result[0].color = new Color(200 / 255f, 198 / 255f, 196 / 255f);
                result[1].text = "��ȭ�� �����Ͽ� ��� �ı��˴ϴ�.";
            }
            // �������� ����
            else
            {
                level--;
                result[0].text = "FAILED";
                result[0].color = new Color(227 / 255f, 78 / 255f, 70 / 255f);
                result[1].text = "��ȭ�� �����Ͽ� �Ѵܰ� �϶��մϴ�.";
            }

            // ��ȭ �� ���� ����
            SetUpgradeInfo();
        }

        // ��ȭ ��ᰡ ������ ��� ��ᰡ �����մϴ� ���
        else
        {
            WarningPanel.gameObject.SetActive(true);
        }
    }

    public void Sell()
    {
        money += sellCost;
        level = 0;
        SetUpgradeInfo();
    }

    public void Keep()
    {
        // ���� ������ ����
        if (haveMaterialDictionary.ContainsKey($"{level}�� ��"))
        {
            haveMaterialDictionary[$"{level}�� ��"]++;
        }
        else
        {
            haveMaterialDictionary.Add($"{level}�� ��", 1);
        }

        // ���� ���� ��� ����
        Text[] text = haveMaterialArea.GetComponentsInChildren<Text>();
        foreach (Text t in text)
        {
            Destroy(t.gameObject);
        }

        // ���� ���� ��� ����
        foreach (KeyValuePair<string, int> have in haveMaterialDictionary)
        {
            textPrefab.text = $"{have.Key} x {have.Value}";
            Instantiate(textPrefab, haveMaterialArea.transform);
        }

        level = 0;
        SetUpgradeInfo();
    }
}
