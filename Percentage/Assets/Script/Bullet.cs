using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int id;
    public bool isPenetrate;
    public bool isSlow;

    void Awake()
    {

    }

    void Start()
    {

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isPenetrate && (collision.CompareTag("Wall") || collision.CompareTag("Object")))
        {
            gameObject.SetActive(false);
            transform.position = transform.parent.position;
        }
    }

    void Update()
    {
        
    }
}
