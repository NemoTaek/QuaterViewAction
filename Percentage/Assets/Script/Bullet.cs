using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int id;
    public int type;
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
        if (collision.CompareTag("Wall") || (!isPenetrate && collision.CompareTag("Object")) || collision.CompareTag("Enemy"))
        {
            if (gameObject.CompareTag("Bullet")) gameObject.SetActive(false);
            else if (gameObject.CompareTag("SkillBullet")) StartCoroutine(SetActiveFalseBullet());
            transform.position = transform.parent.position;
        }
    }

    IEnumerator SetActiveFalseBullet()
    {
        Debug.Log("나는 스킬불렛");
        yield return new WaitForSeconds(10);
        if (gameObject.activeSelf) gameObject.SetActive(false);
    }

    void Update()
    {
        
    }
}
