using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    public enum RoomType { Start, Clear, Battle, Arcade, Shop, Boss };

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
                    Map.instance.itemPrice[i].gameObject.SetActive(!Map.instance.itemPrefab[i].isPurchased);
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
        Image[] mapSquare = GameManager.instance.ui.mapBoard.GetComponentsInChildren<Image>();

        // ���� ��ġ�� ���ϰ� �湮 ǥ��
        mapSquare[Map.instance.mapPosition].color = new Color(1, 1, 1, 0.75f);

        // �ֺ����� ���� ǥ��
        if (leftRoom && !leftRoom.isVisited)
        {
            int mapPosition = Map.instance.mapPosition;
            mapPosition += 9;
            mapSquare[mapPosition].color = new Color(1, 1, 1, 0.25f);
        }
        if (rightRoom && !rightRoom.isVisited)
        {
            int mapPosition = Map.instance.mapPosition;
            mapPosition -= 9;
            mapSquare[mapPosition].color = new Color(1, 1, 1, 0.25f);
        }
        if (upRoom && !upRoom.isVisited)
        {
            int mapPosition = Map.instance.mapPosition;
            mapPosition -= 1;
            mapSquare[mapPosition].color = new Color(1, 1, 1, 0.25f);
        }
        if (downRoom && !downRoom.isVisited)
        {
            int mapPosition = Map.instance.mapPosition;
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
        if(roomType == RoomType.Boss)
        {
            // ��Ż ����
            GameObject portal = GameManager.instance.objectPool.Get(4);
            portal.transform.position = transform.position;

            // ���� ������ ����
            //GameObject bossReward = GameManager.instance.objectPool.Get(5);
            //portal.transform.position = transform.position + Vector3.down;
        }
        else
        {
            // ���� ȹ�� (0: ����, 1: ����)
            int successOrFail = Random.Range(0, 2);
            GameObject reward;
            if (successOrFail == 0)
            {
                reward = GameManager.instance.objectPool.prefabs[0];
                Instantiate(reward, roomReward.transform);
                reward.transform.position = Vector3.zero;
            }
            else
            {
                // ���� ��⿡ �����ϸ� ���̳� ��Ʈ ����
                // 0: 1��, 1: ��Ʈ ����, 2: ������ ��Ʈ
                int randomObject = Random.Range(1, 4);
                reward = GameManager.instance.objectPool.prefabs[randomObject];
                Instantiate(reward, roomReward.transform);
                reward.transform.position = Vector3.zero;
            }
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
