using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public enum AttackType { Melee, Range };    // ��������, ���Ÿ�����
    public AttackType type;
    public int damage;  // ������
    public float speed; // ����
    public int maxAmmo;  // źâ �ϳ��� �ִ� ����
    public int currentAmmo;  // ���� ���� źâ ����

    public BoxCollider meleeArea;   // �������� ����
    public TrailRenderer trailRenderer; // ���� ����Ʈ

    public Transform bulletPosition;    // �Ѿ� ��ġ
    public GameObject bullet;   // �Ѿ�
    public Transform bulletCasePosition;    // źâ ��ġ
    public GameObject bulletCase;   // źâ

    // �Ϲ� �Լ�: UseWeapon() ���η�ƾ -> Swing() �����ƾ -> UseWeapon() ���η�ƾ -> ...
    // �ڷ�ƾ �Լ�: UseWeapon() ���η�ƾ + Swing() �����ƾ �Լ� ����. ��, ���ý����ϴ� �۾�
    public void UseWeapon()
    {
        // StopCoroutine(): �ڷ�ƾ ���� �Լ�
        // StartCoroutine(): �ڷ�ƾ ���� �Լ�
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

    // IEnumerator: ������ �Լ� Ŭ����
    IEnumerator Swing()
    {
        // yield: ����� �����ϴ� Ű����
        // yield Ű���带 ������ ����Ͽ� �ð��� ���� �ۼ� ����

        // ���⸦ ����ϸ� 0.3�� �� Ȱ��ȭ -> 0.3�� �� ���� ���� ���� -> 0.3�� �� ���� ����Ʈ ����
        yield return new WaitForSeconds(0.3f);  // WaitForSwconds(): �־��� ��ġ��ŭ ��ٸ��� �Լ�
        meleeArea.enabled = true;
        trailRenderer.enabled = true;

        yield return new WaitForSeconds(0.3f);
        meleeArea.enabled = false;

        yield return new WaitForSeconds(0.3f);
        trailRenderer.enabled = false;
    }

    IEnumerator Shot()
    {
        // �Ѿ� �߻�
        GameObject instantBullet = Instantiate(bullet, bulletPosition.position, bulletPosition.rotation);
        Rigidbody bulletRigid = instantBullet.GetComponent<Rigidbody>();
        bulletRigid.velocity = bulletPosition.forward * 30;

        yield return null;

        // ź�� ����
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
