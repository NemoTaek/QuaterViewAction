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

    [Header("----- Shop Setting -----")]
    public List<int> itemsInShop;
    public Item[] itemPrefab;
    public Item[] itemPrice;
    public int itemCount;
    public bool isItemSet;

    void Start()
    {
        mapPosition = 41;
        itemCount = 3;
        itemsInShop = new List<int>();
        itemPrefab = new Item[itemCount];
        rooms = GetComponentsInChildren<Room>();
        MapInit();
    }

    void Update()
    {
        
    }

    public void MapInit()
    {
        GameManager.instance.GameInit();

        foreach (Room room in rooms)
        {
            // 이전 스테이지의 잔해들 모두 삭제
            room.isVisited = false;



            // 시작 방이면 현재 방을 시작 방으로 설정
            if (room.roomType == Room.RoomType.Start)
            {
                startRoom = room;
                currentRoom = room;
                currentRoom.isVisited = true;
                room.DoorOpen();
            }

            // 비 전투방에서는 문 열어놓기
            if (room.roomType == Room.RoomType.Clear || room.roomType == Room.RoomType.Shop)
            {
                // 비 전투방에서는 문 열어놓기
                room.DoorOpen();

                // 상점방에는 아이템 깔아두기
                if (room.roomType == Room.RoomType.Shop)
                {
                    SetShopItem(room);
                    SetShopItemPrice();
                    isItemSet = true;
                }
            }
        }
    }

    void SetShopItem(Room shopRoom)
    {
        int index = 0;
        int totalItemCount = GameManager.instance.itemPool.items.Length;

        // 중복이 안되도록 아이템 세팅
        while (index < itemCount)
        {
            int random = Random.Range(1, totalItemCount);
            int isInItemList = itemsInShop.Find(x => x == random);

            if (isInItemList == 0)
            {
                itemsInShop.Add(random);

                // 아이템 생성 후 배치
                itemPrefab[index] = GameManager.instance.itemPool.Get(random);
                itemPrefab[index].transform.position = shopRoom.itemPoint[index].transform.position + Vector3.up;
                itemPrefab[index].Init(GameManager.instance.itemData[random - 1]);
                index++;
            }
        }
    }

    void SetShopItemPrice()
    {
        itemPrice = new Item[itemCount];
        for (int i = 0; i < itemCount; i++)
        {
            // 아이템 가격 세팅
            // 아이템 가격 텍스트는 Canvas에 설정되므로 상점방이 아니면 active를 비활성화 해야한다.
            Item priceText = GameManager.instance.itemPool.Get(0);
            priceText.GetComponent<Text>().text = "$ " + itemPrefab[i].price.ToString();
            itemPrice[i] = priceText;
            itemPrice[i].transform.SetParent(GameManager.instance.itemCanvas.transform);
        }
    }
}
