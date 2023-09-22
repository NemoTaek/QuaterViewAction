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
        // 레이어 마스크는 레이어를 비트로 판단하기 때문에 비트 연산자를 사용하여 작성해야 한한다.
        // 그냥 레이어 자리에 6 이렇게 쓰니까 안먹히더라..
        // 특정 레이어를 제외하려면 ~(1 << 레이어) 이렇게 사용하면 된다.
        //LayerMask layer = (1 << LayerMask.NameToLayer("Room"));
        //colliders = Physics2D.OverlapBoxAll(transform.position, new Vector2(25, 15), 0, layer);

        // 근데 대각선은 굳이 알아낼 필요가 없다...
        // 이번엔 레이캐스트로 상하좌우만 쏴서 검출해보도록 하자
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
        // 클리어한 적이 없는 방에 플레이어가 도착하면 전투 시작
        if(!isClear && isArrive)
        {
            BattleStart();
        }

        // 방에 적이 없을 때
        if(!isClear && enemyCount == 0)
        {
            // 전투중이었다면 전투 종료 로직
            if(isBattle)
            {
                BattleEnd();
            }

            // 애초에 전투를 하지 않았다면 바로 문 열기
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

        // 전투 시작 시 문 폐쇄
        for (int i = 0; i < doors.Length; i++)
        {
            doors[i].gameObject.SetActive(false);
        }

        // 몬스터 소환
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
        // 보상 획득 (0: 성공, 1: 실패)
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

        // 현재 방 클리어
        isClear = true;

        // 전투가 끝났다면 감지되지는 위치의 문을 오픈
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
