using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage;
    public bool isMelee;
    public bool isBossRock;

    void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Floor" && !isMelee && !isBossRock)
        {
            Destroy(gameObject, 3);
        }
    }

    void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.tag == "Wall" && !isMelee)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
