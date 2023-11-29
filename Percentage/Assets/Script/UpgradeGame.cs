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

        // 초기 자본금 100만원
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
        // 강화 비용 계산
        upgradeCost = Mathf.CeilToInt(1000 + (Mathf.Pow(100, 3) * Mathf.Pow(level, 2.7f) / 400));

        // 판매 비용 계산
        sellCost = upgradeCost / 8 * level;

        // 각종 텍스트 세팅
        swordImage.sprite = swordImageSprite[level];
        levelText.text = $"{level}강 > {level + 1}강";
        successText.text = $"성공확률 : {successPercentage[level]}%";
        failureText.text = $"실패확률 : {100 - successPercentage[level] - destroyPercentage[level]}%";
        destroyText.text = $"파괴확률 : {destroyPercentage[level]}%";
        sellCostText.text = sellCost == 0 ? "0" : string.Format("{0:#,###}", sellCost);
        moneyText.text = money == 0 ? "0" : string.Format("{0:#,###}", money);

        // 강화 재료 세팅
        SetUpgradeCondition();

        // 1개는 어짜피 강화 비용으로 있어야 되니까 그냥 넣고, 나머지는 필요하면 활성화 후 세팅, 불필요하면 비활성화
        // 오류가 나지 않도록 부족한 만큼 미리 텍스트 생성
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

        // 세팅 후 넘치면 텍스트 비활성화
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
        needMaterial.Add(string.Format("{0:#,###}원", upgradeCost));

        switch (level)
        {
            case 10:
            case 11:
            case 12:
            case 13:
            case 14:
                needMaterial.Add("강화석 x 1");
                break;
            case 15:
                needMaterial.Add("강화석 x 2");
                needMaterial.Add($"{level - 1}강 검 x 1");
                break;
            case 16:
                needMaterial.Add("강화석 x 3");
                needMaterial.Add($"{level - 1}강 검 x 1");
                break;
            case 17:
                needMaterial.Add("강화석 x 4");
                needMaterial.Add($"{level - 1}강 검 x 1");
                break;
            case 18:
                needMaterial.Add("강화석 x 5");
                needMaterial.Add($"{level - 1}강 검 x 1");
                break;
            case 19:
                needMaterial.Add("강화석 x 6");
                needMaterial.Add($"{level - 1}강 검 x 2");
                break;
        }
    }

    bool CheckUpgradeCondition()
    {
        bool isAvailableUpgrade = false;
        int upgradeStone = 0;
        int materialSword = 0;
        if (haveMaterialDictionary.ContainsKey("강화석")) {
            upgradeStone = haveMaterialDictionary["강화석"];
        }
        if (haveMaterialDictionary.ContainsKey($"{level - 1}강 검"))
        {
            materialSword = haveMaterialDictionary[$"{level - 1}강 검"];
        }

        switch (level)
        {
            // 10강부터는 강화석과 전단계의 검 하나가 강화 재료
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
            // 강화 메소 소모
            money -= upgradeCost;

            // 강화 결과창 세팅
            upgradeResultPanel.gameObject.SetActive(true);
            Text[] result = upgradeResultPanel.GetComponentsInChildren<Text>();

            int random = Random.Range(0, 100);
            // 성공
            if (random < successPercentage[level])
            {
                level++;
                result[0].text = "SUCCESS";
                result[0].color = new Color(235 / 255f, 192 / 255f, 78 / 255f);
                result[1].text = "강화에 성공하여 한단계 상승합니다.";
            }
            // 파괴
            else if (random < successPercentage[level] + destroyPercentage[level])
            {
                level = 0;
                result[0].text = "DESTROYED";
                result[0].color = new Color(200 / 255f, 198 / 255f, 196 / 255f);
                result[1].text = "강화에 실패하여 장비가 파괴됩니다.";
            }
            // 나머지는 실패
            else
            {
                level--;
                result[0].text = "FAILED";
                result[0].color = new Color(227 / 255f, 78 / 255f, 70 / 255f);
                result[1].text = "강화에 실패하여 한단계 하락합니다.";
            }

            // 강화 후 정보 세팅
            SetUpgradeInfo();
        }

        // 강화 재료가 부족할 경우 재료가 부족합니다 출력
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
        // 보유 아이템 세팅
        if (haveMaterialDictionary.ContainsKey($"{level}강 검"))
        {
            haveMaterialDictionary[$"{level}강 검"]++;
        }
        else
        {
            haveMaterialDictionary.Add($"{level}강 검", 1);
        }

        // 기존 보유 재료 삭제
        Text[] text = haveMaterialArea.GetComponentsInChildren<Text>();
        foreach (Text t in text)
        {
            Destroy(t.gameObject);
        }

        // 기존 보유 재료 갱신
        foreach (KeyValuePair<string, int> have in haveMaterialDictionary)
        {
            textPrefab.text = $"{have.Key} x {have.Value}";
            Instantiate(textPrefab, haveMaterialArea.transform);
        }

        level = 0;
        SetUpgradeInfo();
    }
}
