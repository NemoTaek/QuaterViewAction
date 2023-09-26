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
        // bulletId
        // 2: 파이어볼, 3: 총알(불릿파티), 4: 파이어블로우, 5: 백스텝샷(헤드샷), 6: 가드어택, 7: 메테오, 8: 지뢰, 9: 검기, 10: 인페르노라이즈, 11: 암살
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
            // Atan2(y, x): y / x 계산하여 라디안 값을 리턴. 그 후 각도로 변환
            float shotDeg = Mathf.Atan2(dirVec.y, dirVec.x) * Mathf.Rad2Deg - 90;
            bullet.transform.position = new Vector3(shotPosition.x, shotPosition.y, shotPosition.z);
            bullet.transform.rotation = Quaternion.Euler(0, 0, shotDeg);
        }

        bullet.GetComponent<Rigidbody2D>().velocity = dirVec * shotVelocity;
    }
}
