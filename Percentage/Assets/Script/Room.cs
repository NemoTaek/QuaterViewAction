using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    public enum RoomType { Clear, Battle, Arcade, Shop };

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
    public Item[] itemPrice;

    [Header("----- Room Property -----")]
    public RoomType roomType;
    public bool isBattle;
    public int enemyCount = 0;
    public bool isClear;
    public int itemCount;
    public bool isItemSet;

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


        // �� Ÿ�� ����
        // �����濡�� ���� 3���� ������ ��Ƴ���
        // �ƹ��͵� ���� �濡�� �� �������
        if (roomType == RoomType.Clear || roomType == RoomType.Shop)
        {
            if(roomType == RoomType.Shop)
            {
                int totalItemCount = GameManager.instance.itemPool.items.Length;
                itemPrice = new Item[itemCount];

                while (itemsInShop.Count < itemCount)
                {
                    int random = Random.Range(1, totalItemCount);
                    int isInItemList = itemsInShop.Find(x => x == random);

                    if (isInItemList == 0)
                    {
                        itemsInShop.Add(random);
                    }
                }
            }
            DoorOpen();
        }
    }

    void Update()
    {
        // ���� �ִ� �游 �˻�
        if(GameManager.instance.currentRoom.gameObject == gameObject)
        {
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
                for (int i = 0; i < itemsInShop.Count; i++)
                {
                    // �������� �����ϰ� ��ġ
                    Item shopItem = GameManager.instance.itemPool.Get(itemsInShop[i]);
                    shopItem.transform.position = itemPoint[i].transform.position + Vector3.up;

                    // ������ ���� ����
                    itemPrice[i] = GameManager.instance.itemPool.Get(0);
                    itemPrice[i].GetComponent<Text>().text = "$ " + shopItem.price.ToString();

                    // �ؽ�Ʈ�� ����ϱ� ���� �ؽ�Ʈ �������� UI ĵ������ ���� ����
                    // Camera.main.WorldToScreenPoint(): ���� ��ǥ���� ��ũ�� ��ǥ������ �����ϴ� �޼ҵ�
                    itemPrice[i].transform.position = Camera.main.WorldToScreenPoint(itemPoint[i].transform.position - (transform.position) / 2);
                    itemPrice[i].transform.SetParent(GameManager.instance.ui.transform.parent.transform);
                }
                isItemSet = true;
            }
        }
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
        int successOrFail = Random.Range(0, 1);
        if (successOrFail == 0)
        {
            GameObject box = GameManager.instance.objectPool.Get(1);
            box.transform.position = transform.position + Vector3.forward;

            //for(int i=0; i<10; i++)
            //{
            //    GameObject box = GameManager.instance.objectPool.Get(1);
            //    box.transform.position = new Vector3(Random.Range(-5, 5), Random.Range(-3, 3), 1);
            //}
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
}
