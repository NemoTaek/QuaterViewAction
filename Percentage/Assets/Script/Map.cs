using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : Singleton<Map>
{
    [Header("----- Map Setting -----")]
    public Room[] rooms;
    public Room startRoom;
    public Room currentRoom;
    public int mapPosition;
    public Image[] mapSquare;

    [Header("----- Shop Setting -----")]
    public List<int> itemsInShop;
    public Item[] itemPrefab;
    public Item[] itemPrice;
    public int shopItemCount;
    public bool isItemSet;

    void Start()
    {
        // 맵 정중앙은 41번
        mapPosition = 41;
        shopItemCount = 3;
        itemsInShop = new List<int>();
        itemPrefab = new Item[shopItemCount];

        rooms = GetComponentsInChildren<Room>();
        mapSquare = GameManager.instance.ui.mapBoard.GetComponentsInChildren<Image>();
        MapInit();
    }

    void Update()
    {
        
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

        // 아이템 생성 후 배치
        Item goldenItem = GameManager.instance.itemPool.Get(random);
        goldenItem.transform.position = room.itemPoint[0].transform.position;
        goldenItem.Init(GameManager.instance.itemData[random - 1]);
    }

    void SetShopItem(Room room, int count)
    {
        int index = 0;
        int totalItemCount = GameManager.instance.itemPool.items.Length;

        // 중복이 안되도록 아이템 세팅
        // 이미 획득한 아이템은 안나오도록 설정.... 하고싶은데 개수가 적어서 일단은 보류
        while (index < count)
        {
            int random = Random.Range(1, totalItemCount);
            int isInItemList = itemsInShop.Find(x => x == random);

            if (isInItemList == 0)
            {
                itemsInShop.Add(random);

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
