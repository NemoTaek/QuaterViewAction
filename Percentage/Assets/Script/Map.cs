using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : Singleton<Map>
{
    [Header("----- Map Setting -----")]
    int[] roomCount = { 0, 10, 15, 20, 25 };
    public List<int> roomList;
    public List<int> selectRoomArray;
    public Room[] roomPrefabs;
    public Room[] rooms;
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

        roomList = new List<int>();
        selectRoomArray = new List<int>();

        // 맵 정중앙은 41번
        mapPosition = 41;
        shopItemCount = 3;
        itemsInShop = new List<int>();
        itemPrefab = new Item[shopItemCount];

        // 방 랜덤 배치
        roomList.Add(41);
        for (int i = 0; i < roomCount[GameManager.instance.stage]; i++)
        {
            selectRoomArray.Clear();
            rooms = GetComponentsInChildren<Room>();
            ArrangeMap();
        }

        // 맵 세팅
        mapSquare = GameManager.instance.ui.mapBoard.GetComponentsInChildren<Image>();
        MapInit();
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    void ArrangeMap()
    {
        // 위: -1, 아래: +1, 좌: +9, 우: -9
        int nextMapPosition = roomList[roomList.Count - 1];
        int upRoomIndex = nextMapPosition - 1;
        int downRoomIndex = nextMapPosition + 1;
        int leftRoomIndex = nextMapPosition + 9;
        int rightRoomIndex = nextMapPosition - 9;
        int weight = 0;

        // 사방을 보면서 비어있으면(룸 리스트에 없다면) 해당 방향으로 가중치만큼 배열에 개수 추가
        int isExistUpRoom = roomList.Find(x => x == upRoomIndex);
        if (isExistUpRoom == 0)
        {
            weight = (nextMapPosition % 9) - 1;
            for(int i=0; i<weight; i++)
            {
                selectRoomArray.Add(upRoomIndex);
            }
        }
        int isExistDownRoom = roomList.Find(x => x == downRoomIndex);
        if (isExistDownRoom == 0)
        {
            weight = 9 - (nextMapPosition % 9);
            for (int i = 0; i < weight; i++)
            {
                selectRoomArray.Add(downRoomIndex);
            }
        }
        int isExistLeftRoom = roomList.Find(x => x == leftRoomIndex);
        if (isExistLeftRoom == 0)
        {
            weight = 8 - (nextMapPosition / 9);
            for (int i = 0; i < weight; i++)
            {
                selectRoomArray.Add(leftRoomIndex);
            }
        }
        int isExistRightRoom = roomList.Find(x => x == rightRoomIndex);
        if (isExistRightRoom == 0)
        {
            weight = nextMapPosition / 9;
            for (int i = 0; i < weight; i++)
            {
                selectRoomArray.Add(rightRoomIndex);
            }
        }

        // 랜덤으로 설치할 방을 선택
        Room randomRoom = Instantiate(roomPrefabs[Random.Range(0, roomPrefabs.Length)], transform);

        // 위에서 추가한 방 인덱스 배열 중 랜덤으로 하나 선택
        int selectRoomIndex = Random.Range(0, selectRoomArray.Count);
        if (selectRoomArray[selectRoomIndex] == upRoomIndex)
        {
            randomRoom.transform.position = (rooms[rooms.Length - 1].transform.position + Vector3.up * 12);
        }
        else if (selectRoomArray[selectRoomIndex] == downRoomIndex)
        {
            randomRoom.transform.position = (rooms[rooms.Length - 1].transform.position + Vector3.down * 12);
        }
        else if (selectRoomArray[selectRoomIndex] == leftRoomIndex)
        {
            randomRoom.transform.position = (rooms[rooms.Length - 1].transform.position + Vector3.left * 20);
        }
        else if (selectRoomArray[selectRoomIndex] == rightRoomIndex)
        {
            randomRoom.transform.position = (rooms[rooms.Length - 1].transform.position + Vector3.right * 20);
        }
        roomList.Add(selectRoomArray[selectRoomIndex]);
    }

    public void MapInit()
    {
        // 우측 상단 맵 초기화
        GameManager.instance.ui.ClearMapBoard();

        // 각 방마다 설정값 세팅
        foreach (Room room in rooms)
        {
            // 시작 방이면 현재 방을 시작 방으로 설정
            if (room.roomType == Room.RoomType.Start)
            {
                startRoom = room;
                currentRoom = room;
                currentRoom.isVisited = true;
                room.DoorOpen();
            }

            // 비 전투방에서는 문 열어놓기
            if (room.roomType == Room.RoomType.Clear || room.roomType == Room.RoomType.Golden || room.roomType == Room.RoomType.Shop)
            {
                // 비 전투방에서는 문 열어놓기
                room.DoorOpen();

                // 상점방과 황금방에는 아이템 깔아두기
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
}
