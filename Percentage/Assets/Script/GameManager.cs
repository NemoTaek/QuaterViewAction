using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
    [Header("----- Component -----")]
    public CameraMap cam;
    public Fade fadeAnimation;
    public Player player;
    public BulletPool bulletPool;
    public Weapon[] weapon;
    public WeaponData[] weaponData;
    public Skill[] skill;
    public SkillData[] skillData;
    public ObjectPool objectPool;
    public Canvas itemCanvas;
    public ItemPool itemPool;
    public ItemData[] itemData;
    public GameObject itemPricePool;
    public Sprite[] statusEffectIcon;
    public Familiar[] familiarPool;

    [Header("----- UI Component -----")]
    public Canvas uiCanvas;
    public UserInterface ui;
    public StatusInfo statusPanel;
    public RoomReward rewardBoxPanel;
    public GetItemPanel getItemPanel;
    public GameResult gameResultPanel;
    public Sprite[] roomIcon;

    [Header("----- Key Input -----")]
    public bool isRightAttack;
    public bool isLeftAttack;
    public bool isUpAttack;
    public bool isDownAttack;
    public bool isUltimateRightAttack;
    public bool isUltimateLeftAttack;
    public bool isUltimateUpAttack;
    public bool isUltimateDownAttack;

    [Header("----- System Info -----")]
    public int stage;
    public int lastStageIndex;
    public bool isLoading;
    public bool isOpenStatus;
    public bool isOpenBox;
    public bool isOpenItemPanel;
    public int coin;
    public float elapsedTime = 0;
    public List<Sprite> getItemList;

    void Start()
    {
        getItemList = new List<Sprite>();
        GameInit();
    }

    void Update()
    {
        // 게임 시간
        elapsedTime += Time.deltaTime;

        // 키보드 입력
        InputKeyboard();

        // 코인 개수
        ui.coinText.text = coin.ToString();

        // UI창이 열려있으면 시간 멈추도록
        if (isOpenStatus || isOpenBox || isOpenItemPanel) GamePause();
        else GameResume();
    }

    public void GameInit()
    {
        // 스테이지 업
        stage++;

        // 플레이어, 카메라 위치 초기화
        player.transform.position = Vector3.forward;
        cam.transform.position = Vector3.back;

        // 전 스테이지의 아이템 삭제
        itemPool.ClearItemList();
        Item[] items = itemPool.GetComponentsInChildren<Item>(true);
        Item[] itemPrices = itemPricePool.GetComponentsInChildren<Item>(true);
        foreach (Item item in items)
        {
            Destroy(item.gameObject);
        }
        foreach (Item price in itemPrices)
        {
            Destroy(price.gameObject);
        }

        // 전 플레이에서 먹은 속성들 모두 초기화
        if(stage <= 1)
        {
            foreach (Bullet bullet in bulletPool.playerBullets)
            {
                bullet.isPenetrate = false;
                bullet.isSlow = false;
            }
        }
    }

    void InputKeyboard()
    {
        // 공격 입력
        isRightAttack = Input.GetKey(KeyCode.RightArrow);
        isLeftAttack = Input.GetKey(KeyCode.LeftArrow);
        isUpAttack = Input.GetKey(KeyCode.UpArrow);
        isDownAttack = Input.GetKey(KeyCode.DownArrow);
        isUltimateRightAttack = Input.GetKeyUp(KeyCode.RightArrow);
        isUltimateLeftAttack = Input.GetKeyUp(KeyCode.LeftArrow);
        isUltimateUpAttack = Input.GetKeyUp(KeyCode.UpArrow);
        isUltimateDownAttack = Input.GetKeyUp(KeyCode.DownArrow);

        // 인벤토리 창 입력
        if (Input.GetKeyDown(KeyCode.I))
        {
            isOpenStatus = !isOpenStatus;
            statusPanel.gameObject.SetActive(isOpenStatus);
        }

        // 상자 창 닫기 입력
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if(isOpenBox)   CloseRewardBoxPanel();
            if(isOpenItemPanel) CloseItemPanel();
        }

        // 무기 교체 입력
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            // 무기를 교체하면 무기 공격력에 해당하여 플레이어 공격력이 증가가 되고, UI도 변경된다.
            player.powerUp -= weapon[player.currentWeaponIndex].damage;
            player.currentWeaponIndex = player.currentWeaponIndex == player.getWeaponCount - 1 ? 0 : player.currentWeaponIndex + 1;
            player.powerUp += weapon[player.currentWeaponIndex].damage;
            player.hand[player.role].isChanged = true;
            ui.isChanged = true;
        }

        // 스킬 장착 입력
        // KeyCode.Alpha1 == 49
        for(int i=0; i<5; i++)
        {
            // 누르려는 키가 현재 사용중인 스킬이 아니고, 획득한 스킬이면 장착 가능
            if (player.currentSkillIndex != i && i < player.getSkillCount && Input.GetKeyDown((KeyCode)(49 + i)))
            {
                player.currentSkillIndex = i;
                ui.isChanged = true;
            }
        }

        // 액티브 아이템 사용 입력
        // 액티브 아이템을 가지고 있고, 게이지가 모두 차있을 때 스페이스바를 누르면 사용
        if (Input.GetKeyDown(KeyCode.Space) && player.activeItem && player.activeItem.currentGuage == player.activeItem.activeGuage)
        {
            player.activeItem.UseItem(player.activeItem.id);
            player.activeItem.currentGuage = 0;
        }
    }

    public void CloseStatusPanel()
    {
        isOpenStatus = false;
        statusPanel.gameObject.SetActive(false);
    }

    public void CloseRewardBoxPanel()
    {
        isOpenBox = false;
        rewardBoxPanel.gameObject.SetActive(false);
    }

    public void CloseItemPanel()
    {
        isOpenItemPanel = false;
        getItemPanel.gameObject.SetActive(false);
    }

    public GameObject GenerateWeapon()
    {
        GameObject newWeapon = new GameObject();
        //newWeapon.AddComponent<SpriteRenderer>();
        //newWeapon.AddComponent<PolygonCollider2D>();
        //newWeapon.AddComponent<Rigidbody2D>();
        newWeapon.tag = "Weapon";

        return newWeapon;
    }

    public GameObject GenerateSkill()
    {
        GameObject newSkill = new GameObject();
        newSkill.tag = "Skill";

        return newSkill;
    }

    public string SetAttackAnimation(Vector2 dirVec)
    {
        if (dirVec == Vector2.right) return "RightAttack";
        else if (dirVec == Vector2.left) return "LeftAttack";
        else if (dirVec == Vector2.up) return "UpAttack";
        else if (dirVec == Vector2.down) return "DownAttack";

        return null;
    }

    void GamePause()
    {
        Time.timeScale = 0;
    }

    void GameResume()
    {
        Time.timeScale = 1;
    }

    public IEnumerator GameResult()
    {
        yield return new WaitForSeconds(1f);

        gameResultPanel.gameObject.SetActive(true);

        GamePause();
    }
}
