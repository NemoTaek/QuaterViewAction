using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;  // 무기 아이디
    public string weaponNname; // 무기 이름
    public float damage;    // 무기 공격력
    public float[] upgradeDamage;    // 무기 업그레이드 시 상승하는 공격력
    public int level;   // 무기 강화 레벨
    public string desc; // 무기 설명
    public Sprite icon; // 무기 아이콘

    SpriteRenderer spriteRenderer;
    BoxCollider2D col;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<BoxCollider2D>();
    }

    public void Init(WeaponData data)
    {
        id = data.weaponId;
        weaponNname = data.weaponName;
        damage = data.baseDamage;
        upgradeDamage = data.upgradeDamage;
        level = data.level;
        desc = data.weaponDesc;
        icon = data.weaponIcon;

        transform.parent = GameManager.instance.player.hand[(int)data.weaponType].transform;
        spriteRenderer.sprite = icon;
        spriteRenderer.sortingOrder = -1;
        col.isTrigger = true;
        col.size = new Vector2(0.31f, 0.26f);

        // 직업별로 세세하게 위치가 다르기 때문에 설정
        transform.localPosition = new Vector3(0.2f, 0.3f, 0);
        transform.localScale = Vector3.one * 3;
        transform.localRotation = Quaternion.Euler(0, 0, 225);
        
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

    public void SkillFire(Vector2 dirVec, Vector3 playerPosition)
    {
        int playerRole = GameManager.instance.player.role;
        GameObject bullet = playerRole == 1 ? GameManager.instance.ObjectPool.Get(4) : GameManager.instance.ObjectPool.Get(1);

        if (dirVec == Vector2.right)
        {
            bullet.transform.position = new Vector3(playerPosition.x + 0.5f, playerPosition.y, playerPosition.z);
            bullet.transform.rotation = playerRole == 1 ? Quaternion.Euler(0, 0, -90) : Quaternion.Euler(0, 0, 180);
        }
        else if (dirVec == Vector2.left)
        {
            bullet.transform.position = new Vector3(playerPosition.x - 0.5f, playerPosition.y, playerPosition.z);
            bullet.transform.rotation = playerRole == 1 ? Quaternion.Euler(0, 0, 90) : Quaternion.Euler(0, 0, 0);
        }
        else if (dirVec == Vector2.up)
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
