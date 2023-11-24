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
    public bool completeArrangeRooms;
    public bool completeSettingRooms;

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

        // �� ��ġ �Ϸ�
        completeArrangeRooms = true;
    }

    void Start()
    {
        
    }

    void Update()
    {
        if (completeArrangeRooms && !completeSettingRooms)
        {
            // �� ����
            rooms = GetComponentsInChildren<Room>();
            mapSquare = GameManager.instance.ui.mapBoard.GetComponentsInChildren<Image>();
            MapInit();
        }
    }

    void RoomVariableInit()
    {
        // �� ���߾��� 41��
        roomQueue = new Queue<int>();
        roomWeightList = new List<int>();
        settingRooms = new Dictionary<int, Room>();
        settingRoomPositions = new Dictionary<int, Vector3>();
        mapPosition = 41;

        // ������, �����۹� ���� ����Ʈ
        shopItemCount = 3;
        itemsInShop = new List<int>();
        itemPrefab = new Item[shopItemCount];

        // �Ϲݹ� ���� ��ġ        
        // ó�� ����� �� ��� �������� ��������� ����
        settingRooms.Add(41, null);
        settingRoomPositions.Add(41, Vector3.zero);
        int branch = Random.Range(1, 5);
        int[] firstRoomIndex = { 40, 42, 32, 50 };
        Vector3 firstRoomPosition = Vector3.zero;

        // ���� ����
        // �ߺ��� ��ġ�� ������ �ٽ� �̱�
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

                // ť, ��, �� ��ġ�� ���� �߰�
                roomQueue.Enqueue(choiceRoom);
                settingRooms.Add(choiceRoom, roomPrefabs[Random.Range(0, roomPrefabs.Length)]);
                settingRoomPositions.Add(choiceRoom, firstRoomPosition);
            }
        }
    }

    void ArrangeMap()
    {
        // ��: -1, �Ʒ�: +1, ��: +9, ��: -9
        int nextMapPosition = roomQueue.Dequeue();
        int upRoomIndex = nextMapPosition - 1;
        int downRoomIndex = nextMapPosition + 1;
        int leftRoomIndex = nextMapPosition + 9;
        int rightRoomIndex = nextMapPosition - 9;
        int weight = 0;

        // ����� ���鼭 ���������(�� ť�� ���ٸ�) �ش� �������� ����ġ��ŭ �迭�� ���� �߰�
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

        // ������ �߰��� �� �ε��� �迭 �� �������� �ϳ� ����
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

        // ť, ��, �� ��ġ�� �߰�
        roomQueue.Enqueue(roomWeightList[selectRoomIndex]);
        settingRooms.Add(roomWeightList[selectRoomIndex], roomPrefabs[Random.Range(0, roomPrefabs.Length)]);
        settingRoomPositions.Add(roomWeightList[selectRoomIndex], randomRoomPosition);

        // ����ġ �迭 �ʱ�ȭ
        roomWeightList.Clear();
    }

    void ArrangeSpecialRoom()
    {
        List<int> roomIndexList = new List<int>();
        float longestDistance = 0;
        int selectItemRoomIndex = 0;
        int selectShopRoomIndex = 0;
        int selectBossRoomIndex = 0;

        // ��ġ�� �� ��ġ �ε��� �迭 ����, ���� �Ÿ��� �� ���� ���������� ����
        foreach (KeyValuePair<int, Vector3> installedRoom in settingRoomPositions)
        {
            roomIndexList.Add(installedRoom.Key);
            if (longestDistance < installedRoom.Value.magnitude)
            {
                longestDistance = installedRoom.Value.magnitude;
                selectBossRoomIndex = installedRoom.Key;
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
        settingRooms[selectItemRoomIndex] = specialroomPrefabs[0];
        settingRooms[selectShopRoomIndex] = specialroomPrefabs[1];
        settingRooms[selectBossRoomIndex] = specialroomPrefabs[2];
    }

    void InstallRoom()
    {
        Room room = null;

        // �տ��� ������ ��ġ�� ������ ���� ����
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
        // ���� ��� �� �ʱ�ȭ
        GameManager.instance.ui.ClearMapBoard();

        // �� �渶�� ������ ����
        foreach (Room room in rooms)
        {
            // ���� �ִ� �濡�� ������ ���� �ִ��� Ž��
            room.CheckAroundRoom();

            // ���� ���̸� ���� ���� ���� ������ ����
            if (room.roomType == Room.RoomType.Start)
            {
                startRoom = room;
                currentRoom = room;
                currentRoom.isVisited = true;
                room.DoorOpen();
            }

            // �� �����濡���� �� �������
            if (room.roomType == Room.RoomType.Clear || room.roomType == Room.RoomType.Golden || room.roomType == Room.RoomType.Shop)
            {
                // �� �����濡���� �� �������
                room.DoorOpen();

                // ������� Ȳ�ݹ濡�� ������ ��Ƶα�
                if (room.roomType == Room.RoomType.Shop)
                {
                    SetShopItem(room, shopItemCount);
                    SetShopItemPrice();
                    isItemSet = true;
                }
                else if (room.roomType == Room.RoomType.Golden)
                {
                    SetGoldenItem(room);
                    isItemSet = true;
                }
            }
        }

        completeSettingRooms = true;
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
}
