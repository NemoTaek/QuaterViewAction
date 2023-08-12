using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    Rigidbody2D rigid;

    public float damage;
    public int penetrate;   // 관통이라는 뜻
    Vector3 shotDirection;
    float throwVelocity;

    float timer;
    bool isSpear = false;
    bool isBoomerang = false;
    float boomerangSpeed;
    bool isShotGun = false;

    public void Init(float damage, int penetrate)
    {
        this.damage = damage;
        this.penetrate = penetrate;
    }
    public void Init(float damage, int penetrate, Vector3 shotDirection)
    {
        this.damage = damage;
        this.penetrate = penetrate;

        // 관통이 -1인 무기는 근접 무기이므로 그보다 큰것은 원거리 무기
        if (penetrate > -1)
        {
            rigid.velocity = shotDirection * 15f;
        }
    }

    public void Init(int weaponType, float damage, int penetrate, Vector3 shotDirection, float throwVelocity)
    {
        if(weaponType == 5)
        {
            isSpear = true;
        }
        else if (weaponType == 6)
        {
            isBoomerang = true;
            boomerangSpeed = throwVelocity;
        }
        else if (weaponType == 8)
        {
            isShotGun = true;
        }

        this.damage = damage;
        this.penetrate = penetrate;
        this.shotDirection = shotDirection;
        this.throwVelocity = throwVelocity;     // 샷건일 때는 발사 거리

        if(weaponType != 8)
        {
            rigid.velocity = shotDirection * throwVelocity;
        }
        else
        {
            transform.localScale = new Vector3(2, throwVelocity, 1);
            transform.position = GameManager.instance.player.transform.position + shotDirection;
        }
    }

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy") && penetrate > -100)
        {
            // 관통하면 관통력 감소
            penetrate--;

            // 재한 관통을 다 했으면 물리 초기화 및 비활성화
            if(penetrate < 0)
            {
                rigid.velocity = Vector2.zero;
                gameObject.SetActive(false);
            }
        }
    }

    // 플레이어 영역을 벗어나는 총알은 비활성화
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Area") && penetrate > -100)
        {
            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if(isSpear)
        {
            timer += Time.deltaTime;
            if(timer > 0.5f)
            {
                rigid.velocity = Vector2.zero;
                gameObject.SetActive(false);
                timer = 0;
            }
        }
        else if(isBoomerang)
        {
            timer += Time.deltaTime;
            boomerangSpeed -= throwVelocity * Time.deltaTime;
            rigid.velocity = shotDirection * boomerangSpeed;
            transform.Rotate(Vector3.forward * Time.deltaTime * 180);   // 매 프레임마다 10도 회전
            if (timer > 2f)
            {
                rigid.velocity = Vector2.zero;
                gameObject.SetActive(false);
                timer = 0;
            }
        }
        if (isShotGun)
        {
            timer += Time.deltaTime;
            if (timer > 0.2f)
            {
                gameObject.SetActive(false);
                timer = 0;
            }
        }
    }
}
