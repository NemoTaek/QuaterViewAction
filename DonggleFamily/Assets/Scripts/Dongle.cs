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

    // 오브젝트 풀에서 비활성화 된 동글을 다시 활성화시켜 재활용한다.
    // 그래서 합쳐질 때 비활성화 된 동글에 대한 정보를 모두 초기화시켜야 한다.
    void OnDisable()
    {
        // 가지고 있는 bool 타입의 정보 모두 초기화
        level = 0;
        isDrag = false;
        isMerge = false;
        isAttach = false;

        // 위치 초기화
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.zero;

        // 물리 초기화
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
        // 시작 후 동글이 놓기 전 위에서 움직일때
        if(isDrag)
        {
            // Camera.main: 유니티에 있는 메인 카메라에 접근
            // Camera.main.ScreenToWorldPoint(): 스크린 좌표계의 값을 월드 좌표계의 값으로 변환
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // 좌우로 나가지 않도록 경계 설정
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

            // Lerp(a, b, t): 선형 보간. 즉, a와 b의 값 사이에 위치한 값을 추정하기 위해 직선거리에 따라 선형적으로 계산
            // 계산 식: a + (b - a) * t
            // 여기서는 이를 사용해서 마우스 위치를 점점 따라오도록 애니메이션화(?)
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
        rigid.simulated = true; // 다른 물체와 상호작용(물리적 영향을 받겠다)
    }

    // 합체 준비과정: 상태 업데이트 후, 물리영향과 충돌감지를 비활성화
    public void Hide(Vector3 targetPosition)
    {
        isMerge = true;
        rigid.simulated = false;
        circleCollider.enabled = false;
        StartCoroutine(HideDongle(targetPosition));
    }

    // 합체 로직
    IEnumerator HideDongle(Vector3 targetPosition)
    {
        int frameCount = 0;

        // 20프레임동안 서서히 이동
        while(frameCount < 20)
        {
            frameCount++;

            // 게임오버가 아닌 상태면 합체 진행
            if(targetPosition != Vector3.up * 100)
            {
                transform.position = Vector3.Lerp(transform.position, targetPosition, 0.5f);
            }
            // 게임오버면 크기를 0으로 만들어 없어진 것 처럼 보이도록 설정
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

    // 벽이나 다른 동글에 충돌했을 때 사운드 출력
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
            // 레벨이 같은 동글끼리 합치는 로직
            Dongle collisionDongle = collision.gameObject.GetComponent<Dongle>();

            // 닿아있는 동글의 레벨이 같고, 합쳐지는 중이 아닐 때 합칠 수 있음
            if(level == collisionDongle.level && !isMerge && !collisionDongle.isMerge && level < 7 && collisionDongle.level < 7)
            {
                // 두 동글의 위치 파악
                float currentDongleX = transform.position.x;
                float currentDongleY = transform.position.y;
                float collisionDongleX = collisionDongle.transform.position.x;
                float collisionDongleY = collisionDongle.transform.position.y;

                // 내가 아래에 있거나, 동일한 높이에서 오른쪽에 있을 때
                if(currentDongleY < collisionDongleY || (currentDongleY < collisionDongleY && currentDongleX > collisionDongleX))
                {
                    // 충돌체는 사라지고, 나는 흡수하여 커지기
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
