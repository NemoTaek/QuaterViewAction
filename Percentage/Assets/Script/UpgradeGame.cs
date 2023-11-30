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
    public Image warningPanel;
    public Button upgradeButton;
    public Button sellButton;
    public Button keepButton;

    void Awake()
    {
        successPercentage = new int[20] { 100, 95, 90, 85, 80, 75, 70, 65, 60, 55, 50, 45, 40, 35, 30, 25, 20, 15, 10, 5 };
        destroyPercentage = new int[20] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 3, 4, 5, 6, 8, 10, 15, 20 };
        haveMaterialDictionary = new Dictionary<string, long>();
        needMaterialDictionary = new Dictionary<string, long>();

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
            upgradeButton.interactable = true;
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
        swordImage.sprite = swordImageSprite[level];
        levelText.text = $"{level}�� > {level + 1}��";
        successText.text = $"����Ȯ�� : {successPercentage[level]}%";
        failureText.text = $"����Ȯ�� : {100 - successPercentage[level] - destroyPercentage[level]}%";
        destroyText.text = $"�ı�Ȯ�� : {destroyPercentage[level]}%";
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
                upgradeMaterialTexts[index].text = string.Format("{0:#,###}��", have.Value);
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
                level = 0;
                result[0].text = "DESTROYED";
                result[0].color = new Color(200 / 255f, 198 / 255f, 196 / 255f);
                result[1].text = "��ȭ�� �����Ͽ� ��� �ı��˴ϴ�.";
                AudioManager.instance.EffectPlay(AudioManager.Effect.Destroyed);
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
        if (haveMaterialDictionary.ContainsKey($"{level}�� ��"))
        {
            haveMaterialDictionary[$"{level}�� ��"]++;
        }
        else
        {
            haveMaterialDictionary.Add($"{level}�� ��", 1);
        }

        level = 0;
        SetUpgradeInfo();
        AudioManager.instance.EffectPlay(AudioManager.Effect.ButtonClick);
    }
}
