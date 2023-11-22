using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : Singleton<Map>
{
    [Header("----- Map Setting -----")]
    int[] roomCount = { 0, 10, 15, 20, 25 };
    Queue<int> roomQueue;
    List<int> selectRoomArray;
    Vector3[] roomPositionArray;
    List<int> installedRoomArray;
    public Room[] roomPrefabs;
    public Room[] rooms;
    public bool completeArrangeRooms;
    public bool completeSettingRooms;
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

        // �� ��ġ
        for (int i = 0; i < roomCount[GameManager.instance.stage]; i++)
        {
            ArrangeMap();
        }
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
        selectRoomArray = new List<int>();
        roomPositionArray = new Vector3[81];
        installedRoomArray = new List<int>();
        mapPosition = 41;
        roomPositionArray[40] = Vector3.zero;

        // ������, �����۹� ���� ����Ʈ
        shopItemCount = 3;
        itemsInShop = new List<int>();
        itemPrefab = new Item[shopItemCount];

        // �� ���� ��ġ        
        // ó�� ����� �� ��� �������� ��������� ����
        installedRoomArray.Add(41);
        int branch = Random.Range(1, 5);
        int[] firstRoomIndex = { 40, 42, 32, 50 };
        while (roomQueue.Count < branch)
        {
            int choiceRoom = firstRoomIndex[Random.Range(0, 4)];
            if (installedRoomArray.Find(x => x == choiceRoom) == 0)
            {
                Room randomRoom = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Length)], transform);
                if (choiceRoom == 40)
                {
                    randomRoom.transform.position = Vector3.up * 12;
                }
                else if (choiceRoom == 42)
                {
                    randomRoom.transform.position = Vector3.down * 12;
                }
                else if (choiceRoom == 32)
                {
                    randomRoom.transform.position = Vector3.right * 20;
                }
                else if (choiceRoom == 50)
                {
                    randomRoom.transform.position = Vector3.left * 20;
                }
                roomQueue.Enqueue(choiceRoom);
                roomPositionArray[choiceRoom - 1] = randomRoom.transform.position;
                installedRoomArray.Add(choiceRoom);
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
        bool isExistUpRoom = installedRoomArray.Find(x => x == upRoomIndex) != 0;
        if (!isExistUpRoom)
        {
            weight = (nextMapPosition % 9) - 1;
            for(int i=0; i<weight; i++)
            {
                selectRoomArray.Add(upRoomIndex);
            }
        }
        bool isExistDownRoom = installedRoomArray.Find(x => x == downRoomIndex) != 0;
        if (!isExistDownRoom)
        {
            weight = 9 - (nextMapPosition % 9);
            for (int i = 0; i < weight; i++)
            {
                selectRoomArray.Add(downRoomIndex);
            }
        }
        bool isExistLeftRoom = installedRoomArray.Find(x => x == leftRoomIndex) != 0;
        if (!isExistLeftRoom)
        {
            weight = 8 - (nextMapPosition / 9);
            for (int i = 0; i < weight; i++)
            {
                selectRoomArray.Add(leftRoomIndex);
            }
        }
        bool isExistRightRoom = installedRoomArray.Find(x => x == rightRoomIndex) != 0;
        if (!isExistRightRoom)
        {
            weight = nextMapPosition / 9;
            for (int i = 0; i < weight; i++)
            {
                selectRoomArray.Add(rightRoomIndex);
            }
        }

        // �������� ��ġ�� ���� ����
        Room randomRoom = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Length)], transform);

        // ������ �߰��� �� �ε��� �迭 �� �������� �ϳ� ����
        int selectRoomIndex = Random.Range(0, selectRoomArray.Count);
        if (selectRoomArray[selectRoomIndex] == upRoomIndex)
        {
            randomRoom.transform.position = (roomPositionArray[nextMapPosition - 1] + Vector3.up * 12);
        }
        else if (selectRoomArray[selectRoomIndex] == downRoomIndex)
        {
            randomRoom.transform.position = (roomPositionArray[nextMapPosition - 1] + Vector3.down * 12);
        }
        else if (selectRoomArray[selectRoomIndex] == leftRoomIndex)
        {
            randomRoom.transform.position = (roomPositionArray[nextMapPosition - 1] + Vector3.left * 20);
        }
        else if (selectRoomArray[selectRoomIndex] == rightRoomIndex)
        {
            randomRoom.transform.position = (roomPositionArray[nextMapPosition - 1] + Vector3.right * 20);
        }

        // �� ť�� ��ġ �迭���� ��ġ�� �� �����߰�
        roomQueue.Enqueue(selectRoomArray[selectRoomIndex]);
        roomPositionArray[selectRoomArray[selectRoomIndex] - 1] = randomRoom.transform.position;
        installedRoomArray.Add(selectRoomArray[selectRoomIndex]);

        // ����ġ �迭 �ʱ�ȭ
        selectRoomArray.Clear();
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
