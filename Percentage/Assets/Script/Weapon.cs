using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    Bullet bullet;
    public Animator animator;

    public string weaponName; // 무기 이름
    public float damage;    // 무기 데미지

    void Awake()
    {
        animator = GetComponentInParent<Animator>();
    }

    public void Init(string weaponName, float damage)
    {
        this.weaponName = weaponName;
        this.damage = damage;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Shot(Vector2 dirVec, Vector3 playerPosition)
    {
        bullet = GameManager.instance.bulletPool.Get(0);

        if(dirVec == Vector2.right)
        {
            bullet.transform.position = new Vector3(playerPosition.x + 0.5f, playerPosition.y, playerPosition.z);
            bullet.transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else if (dirVec == Vector2.left)
        {
            bullet.transform.position = new Vector3(playerPosition.x - 0.5f, playerPosition.y, playerPosition.z);
            bullet.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if(dirVec == Vector2.up)
        {
            bullet.transform.position = new Vector3(playerPosition.x, playerPosition.y + 0.5f, playerPosition.z);
            bullet.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (dirVec == Vector2.down)
        {
            bullet.transform.position = new Vector3(playerPosition.x, playerPosition.y - 0.5f, playerPosition.z);
            bullet.transform.rotation = Quaternion.Euler(0, 0, 180);
        }

        bullet.rigid.velocity = dirVec * 5;
    }
}
