using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcadeButton : MonoBehaviour
{
    public bool isPressed;
    public ArcadeButton button;
    public GameObject pressedButton;

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isPressed = true;
            gameObject.SetActive(false);
            pressedButton.SetActive(true);
        }
    }
}
