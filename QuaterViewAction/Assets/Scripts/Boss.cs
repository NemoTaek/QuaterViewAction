using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Boss : Enemy
{
    public GameObject missile;
    public Transform missilePortA;
    public Transform missilePortB;
    Vector3 lookVector;             // �÷��̾ ���� ������ ����
    Vector3 tauntVector;            // ������⸦ �� ��ġ
    public bool isLook = true;      // �÷��̾ �ٶ󺸰� �ִ°�

    void Awake()
    {
        // Awake() �Լ��� ����� ���� ��, �ڽĽ�ũ��Ʈ������ �ܵ� ����Ǳ� ������ �θ� �ִ� ������ �ڽĿ��� �ۼ��ؾ� ��
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

        // ���� 0~4���� �������� ����
        int randomAction = Random.Range(0, 5);
        
        // �̻��� �߻�
        if(randomAction <= 1)
        {
            StartCoroutine(MissileShot());
        }
        // ����������
        else if(randomAction <= 3)
        {
            StartCoroutine(RockShot());
        }
        // �������
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
        // ������ ���� �ü� �� ���� ����, �÷��̾�� �浹���� �ʵ��� ����
        nav.isStopped = false;
        isLook = false;
        boxCollider.enabled = false;
        tauntVector = target.position + lookVector;
        animator.SetTrigger("doTaunt");

        // ������� ��
        yield return new WaitForSeconds(1.5f);
        bulletArea.enabled = true;

        // �������
        yield return new WaitForSeconds(0.5f);
        bulletArea.enabled = false;

        // �������� ��
        yield return new WaitForSeconds(1f);
        nav.isStopped = true;
        isLook = true;
        boxCollider.enabled = true;

        StartCoroutine(Think());
    }
}
