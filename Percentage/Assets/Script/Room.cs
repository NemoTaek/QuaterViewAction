using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    public enum RoomType { Start, Clear, Battle, Arcade, Quiz, Golden, Shop, Boss };

    [Header("----- Component -----")]
    public SpawnPoint[] spawnPoint;
    public GameObject[] itemPoint;
    public Door[] doors;
    public Room upRoom;
    public Room rightRoom;
    public Room downRoom;
    public Room leftRoom;

    [Header("----- Room Object -----")]
    public Button[] buttons;
    public GameObject roomReward;
    public Item[] bossDropItems;
    public ItemData[] bossDropItemDatas;

    [Header("----- Room Property -----")]
    public RoomType roomType;
    public bool isVisited;
    public bool isBattle;
    public int enemyCount = 0;
    public bool isClear;
    public bool isItemSet;
    public bool isMapDraw;
    public bool isQuizSet;
    public bool isSelectAnswer;
    Text quizText;
    bool quizAnswer;

    void Awake()
    {
        spawnPoint = GetComponentsInChildren<SpawnPoint>();
        doors = GetComponentsInChildren<Door>(true);
        buttons = GetComponentsInChildren<Button>();
    }

    void OnEnable()
    {
        
    }

    void Start()
    {
        // 현재 있는 방에서 주위에 방이 있는지 탐색
        CheckAroundRoom();
    }

    void Update()
    {
        SettingRooms();
    }

    void CheckAroundRoom()
    {
        // 레이어 마스크는 레이어를 비트로 판단하기 때문에 비트 연산자를 사용하여 작성해야 한한다.
        // 그냥 레이어 자리에 6 이렇게 쓰니까 안먹히더라..
        // 특정 레이어를 제외하려면 ~(1 << 레이어) 이렇게 사용하면 된다.
        //LayerMask layer = (1 << LayerMask.NameToLayer("Room"));
        //colliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(25, 15), 0, layer);

        // 근데 대각선은 굳이 알아낼 필요가 없다...
        // 이번엔 레이캐스트로 상하좌우만 쏴서 검출해보도록 하자
        RaycastHit2D leftRoomRayCast = Physics2D.Raycast(transform.position + Vector3.left * 10, Vector2.left, 3, LayerMask.GetMask("Room"));
        if (leftRoomRayCast) leftRoom = leftRoomRayCast.collider.gameObject.GetComponent<Room>();
        RaycastHit2D rightRoomRayCast = Physics2D.Raycast(transform.position + Vector3.right * 10, Vector2.right, 3, LayerMask.GetMask("Room"));
        if (rightRoomRayCast) rightRoom = rightRoomRayCast.collider.gameObject.GetComponent<Room>();
        RaycastHit2D upRoomRayCast = Physics2D.Raycast(transform.position + Vector3.up * 6, Vector2.up, 3, LayerMask.GetMask("Room"));
        if (upRoomRayCast) upRoom = upRoomRayCast.collider.gameObject.GetComponent<Room>();
        RaycastHit2D downRoomRayCast = Physics2D.Raycast(transform.position + Vector3.down * 6, Vector2.down, 3, LayerMask.GetMask("Room"));
        if (downRoomRayCast) downRoom = downRoomRayCast.collider.gameObject.GetComponent<Room>();
    }

    void SettingRooms()
    {
        // 현재 있는 방만 검사
        if (Map.instance.currentRoom.gameObject == gameObject)
        {
            // 주변 맵 밝히기
            if (!isMapDraw) DrawMap();

            // 스폰 포인트가 있으면 전투방
            if ((roomType == RoomType.Battle || roomType == RoomType.Boss) && spawnPoint.Length > 0)
            {
                // 전투 시작중이 아니라면 전투 시작
                if (!isClear && !isBattle) BattleStart();

                // 보스방이면 보스 체력 UI 활성화
                if (roomType == RoomType.Boss)
                {
                    Enemy boss = GetComponentInChildren<Enemy>();
                    if (boss && boss.type == EnemyData.EnemyType.Boss)
                    {
                        if (boss.isLive && !GameManager.instance.ui.bossUI.activeSelf) GameManager.instance.ui.bossUI.SetActive(true);
                        float currentBossHealth = boss.health;
                        GameManager.instance.ui.bossHealth.sizeDelta = new Vector2(currentBossHealth / boss.maxHealth * 500, 100);
                    }
                }

                // 전투중인데 적을 모두 처치했다면 전투 종료
                if (!isClear && isBattle)
                {
                    if (enemyCount <= 0) BattleEnd(Random.Range(0, 2));
                }
            }

            // 버튼이 있으면 아케이드방
            else if (roomType == RoomType.Arcade)
            {
                // 아케이드 방에도 충분히 적이 있을 수 있다.
                if (!isClear && !isBattle) BattleStart();

                // 버튼을 모두 누르면 방 클리어
                if (!isClear && IsAllButtonPressed()) BattleEnd(Random.Range(0, 2));
            }

            // 퀴즈방
            else if (roomType == RoomType.Quiz && !isClear)
            {
                // 어떻게 할거냐? 우선 내 계획은...
                // 1. 문제가 안보일정도로 아주 흐릿흐릿하게 문제를 낸다.
                // 2. OX 퀴즈로 왼쪽은 O, 오른쪽은 X 버튼을 누르게 한다.
                // 3. 정답을 맞추면 빡스를, 틀리면 배틀을 하게 한다.

                // 퀴즈 UI 세팅 ON
                if(!isQuizSet)
                {
                    quizAnswer = SetQuiz();
                }

                if (!isSelectAnswer && (buttons[0].isPressed || buttons[1].isPressed))
                {
                    isSelectAnswer = true;
                    StartCoroutine(OpenQuiz());
                }
            }

            // 상점방에 들어가면 숨겼던 아이템 가격 다시 활성화
            else if (roomType == RoomType.Shop && Map.instance.isItemSet)
            {
                for (int i = 0; i < Map.instance.itemPrice.Length; i++)
                {
                    // 텍스트를 출력하기 위해 텍스트 프리팹을 UI 캔버스에 맞춰 세팅
                    // Camera.main.WorldToScreenPoint(): 월드 좌표값을 스크린 좌표값으로 변경하는 메소드
                    Map.instance.itemPrice[i].transform.position = Camera.main.WorldToScreenPoint(itemPoint[i].transform.position);

                    // 구매한 아이템은 비활성화
                    Map.instance.itemPrice[i].gameObject.SetActive(!Map.instance.itemPrefab[i].getItem);
                }
            }
        }
        else
        {
            // 상점 방을 나가면 UI를 비활성화 시켜야 한다.
            if (roomType == RoomType.Shop && Map.instance.isItemSet)
            {
                for (int i = 0; i < Map.instance.itemsInShop.Count; i++)
                {
                    Item itemPrice = Map.instance.itemPrice[i];
                    if(itemPrice != null)
                    {
                        if (itemPrice.gameObject.activeSelf) itemPrice.gameObject.SetActive(false);
                    }
                    
                }
            }
        }
    }

    void DrawMap()
    {
        // 맵 UI에서 현재 위치한 방의 투명도는 1, 방문했던 방은 0.75, 주변에 있는 방은 0.25로 설정
        // 참고로 우측 상단이 1번째, 좌측 하단이 81번째. 생성 방향은 상->하, 우->좌

        // 현재 방의 위치는 찐하게 방문 표시
        Map.instance.mapSquare[Map.instance.mapPosition].color = new Color(1, 1, 1, 1);

        // 주변 방 색칠하기
        if (leftRoom)   ColorMap(leftRoom, 9);
        if (rightRoom)  ColorMap(rightRoom, -9);
        if (upRoom)     ColorMap(upRoom, -1);
        if (downRoom)   ColorMap(downRoom, 1);

        isMapDraw = true;
    }

    void ColorMap(Room directionRoom, int roomIndex)
    {
        int mapPosition = Map.instance.mapPosition;
        mapPosition += roomIndex;

        // 주변 방이 방문했던곳이 아니면 투명도 0.25, 방문 했었으면 0.75
        Map.instance.mapSquare[mapPosition].color = !directionRoom.isVisited ? new Color(1, 1, 1, 0.25f) : new Color(1, 1, 1, 0.75f);

        // 특수방이라면 아이콘 추가
        if (directionRoom.roomType == RoomType.Boss || directionRoom.roomType == RoomType.Golden || directionRoom.roomType == RoomType.Shop)
        {
            SpecialRoom(directionRoom, Map.instance.mapSquare[mapPosition]);
        }
    }

    void SpecialRoom(Room room, Image mapSqure)
    {
        Image[] roomIcon = mapSqure.GetComponentsInChildren<Image>(true);
        roomIcon[1].gameObject.SetActive(true);

        if (room.roomType == RoomType.Boss)
        {
            roomIcon[1].sprite = GameManager.instance.roomIcon[0];
        }
        else if (room.roomType == RoomType.Golden)
        {
            roomIcon[1].sprite = GameManager.instance.roomIcon[1];
        }
        else if (room.roomType == RoomType.Shop)
        {
            roomIcon[1].sprite = GameManager.instance.roomIcon[2];
        }
    }

    void InitEnemies()
    {
        // 소환 지점이 있다면 몬스터 소환
        if (spawnPoint.Length > 0)
        {
            for (int i = 0; i < spawnPoint.Length; i++)
            {
                Enemy spawnEnemy = Instantiate(spawnPoint[i].enemy, spawnPoint[i].transform);
                spawnEnemy.Init(GameManager.instance.enemyData[spawnPoint[i].enemy.id]);
                enemyCount++;
            }
        }
    }

    void BattleStart()
    {
        isBattle = true;
        InitEnemies();
    }

    void BattleEnd(int successOrFail)
    {
        if(roomType == RoomType.Boss)
        {
            // 포탈 생성
            GameObject portal = GameManager.instance.objectPool.Get(4);
            portal.transform.position = transform.position;

            // 보스 리워드 생성
            // 위치: 짝수면 -1, 1, -2, 2, -3, 3 .... / 홀수면 0, -1, 1, -2, 2 ....
            //Vector3 itemStartPosition = Vector3.left * (bossDropItems.Length % 2 == 0 ? bossDropItems.Length / 2 + 0.5f : (int)(bossDropItems.Length / 2));

            // 어짜피 지금은 2개니까 위치를 숫자로 박아넣자
            Vector3 itemStartPosition = Vector3.left * 2;
            for (int i = 0; i < bossDropItems.Length; i++)
            {
                Item item = Instantiate(bossDropItems[i], transform);
                item.transform.position = transform.position + Vector3.down * 2 + itemStartPosition + Vector3.right * i * 4;
                item.Init(bossDropItemDatas[i]);
            }
        }
        else
        {
            // 보상 획득 (0: 성공, 1: 실패)
            GameObject reward;

            // 보상이 떨어지는 자리에 오브젝트가 있으면 헷갈리므로 있으면 다른곳에 놓도록 설정
            Vector3 pos = GameManager.instance.CheckAround(transform.position);

            if (successOrFail == 0)
            {
                reward = Instantiate(GameManager.instance.objectPool.prefabs[0], roomReward.transform);
                reward.transform.position = transform.position + pos;
            }
            else
            {
                // 상자 얻기에 실패하면 돈이나 하트 생성
                // 0: 꽝, 1: 1원, 2: 하트 반쪽, 3: 온전한 하트
                int randomObject = Random.Range(0, 4);
                if (randomObject != 0)
                {
                    reward = Instantiate(GameManager.instance.objectPool.prefabs[randomObject], roomReward.transform);
                    reward.transform.position = transform.position + pos;
                }
            }
        }

        // 액티브 아이템이 있다면 게이지 1칸 증가
        if(GameManager.instance.player.activeItem && GameManager.instance.player.activeItem.currentGuage < GameManager.instance.player.activeItem.activeGuage)
        {
            GameManager.instance.player.activeItem.currentGuage++;
            GameManager.instance.ui.isChanged = true;
        }

        // 전투가 끝났다면 감지되지는 위치의 문을 오픈
        DoorOpen();
    }

    public void DoorOpen()
    {
        isClear = true;

        if (upRoom) doors[0].gameObject.SetActive(true);
        if (rightRoom) doors[1].gameObject.SetActive(true);
        if (downRoom) doors[2].gameObject.SetActive(true);
        if (leftRoom) doors[3].gameObject.SetActive(true);
    }

    bool IsAllButtonPressed()
    {
        for(int i=0; i<buttons.Length; i++)
        {
            if (!buttons[i].isPressed) return false;
        }
        return true;
    }

    public void DeleteItem()
    {
        Item[] rewardItems = GetComponentsInChildren<Item>();
        foreach(Item item in rewardItems)
        {
            Destroy(item.gameObject);
        }
    }

    public bool SetQuiz()
    {
        // 퀴즈 문제 부분을 블러 처리 하고싶다
        // 그래서 standard assets를 다운받아서 블러처리 하면 된다고 한다.
        // standard assets - effects - imageEffects 에 가면 블러가 있다고 한다. 근데 봤는데 없다.
        // 거기있는 reademe를 읽어보니 deprecated 되었다고 한다... post-processing 을 사용하라고 한다...


        // 퀴즈 UI 오픈 하고
        Canvas canvas = GetComponentInChildren<Canvas>(true);
        canvas.gameObject.SetActive(true);

        // 랜덤으로 퀴즈 골라서 텍스트와 답 세팅
        Image quizBox = GetComponentInChildren<Image>();
        quizText = quizBox.GetComponentInChildren<Text>();
        int random = Random.Range(0, GameManager.instance.quizList.Count);
        KeyValuePair<string, bool> quiz = GameManager.instance.quizList.ElementAt(random);
        quizText.text = quiz.Key;

        isQuizSet = true;

        return quiz.Value;
    }

    IEnumerator OpenQuiz()
    {
        quizText.material = null;

        yield return new WaitForSeconds(3);

        // 퀴즈 UI 비활성화
        GetComponentInChildren<Canvas>().gameObject.SetActive(false);

        if (buttons[0].isPressed)
        {
            // O를 선택했고, 답이 O라면 성공
            if (quizAnswer) BattleEnd(0);
            // 답이 X라면 실패
            else BattleEnd(1);
        }
        else if (buttons[1].isPressed)
        {
            // X를 선택했고, 답이 O라면 실패
            // 답이 X라면 성공
            if (quizAnswer) BattleEnd(1);
            else BattleEnd(0);
        }
    }
}
