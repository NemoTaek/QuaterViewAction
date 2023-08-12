using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationBorder : MonoBehaviour
{
    public bool isCollide;

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.CompareTag("Tetromino"))
        {
            isCollide = true;
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.gameObject.CompareTag("Tetromino"))
        {
            isCollide = false;
        }
    }
}
