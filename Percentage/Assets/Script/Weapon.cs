using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public Animator animator;

    public string weaponName; // 무기 이름
    public float damage;    // 무기 데미지
    public int level;   // 무기 강화 레벨

    void Awake()
    {
        animator = GetComponentInParent<Animator>();
    }

    public void Init(string weaponName, float damage, int level)
    {
        this.weaponName = weaponName;
        this.damage = damage;
        this.level = level;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Shot(Vector2 dirVec, Vector3 playerPosition)
    {
        int playerRole = GameManager.instance.player.role;
        GameObject bullet = playerRole == 1 ? GameManager.instance.ObjectPool.Get(0) : GameManager.instance.ObjectPool.Get(1);

        if(dirVec == Vector2.right)
        {
            bullet.transform.position = new Vector3(playerPosition.x + 0.5f, playerPosition.y, playerPosition.z);
            bullet.transform.rotation = playerRole == 1 ? Quaternion.Euler(0, 0, -90) : Quaternion.Euler(0, 0, 180);
        }
        else if (dirVec == Vector2.left)
        {
            bullet.transform.position = new Vector3(playerPosition.x - 0.5f, playerPosition.y, playerPosition.z);
            bullet.transform.rotation = playerRole == 1 ? Quaternion.Euler(0, 0, 90) : Quaternion.Euler(0, 0, 0);
        }
        else if(dirVec == Vector2.up)
        {
            bullet.transform.position = new Vector3(playerPosition.x, playerPosition.y + 0.5f, playerPosition.z);
            bullet.transform.rotation = playerRole == 1 ? Quaternion.Euler(0, 0, 0) : Quaternion.Euler(0, 0, -90);
        }
        else if (dirVec == Vector2.down)
        {
            bullet.transform.position = new Vector3(playerPosition.x, playerPosition.y - 0.5f, playerPosition.z);
            bullet.transform.rotation = playerRole == 1 ? Quaternion.Euler(0, 0, 180) : Quaternion.Euler(0, 0, 90);
        }

        bullet.GetComponent<Rigidbody2D>().velocity = dirVec * 5;
    }
}
