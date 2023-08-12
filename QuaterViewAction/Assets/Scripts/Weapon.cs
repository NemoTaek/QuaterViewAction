using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum AttackType { Melee, Range };    // 근접공격, 원거리공격
    public AttackType type;
    public int damage;  // 데미지
    public float speed; // 공속
    public int maxAmmo;  // 탄창 하나의 최대 개수
    public int currentAmmo;  // 현재 남은 탄창 개수

    public BoxCollider meleeArea;   // 근접공격 범위
    public TrailRenderer trailRenderer; // 공격 이펙트

    public Transform bulletPosition;    // 총알 위치
    public GameObject bullet;   // 총알
    public Transform bulletCasePosition;    // 탄창 위치
    public GameObject bulletCase;   // 탄창

    // 일반 함수: UseWeapon() 메인루틴 -> Swing() 서브루틴 -> UseWeapon() 메인루틴 -> ...
    // 코루틴 함수: UseWeapon() 메인루틴 + Swing() 서브루틴 함수 실행. 즉, 동시실행하는 작업
    public void UseWeapon()
    {
        // StopCoroutine(): 코루틴 종료 함수
        // StartCoroutine(): 코루틴 실행 함수
        if (type == AttackType.Melee)
        {
            StopCoroutine("Swing");
            StartCoroutine("Swing");
        }
        else if (type == AttackType.Range)
        {
            currentAmmo--;
            StopCoroutine("Shot");
            StartCoroutine("Shot");
        }
    }

    // IEnumerator: 열거형 함수 클래스
    IEnumerator Swing()
    {
        // yield: 결과를 전달하는 키워드
        // yield 키워드를 여러개 사용하여 시간차 로직 작성 가능

        // 무기를 사용하면 0.3초 후 활성화 -> 0.3초 후 공격 범위 해제 -> 0.3초 후 공격 이펙트 해제
        yield return new WaitForSeconds(0.3f);  // WaitForSwconds(): 주어진 수치만큼 기다리는 함수
        meleeArea.enabled = true;
        trailRenderer.enabled = true;

        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailRenderer.enabled = false;
    }

    IEnumerator Shot()
    {
        // 총알 발사
        GameObject instantBullet = Instantiate(bullet, bulletPosition.position, bulletPosition.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPosition.forward * 30;

        yield return null;

        // 탄피 배출
        GameObject instantCase = Instantiate(bulletCase, bulletCasePosition.position, bulletCasePosition.rotation);
        Rigidbody bulletCaseRigid = instantCase.GetComponent<Rigidbody>();
        Vector3 caseVector = bulletCasePosition.forward * Random.Range(-3, -2) + Vector3.up * Random.Range(2, 3);
        bulletCaseRigid.AddForce(caseVector, ForceMode.Impulse);
        bulletCaseRigid.AddTorque(Vector3.up * 10, ForceMode.Impulse);
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        
    }
}
