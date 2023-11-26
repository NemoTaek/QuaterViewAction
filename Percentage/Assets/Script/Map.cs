using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : Singleton<Map>
{
    [Header("----- Map Init Setting -----")]
    int[] roomCount = { 0, 10, 15, 20, 25 };
    Queue<int> roomQueue;
    List<int> roomWeightList;
    Dictionary<int, Room> settingRooms;
    Dictionary<int, Vector3> settingRoomPositions;
    public Room[] roomPrefabs;
    public Room[] specialroomPrefabs;
    public Room[] rooms;

    [Header("----- Map Setting -----")]
    public Room startRoom;
    public Room currentRoom;
    public int mapPosition;
    public Image[] mapSquare;
    public SpawnPoint spawnPoint;

    [Header("----- Shop Setting -----")]
    public List<int> itemsInShop;
    public Item[] itemPrefab;
    public Item[] itemPrice;
    public int shopItemCount;
    public bool isItemSet;

    protected override void Awake()
    {
        base.Awake();

        // 맵 초기화
        MapSetting();
    }

    void Start()
    {
        
    }

    void Update()
    {

    }

    public void MapSetting()
    {
        // 방 설치 변수 초기화
        RoomVariableInit();

        // 일반방 배치
        for (int i = 0; i < roomCount[GameManager.instance.stage]; i++)
        {
            ArrangeMap();
        }

        // 아이템방, 상점방, 보스방 배치
        ArrangeSpecialRoom();

        // 방 설치
        InstallRoom();

        // 각 방마다 세팅값 설정
        MapInit();
    }

    void RoomVariableInit()
    {
        // 맵 정중앙은 41번
        roomQueue = new Queue<int>();
        roomWeightList = new List<int>();
        settingRooms = new Dictionary<int, Room>();
        settingRoomPositions = new Dictionary<int, Vector3>();
        mapPosition = 41;

        // 상점방, 아이템방 관련 리스트
        shopItemCount = 3;
        itemsInShop = new List<int>();
        itemPrefab = new Item[shopItemCount];

        // 일반방 랜덤 배치        
        // 처음 뻗어나갈 때 몇가지 방향으로 뻗어나갈건지 세팅
        settingRooms.Add(41, startRoom);
        settingRoomPositions.Add(41, Vector3.zero);

        int branch = Random.Range(1, 5);
        int[] firstRoomIndex = { 40, 42, 32, 50 };
        Vector3 firstRoomPosition = Vector3.zero;

        // 가지 뻗기
        // 중복된 위치가 나오면 다시 뽑기
        while (roomQueue.Count < branch)
        {
            int choiceRoom = firstRoomIndex[Random.Range(0, 4)];
            if (!settingRooms.ContainsKey(choiceRoom))
            {
                if (choiceRoom == 40)
                {
                    firstRoomPosition = Vector3.up * 12;
                }
                else if (choiceRoom == 42)
                {
                    firstRoomPosition = Vector3.down * 12;
                }
                else if (choiceRoom == 32)
                {
                    firstRoomPosition = Vector3.right * 20;
                }
                else if (choiceRoom == 50)
                {
                    firstRoomPosition = Vector3.left * 20;
                }

                // 큐, 방, 방 위치에 각각 추가
                roomQueue.Enqueue(choiceRoom);
                settingRooms.Add(choiceRoom, roomPrefabs[Random.Range(0, roomPrefabs.Length)]);
                settingRoomPositions.Add(choiceRoom, firstRoomPosition);
            }
        }
    }

    void ArrangeMap()
    {
        // 위: -1, 아래: +1, 좌: +9, 우: -9
        int nextMapPosition = roomQueue.Dequeue();
        int upRoomIndex = nextMapPosition - 1;
        int downRoomIndex = nextMapPosition + 1;
        int leftRoomIndex = nextMapPosition + 9;
        int rightRoomIndex = nextMapPosition - 9;
        int weight = 0;

        // 사방을 보면서 비어있으면(룸 큐에 없다면) 해당 방향으로 가중치만큼 배열에 개수 추가
        bool isExistUpRoom = settingRooms.ContainsKey(upRoomIndex);
        if (!isExistUpRoom)
        {
            weight = (nextMapPosition - 1) % 9;
            for(int i=0; i<weight; i++)
            {
                roomWeightList.Add(upRoomIndex);
            }
        }
        bool isExistDownRoom = settingRooms.ContainsKey(downRoomIndex);
        if (!isExistDownRoom)
        {
            weight = 8 - ((nextMapPosition - 1) % 9);
            for (int i = 0; i < weight; i++)
            {
                roomWeightList.Add(downRoomIndex);
            }
        }
        bool isExistLeftRoom = settingRooms.ContainsKey(leftRoomIndex);
        if (!isExistLeftRoom)
        {
            weight = 8 - (nextMapPosition / 9);
            for (int i = 0; i < weight; i++)
            {
                roomWeightList.Add(leftRoomIndex);
            }
        }
        bool isExistRightRoom = settingRooms.ContainsKey(rightRoomIndex);
        if (!isExistRightRoom)
        {
            weight = nextMapPosition / 9;
            for (int i = 0; i < weight; i++)
            {
                roomWeightList.Add(rightRoomIndex);
            }
        }

        // 위에서 추가한 방 인덱스 배열 중 랜덤으로 하나 선택
        int selectRoomIndex = Random.Range(0, roomWeightList.Count);
        Vector3 randomRoomPosition = Vector3.zero;

        if (roomWeightList[selectRoomIndex] == upRoomIndex)
        {
            randomRoomPosition = (settingRoomPositions[nextMapPosition] + Vector3.up * 12);
        }
        else if (roomWeightList[selectRoomIndex] == downRoomIndex)
        {
            randomRoomPosition = (settingRoomPositions[nextMapPosition] + Vector3.down * 12);
        }
        else if (roomWeightList[selectRoomIndex] == leftRoomIndex)
        {
            randomRoomPosition = (settingRoomPositions[nextMapPosition] + Vector3.left * 20);
        }
        else if (roomWeightList[selectRoomIndex] == rightRoomIndex)
        {
            randomRoomPosition = (settingRoomPositions[nextMapPosition] + Vector3.right * 20);
        }

        // 큐, 방, 방 위치에 추가
        roomQueue.Enqueue(roomWeightList[selectRoomIndex]);
        settingRooms.Add(roomWeightList[selectRoomIndex], roomPrefabs[Random.Range(0, roomPrefabs.Length)]);
        settingRoomPositions.Add(roomWeightList[selectRoomIndex], randomRoomPosition);

        // 가중치 배열 초기화
        roomWeightList.Clear();
    }

    void ArrangeSpecialRoom()
    {
        List<int> roomIndexList = new List<int>();
        float longestDistance = 0;
        int selectItemRoomIndex = 0;
        int selectShopRoomIndex = 0;
        int selectBossRoomIndex = 0;

        // 설치한 방 위치 인덱스 배열 생성, 제일 거리가 먼 방을 보스룸으로 설정
        foreach (KeyValuePair<int, Vector3> installedRoom in settingRoomPositions)
        {
            roomIndexList.Add(installedRoom.Key);
            if (longestDistance < installedRoom.Value.magnitude)
            {
                longestDistance = installedRoom.Value.magnitude;
                selectBossRoomIndex = installedRoom.Key;
            }
        }

        // 설치한 방 위치 인덱스 배열에서 임의의 방을 각각 아이템방과 상점으로 변경
        while (true)
        {
            int itemRoomRandom = Random.Range(1, roomIndexList.Count);
            int shopRoomRandom = Random.Range(1, roomIndexList.Count);
            selectItemRoomIndex = roomIndexList[itemRoomRandom];
            selectShopRoomIndex = roomIndexList[shopRoomRandom];

            if (selectItemRoomIndex != selectShopRoomIndex && selectItemRoomIndex != selectBossRoomIndex && selectShopRoomIndex != selectBossRoomIndex) break;
        }

        // 방 정보 변경
        settingRooms[selectItemRoomIndex] = specialroomPrefabs[0];
        settingRooms[selectShopRoomIndex] = specialroomPrefabs[1];
        settingRooms[selectBossRoomIndex] = specialroomPrefabs[2];
    }

    void InstallRoom()
    {
        Room room = null;

        // 앞에서 설정한 위치에 설정한 방을 생성
        foreach (KeyValuePair<int, Room> installRoom in settingRooms)
        {
            if (installRoom.Value != null)
            {
                room = Instantiate(installRoom.Value, transform);
                room.transform.position = settingRoomPositions[installRoom.Key];
            }
        }
    }

    public void MapInit()
    {
        // 방과 미니맵 연결
        rooms = GetComponentsInChildren<Room>();
        mapSquare = GameManager.instance.ui.mapBoard.GetComponentsInChildren<Image>();

        // 우측 상단 맵 초기화
        GameManager.instance.ui.ClearMapBoard();

        // 첫 시작 방 설정
        currentRoom = rooms[0];
        currentRoom.isVisited = true;

        // 각 방마다 설정값 세팅
        foreach (Room room in rooms)
        {
            // 상점방과 황금방에는 아이템 깔아두기
            if (room.roomType == Room.RoomType.Shop)
            {
                SetShopItem(room, shopItemCount);
                SetShopItemPrice();
            }
            else if (room.roomType == Room.RoomType.Golden)
            {
                SetGoldenItem(room);
            }
        }

        isItemSet = true;
    }

    void SetGoldenItem(Room room)
    {
        int totalItemCount = GameManager.instance.itemPool.items.Length;
        int random = Random.Range(1, totalItemCount);

        // 이미 배치된 아이템은 다시 배치되면 안된다.
        while (true)
        {
            int isInSetItemList = GameManager.instance.setItemList.Find(x => x == random);

            if (isInSetItemList == 0)
            {
                GameManager.instance.setItemList.Add(random);

                Item goldenItem = GameManager.instance.itemPool.Get(random);
                goldenItem.transform.position = room.itemPoint[0].transform.position;
                goldenItem.Init(GameManager.instance.itemData[random - 1]);
                
                break;
            }
            else
            {
                random = Random.Range(1, totalItemCount);
            }
        }
    }

    void SetShopItem(Room room, int count)
    {
        int index = 0;
        int totalItemCount = GameManager.instance.itemPool.items.Length;

        // 중복이 안되도록 아이템 세팅
        // 이미 획득한 아이템은 안나오도록 설정
        while (index < count)
        {
            int random = Random.Range(1, totalItemCount);
            int isInItemList = itemsInShop.Find(x => x == random);
            int isInSetItemList = GameManager.instance.setItemList.Find(x => x == random);

            // 상점 내에는 중복이 안되고, 지금까지 배치된 적 없는 아이템으로 세팅
            if (isInItemList == 0 && isInSetItemList == 0)
            {
                itemsInShop.Add(random);
                GameManager.instance.setItemList.Add(random);

                // 아이템 생성 후 배치
                itemPrefab[index] = GameManager.instance.itemPool.Get(random);
                itemPrefab[index].transform.position = room.itemPoint[index].transform.position + Vector3.up;
                itemPrefab[index].Init(GameManager.instance.itemData[random - 1]);
                itemPrefab[index].isInShop = true;
                index++;
            }
        }
    }

    void SetShopItemPrice()
    {
        itemPrice = new Item[shopItemCount];
        for (int i = 0; i < shopItemCount; i++)
        {
            // 아이템 가격 세팅
            // 아이템 가격 텍스트는 Canvas에 설정되므로 상점방이 아니면 active를 비활성화 해야한다.
            Item priceText = GameManager.instance.itemPool.Get(0);
            priceText.GetComponent<Text>().text = "$ " + itemPrefab[i].price.ToString();
            itemPrice[i] = priceText;
            itemPrice[i].transform.SetParent(GameManager.instance.itemCanvas.transform);
            
            //Item priceText = Instantiate(GameManager.instance.itemPool.items[0], GameManager.instance.itemCanvas.transform);
            //priceText.GetComponent<Text>().text = "$ " + itemPrefab[i].price.ToString();
            //itemPrice[i] = priceText;
        }
    }

    public void MapClear()
    {
        for(int i = 0; i<rooms.Length; i++)
        {
            Destroy(rooms[i].gameObject);
        }

        // 여기서 GetComponentsInChildren의 길이를 호출하면 destroy 했음에도 불구하고 이 전에있던 개수를 그대로 불러온다.
        // 왜그럴까???
        // Destroy를 호출해도 오브젝트는 바로 파괴되지 않기 때문이다.
        // 프레임 업데이트 후 OnDestroy가 호출되므로 제대로 참조하려면 1프레임을 기다려야 한다.
        // R키를 누르고 모든 메소드가 쭈우우욱 연계되어서 실행되기 때문에 OnDestroy가 호출되지 않고 계속 남아있기 때문이다.

        // 그래서 해결법은
        // 이 파괴되는 오브젝트와 지금 맵 오브젝트와 분리하기 위해 DetachChildren() 메소드를 사용하여 분리시킨다.
        // 이러면 파괴되는 오브젝트(리셋 전의 방)들은 더이상 이 맵의 자식이 아니기때문에 GetComponentsInChildren을 호출해도 불러오지 않게 된다.
        transform.DetachChildren();
    }
}
