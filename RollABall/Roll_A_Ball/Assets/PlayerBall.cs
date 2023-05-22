using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerBall : MonoBehaviour
{
    Rigidbody rigid;
    bool isJumping;
    public int score;
    AudioSource acquireAudio;
    public GameManager manager;

    void Awake()
    {
        score = 0;
        rigid = GetComponent<Rigidbody>();
        isJumping = false;
        acquireAudio = GetComponent<AudioSource>();
    }

    void FixedUpdate()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        rigid.AddForce(new Vector3 (horizontal, 0, vertical), ForceMode.Impulse);
    }

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor")
        {
            isJumping = false;
        }    
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Items")
        {
            Item item = other.GetComponent<Item>();
            score++;
            manager.GetItemCount(score);
            acquireAudio.Play();
            item.gameObject.SetActive(false);   // SetActive: 해당 오브젝트 활성화/비활성화
        }

        if (other.tag == "Goal")
        {
            // find 계열 메소드는 CPU를 사용해서 검색하기 때문에 사용하지 않는 것을 권장
            // Scene을 불러오려면 File - Build Setting에 새로운 Scene을 추가해야함
            if (manager.totalScore == score)
            {
                SceneManager.LoadScene("Stage" + (manager.stage + 1));
            } else
            {
                SceneManager.LoadScene("Stage" + manager.stage);
            }
        }
    }

    void Update()
    {
        if(Input.GetButtonDown("Jump") && !isJumping)
        {
            isJumping = true;
            rigid.AddForce(new Vector3(0, 30, 0), ForceMode.Impulse);
        }
    }
}
