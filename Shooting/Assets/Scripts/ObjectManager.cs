using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    public GameObject enemyLargePrefab;
    public GameObject enemyMediumPrefab;
    public GameObject enemySmallPrefab;
    public GameObject bossPrefab;
    GameObject[] enemyLarge;
    GameObject[] enemyMedium;
    GameObject[] enemySmall;
    GameObject[] boss;

    public GameObject itemCoinPrefab;
    public GameObject itemPowerPrefab;
    public GameObject itemBoomPrefab;
    GameObject[] itemCoin;
    GameObject[] itemPower;
    GameObject[] itemBoom;

    public GameObject playerNormalBulletPrefab;
    public GameObject playerPowerBulletPrefab;
    public GameObject enemyNormalBulletPrefab;
    public GameObject enemyLargeBulletPrefab;
    public GameObject followerBulletPrefab;
    public GameObject bossNormalBulletPrefab;
    public GameObject bossPowerBulletPrefab;
    GameObject[] playerNormalBullet;
    GameObject[] playerPowerBullet;
    GameObject[] enemyNormalBullet;
    GameObject[] enemyLargeBullet;
    GameObject[] followerBullet;
    GameObject[] bossNormalBullet;
    GameObject[] bossPowerBullet;

    public GameObject explosionPrefab;
    GameObject[] explosion;

    GameObject[] targetPool;

    // 게임 첫 로딩 시간 = 장면 배치 + 오브젝트 풀 생성
    // 오브젝트 풀링: 미리 생성해둔 오브젝트 풀에서 활성화/비활성화 하면서 사용하는 게임 최적화 방법
    void Awake()
    {
        enemyLarge = new GameObject[10];
        enemyMedium = new GameObject[10];
        enemySmall = new GameObject[20];
        boss = new GameObject[1];

        itemCoin = new GameObject[20];
        itemPower = new GameObject[10];
        itemBoom = new GameObject[10];

        playerNormalBullet = new GameObject[100];
        playerPowerBullet = new GameObject[100];
        enemyNormalBullet = new GameObject[100];
        enemyLargeBullet = new GameObject[100];
        followerBullet = new GameObject[100];
        bossNormalBullet = new GameObject[1000];
        bossPowerBullet = new GameObject[1000];

        explosion = new GameObject[100];

        Generate();
    }

    void Generate()
    {
        // 적
        for(int i=0; i<enemyLarge.Length; i++)
        {
            // Instantiate(object): 매개변수 오브젝트를 생성하는 함수
            // Instantiate(object, position, rotation): 무엇을 어디에서 어느 방향으로 생성할 것인지 설정
            enemyLarge[i] = Instantiate(enemyLargePrefab);
            enemyLarge[i].SetActive(false);
        }
        for (int i = 0; i < enemyMedium.Length; i++)
        {
            enemyMedium[i] = Instantiate(enemyMediumPrefab);
            enemyMedium[i].SetActive(false);
        }
        for (int i = 0; i < enemySmall.Length; i++)
        {
            enemySmall[i] = Instantiate(enemySmallPrefab);
            enemySmall[i].SetActive(false);
        }
        for (int i = 0; i < boss.Length; i++)
        {
            boss[i] = Instantiate(bossPrefab);
            boss[i].SetActive(false);
        }

        // 아이템
        for (int i = 0; i < itemCoin.Length; i++)
        {
            itemCoin[i] = Instantiate(itemCoinPrefab);
            itemCoin[i].SetActive(false);
        }
        for (int i = 0; i < itemPower.Length; i++)
        {
            itemPower[i] = Instantiate(itemPowerPrefab);
            itemPower[i].SetActive(false);
        }
        for (int i = 0; i < itemBoom.Length; i++)
        {
            itemBoom[i] = Instantiate(itemBoomPrefab);
            itemBoom[i].SetActive(false);
        }

        // 총알
        for (int i = 0; i < playerNormalBullet.Length; i++)
        {
            playerNormalBullet[i] = Instantiate(playerNormalBulletPrefab);
            playerNormalBullet[i].SetActive(false);
        }
        for (int i = 0; i < playerPowerBullet.Length; i++)
        {
            playerPowerBullet[i] = Instantiate(playerPowerBulletPrefab);
            playerPowerBullet[i].SetActive(false);
        }
        for (int i = 0; i < enemyNormalBullet.Length; i++)
        {
            enemyNormalBullet[i] = Instantiate(enemyNormalBulletPrefab);
            enemyNormalBullet[i].SetActive(false);
        }
        for (int i = 0; i < enemyLargeBullet.Length; i++)
        {
            enemyLargeBullet[i] = Instantiate(enemyLargeBulletPrefab);
            enemyLargeBullet[i].SetActive(false);
        }
        for (int i = 0; i < followerBullet.Length; i++)
        {
            followerBullet[i] = Instantiate(followerBulletPrefab);
            followerBullet[i].SetActive(false);
        }
        for (int i = 0; i < bossNormalBullet.Length; i++)
        {
            bossNormalBullet[i] = Instantiate(bossNormalBulletPrefab);
            bossNormalBullet[i].SetActive(false);
        }
        for (int i = 0; i < bossPowerBullet.Length; i++)
        {
            bossPowerBullet[i] = Instantiate(bossPowerBulletPrefab);
            bossPowerBullet[i].SetActive(false);
        }

        for (int i = 0; i < explosion.Length; i++)
        {
            explosion[i] = Instantiate(explosionPrefab);
            explosion[i].SetActive(false);
        }
    }

    public GameObject GetGameObject(string type)
    {
        SetPool(type);

        for (int i = 0; i < targetPool.Length; i++)
        {
            if (!targetPool[i].activeSelf)
            {
                targetPool[i].SetActive(true);
                return targetPool[i];
            }
        }

        return null;
    }

    public GameObject[] GetPool(string type)
    {
        SetPool(type);

        return targetPool;
    }

    public void SetPool(string type)
    {
        switch (type)
        {
            case "enemyLarge":
                targetPool = enemyLarge;
                break;
            case "enemyMedium":
                targetPool = enemyMedium;
                break;
            case "enemySmall":
                targetPool = enemySmall;
                break;
            case "boss":
                targetPool = boss;
                break;

            case "itemCoin":
                targetPool = itemCoin;
                break;
            case "itemPower":
                targetPool = itemPower;
                break;
            case "itemBoom":
                targetPool = itemBoom;
                break;

            case "playerNormalBullet":
                targetPool = playerNormalBullet;
                break;
            case "playerPowerBullet":
                targetPool = playerPowerBullet;
                break;
            case "enemyNormalBullet":
                targetPool = enemyNormalBullet;
                break;
            case "enemyLargeBullet":
                targetPool = enemyLargeBullet;
                break;
            case "followerBullet":
                targetPool = followerBullet;
                break;
            case "bossNormalBullet":
                targetPool = bossNormalBullet;
                break;
            case "bossPowerBullet":
                targetPool = bossPowerBullet;
                break;

            case "explosion":
                targetPool = explosion;
                break;
        }
    }
}
