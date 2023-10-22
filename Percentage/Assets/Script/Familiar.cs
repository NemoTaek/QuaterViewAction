using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Familiar : MonoBehaviour
{
    public int id;
    public bool canAttack;
    public float attackDelay;
    public bool isDelay;

    void Start()
    {
        
    }

    void Update()
    {
        FamiliarPosition();
    }

    void DoActionFamiliar(int id)
    {

    }

    void FamiliarPosition()
    {
        if (GameManager.instance.player.inputVec.x != 0)
        {
            transform.localPosition = GameManager.instance.player.inputVec.x > 0 ? transform.localPosition.magnitude * Vector3.left : transform.localPosition.magnitude * Vector3.right;
        }
    }

    // �����ָӴ� �ɷ�: �� 10���� óġ �� 1�� ���
    public void DropCoin()
    {
        if (GameManager.instance.player.killEnemyCount % 10 == 0)
        {
            Instantiate(GameManager.instance.objectPool.prefabs[1], GameManager.instance.player.familiar.transform);
        }
    }

    // ������ �йи���� �÷��̾ �����ϴ� �������� źȯ�� �߻�
    public IEnumerator FamiliarShot(Vector2 dirVec)
    {
        Bullet bullet = GameManager.instance.bulletPool.Get(0, 0).GetComponent<Bullet>();
        bullet.transform.position = transform.position;

        if (dirVec == Vector2.right)
        {
            bullet.transform.rotation = Quaternion.Euler(0, 0, -90);
        }
        else if (dirVec == Vector2.left)
        {
            bullet.transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if (dirVec == Vector2.up)
        {
            bullet.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (dirVec == Vector2.down)
        {
            bullet.transform.rotation = Quaternion.Euler(0, 0, 180);
        }

        bullet.GetComponent<Rigidbody2D>().velocity = dirVec * 5;

        isDelay = true;
        yield return new WaitForSeconds(attackDelay);
        isDelay = false;
    }
}
