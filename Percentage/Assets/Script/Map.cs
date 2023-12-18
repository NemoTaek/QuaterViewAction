using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomProperty
{
    public Room roomPrefab;
    public Vector3 roomPosition;
    public int roomIndex;
}

public class Map : Singleton<Map>
{
    [Header("----- Map Init Setting -----")]
    int[] roomCount = { 0, 10, 15, 20, 25 };
    List<int> roomWeightList;
    public bool[] isInstalledRoom;
    public List<RoomProperty> roomProperties;
    Queue<RoomProperty> roomPropertiesQueue;
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

    void CreateRoomProperty(int index, Room prefab, Vector3 position, bool roomEnqueue = true)
    {
        // 방 속성 객체 생성
        RoomProperty room = new RoomProperty();
        room.roomIndex = index;
        room.roomPrefab = prefab;
        room.roomPosition = position;

        // 방 속성 객체를 생성 후 리스트에 추가하고 해당 위치에 방이 설치되었다고 설정
        roomProperties.Add(room);
        isInstalledRoom[room.roomIndex] = true;
        if (roomEnqueue) roomPropertiesQueue.Enqueue(room);
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
        // 방 설치 순서를 설정할 큐, 설치할 방 리스트, 설치 유무 배열, 설치할 위치의 가중치 리스트
        roomPropertiesQueue = new Queue<RoomProperty>();
        roomProperties = new List<RoomProperty>();
        isInstalledRoom = new bool[82];
        roomWeightList = new List<int>();

        // 상점방, 아이템방 관련 리스트
        shopItemCount = 3;
        itemsInShop = new List<int>();
        itemPrefab = new Item[shopItemCount];

        // 첫 방 설정
        CreateRoomProperty(41, startRoom, Vector2.zero, false);

        // 일반방 랜덤 배치        
        // 처음 뻗어나갈 때 몇가지 방향으로 뻗어나갈건지 세팅
        int branch = Random.Range(1, 5);
        int[] firstRoomIndex = { 40, 42, 32, 50 };
        Vector3 firstRoomPosition = Vector3.zero;

        // 가지 뻗기
        // 중복된 위치가 나오면 다시 뽑기
        while (roomPropertiesQueue.Count < branch)
        {
            int choiceRoomIndex = firstRoomIndex[Random.Range(0, 4)];
            bool checkRoom = isInstalledRoom[choiceRoomIndex];

            if (!checkRoom)
            {
                if (choiceRoomIndex == 40)
                {
                    firstRoomPosition = Vector3.up * 12;
                }
                else if (choiceRoomIndex == 42)
                {
                    firstRoomPosition = Vector3.down * 12;
                }
                else if (choiceRoomIndex == 32)
                {
                    firstRoomPosition = Vector3.right * 20;
                }
                else if (choiceRoomIndex == 50)
                {
                    firstRoomPosition = Vector3.left * 20;
                }

                // 해당 방 설정
                CreateRoomProperty(choiceRoomIndex, roomPrefabs[Random.Range(0, roomPrefabs.Length)], firstRoomPosition);
            }
        }
    }

    void ArrangeMap()
    {
        // 큐에서 차례대로 방을 꺼내 이어서 설치
        RoomProperty prevRoom = roomPropertiesQueue.Dequeue();

        // 사방을 보면서 비어있으면(선택한 방이 없다면) 해당 방향으로 가중치만큼 배열에 개수 추가
        SetRoomWeightList(prevRoom.roomIndex);

        // 위에서 추가한 방 인덱스 배열 중 랜덤으로 하나 선택
        // 그런데 진짜 낮은 확률로 어디에도 방을 놓을 수 없는 경우가 생기더라
        // 그래서 놓을 수 없으면 설치 가능한 방을 찾을때까지 반복 후 선택해서 이어나가도록 설정
        // 설치 가능한 방을 찾을때까지 반복
        while (roomWeightList.Count <= 0)
        {
            // 가장 마지막으로 저장된 방에서 탐색했을 때 오류가 났기 때문에 그 전까지 방 중 랜덤으로 하나 선택
            // 임시로 선택한 방 인덱스로 다시 가중치 설정
            SetRoomWeightList(roomProperties[Random.Range(0, roomProperties.Count - 1)].roomIndex);

            // 설치 가능한 위치를 찾았으면 그만
            if (roomWeightList.Count > 0) break;
        }

        // 주변에 방을 설치할 수 있으면 랜덤으로 하나 선택한 후 방 설치
        int selectRoomIndex = Random.Range(0, roomWeightList.Count);
        SetNextRoom(selectRoomIndex, prevRoom);

        // 가중치 배열 초기화
        roomWeightList.Clear();
    }

