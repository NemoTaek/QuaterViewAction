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
    public float speed;   // 근접무기면 돌아가는 속도, 투사체면 던지는 주기
    public float baseSpeed;
    float timer;
    int handType;   // 0: 근접, 1: 원거리

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


        // 아이템 타입이 근접이면 0, 원거리면 1
        if ((int)itemData.itemType == 0) handType = 0;
        else if ((int)itemData.itemType == 1 || (int)itemData.itemType == 5 || (int)itemData.itemType == 6 || (int)itemData.itemType == 7) handType = 1;

        Hand hand = player.hands[handType];
        hand.spriteRenderer.sprite = itemData.hand;
        hand.gameObject.SetActive(true);

        // BroadcastMessage(함수 명): 특정 함수 호출을 모든 자식에게 방송하는 함수
        // 선택사항으로 2번째 인자값으로 옵션을 붙일 수 있다
        // 여기서는 있는 애들한테만 메세지를 보내겠다고 선택사항으로 설정
        // 무기 레벨업할 때 기존의 무기에도 기어가 +로 적용되어 공속이 매우 빨라지는 버그가 생김
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

            // 이미 내가 가지고 있는 bullet이라면 가지고 있는거 사용하겠다.
            if (i < transform.childCount)
            {
                bullet = transform.GetChild(i);
            }
            // 추가되었으면 오브젝트 풀링으로 가져오겠다.
            else
            {
                bullet = GameManager.instance.pool.GetPool(prefabId).transform;
                bullet.parent = transform;  // 이를 쓰기 전에 bullet의 부모는 PoolManager. 그래서 Weapon0으로 부모를 변경
            }

            // 생성 시 위치 및 회전 초기화
            bullet.localPosition = Vector3.zero;
            bullet.localRotation = Quaternion.identity;

            // 개수에 따른 무기 위치 지정
            Vector3 rotateVector = Vector3.forward * 360 * i / count;
            bullet.Rotate(rotateVector);
            bullet.Translate(bullet.up * 1.5f, Space.World);

            bullet.GetComponent<Bullet>().Init(damage, -100); // -100: 항상 관통한다
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

        // BroadcastMessage(함수 명): 특정 함수 호출을 모든 자식에게 방송하는 함수
        // 선택사항으로 2번째 인자값으로 옵션을 붙일 수 있다
        // 여기서는 있는 애들한테만 메세지를 보내겠다고 선택사항으로 설정
        // 무기 레벨업할 때 기존의 무기에도 기어가 +로 적용되어 공속이 매우 빨라지는 버그가 생김
        //player.BroadcastMessage("ApplyGear", SendMessageOptions.DontRequireReceiver);
    }

    void Fire()
    {
        if (player.scanner.nearestTarget != null)
        {
            // 총알 방향 설정
            Vector3 targetPosition = player.scanner.nearestTarget.position;
            Vector3 targetDirection = targetPosition - transform.position;
            targetDirection.Normalize();

            // 총알 위치 설정
            Transform bullet = GameManager.instance.pool.GetPool(prefabId).transform;
            bullet.position = transform.position;

            // 대상을 향해 회전하고 초기화
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, targetDirection);  // FromToRotation(축 벡터, 목표): 지정된 축을 중심으로 목표를 향해 회전하는 함수
            bullet.GetComponent<Bullet>().Init(damage, count, targetDirection);

            // 발사!
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
        }
    }

    void Throw()
    {
        Vector3 targetDirection = player.inputVector.normalized;

        if (player.inputVector.magnitude != 0)
        {
            // 총알 위치 및 방향 설정
            Transform bullet = GameManager.instance.pool.GetPool(prefabId).transform;
            bullet.position = transform.position;
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, targetDirection);
            bullet.GetComponent<Bullet>().Init(id, damage, -100, targetDirection, count); // -100: 항상 관통한다

            // 발사!
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
        }
    }

    void Boomerang()
    {
        Vector3 targetDirection = player.inputVector.normalized;

        if (player.inputVector.magnitude != 0)
        {
            // 총알 위치 및 방향 설정
            Transform bullet = GameManager.instance.pool.GetPool(prefabId).transform;
            bullet.position = transform.position;
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, targetDirection);
            bullet.GetComponent<Bullet>().Init(id, damage, -100, targetDirection, count); // -100: 항상 관통한다

            // 발사!
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
        }
    }

    void Bang()
    {
        Vector3 targetDirection = player.inputVector.normalized;

        if (player.inputVector.magnitude != 0)
        {
            // 총알 위치 및 방향 설정
            Transform bullet = GameManager.instance.pool.GetPool(prefabId).transform;
            bullet.position = transform.position;
            bullet.rotation = Quaternion.FromToRotation(Vector3.up, targetDirection);
            bullet.GetComponent<Bullet>().Init(id, damage, -100, targetDirection, count); // -100: 항상 관통한다

            // 발사!
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Range);
        }
    }
}
