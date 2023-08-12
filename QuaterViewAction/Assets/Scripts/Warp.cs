using System.Collections;
using System.Collections.Generic;
using System.Transactions;
using UnityEngine;

public class Warp : MonoBehaviour
{
    public GameManager manager;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            manager.StageStart();
        }
    }
}
