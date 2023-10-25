using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    public enum RoomType { Start, Clear, Battle, Arcade, Golden, Shop, Boss };

    [Header("----- Component -----")]
    public Enemy enemy;
    public SpawnPoint[] spawnPoint;
    public GameObject[] itemPoint;
    public Door[] doors;
    public Room upRoom;
    public Room rightRoom;
    public Room downRoom;
    public Room leftRoom;

    [Header("----- Room Object -----")]
    public Button[] buttons;
    public GameObject roomReward;
    public Item[] bossDropItems;
    public ItemData[] bossDropItemDatas;

    [Header("----- Room Property -----")]
    public RoomType roomType;
    public bool isVisited;
    public bool isBattle;
    public int enemyCount = 0;
    public bool isClear;
    public bool isItemSet;
    public bool isMapDraw;

    void Awake()
    {
        spawnPoint = GetComponentsInChildren<SpawnPoint>();
        doors = GetComponentsInChildren<Door>(true);
        buttons = GetComponentsInChildren<Button>();
    }

    void OnEnable()
    {
        
    }

    void Start()
    {
        // ���� �ִ� �濡�� ������ ���� �ִ��� Ž��
        CheckAroundRoom();
    }

    void Update()
    {
        SettingRooms();
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

    void SettingRooms()
    {
        // ���� �ִ� �游 �˻�
        if (Map.instance.currentRoom.gameObject == gameObject)
        {
            // �ֺ� �� ������
            if (!isMapDraw) DrawMap();

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

            // �����濡 ���� ����� ������ ���� �ٽ� Ȱ��ȭ
            if (roomType == RoomType.Shop && Map.instance.isItemSet)
            {
                for (int i = 0; i < Map.instance.itemPrice.Length; i++)
                {
                    // �ؽ�Ʈ�� ����ϱ� ���� �ؽ�Ʈ �������� UI ĵ������ ���� ����
                    // Camera.main.WorldToScreenPoint(): ���� ��ǥ���� ��ũ�� ��ǥ������ �����ϴ� �޼ҵ�
                    Map.instance.itemPrice[i].transform.position = Camera.main.WorldToScreenPoint(itemPoint[i].transform.position);

                    // ������ �������� ��Ȱ��ȭ
                    Map.instance.itemPrice[i].gameObject.SetActive(!Map.instance.itemPrefab[i].getItem);
                }
            }

            // �������̸� ���� ü�� UI Ȱ��ȭ
            if (roomType == RoomType.Boss)
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
            if (roomType == RoomType.Shop && Map.instance.isItemSet)
            {
                for (int i = 0; i < Map.instance.itemsInShop.Count; i++)
                {
                    Item itemPrice = Map.instance.itemPrice[i];
                    if(itemPrice != null)
                    {
                        if (itemPrice.gameObject.activeSelf) itemPrice.gameObject.SetActive(false);
                    }
                    
                }
            }
        }
    }

    void DrawMap()
    {
        // �� UI���� ���� ��ġ�� ���� ������ 0.75, �ֺ��� �ִ� ���� 0.25�� ����
        // ����� ���� ����� 1��°, ���� �ϴ��� 81��°. ���� ������ ��->��, ��->��

        // ���� ��ġ�� ���ϰ� �湮 ǥ��
        Map.instance.mapSquare[Map.instance.mapPosition].color = new Color(1, 1, 1, 0.75f);

        // �ֺ� �� ���� ǥ��
        if (leftRoom && !leftRoom.isVisited)    ColorMap(leftRoom, 9);
        if (rightRoom && !rightRoom.isVisited)  ColorMap(rightRoom, -9);
        if (upRoom && !upRoom.isVisited)    ColorMap(upRoom, -1);
        if (downRoom && !downRoom.isVisited)    ColorMap(downRoom, 1);

        isMapDraw = true;
    }

    void ColorMap(Room directionRoom, int roomIndex)
    {
        int mapPosition = Map.instance.mapPosition;
        mapPosition += roomIndex;
        Map.instance.mapSquare[mapPosition].color = new Color(1, 1, 1, 0.25f);

        // Ư�����̶�� ������ �߰�
        SpecialRoom(directionRoom, Map.instance.mapSquare[mapPosition]);
    }

    void SpecialRoom(Room room, Image mapSqure)
    {
        if(room.roomType == RoomType.Boss)
        {
            mapSqure.gameObject.SetActive(true);
            Image roomIcon = mapSqure.GetComponentInChildren<Image>();
            roomIcon.sprite = GameManager.instance.roomIcon[0];
        }
        else if (room.roomType == RoomType.Golden)
        {
            mapSqure.gameObject.SetActive(true);
            Image roomIcon = mapSqure.GetComponentInChildren<Image>();
            roomIcon.sprite = GameManager.instance.roomIcon[1];
        }
        else if (room.roomType == RoomType.Shop)
        {
            mapSqure.gameObject.SetActive(true);
            Image roomIcon = mapSqure.GetComponentInChildren<Image>();
            roomIcon.sprite = GameManager.instance.roomIcon[2];
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
        if(roomType == RoomType.Boss)
        {
            // ��Ż ����
            GameObject portal = GameManager.instance.objectPool.Get(4);
            portal.transform.position = transform.position;

            // ���� ������ ����
            // ��ġ: ¦���� -1, 1, -2, 2, -3, 3 .... / Ȧ���� 0, -1, 1, -2, 2 ....
            Vector3 itemStartPosition = Vector3.left * (bossDropItems.Length % 2 == 0 ? bossDropItems.Length / 2 + 0.5f : (int)(bossDropItems.Length / 2));
            for (int i = 0; i < bossDropItems.Length; i++)
            {
                Item item = Instantiate(bossDropItems[i], transform);
                item.transform.position = transform.position + Vector3.down + itemStartPosition + Vector3.right * i;
                item.Init(bossDropItemDatas[i]);
            }
        }
        else
        {
            // ���� ȹ�� (0: ����, 1: ����)
            int successOrFail = Random.Range(0, 2);
            GameObject reward;

            // ������ �������� �ڸ��� ������Ʈ�� ������ �򰥸��Ƿ� ������ �ٸ����� ������ ����
            Vector3 pos = GameManager.instance.CheckAround(transform.position);

            if (successOrFail == 0)
            {
                reward = Instantiate(GameManager.instance.objectPool.prefabs[0], roomReward.transform);
                reward.transform.position = transform.position + pos;
            }
            else
            {
                // ���� ��⿡ �����ϸ� ���̳� ��Ʈ ����
                // 0: 1��, 1: ��Ʈ ����, 2: ������ ��Ʈ
                int randomObject = Random.Range(1, 4);
                reward = Instantiate(GameManager.instance.objectPool.prefabs[randomObject], roomReward.transform);
                reward.transform.position = transform.position + pos;
            }
        }

        // ��Ƽ�� �������� �ִٸ� ������ 1ĭ ����
        if(GameManager.instance.player.activeItem && GameManager.instance.player.activeItem.currentGuage < GameManager.instance.player.activeItem.activeGuage)
        {
            GameManager.instance.player.activeItem.currentGuage++;
            GameManager.instance.ui.isChanged = true;
        }

        // ������ �����ٸ� ���������� ��ġ�� ���� ����
        DoorOpen();
    }

    public void DoorOpen()
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
