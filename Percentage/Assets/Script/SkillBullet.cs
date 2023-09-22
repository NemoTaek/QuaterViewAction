using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBullet : MonoBehaviour
{
    public Rigidbody2D rigid;
    public Animator animator;
    public int id;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void OnEnable()
    {
        if (id == 8)
        {
            gameObject.transform.localScale = Vector3.one;
            StartCoroutine(LandMine());
        }
    }

    void Start()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Wall"))
        {
            gameObject.SetActive(false);
            transform.position = transform.parent.position;
        }

        if (collision.CompareTag("Enemy"))
        {
            if(id == 8)
            {
                StartCoroutine(ExplodeMine());
            }
        }
    }

    void Update()
    {

    }

    IEnumerator LandMine()
    {
        yield return new WaitForSeconds(5);
        if(gameObject.activeSelf) StartCoroutine(ExplodeMine());
    }

    IEnumerator ExplodeMine()
    {
        // ���� ���ڰ� ������� ���� ������ ������� �����Ƿ� �ϴ� �Ⱥ��̰� ����
        gameObject.transform.localScale = Vector3.zero;

        // ���� �ڸ��� ���� ����Ʈ ����
        GameObject explosion = GameManager.instance.objectPool.Get(12);
        explosion.transform.position = transform.position;

        // 0.5�� �� ����
        yield return new WaitForSeconds(0.5f);

        // �Ѵ� �����
        gameObject.SetActive(false);
        explosion.SetActive(false);
    }
}
