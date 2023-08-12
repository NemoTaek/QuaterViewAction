using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    public SpawnData[] spawnData;
    public Transform[] spawnPoint;
    public float levelTime;
    float timer;
    public int level;
    int levelEnemyType;

    // 레벨 8구간으로 나눌거임
    // 1, 1+2, 2, 2+3, 3, 3+4, 4, 1+2+3+4 -> 8구간
    // levelTime = 1 -> level = 1
    // levelTime = 2 -> level = 1 || 2
    // levelTime = 3 -> level = 2
    // levelTime = 4 -> level = 2 || 3
    // levelTime = 5 -> level = 3
    // levelTime = 6 -> level = 3 || 4
    // levelTime = 7 -> level = 4
    // levelTime = 8 -> level = 1 || 2 || 3 || 4

    void Awake()
    {
        spawnPoint = GetComponentsInChildren<Transform>();
        levelTime = GameManager.instance.maxGameTime / 8;
        level = 1;
    }

    void Update()
    {
        if (!GameManager.instance.isTimePassing) return;

        // 게임 시간에 따른 레벨 구간 설정
        if ((int)GameManager.instance.gameTime == (int)levelTime * level)
        {
            level++;
        }

        SelectEnemy();

        // 각 레벨 별 몬스터 소환
        timer += Time.deltaTime;
        //level = Mathf.Min(Mathf.FloorToInt(GameManager.instance.gameTime / levelTime), spawnData.Length - 1);
        float spawnInterval = spawnData[levelEnemyType].spawnTime;
        if(timer > spawnInterval)
        {
            Spawn();
            timer = 0;
        }
    }

    void SelectEnemy()
    {
        switch (level)
        {
            case 1:
                levelEnemyType = 0;
                break;
            case 2:
                levelEnemyType = Random.Range(0, 2);
                break;
            case 3:
                levelEnemyType = 1;
                break;
            case 4:
                levelEnemyType = Random.Range(1, 3);
                break;
            case 5:
                levelEnemyType = 2;
                break;
            case 6:
                levelEnemyType = Random.Range(2, 4);
                break;
            case 7:
                levelEnemyType = 3;
                break;
            case 8:
                levelEnemyType = Random.Range(0, 4);
                break;
        }
    }

    void Spawn()
    {
        GameObject enemy = GameManager.instance.pool.GetPool(0);
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position; // 자기 자신을 포함하여 자식을 세기 때문에 1부터 시작 (0: spawner, 1부터는 point)
        enemy.GetComponent<Enemy>().Init(spawnData[levelEnemyType]);
    }
}

// 직렬화: 개체를 저장 혹은 전송하기 위해 변환
[System.Serializable]
public class SpawnData
{
    public int spriteType;
    public float spawnTime;
    public int health;
    public float speed;
}