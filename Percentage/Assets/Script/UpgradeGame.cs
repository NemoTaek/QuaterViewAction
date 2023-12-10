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

        // 저장된 데이터가 있다면 로드
        LoadData();
    }

    void Start()
    {
        // BGM 재생
        AudioManager.instance.BGMPlay(0);

        // 사용 가능 아이템 기본 세팅
        for (int i=0; i<upgradeGameItemDatas.Length; i++)
        {
            useItemButtonPrefab.Init(upgradeGameItemDatas[i]);
            UpgradeUseItemButton item = Instantiate(useItemButtonPrefab, useItemArea.transform);
            upgradeUseItemButtons[i] = item;
            item.gameObject.SetActive(false);
        }

        // 게임 시작 시 필요한 정보 세팅
        SetUpgradeInfo();
    }

    void Update()
    {
        // 창이 떠있으면 버튼 비활성화
        if (upgradeResultPanel.gameObject.activeSelf || warningPanel.gameObject.activeSelf || twoButtonPanel.gameObject.activeSelf)
        {
            upgradeButton.interactable = false;
            sellButton.interactable = false;
            keepButton.interactable = false;
        }   
        else
        {
            // 최종 강화 시에는 강화를 할 수 없음
            upgradeButton.interactable = level == 20 ? false : true;
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
        sellCost = (upgradeCost / 5) * (5 * (level / 5) * 2 + (level % 5));

        // 각종 텍스트 세팅
        // 최종 레벨과 그 전 레벨로 분리
        if (level == 20)
        {
            upgradeCost = 0;

            needProtectText.gameObject.SetActive(false);
            swordImage.sprite = swordImageSprite[level];
            levelText.text = "최고 강화레벨 달성";
            successText.text = "성공확률 : 0%";
            failureText.text = "실패확률 : 0%";
            destroyText.text = "파괴확률 : 0%";
        }
        else
        {
            // 10강부터 파괴 확률 존재
            if (level >= 10) needProtectText.gameObject.SetActive(true);
            else needProtectText.gameObject.SetActive(false);

            levelText.text = $"{level}강 > {level + 1}강";
            successText.text = $"성공확률 : {successPercentage[level]}%";
            failureText.text = $"실패확률 : {100 - successPercentage[level] - destroyPercentage[level]}%";
            destroyText.text = $"파괴확률 : {destroyPercentage[level]}%";
            needProtectText.text = $"필요 파괴 방지권 개수 : {useProtect[level]}";
        }

        // 공통 출력 텍스트
        swordImage.sprite = swordImageSprite[level];
        sellCostText.text = sellCost == 0 ? "0" : string.Format("{0:#,###}", sellCost);
        moneyText.text = money == 0 ? "0" : string.Format("{0:#,###}", money);

        // 강화 재료 세팅
        SetUpgradeCondition();

        // 기존 보유 재료 세팅
        SetHaveMaterial();
    }

    void SetUpgradeCondition()
    {
        // 강화 재료는 기본적으로 돈이 세팅되고, 추가적으로 강화석이나 하위단계 검이 있음
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
                Instantiate(materialTextPrefab, upgradeMaterialArea.transform);
            }
            upgradeMaterialTexts = upgradeMaterialArea.GetComponentsInChildren<Text>(true);
        }

        int index = 1;
        foreach (KeyValuePair<string, long> have in needMaterialDictionary)
        {
            upgradeMaterialTexts[index].gameObject.SetActive(true);
            if (have.Key.Equals("돈"))
            {
                upgradeMaterialTexts[index].text = level == 20 ? "" : string.Format("{0:#,###}원", have.Value);
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

        // 보유 재료 텍스트가 부족한만큼 먼저 채우기
        if (haveMaterialDictionary.Count >= haveMaterialTexts.Length)
        {
            int haveTextCount = haveMaterialDictionary.Count - haveMaterialTexts.Length + 1;
            for (int i=0; i<haveTextCount; i++)
            {
                Instantiate(materialTextPrefab, haveMaterialArea.transform);
            }
            haveMaterialTexts = haveMaterialArea.GetComponentsInChildren<Text>(true);
        }

        // 사용 가능 아이템 버튼 보유 여부 체크
        foreach (UpgradeUseItemButton itemButton in upgradeUseItemButtons)
        {
            if (itemButton.count > 0) itemButton.gameObject.SetActive(true);
            else itemButton.gameObject.SetActive(false);
        }

        int index = 1;
        foreach (KeyValuePair<string, long> have in haveMaterialDictionary)
        {
            // 보유 재료 목록 텍스트 설정
            haveMaterialTexts[index].gameObject.SetActive(true);
            haveMaterialTexts[index].text = $"{have.Key} x {have.Value}";
            index++;
        }

        // 재료를 소모해서 보유 재료 목록보다 텍스트 수가 더 많으면 나머지 비활성화
        if (haveMaterialDictionary.Count + 1 < haveMaterialTexts.Length)
        {
            for (int i = haveMaterialDictionary.Count + 1; i < haveMaterialTexts.Length; i++)
            {
                haveMaterialTexts[i].gameObject.SetActive(false);
            }
        }

        // 돈 갱신
        moneyText.text = money == 0 ? "0" : string.Format("{0:#,###}", money);
    }

    void AddMaterialDictionary(string item, long count)
    {
        // 재료가 있으면 개수만큼 값 추가, 없으면 키와 값 추가
        // 추가 개수에 음수가 들어갈 수 있다. 만약 모두 사용해서 0 이하가 된다면 삭제
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
            // 앞에서 강화 재료가 있는지 체크를 했으므로 아래 로직은 강화가 가능하다는 전제가 있음
            foreach (KeyValuePair<string, long> have in needMaterialDictionary)
            {
                if (have.Key.Equals("돈")) money -= have.Value;
                else AddMaterialDictionary(have.Key, have.Value * -1);
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
                // 파괴 방지권이 있다면 파괴 방지권을 사용하고 현재 상태 유지
                bool isHaveProtect = haveMaterialDictionary.ContainsKey("파괴 방지권");
                if (isHaveProtect && haveMaterialDictionary["파괴 방지권"] >= useProtect[level])
                {
                    AddMaterialDictionary("파괴 방지권", useProtect[level] * -1);
                    result[0].text = "FAILED";
                    result[0].color = new Color(200 / 255f, 198 / 255f, 196 / 255f);
                    result[1].text = "파괴 방지권을 사용하여 검이 파괴되지 않습니다.";
                    AudioManager.instance.EffectPlay(AudioManager.Effect.Fail);
                }
                // 파괴 방지권이 없거나 개수가 부족하면 파괴
                else
                {
                    // 파괴 시 레벨에 따라 검의 파편 획득
                    AddMaterialDictionary("검의 파편", destroyPercentage[level]);
                    level = 0;
                    result[0].text = "DESTROYED";
                    result[0].color = new Color(200 / 255f, 198 / 255f, 196 / 255f);
                    result[1].text = "강화에 실패하여 장비가 파괴됩니다.";
                    AudioManager.instance.EffectPlay(AudioManager.Effect.Destroyed);
                }
            }
            // 나머지는 실패
            else
            {
                result[0].text = "FAILED";
                result[0].color = new Color(227 / 255f, 78 / 255f, 70 / 255f);

                // 5단위 강화에서는 실패해도 단계가 내려가지 않음
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

        // 강화 후 저장
        SaveData();
    }

    public void Sell()
    {
        money += sellCost;
        level = 0;
        SetUpgradeInfo();
        AudioManager.instance.EffectPlay(AudioManager.Effect.ButtonClick);

        // 판매 후 저장
        SaveData();
    }

    public void Keep()
    {
        // 보유 아이템 세팅
        AddMaterialDictionary($"{level}강 검", 1);

        level = 0;
        SetUpgradeInfo();
        AudioManager.instance.EffectPlay(AudioManager.Effect.ButtonClick);

        // 킵한 후 저장
        SaveData();
    }

    public void ClickOKButton()
    {
        // 자네 지금 뭐하는건가?
        // 행동할 때 한번 더 확인하는 행동은 '구매', '아이템 사용' 이 존재한다.
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
                buyItem = "강화석";
                break;
            case 1:
                buyItem = "강화석 x 10";
                break;
            case 2:
                buyItem = "강화석 x 50";
                break;
            case 3:
                buyItem = "12강 강화권";
                break;
            case 4:
                buyItem = "13강 강화권";
                break;
            case 5:
                buyItem = "14강 강화권";
                break;
            case 6:
                buyItem = "15강 강화권";
                break;
            case 7:
            case 8:
                buyItem = "파괴 방지권";
                break;
            case 9:
            case 10:
                buyItem = "파괴 방지권 x 5";
                break;
            case 11:
            case 12:
                buyItem = "파괴 방지권 x 10";
                break;
        }

        twoButtonPanel.gameObject.SetActive(true);
        twoButtonText.text = $"{buyItem}을(를)\n구매하시겠습니까?";

        AudioManager.instance.EffectPlay(AudioManager.Effect.ButtonClick);
    }

    public void BuyItem()
    {
        bool isBuyComplete = false;
        bool isHaveSwordFragment = haveMaterialDictionary.ContainsKey("검의 파편");

        // 판매 아이템 id 별로 지불 재료(금액)은 차감하고, 상품을 얻는다.
        switch (selectItem)
        {
            case 0:
                // 재료가 있다면
                if (money >= 1000000)
                {
                    // 재료 차감
                    money -= 1000000;

                    // 이미 해당 상품이 있으면 얹고, 없으면 추가
                    AddMaterialDictionary("강화석", 1);

                    // 구매 완료
                    isBuyComplete = true;
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
                    AddMaterialDictionary("강화석", 10);
                    isBuyComplete = true;
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
                    AddMaterialDictionary("강화석", 50);
                    isBuyComplete = true;
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
                    AddMaterialDictionary("검의 파편", -5);
                    AddMaterialDictionary("12강 강화권", 1);
                    upgradeUseItemButtons[0].SetCountText(upgradeUseItemButtons[0].count + 1);
                    isBuyComplete = true;
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
                    AddMaterialDictionary("검의 파편", -7);
                    AddMaterialDictionary("13강 강화권", 1);
                    upgradeUseItemButtons[1].SetCountText(upgradeUseItemButtons[1].count + 1);
                    isBuyComplete = true;
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
                    AddMaterialDictionary("검의 파편", -10);
                    AddMaterialDictionary("14강 강화권", 1);
                    upgradeUseItemButtons[2].SetCountText(upgradeUseItemButtons[2].count + 1);
                    isBuyComplete = true;
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = isHaveSwordFragment ? $"검의 파편 {10 - haveMaterialDictionary["검의 파편"]}개가 부족합니다." : "검의 파편 10개가 부족합니다.";
                }
                break;
            case 6:
                if (isHaveSwordFragment && haveMaterialDictionary["검의 파편"] >= 15) {
                    AddMaterialDictionary("검의 파편", -15);
                    AddMaterialDictionary("15강 강화권", 1);
                    upgradeUseItemButtons[3].SetCountText(upgradeUseItemButtons[3].count + 1);
                    isBuyComplete = true;
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
                    AddMaterialDictionary("파괴 방지권", 1);
                    isBuyComplete = true;
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
                    AddMaterialDictionary("검의 파편", -5);
                    AddMaterialDictionary("파괴 방지권", 1);
                    isBuyComplete = true;
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
                    AddMaterialDictionary("파괴 방지권", 5);
                    isBuyComplete = true;
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = $"{45000000 - money}원이 부족합니다.";
                }
                break;
            case 10:
                if (isHaveSwordFragment && haveMaterialDictionary["검의 파편"] >= 22) {
                    AddMaterialDictionary("검의 파편", -22);
                    AddMaterialDictionary("파괴 방지권", 5);
                    isBuyComplete = true;
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
                    AddMaterialDictionary("파괴 방지권", 10);
                    isBuyComplete = true;
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = $"{80000000 - money}원이 부족합니다.";
                }
                break;
            case 12:
                if (isHaveSwordFragment && haveMaterialDictionary["검의 파편"] >= 40) {
                    AddMaterialDictionary("검의 파편", -40);
                    AddMaterialDictionary("파괴 방지권", 10);
                    isBuyComplete = true;
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

        if (isBuyComplete) AudioManager.instance.EffectPlay(AudioManager.Effect.BuyItem);
        else AudioManager.instance.EffectPlay(AudioManager.Effect.Fail);

        // 아이템 구매 후 저장
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
                buyItem = "12강 강화권";
                break;
            case 4:
                buyItem = "13강 강화권";
                break;
            case 5:
                buyItem = "14강 강화권";
                break;
            case 6:
                buyItem = "15강 강화권";
                break;
        }

        twoButtonPanel.gameObject.SetActive(true);
        twoButtonText.text = $"{buyItem}을(를)\n사용하시겠습니까?";

        AudioManager.instance.EffectPlay(AudioManager.Effect.ButtonClick);
    }

    public void UseItem()
    {
        switch (selectItem)
        {
            case 3:
                if (haveMaterialDictionary.ContainsKey("12강 강화권"))
                {
                    AddMaterialDictionary("12강 강화권", -1);
                    upgradeUseItemButtons[0].SetCountText(upgradeUseItemButtons[0].count - 1);
                    level = 12;
                    SetUpgradeInfo();
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = "12강 강화권이 부족합니다.";
                }
                break;
            case 4:
                if (haveMaterialDictionary.ContainsKey("13강 강화권"))
                {
                    AddMaterialDictionary("13강 강화권", -1);
                    upgradeUseItemButtons[1].SetCountText(upgradeUseItemButtons[1].count - 1);
                    level = 13;
                    SetUpgradeInfo();
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = "13강 강화권이 부족합니다.";
                }
                break;
            case 5:
                if (haveMaterialDictionary.ContainsKey("14강 강화권"))
                {
                    AddMaterialDictionary("14강 강화권", -1);
                    upgradeUseItemButtons[2].SetCountText(upgradeUseItemButtons[2].count - 1);
                    level = 14;
                    SetUpgradeInfo();
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = "14강 강화권이 부족합니다.";
                }
                break;
            case 6:
                if (haveMaterialDictionary.ContainsKey("15강 강화권"))
                {
                    AddMaterialDictionary("15강 강화권", -1);
                    upgradeUseItemButtons[3].SetCountText(upgradeUseItemButtons[3].count - 1);
                    level = 15;
                    SetUpgradeInfo();
                }
                else
                {
                    warningPanel.gameObject.SetActive(true);
                    warningText.text = "15강 강화권이 부족합니다.";
                }
                break;
        }

        // 보유 아이템 갱신
        SetHaveMaterial();

        AudioManager.instance.EffectPlay(AudioManager.Effect.ButtonClick);

        // 아이템 사용 후 저장
        SaveData();
    }

    void SaveData()
    {
        // 저장해야 할 데이터를 Dictionary에 추가 (보유 재료, 돈, 레벨)
        Dictionary<string, long> saveData = new Dictionary<string, long>(haveMaterialDictionary);
        saveData.Add("돈", money);
        saveData.Add("레벨", level);
        string saveDataJson = DataJson.DictionaryToJson(saveData);

        // 지정 경로에 세이브파일 저장
        // 경로가 없다면 생성
        string path = Application.dataPath + "/Data";
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        // 파일 쓰기 전에 암호화를 한번 해볼까?
        // System.Text.Encoding.UTF8.GetBytes(string): 매개변수 문자열을 byte 배열로 인코딩하는 메소드
        // System.Convert.ToBase64String(byte[]): 매개변수 바이트 배열을 인코딩된 base64문자열로 변환하는 메소드
        byte[] byteData = System.Text.Encoding.UTF8.GetBytes(saveDataJson);
        string encodedJson = System.Convert.ToBase64String(byteData);

        // File.WriteAllText(path, text): 새 파일을 만들고, 파일을 쓴 후 파일을 닫음. 이미 파일이 존재하면 덮어씀
        File.WriteAllText(path + "/saveData.json", encodedJson);
    }

    void LoadData()
    {
        // 세이브파일 로드
        string path = Application.dataPath + "/Data";

        // File.Exists(path): 해당 경로에 파일이 있는지 확인
        bool isExistsDataFile = File.Exists(path + "/saveData.json");
        if (isExistsDataFile)
        {
            // File.ReadAllText(path): 텍스트파일을 열고, 모든 텍스트를 읽은 후 파일을 닫음
            string loadDataJson = File.ReadAllText(path + "/saveData.json");

            // 마찬가지로 암호화된 파일을 해독해볼까?
            // System.Convert.FromBase64String(string): 인코딩된 base64문자열로부터 byte 배열로 변환하는 메소드
            // System.Text.Encoding.UTF8.GetString(byte[]): 매개변수 byte 배열에서 문자열로 인코딩하는 메소드
            byte[] byteData = System.Convert.FromBase64String(loadDataJson);
            string encodedJson = System.Text.Encoding.UTF8.GetString(byteData);

            // 로드할 파일이 있으면 로드
            if (encodedJson != null && encodedJson.Length > 0)
            {
                Dictionary<string, long> loadData = DataJson.DictionaryFromJson<string, long>(encodedJson);

                // 파싱한 Dictionary를 순회하며 money와 level 따로 저장 후 삭제
                foreach (KeyValuePair<string, long> data in loadData)
                {
                    if (data.Key == "돈")
                    {
                        money = data.Value;
                    }
                    else if (data.Key == "레벨")
                    {
                        level = (int)data.Value;
                    }
                }
                loadData.Remove("돈");
                loadData.Remove("레벨");

                // 나머지는 보유 재료 Dictionary로 설정
                haveMaterialDictionary = loadData;
            }
        }
        else
        {
            // 기본 자산 10억
            money = 1000000000;
            SaveData();
        }
    }

    public void ExitUpgradeGame()
    {
        // 나가기 전 데이터 저장
        SaveData();

        AudioManager.instance.BGMStop();
        SceneManager.LoadScene("Intro");
    }
}
