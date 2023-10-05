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
            // ���� ���������� ���ص� ��� ����
            room.isVisited = false;



            // ���� ���̸� ���� ���� ���� ������ ����
            if (room.roomType == Room.RoomType.Start)
            {
                startRoom = room;
                currentRoom = room;
                currentRoom.isVisited = true;
                room.DoorOpen();
            }

            // �� �����濡���� �� �������
            if (room.roomType == Room.RoomType.Clear || room.roomType == Room.RoomType.Shop)
            {
                // �� �����濡���� �� �������
                room.DoorOpen();

                // �����濡�� ������ ��Ƶα�
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

        // �ߺ��� �ȵǵ��� ������ ����
        while (index < itemCount)
        {
            int random = Random.Range(1, totalItemCount);
            int isInItemList = itemsInShop.Find(x => x == random);

            if (isInItemList == 0)
            {
                itemsInShop.Add(random);

                // ������ ���� �� ��ġ
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
            // ������ ���� ����
            // ������ ���� �ؽ�Ʈ�� Canvas�� �����ǹǷ� �������� �ƴϸ� active�� ��Ȱ��ȭ �ؾ��Ѵ�.
            Item priceText = GameManager.instance.itemPool.Get(0);
            priceText.GetComponent<Text>().text = "$ " + itemPrefab[i].price.ToString();
            itemPrice[i] = priceText;
            itemPrice[i].transform.SetParent(GameManager.instance.itemCanvas.transform);
        }
    }
}
