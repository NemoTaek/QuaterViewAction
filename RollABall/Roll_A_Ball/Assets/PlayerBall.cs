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
            item.gameObject.SetActive(false);   // SetActive: �ش� ������Ʈ Ȱ��ȭ/��Ȱ��ȭ
        }

        if (other.tag == "Goal")
        {
            // find �迭 �޼ҵ�� CPU�� ����ؼ� �˻��ϱ� ������ ������� �ʴ� ���� ����
            // Scene�� �ҷ������� File - Build Setting�� ���ο� Scene�� �߰��ؾ���
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
