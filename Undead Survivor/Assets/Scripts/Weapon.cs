using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    Player player;

    public int id;
    public int prefabId;
    public float damage;
    public float baseDamage;
    public int count;
    public float speed;   // ��������� ���ư��� �ӵ�, ����ü�� ������ �ֱ�
    public float baseSpeed;
    float timer;
    int handType;   // 0: ����, 1: ���Ÿ�

    public void Init(ItemData itemData)
    {
        name = "Weapon " + itemData.itemId;
        transform.parent = player.transform;
        transform.localPosition = Vector3.zero;

        id = itemData.itemId;
        damage = itemData.baseDamage * GameManager.instance.gameCharacters[GameManager.instance.playerId].characterData.baseDamage;
        baseDamage = itemData.baseDamage * GameManager.instance.gameCharacters[GameManager.instance.playerId].characterData.baseDamage;
        count = itemData.baseCount + GameManager.instance.gameCharacters[GameManager.instance.playerId].characterData.baseCount;

        for (int i = 0; i < GameManager.instance.pool.prefabs.Length; i++)
        {
            if (itemData.projectile == GameManager.instance.pool.prefabs[i])
            {
                prefabId = i;
                break;
            }
        }

        switch (id)
        {
            case 0:
                baseSpeed = itemData.baseSpeed * GameManager.instance.gameCharacters[GameManager.instance.playerId].characterData.baseWeaponSpeed;
                speed = itemData.baseSpeed * GameManager.instance.gameCharacters[GameManager.instance.playerId].characterData.baseWeaponSpeed;
                Positioning();
                break;
            default:
                baseSpeed = itemData.baseSpeed * GameManager.instance.gameCharacters[GameManager.instance.playerId].characterData.baseWeaponRate;
                speed = itemData.baseSpeed *GameManager.instance.gameCharacters[GameManager.instance.playerId].characterData.baseWeaponRate;
                break;
        }


        // ������ Ÿ���� �����̸� 0, ���Ÿ��� 1
        if ((int)itemData.itemType == 0) handType = 0;
        else if ((int)itemData.itemType == 1 || (int)itemData.itemType == 5 || (int)itemData.itemType == 6 || (int)itemData.itemType == 7) handType = 1;

        Hand hand = player.hands[handType];
        hand.spriteRenderer.sprite = itemData.hand;
        hand.gameObject.SetActive(true);

        // BroadcastMessage(�Լ� ��): Ư�� �Լ� ȣ���� ��� �ڽĿ��� ����ϴ� �Լ�
        // ���û������� 2��° ���ڰ����� �ɼ��� ���� �� �ִ�
        // ���⼭�� �ִ� �ֵ����׸� �޼����� �����ڴٰ� ���û������� ����
        // ���� �������� �� ������ ���⿡�� �� +�� ����Ǿ� ������ �ſ� �������� ���װ� ����
        //player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    void Awake()
    {
        player = GameManager.instance.player;
    }

    void Update()
    {
        if (!GameManager.instance.isTimePassing) return;

        switch (id)
        {
            case 0:
                transform.Rotate(Vector3.forward * speed * Time.deltaTime);
                break;
            case 1:
            case 7:
                timer += Time.deltaTime;
                if (timer > speed)
                {
                    timer = 0f;
                    Fire();
                }
                break;
            case 5:
                timer += Time.deltaTime;
                if (timer > speed)
                {
                    timer = 0f;
                    Throw();
                }
                break;
            case 6:
                timer += Time.deltaTime;
                if (timer > speed)
                {
                    timer = 0f;
                    Boomerang();
                }
                break;
            case 8:
                timer += Time.deltaTime;
                if (timer > speed)
                {
                    timer = 0f;
                    Bang();
                }
                break;
        }
    }

    void Positioning()
    {
        for (int i = 0; i < count; i++)
        {
            Transform bullet;

            // �̹� ���� ������ �ִ� bullet�̶�� ������ �ִ°� ����ϰڴ�.
            if (i < transform.childCount)
            {
                bullet = transform.GetChild(i);
            }
            // �߰��Ǿ����� ������Ʈ Ǯ������ �������ڴ�.
            else
            {
                bullet = GameManager.instance.pool.GetPool(prefabId).transform;
                bullet.parent = transform;  // �̸� ���� ���� bullet�� �θ�� PoolManager. �׷��� Weapon0���� �θ� ����
            }

            // ���� �� ��ġ �� ȸ�� �ʱ�ȭ
            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            // ������ ���� ���� ��ġ ����
            Vector3 rotateVector = Vector3.forward * 360 * i / count;
            bullet.Rotate(rotateVector);
            bullet.Translate(bullet.up * 1.5f, Space.World);

            bullet.GetComponent<Bullet>().Init(damage, -100); // -100: �׻� �����Ѵ�
        }
    }

    public void LevelUp(float damageUp, int countUp)
    {
        this.damage = damageUp;
        this.count = countUp;

        if (id == 0)
        {
            Positioning();
        }

        // BroadcastMessage(�Լ� ��): Ư�� �Լ� ȣ���� ��� �ڽĿ��� ����ϴ� �Լ�
        // ���û������� 2��° ���ڰ����� �ɼ��� ���� �� �ִ�
        // ���⼭�� �ִ� �ֵ����׸� �޼����� �����ڴٰ� ���û������� ����
        // ���� �������� �� ������ ���⿡�� �� +�� ����Ǿ� ������ �ſ� �������� ���װ� ����
        //player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    void Fire()
    {
        if (player.scanner.nearestTarget != null)
        {
            // �Ѿ� ���� ����
            Vector3 targetPosition = player.scanner.nearestTarget.position;
            Vector3 targetDirection = targetPosition - transform.position;
            targetDirection.Normalize();

            // �Ѿ� ��ġ ����
            Transform bullet = GameManager.instance.pool.GetPool(prefabId).transform;
            bullet.position = transform.position;

            // ����� ���� ȸ���ϰ� �ʱ�ȭ
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, targetDirection);  // FromToRotation(�� ����, ��ǥ): ������ ���� �߽����� ��ǥ�� ���� ȸ���ϴ� �Լ�
            bullet.GetComponent<Bullet>().Init(damage, count, targetDirection);

            // �߻�!
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
        }
    }

    void Throw()
    {
        Vector3 targetDirection = player.inputVector.normalized;

        if (player.inputVector.magnitude != 0)
        {
            // �Ѿ� ��ġ �� ���� ����
            Transform bullet = GameManager.instance.pool.GetPool(prefabId).transform;
            bullet.position = transform.position;
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, targetDirection);
            bullet.GetComponent<Bullet>().Init(id, damage, -100, targetDirection, count); // -100: �׻� �����Ѵ�

            // �߻�!
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
        }
    }

    void Boomerang()
    {
        Vector3 targetDirection = player.inputVector.normalized;

        if (player.inputVector.magnitude != 0)
        {
            // �Ѿ� ��ġ �� ���� ����
            Transform bullet = GameManager.instance.pool.GetPool(prefabId).transform;
            bullet.position = transform.position;
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, targetDirection);
            bullet.GetComponent<Bullet>().Init(id, damage, -100, targetDirection, count); // -100: �׻� �����Ѵ�

            // �߻�!
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
        }
    }

    void Bang()
    {
        Vector3 targetDirection = player.inputVector.normalized;

        if (player.inputVector.magnitude != 0)
        {
            // �Ѿ� ��ġ �� ���� ����
            Transform bullet = GameManager.instance.pool.GetPool(prefabId).transform;
            bullet.position = transform.position;
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, targetDirection);
            bullet.GetComponent<Bullet>().Init(id, damage, -100, targetDirection, count); // -100: �׻� �����Ѵ�

            // �߻�!
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
        }
    }
}
