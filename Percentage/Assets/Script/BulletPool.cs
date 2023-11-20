using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public Bullet[] playerBullets;
    public Bullet[] enemyBullets;
    public Bullet[] familiarBullets;
    List<Bullet>[] playerBulletPools;
    List<Bullet>[] enemyBulletPools;
    List<Bullet>[] familiarBulletPools;

    void Awake()
    {
        playerBulletPools = new List<Bullet>[playerBullets.Length];
        enemyBulletPools = new List<Bullet>[enemyBullets.Length];
        familiarBulletPools = new List<Bullet>[enemyBullets.Length];

        for (int i = 0; i < playerBulletPools.Length; i++)
        {
            playerBulletPools[i] = new List<Bullet>();
        }
        for (int i = 0; i < enemyBulletPools.Length; i++)
        {
            enemyBulletPools[i] = new List<Bullet>();
        }
        for (int i = 0; i < familiarBulletPools.Length; i++)
        {
            familiarBulletPools[i] = new List<Bullet>();
        }
    }

    public Bullet Get(int type, int index)
    {
        List<Bullet>[] pools = new List<Bullet>[100000];
        Bullet[] bullets = new Bullet[100000];

        switch (type)
        {
            case 0:
                pools = playerBulletPools;
                bullets = playerBullets;
                break;
            case 1:
                pools = enemyBulletPools;
                bullets = enemyBullets;
                break;
            case 2:
                pools = familiarBulletPools;
                bullets = familiarBullets;
                break;
        }

        Bullet select = null;

        foreach (Bullet bullet in pools[index])
        {
            if (!bullet.gameObject.activeSelf)
            {
                select = bullet;
                select.gameObject.SetActive(true);
                break;
            }
        }

        if (!select)
        {
            select = Instantiate(bullets[index], transform);
            pools[index].Add(select);
        }

        return select;
    }
}
