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
        // �� ���߾��� 41��
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
        // ���� ��� �� �ʱ�ȭ
        GameManager.instance.ui.ClearMapBoard();

        // �� �渶�� ������ ����
        foreach (Room room in rooms)
        {
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
    }

    void SetGoldenItem(Room room)
    {
        int totalItemCount = GameManager.instance.itemPool.items.Length;
        int random = Random.Range(1, totalItemCount);

        // ������ ���� �� ��ġ
        Item goldenItem = GameManager.instance.itemPool.Get(random);
        goldenItem.transform.position = room.itemPoint[0].transform.position;
        goldenItem.Init(GameManager.instance.itemData[random - 1]);
    }

    void SetShopItem(Room room, int count)
    {
        int index = 0;
        int totalItemCount = GameManager.instance.itemPool.items.Length;

        // �ߺ��� �ȵǵ��� ������ ����
        // �̹� ȹ���� �������� �ȳ������� ����.... �ϰ������ ������ ��� �ϴ��� ����
        while (index < count)
        {
            int random = Random.Range(1, totalItemCount);
            int isInItemList = itemsInShop.Find(x => x == random);

            if (isInItemList == 0)
            {
                itemsInShop.Add(random);

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
