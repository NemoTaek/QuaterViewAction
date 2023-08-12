using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour
{
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // 컴포넌트가 활성화 될 때 호출되는 생명주기 함수
    void OnEnable()
    {
        Invoke("EndExplosion", 1);
    }

    public void StartExplosion(string target)
    {
        animator.SetTrigger("OnExplosion");

        switch(target)
        {
            case "EnemySmall":
                transform.localScale = Vector3.one * 0.7f;
                break;
            case "EnemyMedium":
            case "Player":
                transform.localScale = Vector3.one;
                break;
            case "EnemyLarge":
                transform.localScale = Vector3.one * 2f;
                break;
            case "Boss":
                transform.localScale = Vector3.one * 3f;
                break;
        }
    }

    void EndExplosion()
    {
        gameObject.SetActive(false);
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
