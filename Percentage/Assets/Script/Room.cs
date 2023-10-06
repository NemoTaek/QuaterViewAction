using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    public enum RoomType { Start, Clear, Battle, Arcade, Shop, Boss };

    [Header("----- Component -----")]
    public Enemy enemy;
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

    [Header("----- Room Property -----")]
    public RoomType roomType;
    public bool isVisited;
    public bool isBattle;
    public int enemyCount = 0;
    public bool isClear;
    public bool isItemSet;
    public bool isMapDraw;

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
            if (spawnPoint.Length > 0)
            {
                // 전투 시작중이 아니라면 전투 시작
                if (!isClear && !isBattle) BattleStart();

                // 전투중인데 적을 모두 처치했다면 전투 종료
                else if (!isClear && isBattle)
                {
                    if (enemyCount <= 0) BattleEnd();
                }
            }

            // 버튼이 있으면 아케이드방
            else if (buttons.Length > 0)
            {
                // 버튼을 모두 누르면 방 클리어
                if (!isClear && IsAllButtonPressed()) BattleEnd();
            }

            // 상점방에 들어가면 숨겼던 아이템 가격 다시 활성화
            if (roomType == RoomType.Shop && Map.instance.isItemSet)
            {
                for (int i = 0; i < Map.instance.itemPrice.Length; i++)
                {
                    // 텍스트를 출력하기 위해 텍스트 프리팹을 UI 캔버스에 맞춰 세팅
                    // Camera.main.WorldToScreenPoint(): 월드 좌표값을 스크린 좌표값으로 변경하는 메소드
                    Map.instance.itemPrice[i].transform.position = Camera.main.WorldToScreenPoint(itemPoint[i].transform.position);

                    // 구매한 아이템은 비활성화
                    Map.instance.itemPrice[i].gameObject.SetActive(!Map.instance.itemPrefab[i].isPurchased);
                }
            }

            // 보스방이면 보스 체력 UI 활성화
            if (roomType == RoomType.Boss)
            {
                Boss boss = GetComponentInChildren<Boss>();
                if (boss)
                {
                    if (boss.isLive && !GameManager.instance.ui.bossUI.activeSelf) GameManager.instance.ui.bossUI.SetActive(true);
                    float currentBossHealth = boss.health;
                    GameManager.instance.ui.bossHealth.sizeDelta = new Vector2(currentBossHealth / boss.maxHealth * 500, 100);
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
        // 맵 UI에서 현재 위치한 방의 투명도는 0.75, 주변에 있는 방은 0.25로 설정
        // 참고로 우측 상단이 1번째, 좌측 하단이 81번째. 생성 방향은 상->하, 우->좌
        Image[] mapSquare = GameManager.instance.ui.mapBoard.GetComponentsInChildren<Image>();

        // 현재 위치는 찐하게 방문 표시
        mapSquare[Map.instance.mapPosition].color = new Color(1, 1, 1, 0.75f);

        // 주변방으 옅게 표시
        if (leftRoom && !leftRoom.isVisited)
        {
            int mapPosition = Map.instance.mapPosition;
            mapPosition += 9;
            mapSquare[mapPosition].color = new Color(1, 1, 1, 0.25f);
        }
        if (rightRoom && !rightRoom.isVisited)
        {
            int mapPosition = Map.instance.mapPosition;
            mapPosition -= 9;
            mapSquare[mapPosition].color = new Color(1, 1, 1, 0.25f);
        }
        if (upRoom && !upRoom.isVisited)
        {
            int mapPosition = Map.instance.mapPosition;
            mapPosition -= 1;
            mapSquare[mapPosition].color = new Color(1, 1, 1, 0.25f);
        }
        if (downRoom && !downRoom.isVisited)
        {
            int mapPosition = Map.instance.mapPosition;
            mapPosition += 1;
            mapSquare[mapPosition].color = new Color(1, 1, 1, 0.25f);
        }

        isMapDraw = true;
    }

    void BattleStart()
    {
        isBattle = true;

        // 전투 시작 시 문 폐쇄
        //for (int i = 0; i < doors.Length; i++)
        //{
        //    doors[i].gameObject.SetActive(false);
        //}

        // 소환 지점이 있다면 몬스터 소환
        if(spawnPoint.Length > 0)
        {
            for (int i = 0; i < spawnPoint.Length; i++)
            {
                Instantiate(enemy, spawnPoint[i].transform);
                enemyCount++;
            }
        }
    }

    void BattleEnd()
    {
        if(roomType == RoomType.Boss)
        {
            // 포탈 생성
            GameObject portal = GameManager.instance.objectPool.Get(4);
            portal.transform.position = transform.position;

            // 보스 리워드 생성
            //GameObject bossReward = GameManager.instance.objectPool.Get(5);
            //portal.transform.position = transform.position + Vector3.down;
        }
        else
        {
            // 보상 획득 (0: 성공, 1: 실패)
            int successOrFail = Random.Range(0, 2);
            GameObject reward;
            if (successOrFail == 0)
            {
                reward = GameManager.instance.objectPool.prefabs[0];
                Instantiate(reward, roomReward.transform);
                reward.transform.position = Vector3.zero;
            }
            else
            {
                // 상자 얻기에 실패하면 돈이나 하트 생성
                // 0: 1원, 1: 하트 반쪽, 2: 온전한 하트
                int randomObject = Random.Range(1, 4);
                reward = GameManager.instance.objectPool.prefabs[randomObject];
                Instantiate(reward, roomReward.transform);
                reward.transform.position = Vector3.zero;
            }
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
}
