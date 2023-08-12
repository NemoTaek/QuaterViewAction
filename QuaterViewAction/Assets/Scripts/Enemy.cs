using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum EnemyType { A, B, C, D };   // D�� ������ �������� �Ϲ�. D�� ���� ������ Ư�� ����
    public EnemyType type;
    public int maxHealth;
    public int health;
    public int score;

    public Transform target;
    public bool isChase;
    public BoxCollider bulletArea;
    public GameObject enemyBullet;
    public GameObject[] coin;
    public bool isAttack;
    public bool isDead;

    public Rigidbody rigid;
    public BoxCollider boxCollider;
    public MeshRenderer[] meshRenderer;
    public NavMeshAgent nav;   // Ÿ���� �ڵ����� ���󰡴� ������̼� ������Ʈ. �̸� ����Ϸ��� UnityEngine.AI�� ����ؾ���.
    public Animator animator;
    public GameManager manager;

    void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshRenderer = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        nav.isStopped = true;

        if (type != EnemyType.D)
        {
            Invoke("ChaseStart", 2);
        }
    }

    void Start()
    {

    }

    void Update()
    {
        if (nav.enabled && type != EnemyType.D)
        {
            nav.SetDestination(target.position);
            nav.isStopped = !isChase;
        }

    }

    void FixedUpdate()
    {
        Targeting();
        FreezeVelocity();
    }

    void OnTriggerEnter(Collider other)
    {
        
        if (other.tag == "Melee")
        {
            Weapon weapon = other.GetComponent<Weapon>();
            health -= weapon.damage;
            Vector3 reactVector = transform.position - other.transform.position;    // �˹� ����
            StartCoroutine(OnDamage(reactVector, false));
        }
        else if(other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            health -= bullet.damage;
            Vector3 reactVector = transform.position - other.transform.position;    // �˹� ����
            Destroy(other.gameObject);  // �Ѿ��� ���� �¾��� �� ����
            StartCoroutine(OnDamage(reactVector, false));
        }
    }

    IEnumerator OnDamage(Vector3 reactVector, bool isGrenade)
    {
        // �ǰݽ� ���� ���������� ���� �� �˹�
        foreach (MeshRenderer mesh in meshRenderer)
        {
            mesh.material.color = Color.red;
        }

        reactVector = reactVector.normalized;
        reactVector += Vector3.up;

        if (isGrenade)
        {
            rigid.freezeRotation = false;
            rigid.AddForce(reactVector * 10, ForceMode.Impulse);
            rigid.AddTorque(reactVector * 15, ForceMode.Impulse);
        }
        else
        {
            rigid.AddForce(reactVector * 5, ForceMode.Impulse);
        }
        

        yield return new WaitForSeconds(0.1f);

        // ü���� ���������� 0.1�� �Ŀ� �ٽ� �Ͼ������ ����
        if(health > 0)
        {
            foreach (MeshRenderer mesh in meshRenderer)
            {
                mesh.material.color = Color.white;
            }
        }
        // ü���� 0 ���ϸ� ȸ������ �����ϰ� 4���Ŀ� ��������� ����
        else 
        {
            foreach (MeshRenderer mesh in meshRenderer)
            {
                mesh.material.color = Color.gray;
            }
            gameObject.layer = 12;  // ���� �� ���̾� ��ȣ

            isDead = true;
            if (type != EnemyType.D)
            {
                chaseStop();    // �׾����� �׸� �Ѿƿ�
            }
            nav.enabled = false;    // �ǰ� ȿ���� �����ϱ� ���� �׺���̼� ��Ȱ��
            animator.SetTrigger("doDie");   // �״� �ִϸ��̼�

            // �÷��̾�� ���� �߰�
            Player player = target.GetComponent<Player>();
            player.score += score;

            // ������ ���� ������ ī��Ʈ ����
            if (type == EnemyType.D)
            {
                for(int i=0; i<5; i++)
                {
                    int randomCoin = Random.Range(0, 3);
                    float spreadRandom = Random.Range(-10, 10);
                    Instantiate(coin[randomCoin], transform.position + new Vector3(1, 0, 1) * spreadRandom, Quaternion.identity);
                }
            }
            else
            {
                int randomCoin = Random.Range(0, 3);
                Instantiate(coin[randomCoin], transform.position, Quaternion.identity);

                switch(type)
                {
                    case EnemyType.A:
                        manager.remainEnemyACount--;
                        break;
                    case EnemyType.B:
                        manager.remainEnemyBCount--;
                        break;
                    case EnemyType.C:
                        manager.remainEnemyCCount--;
                        break;
                }
            }

            Destroy(gameObject, 4); // �����
        }
    }

    public void HitByGrenade(Vector3 explosionPosition)
    {
        health -= 100;
        Vector3 reactVector = transform.position - explosionPosition;    // �˹� ����
        StartCoroutine(OnDamage(reactVector, true));
    }

    void FreezeVelocity()
    {
        if(isChase)
        {
            // angularVelocity: ���� ȸ�� �ӵ�
            rigid.angularVelocity = Vector3.zero;
            rigid.velocity = Vector3.zero;
        }
    }

    void ChaseStart()
    {
        isChase = true;
        animator.SetBool("isWalk", true);
    }

    void chaseStop()
    {
        isChase = false;
        animator.SetBool("isWalk", false);
    }

    void Targeting()
    {
        if (isDead || type == EnemyType.D) return;

        float targetRadius = 0;
        float targetRange = 0;

        switch(type)
        {
            case EnemyType.A:
                targetRadius = 1.5f;
                targetRange = 3f;
                break;
            case EnemyType.B:
                targetRadius = 1f;
                targetRange = 12f;
                break;
            case EnemyType.C:
                targetRadius = 0.5f;
                targetRange = 25f;
                break;
        }

        RaycastHit[] rayHits = Physics.SphereCastAll(transform.position, targetRadius, transform.forward, targetRange, LayerMask.GetMask("Player"));
        if(rayHits.Length > 0 && !isAttack)
        {
            StartCoroutine(Attack());
        }
    }

    IEnumerator Attack()
    {
        // �켱 ����
        isChase = false;

        // ���� ����
        isAttack = true;
        animator.SetBool("isAttack", true);

        switch (type)
        {
            case EnemyType.A:
                yield return new WaitForSeconds(0.2f);  // ���� ��� ������
                bulletArea.enabled = true;

                yield return new WaitForSeconds(1f);  // ���� ������
                bulletArea.enabled = false;

                yield return new WaitForSeconds(1f);  // ���� ������
                break;
            case EnemyType.B:
                yield return new WaitForSeconds(0.1f);  // ���� ��� ������
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                bulletArea.enabled = true;

                yield return new WaitForSeconds(0.5f);  // ���� ������
                rigid.velocity = Vector3.zero;
                bulletArea.enabled = false;

                yield return new WaitForSeconds(2f);  // ���� ������
                break;
            case EnemyType.C:
                yield return new WaitForSeconds(0.5f);  // ���� ��� ������
                GameObject instantEnemyBullet = Instantiate(enemyBullet, transform.position + Vector3.up * 2, transform.rotation);
                Rigidbody rigidBullet = instantEnemyBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;

                yield return new WaitForSeconds(2f);  // ���� ������
                break;
        }

        // �ٽ� �������
        isChase = true;
        isAttack = false;
        animator.SetBool("isAttack", false);
    }
}
