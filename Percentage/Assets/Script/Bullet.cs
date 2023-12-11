using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int id;
    public int type;
    public float damage;
    public bool isPenetrate;
    public bool isSlow;

    void Awake()
    {

    }

    void Start()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (id != 3)
        {
            if (collision.CompareTag("Wall") || (!isPenetrate && collision.CompareTag("Object")))
            {
                if (gameObject.CompareTag("Bullet") || gameObject.CompareTag("EnemyBullet") || gameObject.CompareTag("FamiliarBullet")) gameObject.SetActive(false);
                else if (gameObject.CompareTag("SkillBullet")) StartCoroutine(SetActiveFalseBullet());
            }
            if (collision.CompareTag("Enemy"))
            {
                if (gameObject.CompareTag("Bullet") || gameObject.CompareTag("FamiliarBullet")) gameObject.SetActive(false);
                else if (gameObject.CompareTag("SkillBullet")) StartCoroutine(SetActiveFalseBullet());
            }
        }
    }

    IEnumerator SetActiveFalseBullet()
    {
        yield return new WaitForSeconds(10);
        if (gameObject.activeSelf) gameObject.SetActive(false);
    }

    void Update()
    {
        
    }
}
