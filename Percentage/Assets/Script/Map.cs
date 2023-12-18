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

        // �� �ʱ�ȭ
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
        // �� �Ӽ� ��ü ����
        RoomProperty room = new RoomProperty();
        room.roomIndex = index;
        room.roomPrefab = prefab;
        room.roomPosition = position;

        // �� �Ӽ� ��ü�� ���� �� ����Ʈ�� �߰��ϰ� �ش� ��ġ�� ���� ��ġ�Ǿ��ٰ� ����
        roomProperties.Add(room);
        isInstalledRoom[room.roomIndex] = true;
        if (roomEnqueue) roomPropertiesQueue.Enqueue(room);
    }

    public void MapSetting()
    {
        // �� ��ġ ���� �ʱ�ȭ
        RoomVariableInit();

        // �Ϲݹ� ��ġ
        for (int i = 0; i < roomCount[GameManager.instance.stage]; i++)
        {
            ArrangeMap();
        }

        // �����۹�, ������, ������ ��ġ
        ArrangeSpecialRoom();

        // �� ��ġ
        InstallRoom();

        // �� �渶�� ���ð� ����
        MapInit();
    }

    void RoomVariableInit()
    {
        // �� ��ġ ������ ������ ť, ��ġ�� �� ����Ʈ, ��ġ ���� �迭, ��ġ�� ��ġ�� ����ġ ����Ʈ
        roomPropertiesQueue = new Queue<RoomProperty>();
        roomProperties = new List<RoomProperty>();
        isInstalledRoom = new bool[82];
        roomWeightList = new List<int>();

        // ������, �����۹� ���� ����Ʈ
        shopItemCount = 3;
        itemsInShop = new List<int>();
        itemPrefab = new Item[shopItemCount];

        // ù �� ����
        CreateRoomProperty(41, startRoom, Vector2.zero, false);

        // �Ϲݹ� ���� ��ġ        
        // ó�� ����� �� ��� �������� ��������� ����
        int branch = Random.Range(1, 5);
        int[] firstRoomIndex = { 40, 42, 32, 50 };
        Vector3 firstRoomPosition = Vector3.zero;

        // ���� ����
        // �ߺ��� ��ġ�� ������ �ٽ� �̱�
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

                // �ش� �� ����
                CreateRoomProperty(choiceRoomIndex, roomPrefabs[Random.Range(0, roomPrefabs.Length)], firstRoomPosition);
            }
        }
    }

    void ArrangeMap()
    {
        // ť���� ���ʴ�� ���� ���� �̾ ��ġ
        RoomProperty prevRoom = roomPropertiesQueue.Dequeue();

        // ����� ���鼭 ���������(������ ���� ���ٸ�) �ش� �������� ����ġ��ŭ �迭�� ���� �߰�
        SetRoomWeightList(prevRoom.roomIndex);

        // ������ �߰��� �� �ε��� �迭 �� �������� �ϳ� ����
        // �׷��� ��¥ ���� Ȯ���� ��𿡵� ���� ���� �� ���� ��찡 �������
        // �׷��� ���� �� ������ ��ġ ������ ���� ã�������� �ݺ� �� �����ؼ� �̾������ ����
        // ��ġ ������ ���� ã�������� �ݺ�
        while (roomWeightList.Count <= 0)
        {
            // ���� ���������� ����� �濡�� Ž������ �� ������ ���� ������ �� ������ �� �� �������� �ϳ� ����
            // �ӽ÷� ������ �� �ε����� �ٽ� ����ġ ����
            SetRoomWeightList(roomProperties[Random.Range(0, roomProperties.Count - 1)].roomIndex);

            // ��ġ ������ ��ġ�� ã������ �׸�
            if (roomWeightList.Count > 0) break;
        }

        // �ֺ��� ���� ��ġ�� �� ������ �������� �ϳ� ������ �� �� ��ġ
        int selectRoomIndex = Random.Range(0, roomWeightList.Count);
        SetNextRoom(selectRoomIndex, prevRoom);

        // ����ġ �迭 �ʱ�ȭ
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

        // ��ġ�� �� ��ġ �ε��� �迭 ����, ���� �Ÿ��� �� ���� ���������� ����
        foreach (RoomProperty roomProperty in roomProperties)
        {
            roomIndexList.Add(roomProperty.roomIndex);
            if (longestDistance < roomProperty.roomPosition.magnitude)
            {
                longestDistance = roomProperty.roomPosition.magnitude;
                selectBossRoomIndex = roomProperty.roomIndex;
            }
        }

        // ��ġ�� �� ��ġ �ε��� �迭���� ������ ���� ���� �����۹�� �������� ����
        while (true)
        {
            int itemRoomRandom = Random.Range(1, roomIndexList.Count);
            int shopRoomRandom = Random.Range(1, roomIndexList.Count);
            selectItemRoomIndex = roomIndexList[itemRoomRandom];
            selectShopRoomIndex = roomIndexList[shopRoomRandom];

            if (selectItemRoomIndex != selectShopRoomIndex && selectItemRoomIndex != selectBossRoomIndex && selectShopRoomIndex != selectBossRoomIndex) break;
        }

        // �� ���� ����
        foreach (RoomProperty roomProperty in roomProperties)
        {
            if (roomProperty.roomIndex == selectItemRoomIndex) roomProperty.roomPrefab = specialroomPrefabs[0];
            if (roomProperty.roomIndex == selectShopRoomIndex) roomProperty.roomPrefab = specialroomPrefabs[1];
            if (roomProperty.roomIndex == selectBossRoomIndex) roomProperty.roomPrefab = specialroomPrefabs[2];
        }
    }

    void InstallRoom()
    {
        // �տ��� ������ ��ġ�� ������ ���� ����
        foreach (RoomProperty roomProperty in roomProperties)
        {
            Room room = Instantiate(roomProperty.roomPrefab, transform);
            room.transform.position = roomProperty.roomPosition;
        }
    }

    public void MapInit()
    {
        // ��� �̴ϸ� ����
        rooms = GetComponentsInChildren<Room>();
        mapSquare = GameManager.instance.ui.mapBoard.GetComponentsInChildren<Image>();

        // ���� ��� �� �ʱ�ȭ
        GameManager.instance.ui.ClearMapBoard();

        // ù ���� �� ����
        currentRoom = rooms[0];
        currentRoom.isVisited = true;

        // �� �渶�� ������ ����
        foreach (Room room in rooms)
        {
            // ������� Ȳ�ݹ濡�� ������ ��Ƶα�
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

        // �̹� ��ġ�� �������� �ٽ� ��ġ�Ǹ� �ȵȴ�.
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

        // �ߺ��� �ȵǵ��� ������ ����
        // �̹� ȹ���� �������� �ȳ������� ����
        while (index < count)
        {
            int random = Random.Range(1, totalItemCount);
            int isInItemList = itemsInShop.Find(x => x == random);
            int isInSetItemList = GameManager.instance.setItemList.Find(x => x == random);

            // ���� ������ �ߺ��� �ȵǰ�, ���ݱ��� ��ġ�� �� ���� ���������� ����
            if (isInItemList == 0 && isInSetItemList == 0)
            {
                itemsInShop.Add(random);
                GameManager.instance.setItemList.Add(random);

                // ������ ���� �� ��ġ
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
            // ������ ���� ����
            // ������ ���� �ؽ�Ʈ�� Canvas�� �����ǹǷ� �������� �ƴϸ� active�� ��Ȱ��ȭ �ؾ��Ѵ�.
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

        // ���⼭ GetComponentsInChildren�� ���̸� ȣ���ϸ� destroy �������� �ұ��ϰ� �� �����ִ� ������ �״�� �ҷ��´�.
        // �ֱ׷���???
        // Destroy�� ȣ���ص� ������Ʈ�� �ٷ� �ı����� �ʱ� �����̴�.
        // ������ ������Ʈ �� OnDestroy�� ȣ��ǹǷ� ����� �����Ϸ��� 1�������� ��ٷ��� �Ѵ�.
        // RŰ�� ������ ��� �޼ҵ尡 �޿��� ����Ǿ ����Ǳ� ������ OnDestroy�� ȣ����� �ʰ� ��� �����ֱ� �����̴�.

        // �׷��� �ذ����
        // �� �ı��Ǵ� ������Ʈ�� ���� �� ������Ʈ�� �и��ϱ� ���� DetachChildren() �޼ҵ带 ����Ͽ� �и���Ų��.
        // �̷��� �ı��Ǵ� ������Ʈ(���� ���� ��)���� ���̻� �� ���� �ڽ��� �ƴϱ⶧���� GetComponentsInChildren�� ȣ���ص� �ҷ����� �ʰ� �ȴ�.
        transform.DetachChildren();
    }
}
