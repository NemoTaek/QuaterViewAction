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
    Text warningText;

    void Awake()
    {
        successPercentage = new int[20] { 100, 95, 90, 85, 80, 75, 70, 65, 60, 55, 50, 45, 40, 35, 30, 25, 20, 15, 10, 5 };
        destroyPercentage = new int[20] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 1, 2, 3, 4, 5, 6, 8, 10, 15, 20 };
        haveMaterialDictionary = new Dictionary<string, long>();
        needMaterialDictionary = new Dictionary<string, long>();
        warningText = warningPanel.GetComponentInChildren<Text>();

        // 초기 자본금 100만원
        //money = 100000000;
        money = 100000000000;
        haveMaterialDictionary.Add("강화석", 10000);
        haveMaterialDictionary.Add("14강 검", 10000);
        haveMaterialDictionary.Add("15강 검", 10000);
        haveMaterialDictionary.Add("16강 검", 10000);
        haveMaterialDictionary.Add("17강 검", 10000);
        haveMaterialDictionary.Add("18강 검", 10000);
        haveMaterialDictionary.Add("19강 검", 10000);
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

        // 상자 창 닫기 입력
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            upgradeResultPanel.gameObject.SetActive(false);
            warningPanel.gameObject.SetActive(false);
        }
    }

    void SetUpgradeInfo()
    {
        // 강화 비용 계산
        upgradeCost = Mathf.CeilToInt(1000 + (Mathf.Pow(100, 3) * Mathf.Pow(level, 2.7f) / 400));

        // 판매 비용 계산
        sellCost = upgradeCost / 5 * level;

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

        // 기존 보유 재료 세팅
        SetHaveMaterial();
    }

    void SetUpgradeCondition()
    {
        needMaterialDictionary.Clear();
        needMaterialDictionary.Add("돈", upgradeCost);

        switch (level)
        {
            case 10:
            case 11:
            case 12:
            case 13:
            case 14:
                needMaterialDictionary.Add("강화석", 1);
                break;
            case 15:
                needMaterialDictionary.Add("강화석", 2);
                needMaterialDictionary.Add($"{level - 1}강 검", 1);
                break;
            case 16:
                needMaterialDictionary.Add("강화석", 3);
                needMaterialDictionary.Add($"{level - 1}강 검", 1);
                break;
            case 17:
                needMaterialDictionary.Add("강화석", 4);
                needMaterialDictionary.Add($"{level - 1}강 검", 1);
                break;
            case 18:
                needMaterialDictionary.Add("강화석", 5);
                needMaterialDictionary.Add($"{level - 1}강 검", 1);
                break;
            case 19:
                needMaterialDictionary.Add("강화석", 6);
                needMaterialDictionary.Add($"{level - 1}강 검", 2);
                break;
        }

        // 1개는 어짜피 강화 비용으로 있어야 되니까 그냥 넣고, 나머지는 필요하면 활성화 후 세팅, 불필요하면 비활성화
        // 오류가 나지 않도록 부족한 만큼 미리 텍스트 생성
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
            if (have.Key.Equals("돈"))
            {
                upgradeMaterialTexts[index].text = string.Format("{0:#,###}원", have.Value);
            }
            else
            {
                upgradeMaterialTexts[index].text = $"{have.Key} x {have.Value}";
            }
            index++;
        }

        // 세팅 후 넘치면 텍스트 비활성화
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

    bool CheckUpgradeCondition()
    {
        bool isAvailableUpgrade = false;
        long upgradeStone = 0;
        long materialSword = 0;
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
            // 강화 재료 소모
            foreach (KeyValuePair<string, long> have in needMaterialDictionary)
            {
                if (have.Key.Equals("돈"))
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
                AudioManager.instance.EffectPlay(AudioManager.Effect.Success);
            }
            // 파괴
            else if (random < successPercentage[level] + destroyPercentage[level])
            {
                level = 0;
                result[0].text = "DESTROYED";
                result[0].color = new Color(200 / 255f, 198 / 255f, 196 / 255f);
                result[1].text = "강화에 실패하여 장비가 파괴됩니다.";
                AudioManager.instance.EffectPlay(AudioManager.Effect.Destroyed);
            }
            // 나머지는 실패
            else
            {
                result[0].text = "FAILED";
                result[0].color = new Color(227 / 255f, 78 / 255f, 70 / 255f);

                if (level % 5 != 0)
                {
                    level--;
                    result[1].text = "강화에 실패하여 한단계 하락합니다.";
                }
                else
                {
                    result[1].text = "강화에 실패하였습니다.";
                }
                
                AudioManager.instance.EffectPlay(AudioManager.Effect.Fail);
            }

            // 강화 후 정보 세팅
            SetUpgradeInfo();
        }

        // 강화 재료가 부족할 경우 재료가 부족합니다 출력
        else
        {
            warningPanel.gameObject.SetActive(true);
            warningText.text = "강화 재료가 부족합니다.";
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
        // 보유 아이템 세팅
        if (haveMaterialDictionary.ContainsKey($"{level}강 검"))
        {
            haveMaterialDictionary[$"{level}강 검"]++;
        }
        else
        {
            haveMaterialDictionary.Add($"{level}강 검", 1);
        }

        level = 0;
        SetUpgradeInfo();
        AudioManager.instance.EffectPlay(AudioManager.Effect.ButtonClick);
    }

    public void BuyItem(int itemId)
    {
        bool isHaveStone = haveMaterialDictionary.ContainsKey("강화석");
        bool isHave12Upgrade = haveMaterialDictionary.ContainsKey("12강 강화권");
        bool isHave13Upgrade = haveMaterialDictionary.ContainsKey("13강 강화권");
        bool isHave14Upgrade = haveMaterialDictionary.ContainsKey("14강 강화권");
        bool isHave15Upgrade = haveMaterialDictionary.ContainsKey("15강 강화권");
        bool isHaveProtect = haveMaterialDictionary.ContainsKey("파괴 방지권");
        bool isHaveSwordFragment = haveMaterialDictionary.ContainsKey("검의 파편");

        // 판매 아이템 id 별로 지불 재료(금액)은 차감하고, 상품을 얻는다.
        switch (itemId)
        {
            case 0:
                // 재료가 있다면
                if (money >= 1000000)
                {
                    // 재료 차감
                    money -= 1000000;

                    // 이미 해당 상품이 있으면 얹고, 없으면 추가
                    if (isHaveStone) haveMaterialDictionary["강화석"]++;
                    else haveMaterialDictionary.Add("강화석", 1);
                }
                // 재료가 없으면 못산다 창 노출
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = $"{1000000 - money}원이 부족합니다.";
                }
                break;
            case 1:
                if (money >= 9000000)
                {
                    money -= 9000000;

                    if (isHaveStone) haveMaterialDictionary["강화석"] += 10;
                    else haveMaterialDictionary.Add("강화석", 10);
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = $"{9000000 - money}원이 부족합니다.";
                }
                break;
            case 2:
                if (money >= 40000000)
                {
                    money -= 40000000;

                    if (isHaveStone) haveMaterialDictionary["강화석"] += 50;
                    else haveMaterialDictionary.Add("강화석", 50);
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = $"{40000000 - money}원이 부족합니다.";
                }
                break;
            case 3:
                if (isHaveSwordFragment && haveMaterialDictionary["검의 파편"] >= 5)
                {
                    haveMaterialDictionary["검의 파편"] -= 5;

                    if (isHave12Upgrade) haveMaterialDictionary["12강 강화권"]++;
                    else haveMaterialDictionary.Add("12강 강화권", 1);
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = isHaveSwordFragment ? $"검의 파편 {5 - haveMaterialDictionary["검의 파편"]}개가 부족합니다." : "검의 파편 5개 부족합니다.";
                }
                break;
            case 4:
                if (isHaveSwordFragment && haveMaterialDictionary["검의 파편"] >= 7)
                {
                    haveMaterialDictionary["검의 파편"] -= 7;

                    if (isHave13Upgrade) haveMaterialDictionary["13강 강화권"]++;
                    else haveMaterialDictionary.Add("13강 강화권", 1);
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = isHaveSwordFragment ? $"검의 파편 {7 - haveMaterialDictionary["검의 파편"]}개가 부족합니다." : "검의 파편 7개가 부족합니다.";
                }
                break;
            case 5:
                if (isHaveSwordFragment && haveMaterialDictionary["검의 파편"] >= 10)
                {
                    haveMaterialDictionary["검의 파편"] -= 10;

                    if (isHave14Upgrade) haveMaterialDictionary["14강 강화권"]++;
                    else haveMaterialDictionary.Add("14강 강화권", 1);
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = isHaveSwordFragment ? $"검의 파편 {10 - haveMaterialDictionary["검의 파편"]}개가 부족합니다." : "검의 파편 10개가 부족합니다.";
                }
                break;
            case 6:
                if (isHaveSwordFragment && haveMaterialDictionary["검의 파편"] >= 15) {
                    haveMaterialDictionary["검의 파편"] -= 15;

                    if (isHave15Upgrade) haveMaterialDictionary["15강 강화권"]++;
                    else haveMaterialDictionary.Add("15강 강화권", 1);
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = isHaveSwordFragment ? $"검의 파편 {15 - haveMaterialDictionary["검의 파편"]}개가 부족합니다." : "검의 파편 15개가 부족합니다.";
                }
                break;
            case 7:
                if (money >= 10000000)
                {
                    money -= 10000000;

                    if (isHaveProtect) haveMaterialDictionary["파괴 방지권"]++;
                    else haveMaterialDictionary.Add("파괴 방지권", 1);
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = $"{10000000 - money}원이 부족합니다.";
                }
                break;
            case 8:
                if (isHaveSwordFragment && haveMaterialDictionary["검의 파편"] >= 5)
                {
                    haveMaterialDictionary["검의 파편"] -= 5;

                    if (isHaveProtect) haveMaterialDictionary["파괴 방지권"]++;
                    else haveMaterialDictionary.Add("파괴 방지권", 1);
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = isHaveSwordFragment ? $"검의 파편 {5 - haveMaterialDictionary["검의 파편"]}개가 부족합니다." : "검의 파편 5개가 부족합니다.";
                }
                break;
            case 9:
                if (money >= 45000000)
                {
                    money -= 45000000;

                    if (isHaveProtect) haveMaterialDictionary["파괴 방지권"] += 5;
                    else haveMaterialDictionary.Add("파괴 방지권", 5);
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = $"{45000000 - money}원이 부족합니다.";
                }
                break;
            case 10:
                if (isHaveSwordFragment && haveMaterialDictionary["검의 파편"] >= 22) {
                    haveMaterialDictionary["검의 파편"] -= 22;

                    if (isHaveProtect) haveMaterialDictionary["파괴 방지권"] += 5;
                    else haveMaterialDictionary.Add("파괴 방지권", 5);
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = isHaveSwordFragment ? $"검의 파편 {22 - haveMaterialDictionary["검의 파편"]}개가 부족합니다." : "검의 파편 22개가 부족합니다.";
                }
                break;
            case 11:
                if (money >= 80000000)
                {
                    money -= 80000000;

                    if (isHaveProtect) haveMaterialDictionary["파괴 방지권"] += 10;
                    else haveMaterialDictionary.Add("파괴 방지권", 10);
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = $"{80000000 - money}원이 부족합니다.";
                }
                break;
            case 12:
                if (isHaveSwordFragment && haveMaterialDictionary["검의 파편"] >= 40) {
                    haveMaterialDictionary["검의 파편"] -= 40;

                    if (isHaveProtect) haveMaterialDictionary["파괴 방지권"] += 10;
                    else haveMaterialDictionary.Add("파괴 방지권", 10);
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = isHaveSwordFragment ? $"검의 파편 {40 - haveMaterialDictionary["검의 파편"]}개가 부족합니다." : "검의 파편 40개가 부족합니다.";
                }
                break;
        }

        // 보유 아이템 갱신
        SetHaveMaterial();
    }
}
