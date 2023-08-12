using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // �÷��̾� �̵� ����
    float horizontal;
    float vertical;
    Vector3 moveVector;
    public float speed;
    public Camera followCamera; // ���콺 Ŭ���� ������ �÷��̾� �̵�
    bool isTouchWall;   // ���� ��Ҵ°�

    // �÷��̾� �ൿ ����
    // ing �Ⱥ����� �ش� �ൿ�� �ϳ� ���ϳ�, ������ �ϴ� ���ΰ� �ƴѰ�
    bool isWalk;
    float walkSpeed;
    bool isJump;    // ���� Ű�� �����°�?
    bool isJumping; // �������ΰ�?
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

    // �÷��̾� ���� ����
    public int ammo;
    public int maxAmmo;
    public int coin;
    public int maxCoin;
    public int health;
    public int maxHealth;
    public int bomb;
    public int maxBomb;
    public int score;

    // ��Ÿ ������Ʈ
    Animator animator;
    Rigidbody rigid;
    MeshRenderer[] meshRenderer;
    public GameManager manager;

    // ������ ���� ����
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
        animator = GetComponentInChildren<Animator>();  // �ڽĿ��� �־��� ������ InChildren���� ����
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
        moveVector.Normalize(); // normalize(): ���Ⱚ�� 1�� �������ִ� �Լ�

        if(isDodge)
        {
            moveVector = dodgeVector;
        }

        // ������ �� ����ϰ� �����̱� ������ ���⼭�� �̵��ϸ鼭 ������ ������ ����
        if (!isAttackReady)
        {
            moveVector = Vector3.zero;
        }

        // �⺻ ���´� �޸��� ����
        animator.SetBool("isRun", moveVector != Vector3.zero);

        // ���� ����ƮŰ�� ������ ������ �ȴ� ���·� ��ȯ
        if (isWalk)
        {
            walkSpeed = 0.5f;
        }
        animator.SetBool("isWalk", isWalk);

        // ���� �ȴ�Ҵٸ� �̵�
        if(!isTouchWall)
        {
            transform.position += moveVector * speed * walkSpeed * Time.deltaTime;
        }
    }

    void Turn()
    {
        // LookAt(): ������ ���͸� ���Ͽ� ȸ���ϴ� �Լ�
        transform.LookAt(transform.position + moveVector);

        // ScreenPointToRay(): ��ũ������ ����� Ray�� ��� �Լ�
        if(Input.GetButton("Point"))
        {
            Ray ray = followCamera.ScreenPointToRay(Input.mousePosition);
            RaycastHit rayHit;
            if (Physics.Raycast(ray, out rayHit, 100))   // out: returnó�� ��ȯ���� �־��� ������ �����ϴ� Ű����
            {
                Vector3 nextVector = rayHit.point - transform.position;
                nextVector.y = 0;   // ���̴� �����ϵ��� y�� 0���� �ʱ�ȭ
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
        // ��ȣ�ۿ� Ű�� ������ ������ ������Ʈ�� ������
        if(isInteraction && nearObject != null)
        {
            // �� ������Ʈ�� �����̸�
            if(nearObject.tag == "Weapon")
            {
                // ���⸦ �����ϰ� ȭ�鿡 �ִ� �������� ����
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.value;
                hasWeapon[weaponIndex] = true;
                Destroy(nearObject);
            }

            // �� ������Ʈ�� �����̸�
            if (nearObject.tag == "Shop")
            {
                // ���� ����
                Shop shop = nearObject.GetComponent<Shop>();
                shop.Enter(this);
                isShopping = true;
            }
        }
    }

    void Equip()
    {
        // ���� ��ü ��Ÿ��
        if (weaponEquipDelay < maxWeaponEquipDelay || isDodge) return;

        // �� ���� ���� Ű�� ������ �ش� ���� ����
        // ���� ���⸦ �������� �ʰų� ���� �������� ������ ĵ��
        if (equipWeapon1 && (!hasWeapon[0] || currentEquipWeaponIndex == 0)) return;
        if (equipWeapon1) currentEquipWeaponIndex = 0;
        if (equipWeapon2 && (!hasWeapon[1] || currentEquipWeaponIndex == 1)) return;
        if (equipWeapon2) currentEquipWeaponIndex = 1;
        if (equipWeapon3 && (!hasWeapon[2] || currentEquipWeaponIndex == 2)) return;
        if (equipWeapon3) currentEquipWeaponIndex = 2;

        // ���� ��ü ��ư ������ ������ ������� ��ü
        if (changeWeapon)
        {
            // �������ִ� ���� ������ 1�� ���ϸ� ĵ��
            if (GetWeaponCount() <= 1) return;

            currentEquipWeaponIndex = CheckWeapon();
            if (!hasWeapon[currentEquipWeaponIndex]) return;
        }

        // Ű ������ �ش� ���� Ȱ��ȭ
        if (equipWeapon1 || equipWeapon2 || equipWeapon3 || changeWeapon)
        {
            // �������� ����� �����ϰ�
            for (int i=0; i<weaponList.Length; i++)
            {
                if (weaponList[i] != null)
                {
                    weaponList[i].gameObject.SetActive(false);
                }
            }

            // ���� �ε����� ���� ����
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
        // ������ ���Ⱑ ������ ĵ��
        if (currentEquipWeaponIndex < 0 || currentEquipWeaponIndex >= weaponList.Length) return;

        // ���� ������ ����
        attackDelay += Time.deltaTime;
        isAttackReady = weaponList[currentEquipWeaponIndex].speed < attackDelay;

        // ���� Ű�� ������ �غ� �Ǿ��ٸ�, �׸��� ȸ�����̰ų� ���� ��ü���� �ƴ϶�� ����
        if(isAttack && isAttackReady && !isDodge && !isSwap && !isReloading && !isShopping)
        {
            // ���� Ÿ���� ���������̸� ����, �ƴϸ� �߻�
            string weaponType = weaponList[currentEquipWeaponIndex].type == Weapon.AttackType.Melee ? "doSwing" : "doShot";

            // ���Ÿ� ���⸦ ����ִµ� ���� ���� źâ�� 0�̸� ������
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
        // ���⸦ ������� �ʰų�, �������⸦ �����ϰ� �ְų�, ���� źâ�� ������ ĵ��
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
        // ������ �Ѿ� ���� ���
        int neededBullet = weaponList[currentEquipWeaponIndex].maxAmmo - weaponList[currentEquipWeaponIndex].currentAmmo;

        // ������ �ϰ� �׸�ŭ ammo ����
        weaponList[currentEquipWeaponIndex].currentAmmo += neededBullet;
        ammo -= neededBullet;

        isReloading = false;
    }

    void FreezeRotation()
    {
        // angularVelocity: ���� ȸ�� �ӵ�
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
            // ������ ���� ������ ����
            Vector3 throwVector = transform.forward * 10 + transform.up * 10;

            // �� ��ġ���� ��ź�� �����ϰ� ������ �������� ����
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

            // �����ϴ� ���� �̻��Ͻ�� EnemyBullet�� ���̴� rigidbody�� ����
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
        // 1�� �����ð�
        isDamage = true;
        foreach(MeshRenderer mesh in meshRenderer)
        {
            mesh.material.color = Color.gray;
        }

        // �ǰ� �� ü���� 0 ���ϸ� ���ӿ���
        if (health <= 0)
        {
            OnDie();
        }

        // ������ ������� ���� �ǰ� �� �˹�
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
