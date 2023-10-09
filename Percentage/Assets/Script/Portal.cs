using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Portal : MonoBehaviour
{
    void Start()
    {
        
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // ���� ���������� ���ų� ���� Ŭ����ų�
            //isGameClear = true;
            //StartCoroutine(GameManager.instance.GameResult());

            StartCoroutine(LoadScene());
            StartCoroutine(UnloadScene());
            GameManager.instance.GameInit();

            gameObject.SetActive(false);
        }
    }

    void Update()
    {
        
    }

    IEnumerator LoadScene()
    {
        yield return SceneManager.LoadSceneAsync("Loading", LoadSceneMode.Additive);
    }

    IEnumerator UnloadScene()
    {
        if(SceneManager.GetSceneByName("Stage1").IsValid())
        {
            yield return SceneManager.UnloadSceneAsync("Stage1");
        }
    }
}
