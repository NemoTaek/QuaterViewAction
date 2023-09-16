using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Enemy enemy;
    public Transform[] spawnPoint;
    public Door[] doors;

    bool isBattle;
    public int enemyCount = 0;

    void Awake()
    {
        doors = GetComponentsInChildren<Door>();
    }

    void OnEnable()
    {
        //BattleStart();
    }

    void Start()
    {
        
    }

    void Update()
    {
        if(enemyCount == 0 && isBattle)
        {
            BattleEnd();
        }
    }

    void BattleStart()
    {
        isBattle = true;

        // 문 닫기
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
    }

    void BattleEnd()
    {
        // 문 열기
        for (int i = 0; i < doors.Length; i++)
        {
            doors[i].gameObject.SetActive(true);
        }

        // 보상 획득 (0: 성공, 1: 실패)
        int successOrFail = Random.Range(0, 1);
        if (successOrFail == 0)
        {
            GameObject box = GameManager.instance.ObjectPool.Get(3);
            box.transform.position = transform.position + Vector3.forward;
        }

        isBattle = false;
    }
}