    void SetRoomWeightList(int prevRoomIndex)
    {
        int weight = 0;
        int up = prevRoomIndex - 1;
        int down = prevRoomIndex + 1;
        int left = prevRoomIndex + 9;
        int right = prevRoomIndex - 9;

        if (up > 0 && !isInstalledRoom[up])
        {
            weight = (prevRoomIndex - 1) % 9;
            for (int i = 0; i < weight; i++)
            {
                roomWeightList.Add(up);
            }
        }
        if (down < 82 && !isInstalledRoom[down])
        {
            weight = 8 - ((prevRoomIndex - 1) % 9);
            for (int i = 0; i < weight; i++)
            {
                roomWeightList.Add(down);
            }
        }
        if (left < 82 && !isInstalledRoom[left])
        {
            weight = 8 - ((prevRoomIndex / 9) - 1);
            for (int i = 0; i < weight; i++)
            {
                roomWeightList.Add(left);
            }
        }
        if (right > 0 && !isInstalledRoom[right])
        {
            weight = (prevRoomIndex / 9) - 1;
            for (int i = 0; i < weight; i++)
            {
                roomWeightList.Add(right);
            }
        }
    }

    void SetNextRoom(int selectRoomIndex, RoomProperty prevRoom)
    {
        int choiceRoomIndex = 0;
        Vector3 choiceRoomPosition = prevRoom.roomPosition;

        if (roomWeightList[selectRoomIndex] == prevRoom.roomIndex - 1)
        {
            choiceRoomIndex = prevRoom.roomIndex - 1;
            choiceRoomPosition += Vector3.up * 12;
        }
        else if (roomWeightList[selectRoomIndex] == prevRoom.roomIndex + 1)
        {
            choiceRoomIndex = prevRoom.roomIndex + 1;
            choiceRoomPosition += Vector3.down * 12;
        }
        else if (roomWeightList[selectRoomIndex] == prevRoom.roomIndex + 9)
        {
            choiceRoomIndex = prevRoom.roomIndex + 9;
            choiceRoomPosition += Vector3.left * 20;
        }
        else if (roomWeightList[selectRoomIndex] == prevRoom.roomIndex - 9)
        {
            choiceRoomIndex = prevRoom.roomIndex - 9;
            choiceRoomPosition += Vector3.right * 20;
        }

        CreateRoomProperty(choiceRoomIndex, roomPrefabs[Random.Range(0, roomPrefabs.Length)], choiceRoomPosition);
    }

    void ArrangeSpecialRoom()
    {
        List<int> roomIndexList = new List<int>();
        float longestDistance = 0;
        int selectItemRoomIndex = 0;
        int selectShopRoomIndex = 0;
        int selectBossRoomIndex = 0;

        // 설치한 방 위치 인덱스 배열 생성, 제일 거리가 먼 방을 보스룸으로 설정
        foreach (RoomProperty roomProperty in roomProperties)
        {
            roomIndexList.Add(roomProperty.roomIndex);
            if (longestDistance < roomProperty.roomPosition.magnitude)
            {
                longestDistance = roomProperty.roomPosition.magnitude;
                selectBossRoomIndex = roomProperty.roomIndex;
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
        foreach (RoomProperty roomProperty in roomProperties)
        {
            if (roomProperty.roomIndex == selectItemRoomIndex) roomProperty.roomPrefab = specialroomPrefabs[0];
            if (roomProperty.roomIndex == selectShopRoomIndex) roomProperty.roomPrefab = specialroomPrefabs[1];
            if (roomProperty.roomIndex == selectBossRoomIndex) roomProperty.roomPrefab = specialroomPrefabs[2];
        }
    }

    void InstallRoom()
    {
        // 앞에서 설정한 위치에 설정한 방을 생성
        foreach (RoomProperty roomProperty in roomProperties)
        {
            Room room = Instantiate(roomProperty.roomPrefab, transform);
            room.transform.position = roomProperty.roomPosition;
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
