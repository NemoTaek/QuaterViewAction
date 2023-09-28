using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    public enum RoomType { Start, Clear, Battle, Arcade, Shop, Boss };

    [Header("----- Component -----")]
    public Enemy enemy;
    public SpawnPoint[] spawnPoint;
    public Door[] doors;
    public Room upRoom;
    public Room rightRoom;
    public Room downRoom;
    public Room leftRoom;
    public GameObject[] itemPoint;

    [Header("----- Room Object -----")]
    public Button[] buttons;
    public List<int> itemsInShop;
    public Item[] itemPrefab;
    public Item[] itemPrice;

    [Header("----- Room Property -----")]
    public RoomType roomType;
    public bool isVisited;
    public bool isBattle;
    public int enemyCount = 0;
    public bool isClear;
    public int itemCount;
    public bool isItemSet;
    public bool isMapDraw;

    void Awake()
    {
        spawnPoint = GetComponentsInChildren<SpawnPoint>();
        doors = GetComponentsInChildren<Door>(true);
        buttons = GetComponentsInChildren<Button>();

        itemsInShop = new List<int>();
    }

    void OnEnable()
    {
        
    }

    void Start()
    {
        CheckAroundRoom();

        // �� Ÿ�� ����
        // �����濡�� ���� 3���� ������ ��Ƴ���
        if (roomType == RoomType.Start || roomType == RoomType.Clear || roomType == RoomType.Shop)
        {
            if(roomType == RoomType.Shop)   SetShopItem();

            // ���̳� ���� ������Ʈ�� ���� �濡�� �� �������
            DoorOpen();
        }
    }

    void Update()
    {
        // ���� �ִ� �游 �˻�
        if (GameManager.instance.currentRoom.gameObject == gameObject)
        {
            // �ֺ� �� ������
            if (!isMapDraw)  DrawMap();

            // ���� ����Ʈ�� ������ ������
            if (spawnPoint.Length > 0)
            {
                // ���� �������� �ƴ϶�� ���� ����
                if (!isClear && !isBattle) BattleStart();

                // �������ε� ���� ��� óġ�ߴٸ� ���� ����
                else if (!isClear && isBattle)
                {
                    if (enemyCount <= 0) BattleEnd();
                }
            }

            // ��ư�� ������ �����̵��
            else if (buttons.Length > 0)
            {
                // ��ư�� ��� ������ �� Ŭ����
                if (!isClear && IsAllButtonPressed()) BattleEnd();
            }

            if (roomType == RoomType.Shop && !isItemSet)
            {
                // �������� �ѹ��� ���õ��� �ʾ����� ���� �迭 �ʱ�ȭ �� ����
                itemPrice = GameManager.instance.itemCanvas.GetComponentsInChildren<Item>(true);
                if (itemPrice.Length == 0)
                {
                    itemPrice = new Item[itemCount];
                    SetShopItemPrice();
                }

                // ���� ������ ������ �ٽ� Ȱ��ȭ
                else
                {
                    for (int i = 0; i < itemPrice.Length; i++)
                    {
                        // ������ �������� ��Ȱ��ȭ
                        itemPrice[i].gameObject.SetActive(!itemPrefab[i].isPurchased);
                    }
                }

                isItemSet = true;
            }

            // �������̸� ���� ü�� UI Ȱ��ȭ
            if(roomType == RoomType.Boss)
            {
                Boss boss = GetComponentInChildren<Boss>();
                if (boss)
                {
                    if (boss.isLive && !GameManager.instance.ui.bossUI.activeSelf) GameManager.instance.ui.bossUI.SetActive(true);
                    float currentBossHealth = boss.health;
                    GameManager.instance.ui.bossHealth.sizeDelta = new Vector2(currentBossHealth / boss.maxHealth * 500, 100);
                }
            }
        }
        else
        {
            // ���� ���� ������ UI�� ��Ȱ��ȭ ���Ѿ� �Ѵ�.
            if(roomType == RoomType.Shop)
            {
                isItemSet = false;
                for (int i = 0; i < itemsInShop.Count; i++)
                {
                    itemPrice[i].gameObject.SetActive(false);
                }
            }
        }
    }

    void CheckAroundRoom()
    {
        // ���̾� ����ũ�� ���̾ ��Ʈ�� �Ǵ��ϱ� ������ ��Ʈ �����ڸ� ����Ͽ� �ۼ��ؾ� ���Ѵ�.
        // �׳� ���̾� �ڸ��� 6 �̷��� ���ϱ� �ȸ�������..
        // Ư�� ���̾ �����Ϸ��� ~(1 << ���̾�) �̷��� ����ϸ� �ȴ�.
        //LayerMask layer = (1 << LayerMask.NameToLayer("Room"));
        //colliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(25, 15), 0, layer);

        // �ٵ� �밢���� ���� �˾Ƴ� �ʿ䰡 ����...
        // �̹��� ����ĳ��Ʈ�� �����¿츸 ���� �����غ����� ����
        RaycastHit2D leftRoomRayCast = Physics2D.Raycast(transform.position + Vector3.left * 10, Vector2.left, 3, LayerMask.GetMask("Room"));
        if (leftRoomRayCast) leftRoom = leftRoomRayCast.collider.gameObject.GetComponent<Room>();
        RaycastHit2D rightRoomRayCast = Physics2D.Raycast(transform.position + Vector3.right * 10, Vector2.right, 3, LayerMask.GetMask("Room"));
        if (rightRoomRayCast) rightRoom = rightRoomRayCast.collider.gameObject.GetComponent<Room>();
        RaycastHit2D upRoomRayCast = Physics2D.Raycast(transform.position + Vector3.up * 6, Vector2.up, 3, LayerMask.GetMask("Room"));
        if (upRoomRayCast) upRoom = upRoomRayCast.collider.gameObject.GetComponent<Room>();
        RaycastHit2D downRoomRayCast = Physics2D.Raycast(transform.position + Vector3.down * 6, Vector2.down, 3, LayerMask.GetMask("Room"));
        if (downRoomRayCast) downRoom = downRoomRayCast.collider.gameObject.GetComponent<Room>();
    }

    void DrawMap()
    {
        // �� UI���� ���� ��ġ�� ���� ������ 0.75, �ֺ��� �ִ� ���� 0.25�� ����
        // ����� ���� ����� 1��°, ���� �ϴ��� 81��°. ���� ������ ��->��, ��->��
        Image[] mapSquare = GameManager.instance.ui.mapBoard.GetComponentsInChildren<Image>();

        // ���� ��ġ�� ���ϰ� �湮 ǥ��
        mapSquare[GameManager.instance.mapPosition].color = new Color(1, 1, 1, 0.75f);

        // �ֺ����� ���� ǥ��
        if (leftRoom && !leftRoom.isVisited)
        {
            int mapPosition = GameManager.instance.mapPosition;
            mapPosition += 9;
            mapSquare[mapPosition].color = new Color(1, 1, 1, 0.25f);
        }
        if (rightRoom && !rightRoom.isVisited)
        {
            int mapPosition = GameManager.instance.mapPosition;
            mapPosition -= 9;
            mapSquare[mapPosition].color = new Color(1, 1, 1, 0.25f);
        }
        if (upRoom && !upRoom.isVisited)
        {
            int mapPosition = GameManager.instance.mapPosition;
            mapPosition -= 1;
            mapSquare[mapPosition].color = new Color(1, 1, 1, 0.25f);
        }
        if (downRoom && !downRoom.isVisited)
        {
            int mapPosition = GameManager.instance.mapPosition;
            mapPosition += 1;
            mapSquare[mapPosition].color = new Color(1, 1, 1, 0.25f);
        }

        isMapDraw = true;
    }

    void BattleStart()
    {
        isBattle = true;

        // ���� ���� �� �� ���
        //for (int i = 0; i < doors.Length; i++)
        //{
        //    doors[i].gameObject.SetActive(false);
        //}

        // ��ȯ ������ �ִٸ� ���� ��ȯ
        if(spawnPoint.Length > 0)
        {
            for (int i = 0; i < spawnPoint.Length; i++)
            {
                Instantiate(enemy, spawnPoint[i].transform);
                enemyCount++;
            }
        }
    }

    void BattleEnd()
    {
        // ���� ȹ�� (0: ����, 1: ����)
        int successOrFail = Random.Range(0, 2);
        if (successOrFail == 0)
        {
            GameObject box = GameManager.instance.objectPool.Get(0);
            box.transform.position = transform.position + Vector3.forward;

            //for(int i=0; i<10; i++)
            //{
            //    GameObject box = GameManager.instance.objectPool.Get(1);
            //    box.transform.position = new Vector3(Random.Range(-5, 5), Random.Range(-3, 3), 1);
            //}
        }
        else
        {
            // ���� ��⿡ �����ϸ� ���̳� ��Ʈ ����
            // 0: 1��, 1: ��Ʈ ����, 2: ������ ��Ʈ
            int randomObject = Random.Range(1, 4);
            GameObject reward = GameManager.instance.objectPool.Get(randomObject);
            reward.transform.position = transform.position;
        }

        //isBattle = false;

        // ������ �����ٸ� ���������� ��ġ�� ���� ����
        DoorOpen();
    }

    void DoorOpen()
    {
        isClear = true;

        if (upRoom) doors[0].gameObject.SetActive(true);
        if (rightRoom) doors[1].gameObject.SetActive(true);
        if (downRoom) doors[2].gameObject.SetActive(true);
        if (leftRoom) doors[3].gameObject.SetActive(true);
    }

    bool IsAllButtonPressed()
    {
        for(int i=0; i<buttons.Length; i++)
        {
            if (!buttons[i].isPressed) return false;
        }
        return true;
    }

    void SetShopItem()
    {
        int index = 0;
        int totalItemCount = GameManager.instance.itemPool.items.Length;

        // ó�� ������ ����
        itemPrice = new Item[itemCount];
        for (int i = 0; i < itemCount; i++)
        {
            itemPrice[i] = GameManager.instance.itemPool.Get(0);
            itemPrice[i].gameObject.SetActive(false);
        }

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
                itemPrefab[index].transform.position = itemPoint[index].transform.position + Vector3.up;
                itemPrefab[index].Init(GameManager.instance.itemData[random - 1]);
                index++;
            }
        }
    }

    void SetShopItemPrice()
    {
        for (int i = 0; i < itemPrice.Length; i++)
        {
            // ������ ���� ����
            itemPrice[i] = GameManager.instance.itemPool.Get(0);
            itemPrice[i].GetComponent<Text>().text = "$ " + itemPrefab[i].price.ToString();

            // �ؽ�Ʈ�� ����ϱ� ���� �ؽ�Ʈ �������� UI ĵ������ ���� ����
            // Camera.main.WorldToScreenPoint(): ���� ��ǥ���� ��ũ�� ��ǥ������ �����ϴ� �޼ҵ�
            itemPrice[i].transform.position = Camera.main.WorldToScreenPoint(itemPoint[i].transform.position - (transform.position) / 2);
            itemPrice[i].transform.SetParent(GameManager.instance.itemCanvas.transform);

            // ������ �������� ��Ȱ��ȭ
            if (itemPrefab[i].isPurchased)
            {
                itemPrice[i].gameObject.SetActive(false);
            }
        }
    }
}
