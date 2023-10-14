using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPool : MonoBehaviour
{
    public Bullet[] playerBullets;
    public Bullet[] enemyBullets;
    List<Bullet>[] playerBulletPools;
    List<Bullet>[] enemyBulletPools;

    void Awake()
    {
        playerBulletPools = new List<Bullet>[playerBullets.Length];
        enemyBulletPools = new List<Bullet>[enemyBullets.Length];

        for (int i = 0; i < playerBulletPools.Length; i++)
        {
            playerBulletPools[i] = new List<Bullet>();
        }
        for (int i = 0; i < enemyBulletPools.Length; i++)
        {
            enemyBulletPools[i] = new List<Bullet>();
        }
    }

    public Bullet Get(int type, int index)
    {
        List<Bullet>[] pools = type == 0 ? playerBulletPools : enemyBulletPools;
        Bullet[] bullets = type == 0 ? playerBullets : enemyBullets;
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
