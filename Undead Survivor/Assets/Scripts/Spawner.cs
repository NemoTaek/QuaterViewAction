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

    // ���� 8�������� ��������
    // 1, 1+2, 2, 2+3, 3, 3+4, 4, 1+2+3+4 -> 8����
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

        // ���� �ð��� ���� ���� ���� ����
        if ((int)GameManager.instance.gameTime == (int)levelTime * level)
        {
            level++;
        }

        SelectEnemy();

        // �� ���� �� ���� ��ȯ
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
        enemy.transform.position = spawnPoint[Random.Range(1, spawnPoint.Length)].position; // �ڱ� �ڽ��� �����Ͽ� �ڽ��� ���� ������ 1���� ���� (0: spawner, 1���ʹ� point)
        enemy.GetComponent<Enemy>().Init(spawnData[levelEnemyType]);
    }
}

// ����ȭ: ��ü�� ���� Ȥ�� �����ϱ� ���� ��ȯ
[System.Serializable]
public class SpawnData
{
    public int spriteType;
    public float spawnTime;
    public int health;
    public float speed;
}