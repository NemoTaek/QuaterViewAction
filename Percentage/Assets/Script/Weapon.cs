using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;  // 무기 아이디
    public string weaponName; // 무기 이름
    public float damage;    // 무기 공격력
    public float[] upgradeDamage;    // 무기 업그레이드 시 상승하는 공격력
    public int level;   // 무기 강화 레벨
    public int maxLevel;    // 무기 최고 레벨
    public string desc; // 무기 설명
    public Sprite icon; // 무기 아이콘

    SpriteRenderer sr;
    PolygonCollider2D polygon;
    Rigidbody2D rigid;

    public bool isPenetrate;
    public bool isSlow;

    void Awake()
    {

    }

    public void Init(WeaponData data)
    {
        // 무기 기본 세팅
        id = data.weaponId;
        weaponName = data.weaponName;
        damage = data.baseDamage;
        upgradeDamage = data.upgradeDamage;
        level = data.level;
        maxLevel = data.maxLevel;
        desc = data.weaponDesc;
        icon = data.weaponIcon;

        transform.parent = GameManager.instance.player.hand[(int)data.weaponType].transform;
        sr = gameObject.AddComponent<SpriteRenderer>();
        sr.sprite = icon;
        sr.sortingOrder = -1;

        // 무기가 피격범위에 닿아도 플레이어가 피격 처리되지 않도록
        polygon = gameObject.AddComponent<PolygonCollider2D>();
        rigid = gameObject.AddComponent<Rigidbody2D>();
        rigid.bodyType = RigidbodyType2D.Kinematic;
        polygon.isTrigger = true;

        // 용천권 이미지가 좀 그래서 뒤집어야 손잡이가 플레이어 쪽으로 옴...
        if(id == 13)    sr.flipX = true;

        // 직업별로 세세하게 위치가 다르기 때문에 설정
        transform.localScale = Vector3.one * 3;
        if (GameManager.instance.player.role == 0 || GameManager.instance.player.role == 1)
        {
            transform.localPosition = new Vector3(0.2f, 0.3f, 0);
            transform.localRotation = Quaternion.Euler(0, 180, -90);
        }
        else if (GameManager.instance.player.role == 2 || GameManager.instance.player.role == 3)
        {
            transform.localPosition = new Vector3(0.3f, -0.1f, 0);
            transform.localRotation = Quaternion.Euler(0, 180, -45);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void Shot(int bulletId, Vector2 dirVec, Vector3 shotPosition, float shotVelocity)
    {
        Bullet bullet = GameManager.instance.bulletPool.Get(0, bulletId).GetComponent<Bullet>();

        if (dirVec == Vector2.right)
        {
            bullet.transform.position = new Vector3(shotPosition.x + 0.5f, shotPosition.y, shotPosition.z);
            bullet.transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else if (dirVec == Vector2.left)
        {
            bullet.transform.position = new Vector3(shotPosition.x - 0.5f, shotPosition.y, shotPosition.z);
            bullet.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (dirVec == Vector2.up)
        {
            bullet.transform.position = new Vector3(shotPosition.x, shotPosition.y + 0.5f, shotPosition.z);
            bullet.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (dirVec == Vector2.down)
        {
            bullet.transform.position = new Vector3(shotPosition.x, shotPosition.y - 0.5f, shotPosition.z);
            bullet.transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else
        {
            // Atan2(y, x): y / x 계산하여 라디안 값을 리턴. 그 후 각도로 변환
            float shotDeg = Mathf.Atan2(dirVec.y, dirVec.x) * Mathf.Rad2Deg - 90;
            bullet.transform.position = new Vector3(shotPosition.x, shotPosition.y, shotPosition.z);
            bullet.transform.rotation = Quaternion.Euler(0, 0, shotDeg);
        }

        bullet.GetComponent<Rigidbody2D>().velocity = dirVec * shotVelocity;
    }
}
