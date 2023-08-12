using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;
    Vector3 lookVector;             // 플레이어가 가는 방향을 예측
    Vector3 tauntVector;            // 내려찍기를 할 위치
    public bool isLook = true;      // 플레이어를 바라보고 있는가

    void Awake()
    {
        // Awake() 함수는 상속을 받을 때, 자식스크립트에서만 단독 실행되기 때문에 부모에 있던 로직을 자식에서 작성해야 함
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();
        meshRenderer = GetComponentsInChildren<MeshRenderer>();
        nav = GetComponent<NavMeshAgent>();
        animator = GetComponentInChildren<Animator>();

        nav.isStopped = true;
        StartCoroutine(Think());
    }

    void Update()
    {
        if (isDead)
        {
            StopAllCoroutines();
            return;
        }

        if (isLook)
        {
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            lookVector = new Vector3 (horizontal, 0, vertical) * 5f;
            transform.LookAt(target.position + lookVector);
        }
        else
        {
            nav.SetDestination(tauntVector);
        }
    }

    IEnumerator Think()
    {
        yield return new WaitForSeconds(0.1f);

        // 패턴 0~4까지 랜덤으로 실행
        int randomAction = Random.Range(0, 5);
        
        // 미사일 발사
        if(randomAction <= 1)
        {
            StartCoroutine(MissileShot());
        }
        // 돌굴러가유
        else if(randomAction <= 3)
        {
            StartCoroutine(RockShot());
        }
        // 내려찍기
        else
        {
            StartCoroutine(TauntShot());
        }
    }

    IEnumerator MissileShot()
    {
        animator.SetTrigger("doShot");

        yield return new WaitForSeconds(0.2f);
        GameObject instantMissileA = Instantiate(missile, missilePortA.position, missilePortA.rotation);
        BossMissile bossMissileA = instantMissileA.GetComponent<BossMissile>();
        bossMissileA.target = target;

        yield return new WaitForSeconds(0.3f);
        GameObject instantMissileB = Instantiate(missile, missilePortB.position, missilePortB.rotation);
        BossMissile bossMissileB = instantMissileB.GetComponent<BossMissile>();
        bossMissileB.target = target;

        yield return new WaitForSeconds(2f);

        StartCoroutine(Think());
    }

    IEnumerator RockShot()
    {
        isLook = false;
        animator.SetTrigger("doBigShot");

        Instantiate(enemyBullet, transform.position, transform.rotation);
        yield return new WaitForSeconds(3f);

        isLook = true;
        StartCoroutine(Think());
    }

    IEnumerator TauntShot()
    {
        // 점프할 때는 시선 및 추적 해제, 플레이어와 충돌하지 않도록 설정
        nav.isStopped = false;
        isLook = false;
        boxCollider.enabled = false;
        tauntVector = target.position + lookVector;
        animator.SetTrigger("doTaunt");

        // 내려찍기 전
        yield return new WaitForSeconds(1.5f);
        bulletArea.enabled = true;

        // 내려찍기
        yield return new WaitForSeconds(0.5f);
        bulletArea.enabled = false;

        // 내려찍은 후
        yield return new WaitForSeconds(1f);
        nav.isStopped = true;
        isLook = true;
        boxCollider.enabled = true;

        StartCoroutine(Think());
    }
}
