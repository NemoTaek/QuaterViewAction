using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Room : MonoBehaviour
{
    public enum RoomType { Start, Clear, Battle, Arcade, Quiz, Golden, Shop, Boss };

    [Header("----- Component -----")]
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
    public bool isQuizSet;
    public bool isSelectAnswer;
    Text quizText;
    bool quizAnswer;

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
            if ((roomType == RoomType.Battle || roomType == RoomType.Boss) && spawnPoint.Length > 0)
            {
                // ���� �������� �ƴ϶�� ���� ����
                if (!isClear && !isBattle) BattleStart();

                // �������̸� ���� ü�� UI Ȱ��ȭ
                if (roomType == RoomType.Boss)
                {
                    Enemy boss = GetComponentInChildren<Enemy>();
                    if (boss && boss.type == EnemyData.EnemyType.Boss)
                    {
                        if (boss.isLive && !GameManager.instance.ui.bossUI.activeSelf) GameManager.instance.ui.bossUI.SetActive(true);
                        float currentBossHealth = boss.health;
                        GameManager.instance.ui.bossHealth.sizeDelta = new Vector2(currentBossHealth / boss.maxHealth * 500, 100);
                    }
                }

                // �������ε� ���� ��� óġ�ߴٸ� ���� ����
                if (!isClear && isBattle)
                {
                    if (enemyCount <= 0) BattleEnd(Random.Range(0, 2));
                }
            }

            // ��ư�� ������ �����̵��
            else if (roomType == RoomType.Arcade)
            {
                // �����̵� �濡�� ����� ���� ���� �� �ִ�.
                if (!isClear && !isBattle) BattleStart();

                // ��ư�� ��� ������ �� Ŭ����
                if (!isClear && IsAllButtonPressed()) BattleEnd(Random.Range(0, 2));
            }

            // �����
            else if (roomType == RoomType.Quiz && !isClear)
            {
                // ��� �Ұų�? �켱 �� ��ȹ��...
                // 1. ������ �Ⱥ��������� ���� �帴�帴�ϰ� ������ ����.
                // 2. OX ����� ������ O, �������� X ��ư�� ������ �Ѵ�.
                // 3. ������ ���߸� ������, Ʋ���� ��Ʋ�� �ϰ� �Ѵ�.

                // ���� UI ���� ON
                if(!isQuizSet)
                {
                    quizAnswer = SetQuiz();
                }

                if (!isSelectAnswer && (buttons[0].isPressed || buttons[1].isPressed))
                {
                    isSelectAnswer = true;
                    StartCoroutine(OpenQuiz());
                }
            }

            // �����濡 ���� ����� ������ ���� �ٽ� Ȱ��ȭ
            else if (roomType == RoomType.Shop && Map.instance.isItemSet)
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
        // �� UI���� ���� ��ġ�� ���� ������ 1, �湮�ߴ� ���� 0.75, �ֺ��� �ִ� ���� 0.25�� ����
        // ����� ���� ����� 1��°, ���� �ϴ��� 81��°. ���� ������ ��->��, ��->��

        // ���� ���� ��ġ�� ���ϰ� �湮 ǥ��
        Map.instance.mapSquare[Map.instance.mapPosition].color = new Color(1, 1, 1, 1);

        // �ֺ� �� ��ĥ�ϱ�
        if (leftRoom)   ColorMap(leftRoom, 9);
        if (rightRoom)  ColorMap(rightRoom, -9);
        if (upRoom)     ColorMap(upRoom, -1);
        if (downRoom)   ColorMap(downRoom, 1);

        isMapDraw = true;
    }

    void ColorMap(Room directionRoom, int roomIndex)
    {
        int mapPosition = Map.instance.mapPosition;
        mapPosition += roomIndex;

        // �ֺ� ���� �湮�ߴ����� �ƴϸ� ���� 0.25, �湮 �߾����� 0.75
        Map.instance.mapSquare[mapPosition].color = !directionRoom.isVisited ? new Color(1, 1, 1, 0.25f) : new Color(1, 1, 1, 0.75f);

        // Ư�����̶�� ������ �߰�
        if (directionRoom.roomType == RoomType.Boss || directionRoom.roomType == RoomType.Golden || directionRoom.roomType == RoomType.Shop)
        {
            SpecialRoom(directionRoom, Map.instance.mapSquare[mapPosition]);
        }
    }

    void SpecialRoom(Room room, Image mapSqure)
    {
        Image[] roomIcon = mapSqure.GetComponentsInChildren<Image>(true);
        roomIcon[1].gameObject.SetActive(true);

        if (room.roomType == RoomType.Boss)
        {
            roomIcon[1].sprite = GameManager.instance.roomIcon[0];
        }
        else if (room.roomType == RoomType.Golden)
        {
            roomIcon[1].sprite = GameManager.instance.roomIcon[1];
        }
        else if (room.roomType == RoomType.Shop)
        {
            roomIcon[1].sprite = GameManager.instance.roomIcon[2];
        }
    }

    void InitEnemies()
    {
        // ��ȯ ������ �ִٸ� ���� ��ȯ
        if (spawnPoint.Length > 0)
        {
            for (int i = 0; i < spawnPoint.Length; i++)
            {
                Enemy spawnEnemy = Instantiate(spawnPoint[i].enemy, spawnPoint[i].transform);
                spawnEnemy.Init(GameManager.instance.enemyData[spawnPoint[i].enemy.id]);
                enemyCount++;
            }
        }
    }

    void BattleStart()
    {
        isBattle = true;
        InitEnemies();
    }

    void BattleEnd(int successOrFail)
    {
        if(roomType == RoomType.Boss)
        {
            // ��Ż ����
            GameObject portal = GameManager.instance.objectPool.Get(4);
            portal.transform.position = transform.position;

            // ���� ������ ����
            // ��ġ: ¦���� -1, 1, -2, 2, -3, 3 .... / Ȧ���� 0, -1, 1, -2, 2 ....
            //Vector3 itemStartPosition = Vector3.left * (bossDropItems.Length % 2 == 0 ? bossDropItems.Length / 2 + 0.5f : (int)(bossDropItems.Length / 2));

            // ��¥�� ������ 2���ϱ� ��ġ�� ���ڷ� �ھƳ���
            Vector3 itemStartPosition = Vector3.left * 2;
            for (int i = 0; i < bossDropItems.Length; i++)
            {
                Item item = Instantiate(bossDropItems[i], transform);
                item.transform.position = transform.position + Vector3.down * 2 + itemStartPosition + Vector3.right * i * 4;
                item.Init(bossDropItemDatas[i]);
            }
        }
        else
        {
            // ���� ȹ�� (0: ����, 1: ����)
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
                // 0: ��, 1: 1��, 2: ��Ʈ ����, 3: ������ ��Ʈ
                int randomObject = Random.Range(0, 4);
                if (randomObject != 0)
                {
                    reward = Instantiate(GameManager.instance.objectPool.prefabs[randomObject], roomReward.transform);
                    reward.transform.position = transform.position + pos;
                }
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

    public void DeleteItem()
    {
        Item[] rewardItems = GetComponentsInChildren<Item>();
        foreach(Item item in rewardItems)
        {
            Destroy(item.gameObject);
        }
    }

    public bool SetQuiz()
    {
        // ���� ���� �κ��� �� ó�� �ϰ�ʹ�
        // �׷��� standard assets�� �ٿ�޾Ƽ� ��ó�� �ϸ� �ȴٰ� �Ѵ�.
        // standard assets - effects - imageEffects �� ���� ���� �ִٰ� �Ѵ�. �ٵ� �ôµ� ����.
        // �ű��ִ� reademe�� �о�� deprecated �Ǿ��ٰ� �Ѵ�... post-processing �� ����϶�� �Ѵ�...


        // ���� UI ���� �ϰ�
        Canvas canvas = GetComponentInChildren<Canvas>(true);
        canvas.gameObject.SetActive(true);

        // �������� ���� ��� �ؽ�Ʈ�� �� ����
        Image quizBox = GetComponentInChildren<Image>();
        quizText = quizBox.GetComponentInChildren<Text>();
        int random = Random.Range(0, GameManager.instance.quizList.Count);
        KeyValuePair<string, bool> quiz = GameManager.instance.quizList.ElementAt(random);
        quizText.text = quiz.Key;

        isQuizSet = true;

        return quiz.Value;
    }

    IEnumerator OpenQuiz()
    {
        quizText.material = null;

        yield return new WaitForSeconds(3);

        // ���� UI ��Ȱ��ȭ
        GetComponentInChildren<Canvas>().gameObject.SetActive(false);

        if (buttons[0].isPressed)
        {
            // O�� �����߰�, ���� O��� ����
            if (quizAnswer) BattleEnd(0);
            // ���� X��� ����
            else BattleEnd(1);
        }
        else if (buttons[1].isPressed)
        {
            // X�� �����߰�, ���� O��� ����
            // ���� X��� ����
            if (quizAnswer) BattleEnd(1);
            else BattleEnd(0);
        }
    }
}
