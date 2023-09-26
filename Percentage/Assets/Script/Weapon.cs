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
    public string desc; // ���� ����
    public Sprite icon; // ���� ������

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
        weaponName = data.weaponName;
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
        // bulletId
        // 2: ���̾, 3: �Ѿ�(�Ҹ���Ƽ), 4: ���̾��ο�, 5: �齺�ܼ�(��弦), 6: �������, 7: ���׿�, 8: ����, 9: �˱�, 10: ���丣�������, 11: �ϻ�
        GameObject bullet = GameManager.instance.objectPool.Get(bulletId);

        int playerRole = GameManager.instance.player.role;

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
