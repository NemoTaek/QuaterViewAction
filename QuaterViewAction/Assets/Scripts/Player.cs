using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // 플레이어 이동 변수
    float horizontal;
    float vertical;
    Vector3 moveVector;
    public float speed;
    public Camera followCamera; // 마우스 클릭한 곳으로 플레이어 이동
    bool isTouchWall;   // 벽에 닿았는가

    // 플레이어 행동 변수
    // ing 안붙은건 해당 행동을 하냐 안하냐, 붙은건 하는 중인가 아닌가
    bool isWalk;
    float walkSpeed;
    bool isJump;    // 점프 키를 눌렀는가?
    bool isJumping; // 점프중인가?
    bool isDodge;
    Vector3 dodgeVector;
    bool isAttack;
    float attackDelay;
    bool isAttackReady = true;
    bool isReload;
    bool isReloading;
    bool throwBomb;
    bool isDamage;
    bool isShopping;
    bool isDead;

    // 플레이어 로직 변수
    public int ammo;
    public int maxAmmo;
    public int coin;
    public int maxCoin;
    public int health;
    public int maxHealth;
    public int bomb;
    public int maxBomb;
    public int score;

    // 기타 오브젝트
    Animator animator;
    Rigidbody rigid;
    MeshRenderer[] meshRenderer;
    public GameManager manager;

    // 아이템 관련 변수
    GameObject nearObject;
    bool isInteraction;
    public Weapon[] weaponList;
    public GameObject grenadeObject;
    public bool[] hasWeapon;
    bool equipWeapon1;
    bool equipWeapon2;
    bool equipWeapon3;
    bool changeWeapon;
    public int currentEquipWeaponIndex = -1;
    public float weaponEquipDelay;
    public float maxWeaponEquipDelay;
    bool isSwap;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();  // 자식에게 넣었기 때문에 InChildren으로 세팅
        rigid = GetComponent<Rigidbody>();
        meshRenderer = GetComponentsInChildren<MeshRenderer>();

        walkSpeed = 1f;
    }

    void Start()
    {
        
    }

    
    void Update()
    {
        if(!isDead)
        {
            InputKey();
            Move();
            Turn();
            Jump();
            Dodge();
            Interaction();
            Equip();
            ChangeWeaponDelay();
            Attack();
            Reload();
            ThrowBombs();
        }
    }

    void FixedUpdate()
    {
        FreezeRotation();
        StopToWall();
    }

    void InputKey()
    {
        horizontal = Input.GetAxisRaw("Horizontal");
        vertical = Input.GetAxisRaw("Vertical");
        isWalk = Input.GetButton("Walk");
        isJump = Input.GetButtonDown("Jump");
        isInteraction = Input.GetButtonDown("Interaction");
        equipWeapon1 = Input.GetButtonDown("Equip1");
        equipWeapon2 = Input.GetButtonDown("Equip2");
        equipWeapon3 = Input.GetButtonDown("Equip3");
        changeWeapon = Input.GetButtonDown("ChangeWeapon");
        isAttack = Input.GetButton("Fire1");
        isReload = Input.GetButtonDown("Reload");
        throwBomb = Input.GetButtonDown("Bomb");
    }

    void Move()
    {
        
        moveVector = new Vector3(horizontal, 0, vertical);
        moveVector.Normalize(); // normalize(): 방향값을 1로 보정해주는 함수

        if(isDodge)
        {
            moveVector = dodgeVector;
        }

        // 공격할 때 어색하게 움직이기 때문에 여기서는 이동하면서 공격을 막도록 설정
        if (!isAttackReady)
        {
            moveVector = Vector3.zero;
        }

        // 기본 상태는 달리는 상태
        animator.SetBool("isRun", moveVector != Vector3.zero);

        // 왼쪽 쉬프트키를 누르고 있으면 걷는 상태로 변환
        if (isWalk)
        {
            walkSpeed = 0.5f;
        }
        animator.SetBool("isWalk", isWalk);

        // 벽에 안닿았다면 이동
        if(!isTouchWall)
        {
            transform.position += moveVector * speed * walkSpeed * Time.deltaTime;
        }
    }

    void Turn()
    {
        // LookAt(): 지정된 벡터를 향하여 회전하는 함수
        transform.LookAt(transform.position + moveVector);

        // ScreenPointToRay(): 스크린에서 월드로 Ray를 쏘는 함수
        if(Input.GetButton("Point"))
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))   // out: return처럼 반환값을 주어진 변수에 저장하는 키워드
            {
                Vector3 nextVector = rayHit.point - transform.position;
                nextVector.y = 0;   // 높이는 무시하도록 y축 0으로 초기화
                transform.LookAt(transform.position + nextVector);
            }
        }
    }

    void Jump()
    {
        
        if(isJump && moveVector == Vector3.zero && !isJumping && !isDodge)
        {
            isJumping = true;
            animator.SetBool("isJump", true);
            animator.SetTrigger("doJump");
            rigid.AddForce(Vector3.up * 10, ForceMode.Impulse);
        }
    }

    void Dodge()
    {
        if (isJump && moveVector != Vector3.zero && !isJumping && !isDodge && !isSwap)
        {
            dodgeVector = moveVector;
            speed *= 2;
            isDodge = true;
            animator.SetTrigger("doDodge");

            Invoke("DodgeOut", 0.5f);
        }
    }

    void DodgeOut()
    {
        speed /= 2f;
        isDodge = false;
    }

    void Interaction()
    {
        // 상호작용 키를 눌렀고 주위에 오브젝트가 있으면
        if(isInteraction && nearObject != null)
        {
            // 그 오브젝트가 무기이면
            if(nearObject.tag == "Weapon")
            {
                // 무기를 장착하고 화면에 있는 아이템을 삭제
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapon[weaponIndex] = true;
                Destroy(nearObject);
            }

            // 그 오브젝트가 상점이면
            if (nearObject.tag == "Shop")
            {
                // 상점 입장
                Shop shop = nearObject.GetComponent<Shop>();
                shop.Enter(this);
                isShopping = true;
            }
        }
    }

    void Equip()
    {
        // 무기 교체 쿨타임
        if (weaponEquipDelay < maxWeaponEquipDelay || isDodge) return;

        // 각 무기 장착 키를 누르면 해당 무기 장착
        // 만약 무기를 갖고있지 않거나 현재 장착중인 무기라면 캔슬
        if (equipWeapon1 && (!hasWeapon[0] || currentEquipWeaponIndex == 0)) return;
        if (equipWeapon1) currentEquipWeaponIndex = 0;
        if (equipWeapon2 && (!hasWeapon[1] || currentEquipWeaponIndex == 1)) return;
        if (equipWeapon2) currentEquipWeaponIndex = 1;
        if (equipWeapon3 && (!hasWeapon[2] || currentEquipWeaponIndex == 2)) return;
        if (equipWeapon3) currentEquipWeaponIndex = 2;

        // 무기 교체 버튼 누르면 장착한 순서대로 교체
        if (changeWeapon)
        {
            // 가지고있는 무기 개수가 1개 이하면 캔슬
            if (GetWeaponCount() <= 1) return;

            currentEquipWeaponIndex = CheckWeapon();
            if (!hasWeapon[currentEquipWeaponIndex]) return;
        }

        // 키 누르면 해당 무기 활성화
        if (equipWeapon1 || equipWeapon2 || equipWeapon3 || changeWeapon)
        {
            // 장착중인 무기는 해제하고
            for (int i=0; i<weaponList.Length; i++)
            {
                if (weaponList[i] != null)
                {
                    weaponList[i].gameObject.SetActive(false);
                }
            }

            // 현재 인덱스의 무기 장착
            weaponList[currentEquipWeaponIndex].gameObject.SetActive(true);
            animator.SetTrigger("doSwap");
            isSwap = true;
            Invoke("SwapOut", 0.5f);
            weaponEquipDelay = 0;
            
        }
    }

    int GetWeaponCount()
    {
        int count = 0;
        for (int i = 0; i < weaponList.Length; i++)
        {
            if (hasWeapon[i])
            {
                count++;
            }
        }
        return count;
    }

    int CheckWeapon()
    {
        int tempWeaponIndex = currentEquipWeaponIndex == weaponList.Length - 1 ? 0 : currentEquipWeaponIndex + 1;

        for (int i = 0; i < weaponList.Length; i++)
        {
            if (!hasWeapon[tempWeaponIndex])
            {
                tempWeaponIndex = currentEquipWeaponIndex == weaponList.Length - 1 ? 0 : currentEquipWeaponIndex + 1;
            }
        }
        return tempWeaponIndex;
    }

    void ChangeWeaponDelay()
    {
        weaponEquipDelay += Time.deltaTime;
    }

    void SwapOut()
    {
        isSwap = false;
    }

    void Attack()
    {
        // 장착한 무기가 없으면 캔슬
        if (currentEquipWeaponIndex < 0 || currentEquipWeaponIndex >= weaponList.Length) return;

        // 공격 딜레이 설정
        attackDelay += Time.deltaTime;
        isAttackReady = weaponList[currentEquipWeaponIndex].speed < attackDelay;

        // 공격 키를 눌렀고 준비가 되었다면, 그리고 회피중이거나 무기 교체중이 아니라면 공격
        if(isAttack && isAttackReady && !isDodge && !isSwap && !isReloading && !isShopping)
        {
            // 무기 타입이 근접공격이면 스윙, 아니면 발사
            string weaponType = weaponList[currentEquipWeaponIndex].type == Weapon.AttackType.Melee ? "doSwing" : "doShot";

            // 원거리 무기를 들고있는데 현재 남은 탄창이 0이면 재장전
            if(weaponType == "doShot" && weaponList[currentEquipWeaponIndex].currentAmmo == 0)
            {
                isReload = true;
                Reload();
                return;
            }

            weaponList[currentEquipWeaponIndex].UseWeapon();
            animator.SetTrigger(weaponType);
            attackDelay = 0;
        }
    }

    void Reload()
    {
        // 무기를 들고있지 않거나, 근접무기를 장착하고 있거나, 남은 탄창이 없으면 캔슬
        if (currentEquipWeaponIndex == -1) return;
        if (weaponList[currentEquipWeaponIndex].type == Weapon.AttackType.Melee) return;
        if (ammo == 0) return;

        if (isReload && !isJumping && !isDodge && !isSwap && isAttackReady && !isReloading && !isShopping)
        {
            isReload = false;
            if (weaponList[currentEquipWeaponIndex].maxAmmo == weaponList[currentEquipWeaponIndex].currentAmmo) return;
            animator.SetTrigger("doReload");
            isReloading = true;
            Invoke("ReloadOut", 1f);
        }
    }

    void ReloadOut()
    {
        // 부족한 총알 개수 계산
        int neededBullet = weaponList[currentEquipWeaponIndex].maxAmmo - weaponList[currentEquipWeaponIndex].currentAmmo;

        // 재장전 하고 그만큼 ammo 감소
        weaponList[currentEquipWeaponIndex].currentAmmo += neededBullet;
        ammo -= neededBullet;

        isReloading = false;
    }

    void FreezeRotation()
    {
        // angularVelocity: 물리 회전 속도
        rigid.angularVelocity = Vector3.zero;
    }

    void StopToWall()
    {
        Debug.DrawRay(transform.position, transform.forward * 5, Color.green);
        isTouchWall = Physics.Raycast(transform.position, transform.forward, 5, LayerMask.GetMask("Wall"));
    }

    void ThrowBombs()
    {
        if (bomb == 0) return;

        if (throwBomb && !isReload && !isSwap)
        {
            // 앞으로 위로 던지는 방향
            Vector3 throwVector = transform.forward * 10 + transform.up * 10;

            // 내 위치에서 폭탄을 생성하고 던지는 방향으로 던짐
            GameObject instantGrenade = Instantiate(grenadeObject, transform.position, transform.rotation);
            Rigidbody rigidGrenade = instantGrenade.GetComponent<Rigidbody>();
            rigidGrenade.AddForce(throwVector, ForceMode.Impulse);
            rigidGrenade.AddTorque(Vector3.back * 10, ForceMode.Impulse);
            bomb--;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            isJumping = false;
            animator.SetBool("isJump", false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            Item item = other.GetComponent<Item>();
            switch(item.type)
            {
                case Item.ItemType.Ammo:
                    if(ammo < maxAmmo)
                    {
                        ammo += item.value;
                        if(ammo > maxAmmo)
                        {
                            ammo = maxAmmo;
                        }
                    }
                    else
                    {
                        ammo = maxAmmo;
                    }
                    break;
                case Item.ItemType.Coin:
                    if (coin < maxCoin)
                    {
                        coin += item.value;
                        if (coin > maxCoin)
                        {
                            coin = maxCoin;
                        }
                    }
                    else
                    {
                        coin = maxCoin;
                    }
                    break;
                case Item.ItemType.Heart:
                    if (health < maxHealth)
                    {
                        health += item.value;
                        if (health > maxHealth)
                        {
                            health = maxHealth;
                        }
                    }
                    else
                    {
                        health = maxHealth;
                    }
                    break;
                case Item.ItemType.Grenade:
                    if (bomb < maxBomb)
                    {
                        bomb += item.value;
                        if (bomb > maxBomb)
                        {
                            bomb = maxBomb;
                        }
                    }
                    else
                    {
                        bomb = maxBomb;
                    }
                    break;
            }
            Destroy(other.gameObject);
        }

        if(other.tag == "EnemyBullet")
        {
            if(!isDamage && !isDead)
            {
                Bullet enemyBullet = other.GetComponent<Bullet>();
                health -= enemyBullet.damage;

                bool isBossTakeDown = other.name == "TakeDown Area";
                StartCoroutine(OnDamage(isBossTakeDown));
            }

            // 몸박하는 적과 미사일쏘는 EnemyBullet의 차이는 rigidbody의 유무
            if (other.GetComponent<Rigidbody>() != null)
            {
                Destroy(other.gameObject);
            }
        }
    }

    void OnTriggerStay(Collider other)
    {
        if(other.tag == "Weapon" || other.tag == "Shop")
        {
            nearObject = other.gameObject;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObject = null;
        }
        if (other.tag == "Shop")
        {
            Shop shop = nearObject.GetComponent<Shop>();
            shop.Exit();
            isShopping = false;
            nearObject = null;
        }
    }

    IEnumerator OnDamage(bool isBossTakeDown)
    {
        // 1초 무적시간
        isDamage = true;
        foreach(MeshRenderer mesh in meshRenderer)
        {
            mesh.material.color = Color.gray;
        }

        // 피격 후 체력이 0 이하면 게임오버
        if (health <= 0)
        {
            OnDie();
        }

        // 보스의 내려찍기 패턴 피격 시 넉백
        if (isBossTakeDown)
        {
            rigid.AddForce(transform.forward * -25, ForceMode.Impulse);
        }

        yield return new WaitForSeconds(1f);
        isDamage = false;
        foreach (MeshRenderer mesh in meshRenderer)
        {
            mesh.material.color = Color.white;
        }

        if (isBossTakeDown)
        {
            rigid.velocity = Vector3.zero;
        }
    }

    void OnDie()
    {
        animator.SetTrigger("doDie");
        isDead = true;
        manager.GameOver();
    }
}
