using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

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
    public EnemyData[] enemyData;

    [Header("----- UI Component -----")]
    public Canvas uiCanvas;
    public UserInterface ui;
    public StatusInfo statusPanel;
    public RoomReward rewardBoxPanel;
    public GetItemPanel getItemPanel;
    public GameResult gameResultPanel;
    public Sprite[] roomIcon;
    public Image blur;
    public Image fade;

    [Header("----- Key Input -----")]
    public bool isRightAttack;
    public bool isLeftAttack;
    public bool isUpAttack;
    public bool isDownAttack;
    public bool isUltimateRightAttack;
    public bool isUltimateLeftAttack;
    public bool isUltimateUpAttack;
    public bool isUltimateDownAttack;
    public bool isInputRKey;
    public float rKeyTimer;

    [Header("----- System Info -----")]
    public int stage;
    public bool isLoading;
    public bool isOpenStatus;
    public bool isOpenBox;
    public bool isOpenItemPanel;
    public int coin;
    public float elapsedTime = 0;
    public List<Item> getItemList;
    public List<int> setItemList;
    public Dictionary<string, bool> quizList;

    void Start()
    {
        getItemList = new List<Item>();
        setItemList = new List<int>();

        GameInit();
        SetQuizList();
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

    void ScreenInit()
    {
        // 스테이지 이동 시 blur와 fade 다시 초기화
        Color blurColor = blur.color;
        blurColor.a = 0;
        blur.color = blurColor;
        blur.rectTransform.localScale = Vector3.one;
        Color fadeColor = fade.color;
        fadeColor.a = 0;
        fade.color = fadeColor;

        // 플레이어, 카메라 위치 초기화
        player.transform.position = Vector3.forward;
        cam.transform.position = Vector3.back;
    }

    void ItemInit()
    {
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
    }

    void PropertyInit()
    {
        foreach (Bullet bullet in bulletPool.playerBullets)
        {
            bullet.isPenetrate = false;
            bullet.isSlow = false;
        }
    }

    public void GameInit()
    {
        stage++;
        ScreenInit();
        ItemInit();

        // 1스테이지면 처음 시작하는 것이므로 전 플레이에서 먹은 속성들 모두 초기화
        if(stage <= 1)
        {
            PropertyInit();
        }

        // 사운드 추가
        AudioManager.instance.BGMPlay(Random.Range(1, 15));

        // 다시 플레이어 움직임 활성화
        player.stopMove = false;
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
            AudioManager.instance.EffectPlay(AudioManager.Effect.PanelOpen);
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
            //player.powerUp += weapon[player.currentWeaponIndex].damage;
            player.hand[player.role].isWeaponChanged = true;
            ui.isChanged = true;
            AudioManager.instance.EffectPlay(AudioManager.Effect.Swap);
        }

        // 스킬 장착 입력
        // KeyCode.Alpha1 == 49
        for(int i=0; i<5; i++)
        {
            // 누르려는 키가 현재 사용중인 스킬이 아니고, 획득한 스킬이면 장착 가능
            if (player.currentSkillIndex != i && i < player.getSkillCount && Input.GetKeyDown((KeyCode)(49 + i)))
            {
                player.currentSkillIndex = i;
                player.hand[player.role].isSkillChanged = true;
                ui.isChanged = true;
            }
        }

        // 액티브 아이템 사용 입력
        // 액티브 아이템을 가지고 있고, 게이지가 모두 차있을 때 스페이스바를 누르면 사용
        // 액티브 게이지가 -1이면 무한 사용 가능을 의미
        if (Input.GetKeyDown(KeyCode.Space) && player.activeItem && (player.activeItem.currentGuage == player.activeItem.activeGuage || player.activeItem.activeGuage == -1))
        {
            StartCoroutine(player.activeItem.UseItem(player.activeItem.id));
            player.activeItem.currentGuage = 0;
            ui.isChanged = true;
        }

        // 다시시작 키 입력
        isInputRKey = Input.GetKey(KeyCode.R);
        if (isInputRKey)
        {
            rKeyTimer += Time.deltaTime;
            if (rKeyTimer >= 3f)
            {
                rKeyTimer = 0;
                isInputRKey = false;
                StartCoroutine(GameRestart());
                //GameRestart();
            }
        }
        else
        {
            rKeyTimer = 0;
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

    public Vector3 CheckAround(Vector3 position)
    {
        RaycastHit2D rayCast = Physics2D.Raycast(position, Vector2.up, 0.1f, LayerMask.GetMask("Object"));
        Vector3 resultPosition = Vector3.zero;

        // 가운데 자리에 무언가 있으면 사방을 한번 둘러본다. 없으면 위에 초기화 한대로 가운데에 떨어질것이다.
        if (rayCast.collider)
        {
            int count = 1;
            while (count < 5)
            {
                RaycastHit2D upObjectRayCast = Physics2D.Raycast(position + Vector3.up * count, Vector2.up, 0.1f, LayerMask.GetMask("Object"));
                if (!upObjectRayCast.collider)
                {
                    resultPosition = Vector3.up * count;
                    break;
                }
                RaycastHit2D rightObjectRayCast = Physics2D.Raycast(position + Vector3.right * count, Vector2.right, 0.1f, LayerMask.GetMask("Object"));
                if (!rightObjectRayCast.collider)
                {
                    resultPosition = Vector3.right * count;
                    break;
                }
                RaycastHit2D downObjectRayCast = Physics2D.Raycast(position + Vector3.down * count, Vector2.down, 0.1f, LayerMask.GetMask("Object"));
                if (!downObjectRayCast.collider)
                {
                    resultPosition = Vector3.down * count;
                    break;
                }
                RaycastHit2D leftObjectRayCast = Physics2D.Raycast(position + Vector3.left * count, Vector2.left, 0.1f, LayerMask.GetMask("Object"));
                if (!leftObjectRayCast.collider)
                {
                    resultPosition = Vector3.left * count;
                    break;
                }

                // 여기까지 왔으면 사방에 무언가 있는것이므로 거리를 늘려 다시 재본다.
                count++;
            }
        }

        return resultPosition;
    }

    public void SetQuizList()
    {
        quizList = new Dictionary<string, bool>()
        {
            { "2 + 2 * 2 = 8", false },
            { "2 + 2 * 2 = 6", true },
            { "남극에는 우편번호가 없다.", true },
            { "남극에는 우편번호가 있다.", false },
            { "새는 뒤로도 날 수 있다.", true },
            { "새는 뒤로 날 수 없다.", false },
        };
    }

    public IEnumerator WaitSeconds(float time)
    {
        yield return new WaitForSeconds(time);
        ScreenInit();
    }

    public IEnumerator Blur()
    {
        // 모자이크 된 것처럼 블러 처리 하고 점점 확대
        Color blurColor = blur.color;
        for (float i = 0f; i <= 1f; i += 0.01f)
        {
            blurColor.a = i;
            blur.color = blurColor;
            blur.rectTransform.localScale += new Vector3(i / 5, i / 5, 0);
            yield return new WaitForSeconds(0.02f);
        }

        // 확대 후 화면 점점 어둡게
        Color fadeColor = fade.color;
        for (float i = 0f; i <= 1f; i += 0.01f)
        {
            fadeColor.a = i;
            fade.color = fadeColor;
            yield return new WaitForSeconds(0.01f);
        }
    }

    public IEnumerator Sharpen()
    {
        // 모자이크 된 것처럼 블러 처리 하고 점점 확대
        Color blurColor = blur.color;
        for (float i = 0f; i <= 1f; i += 0.01f)
        {
            blurColor.a = i;
            blur.color = blurColor;
            blur.rectTransform.localScale += new Vector3(i / 5, i / 5, 0);
            yield return new WaitForSeconds(0.02f);
        }

        // 확대 후 화면 점점 어둡게
        Color fadeColor = fade.color;
        for (float i = 0f; i <= 1f; i += 0.01f)
        {
            fadeColor.a = i;
            fade.color = fadeColor;
            yield return new WaitForSeconds(0.01f);
        }
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

    IEnumerator GameRestart()
    {
        //// 블러처리
        //StartCoroutine(Blur());

        //// 아이템 및 속성 초기화
        //Map.instance.isItemSet = false;
        //Map.instance.MapClear();
        //ItemInit();
        //PropertyInit();

        //// 맵 재구조화
        //Map.instance.MapSetting();

        //// 5초 쉬고 BGM 재생, 다시 화면 보여주고 움직일 수 있도록 설정
        //yield return new WaitForSeconds(5f);
        //AudioManager.instance.BGMPlay(Random.Range(1, 15));
        //ScreenInit();
        //player.stopMove = false;

        // 수정의 수정을 거듭한 결과 바로 첫 스테이지에서 리스타트를 해서 정상 결과에 나오는 것은 성공했습니다.
        // 하지만 다음 스테이지로 넘어가서 다시 첫 스테이지로 시작하는 것이 지금 스테이지가 씬 단위로 나뉘어 있어 불가능 하다는 것을 알았습니다.
        // 그래서 일단은 portal로 스테이지를 이동하는 로직을 사용하여 첫 스테이지로 돌아가는 로직을 사용하였습니다.
        ui.ClearMapBoard();
        StartCoroutine(UnloadCurrentStageScene());
        StartCoroutine(LoadStageScene(1));

        // 스테이지 및 플레이어 초기화
        stage = 0;
        player.CharacterInit();
        player.SetCharacterStatus();

        // 게임 초기화
        GameInit();

        // 확대 후 화면 점점 밝게
        Color fadeColor = fade.color;
        for (float i = 1f; i >= 0f; i -= 0.05f)
        {
            fadeColor.a = i;
            fade.color = fadeColor;
            yield return new WaitForSeconds(0.05f);
        }
    }

    public IEnumerator LoadStageScene(int loadStage)
    {
        yield return SceneManager.LoadSceneAsync(loadStage + 3, LoadSceneMode.Additive);
    }

    public IEnumerator LoadLoadingScene()
    {
        yield return SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
    }

    public IEnumerator UnloadCurrentStageScene()
    {
        if (SceneManager.GetSceneByBuildIndex(stage + 3).IsValid())
        {
            yield return SceneManager.UnloadSceneAsync(stage + 3);
        }
    }

    public void GameDataManage(string itemName, int itemCount)
    {
        // 세이브파일 로드
        string path = Application.dataPath + "/Data";

        // File.Exists(path): 해당 경로에 파일이 있는지 확인
        bool isExistsDataFile = File.Exists(path + "/saveData.json");
        if (isExistsDataFile)
        {
            string loadDataJson = File.ReadAllText(path + "/saveData.json");
            byte[] byteLoadData = System.Convert.FromBase64String(loadDataJson);
            string encodedLoadJson = System.Text.Encoding.UTF8.GetString(byteLoadData);

            if (encodedLoadJson != null && encodedLoadJson.Length > 0)
            {
                Dictionary<string, long> loadData = DataJson.DictionaryFromJson<string, long>(encodedLoadJson);

                // 파싱한 Dictionary를 순회하며 검의 파편 아이템이 있다면 개수 증가
                if (loadData.ContainsKey(itemName))
                {
                    loadData[itemName] += itemCount;
                }
                else
                {
                    loadData.Add(itemName, itemCount);
                }

                // 수정 후 다시 저장
                string saveDataJson = DataJson.DictionaryToJson(loadData);
                byte[] byteSaveData = System.Text.Encoding.UTF8.GetBytes(saveDataJson);
                string encodedSaveJson = System.Convert.ToBase64String(byteSaveData);
                File.WriteAllText(path + "/saveData.json", encodedSaveJson);
            }
        }
        else
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            Dictionary<string, long> saveData = new Dictionary<string, long>();
            saveData.Add(itemName, itemCount);

            string saveDataJson = DataJson.DictionaryToJson(saveData);
            byte[] byteSaveData = System.Text.Encoding.UTF8.GetBytes(saveDataJson);
            string encodedSaveJson = System.Convert.ToBase64String(byteSaveData);
            File.WriteAllText(path + "/saveData.json", encodedSaveJson);
        }
    }
}
