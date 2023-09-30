using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [Header("----- Component -----")]
    public static GameManager instance;
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
    public Room currentRoom;
    public ItemData[] itemData;
    public Map map;

    [Header("----- UI Component -----")]
    public UserInterface ui;
    public StatusInfo statusPanel;
    public RoomReward rewardBoxPanel;
    public GetItemPanel getItemPanel;
    public GameResult gameResultPanel;

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
    public bool isOpenStatus;
    public bool isOpenBox;
    public bool isOpenItemPanel;
    public int coin;
    public int mapPosition = 41;
    public float elapsedTime = 0;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if (currentRoom.roomType == Room.RoomType.Start)    currentRoom.isVisited = true;
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
        newWeapon.AddComponent<SpriteRenderer>();
        newWeapon.AddComponent<BoxCollider2D>();
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

    public IEnumerator GameOver()
    {
        yield return new WaitForSeconds(1f);

        gameResultPanel.gameObject.SetActive(true);

        GamePause();
    }
}
