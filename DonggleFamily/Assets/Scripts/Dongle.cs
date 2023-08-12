using System.Collections;
using UnityEngine;

public class Dongle : MonoBehaviour
{
    public GameManager manager;
    public Rigidbody2D rigid;
    CircleCollider2D circleCollider;
    Animator animator;
    public ParticleSystem effect;
    SpriteRenderer spriteRenderer;

    public bool isDrag;
    public int level;
    public bool isMerge;
    float lineStayTime;
    public bool isAttach;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        circleCollider = GetComponent<CircleCollider2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        animator.SetInteger("Level", level);
    }

    // ������Ʈ Ǯ���� ��Ȱ��ȭ �� ������ �ٽ� Ȱ��ȭ���� ��Ȱ���Ѵ�.
    // �׷��� ������ �� ��Ȱ��ȭ �� ���ۿ� ���� ������ ��� �ʱ�ȭ���Ѿ� �Ѵ�.
    void OnDisable()
    {
        // ������ �ִ� bool Ÿ���� ���� ��� �ʱ�ȭ
        level = 0;
        isDrag = false;
        isMerge = false;
        isAttach = false;

        // ��ġ �ʱ�ȭ
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.zero;

        // ���� �ʱ�ȭ
        rigid.simulated = false;
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;
        circleCollider.enabled = true;
    }

    void Start()
    {
        
    }

    void Update()
    {
        // ���� �� ������ ���� �� ������ �����϶�
        if(isDrag)
        {
            // Camera.main: ����Ƽ�� �ִ� ���� ī�޶� ����
            // Camera.main.ScreenToWorldPoint(): ��ũ�� ��ǥ���� ���� ���� ��ǥ���� ������ ��ȯ
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // �¿�� ������ �ʵ��� ��� ����
            float leftBorder = -4.1f + transform.localScale.x / 2f;
            float rightBorder = 4.1f - transform.localScale.x / 2f;

            if (mousePosition.x < leftBorder)
            {
                mousePosition.x = leftBorder;
            }
            if (mousePosition.x > rightBorder)
            {
                mousePosition.x = rightBorder;
            }

            mousePosition.y = 8;
            mousePosition.z = 0;

            // Lerp(a, b, t): ���� ����. ��, a�� b�� �� ���̿� ��ġ�� ���� �����ϱ� ���� �����Ÿ��� ���� ���������� ���
            // ��� ��: a + (b - a) * t
            // ���⼭�� �̸� ����ؼ� ���콺 ��ġ�� ���� ��������� �ִϸ��̼�ȭ(?)
            transform.position = Vector3.Lerp(transform.position, mousePosition, 0.05f);
        }
        
    }

    public void Drag()
    {
        isDrag = true;
    }

    public void Drop()
    {
        isDrag = false;
        rigid.simulated = true; // �ٸ� ��ü�� ��ȣ�ۿ�(������ ������ �ްڴ�)
    }

    // ��ü �غ����: ���� ������Ʈ ��, ��������� �浹������ ��Ȱ��ȭ
    public void Hide(Vector3 targetPosition)
    {
        isMerge = true;
        rigid.simulated = false;
        circleCollider.enabled = false;
        StartCoroutine(HideDongle(targetPosition));
    }

    // ��ü ����
    IEnumerator HideDongle(Vector3 targetPosition)
    {
        int frameCount = 0;

        // 20�����ӵ��� ������ �̵�
        while(frameCount < 20)
        {
            frameCount++;

            // ���ӿ����� �ƴ� ���¸� ��ü ����
            if(targetPosition != Vector3.up * 100)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, 0.5f);
            }
            // ���ӿ����� ũ�⸦ 0���� ����� ������ �� ó�� ���̵��� ����
            else
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.2f);
            }

            yield return null;
        }

        manager.score += (int)Mathf.Pow(2, level);

        isMerge = false;
        gameObject.SetActive(false);
    }

    void LevelUp()
    {
        isMerge = true;
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;
        StartCoroutine(LevelUpDongle());
    }

    IEnumerator LevelUpDongle()
    {
        yield return new WaitForSeconds(0.2f);
        animator.SetInteger("Level", level+1);
        EffectPlay();
        manager.SfxPlay(GameManager.Sfx.LevelUp);

        yield return new WaitForSeconds(0.3f);
        level++;

        manager.maxLevel = Mathf.Max(level, manager.maxLevel);
        isMerge = false;
    }

    public void EffectPlay()
    {
        effect.transform.position = transform.position;
        effect.transform.localScale = transform.localScale;
        effect.Play();
    }

    // ���̳� �ٸ� ���ۿ� �浹���� �� ���� ���
    IEnumerator AttachRoutine()
    {
        if(isAttach)
        {
            yield break;
        }

        isAttach = true;
        manager.SfxPlay(GameManager.Sfx.Attach);

        yield return new WaitForSeconds(0.2f);
        isAttach = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(AttachRoutine());
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Dongle")
        {
            // ������ ���� ���۳��� ��ġ�� ����
            Dongle collisionDongle = collision.gameObject.GetComponent<Dongle>();

            // ����ִ� ������ ������ ����, �������� ���� �ƴ� �� ��ĥ �� ����
            if(level == collisionDongle.level && !isMerge && !collisionDongle.isMerge && level < 7 && collisionDongle.level < 7)
            {
                // �� ������ ��ġ �ľ�
                float currentDongleX = transform.position.x;
                float currentDongleY = transform.position.y;
                float collisionDongleX = collisionDongle.transform.position.x;
                float collisionDongleY = collisionDongle.transform.position.y;

                // ���� �Ʒ��� �ְų�, ������ ���̿��� �����ʿ� ���� ��
                if(currentDongleY < collisionDongleY || (currentDongleY < collisionDongleY && currentDongleX > collisionDongleX))
                {
                    // �浹ü�� �������, ���� ����Ͽ� Ŀ����
                    collisionDongle.Hide(transform.position);
                    LevelUp();
                }
            }
        }
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.tag == "Finish")
        {
            lineStayTime += Time.deltaTime;
            if(lineStayTime > 2)
            {
                spriteRenderer.color = new Color(0.9f, 0.2f, 0.2f);
            }
            if (lineStayTime > 5)
            {
                manager.GameOver();
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            lineStayTime = 0;
            spriteRenderer.color = Color.white;
        }
    }
}
