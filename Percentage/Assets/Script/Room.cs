using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Enemy enemy;
    public Transform[] spawnPoint;
    public Door[] doors;
    public Room upRoom;
    public Room rightRoom;
    public Room downRoom;
    public Room leftRoom;

    public bool isArrive;
    public bool isBattle;
    public bool isClear;
    public int enemyCount = 0;

    void Awake()
    {
        doors = GetComponentsInChildren<Door>();
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
    }

    void Update()
    {
        // Ŭ������ ���� ���� �濡 �÷��̾ �����ϸ� ���� ����
        if(!isClear && isArrive)
        {
            BattleStart();
        }

        // �濡 ���� ���� ��
        if(!isClear && enemyCount == 0)
        {
            // �������̾��ٸ� ���� ���� ����
            if(isBattle)
            {
                BattleEnd();
            }

            // ���ʿ� ������ ���� �ʾҴٸ� �ٷ� �� ����
            else
            {
                DoorOpen();
            }
        }
    }

    void BattleStart()
    {
        isBattle = true;
        isArrive = false;

        // ���� ���� �� �� ���
        for (int i = 0; i < doors.Length; i++)
        {
            doors[i].gameObject.SetActive(false);
        }

        // ���� ��ȯ
        for (int i = 0; i < spawnPoint.Length; i++)
        {
            Instantiate(enemy, spawnPoint[i]);
            enemyCount++;
        }

        if(enemyCount == 0)
        {
            BattleEnd();
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

        isBattle = false;

        // ���� �� Ŭ����
        isClear = true;

        // ������ �����ٸ� ���������� ��ġ�� ���� ����
        DoorOpen();
    }

    void DoorOpen()
    {
        if (upRoom) doors[0].gameObject.SetActive(true);
        if (rightRoom) doors[1].gameObject.SetActive(true);
        if (downRoom) doors[2].gameObject.SetActive(true);
        if (leftRoom) doors[3].gameObject.SetActive(true);
    }
}
