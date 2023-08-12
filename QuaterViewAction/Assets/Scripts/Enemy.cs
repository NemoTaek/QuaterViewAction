using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    public enum EnemyType { A, B, C, D };   // D가 보스고 나머지는 일반. D에 대한 로직을 특별 관리
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
    public NavMeshAgent nav;   // 타겟을 자동으로 따라가는 내비게이션 컴포넌트. 이를 사용하려면 UnityEngine.AI를 사용해야함.
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
            Vector3 reactVector = transform.position - other.transform.position;    // 넉백 방향
            StartCoroutine(OnDamage(reactVector, false));
        }
        else if(other.tag == "Bullet")
        {
            Bullet bullet = other.GetComponent<Bullet>();
            health -= bullet.damage;
            Vector3 reactVector = transform.position - other.transform.position;    // 넉백 방향
            Destroy(other.gameObject);  // 총알은 적에 맞았을 시 삭제
            StartCoroutine(OnDamage(reactVector, false));
        }
    }

    IEnumerator OnDamage(Vector3 reactVector, bool isGrenade)
    {
        // 피격시 색을 빨간색으로 변경 후 넉백
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

        // 체력이 남아있으면 0.1초 후에 다시 하얀색으로 변경
        if(health > 0)
        {
            foreach (MeshRenderer mesh in meshRenderer)
            {
                mesh.material.color = Color.white;
            }
        }
        // 체력이 0 이하면 회색으로 변경하고 4초후에 사라지도록 설정
        else 
        {
            foreach (MeshRenderer mesh in meshRenderer)
            {
                mesh.material.color = Color.gray;
            }
            gameObject.layer = 12;  // 죽은 적 레이어 번호

            isDead = true;
            if (type != EnemyType.D)
            {
                chaseStop();    // 죽었으니 그만 쫓아와
            }
            nav.enabled = false;    // 피격 효과를 유지하기 위해 네비게이션 비활성
            animator.SetTrigger("doDie");   // 죽는 애니메이션

            // 플레이어에게 점수 추가
            Player player = target.GetComponent<Player>();
            player.score += score;

            // 죽으면 동전 떨구고 카운트 감소
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

            Destroy(gameObject, 4); // 사라져
        }
    }

    public void HitByGrenade(Vector3 explosionPosition)
    {
        health -= 100;
        Vector3 reactVector = transform.position - explosionPosition;    // 넉백 방향
        StartCoroutine(OnDamage(reactVector, true));
    }

    void FreezeVelocity()
    {
        if(isChase)
        {
            // angularVelocity: 물리 회전 속도
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
        // 우선 멈춰
        isChase = false;

        // 공격 설정
        isAttack = true;
        animator.SetBool("isAttack", true);

        switch (type)
        {
            case EnemyType.A:
                yield return new WaitForSeconds(0.2f);  // 공격 모션 딜레이
                bulletArea.enabled = true;

                yield return new WaitForSeconds(1f);  // 공격 딜레이
                bulletArea.enabled = false;

                yield return new WaitForSeconds(1f);  // 공격 딜레이
                break;
            case EnemyType.B:
                yield return new WaitForSeconds(0.1f);  // 공격 모션 딜레이
                rigid.AddForce(transform.forward * 20, ForceMode.Impulse);
                bulletArea.enabled = true;

                yield return new WaitForSeconds(0.5f);  // 공격 딜레이
                rigid.velocity = Vector3.zero;
                bulletArea.enabled = false;

                yield return new WaitForSeconds(2f);  // 공격 딜레이
                break;
            case EnemyType.C:
                yield return new WaitForSeconds(0.5f);  // 공격 모션 딜레이
                GameObject instantEnemyBullet = Instantiate(enemyBullet, transform.position + Vector3.up * 2, transform.rotation);
                Rigidbody rigidBullet = instantEnemyBullet.GetComponent<Rigidbody>();
                rigidBullet.velocity = transform.forward * 20;

                yield return new WaitForSeconds(2f);  // 공격 딜레이
                break;
        }

        // 다시 원래대로
        isChase = true;
        isAttack = false;
        animator.SetBool("isAttack", false);
    }
}
