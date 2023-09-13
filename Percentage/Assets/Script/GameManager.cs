using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering.LookDev;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public Fade fadeAnimation;
    public Player player;
    public Weapon[] weapon;
    public WeaponData[] weaponData;
    public ObjectPool ObjectPool;

    public UserInterface ui;
    public StatusInfo statusPanel;
    public RoomReward rewardBoxPanel;

    public int coin;

    public bool isAttack;
    public bool isRightAttack;
    public bool isLeftAttack;
    public bool isUpAttack;
    public bool isDownAttack;
    public bool isOpenStatus;
    public bool isOpenBox;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        // 키보드 입력
        InputKeyboard();

        // 코인 개수
        ui.coinText.text = coin.ToString();
    }

    void InputKeyboard()
    {
        isAttack = Input.GetButtonDown("Fire1") || Input.GetButtonDown("Fire2");
        isRightAttack = Input.GetKeyDown(KeyCode.RightArrow);
        isLeftAttack = Input.GetKeyDown(KeyCode.LeftArrow);
        isUpAttack = Input.GetKeyDown(KeyCode.UpArrow);
        isDownAttack = Input.GetKeyDown(KeyCode.DownArrow);

        if (Input.GetKeyDown(KeyCode.I))
        {
            isOpenStatus = !isOpenStatus;
            statusPanel.gameObject.SetActive(isOpenStatus);
        }

        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            isOpenBox = false;
            rewardBoxPanel.gameObject.SetActive(false);
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

    public GameObject GenerateWeapon()
    {
        GameObject newWeapon = new GameObject();
        newWeapon.AddComponent<SpriteRenderer>();
        newWeapon.AddComponent<BoxCollider2D>();
        newWeapon.tag = "Weapon";

        return newWeapon;
    }
}
