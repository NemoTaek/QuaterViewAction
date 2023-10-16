using System.Collections;
using System.Collections.Generic;
using System.Data;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;  // ���� ���̵�
    public string weaponName; // ���� �̸�
    public float damage;    // ���� ���ݷ�
    public float[] upgradeDamage;    // ���� ���׷��̵� �� ����ϴ� ���ݷ�
    public int level;   // ���� ��ȭ ����
    public int maxLevel;    // ���� �ְ� ����
    public string desc; // ���� ����
    public Sprite icon; // ���� ������

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
        // ���� �⺻ ����
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

        // ���Ⱑ �ǰݹ����� ��Ƶ� �÷��̾ �ǰ� ó������ �ʵ���
        polygon = gameObject.AddComponent<PolygonCollider2D>();
        rigid = gameObject.AddComponent<Rigidbody2D>();
        rigid.bodyType = RigidbodyType2D.Kinematic;
        polygon.isTrigger = true;

        // ��õ�� �̹����� �� �׷��� ������� �����̰� �÷��̾� ������ ��...
        if(id == 13)    sr.flipX = true;

        // �������� �����ϰ� ��ġ�� �ٸ��� ������ ����
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
            // Atan2(y, x): y / x ����Ͽ� ���� ���� ����. �� �� ������ ��ȯ
            float shotDeg = Mathf.Atan2(dirVec.y, dirVec.x) * Mathf.Rad2Deg - 90;
            bullet.transform.position = new Vector3(shotPosition.x, shotPosition.y, shotPosition.z);
            bullet.transform.rotation = Quaternion.Euler(0, 0, shotDeg);
        }

        bullet.GetComponent<Rigidbody2D>().velocity = dirVec * shotVelocity;
    }
}
