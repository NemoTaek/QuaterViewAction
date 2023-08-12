using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkEffect : MonoBehaviour
{
    public string targetMessage;    // �ؽ�Ʈ �ִϸ��̼� ��� �޼���
    public int charPerSeconds;  // 1�ʿ� ����� �����ٰ��̳�
    Text text;  // �ִϸ��̼����� ������ �ؽ�Ʈ ����
    int index;  // �޼����� ���� �ϳ��ϳ��� �ε���
    public GameObject endCursor;
    public bool isTalking;   // ���ϴ� ���ΰ�?

    void Awake()
    {
        text = GetComponent<Text>();
    }

    public void SetMessage(string message)
    {
        if(isTalking)
        {
            text.text = targetMessage;
            CancelInvoke();
            EffectEnd();
        }
        else
        {
            targetMessage = message;
            EffectStart();
        }
    }

    void EffectStart()
    {
        text.text = "";
        index = 0;
        endCursor.SetActive(false);

        Invoke("Effecting", 1.0f / charPerSeconds);
    }

    void Effecting()
    {
        if(text.text == targetMessage)
        {
            EffectEnd();
            return;
        }

        // ���� �ϳ��� cps �ð����� �������� ����
        isTalking = true;
        text.text += targetMessage[index];
        index++;
        Invoke("Effecting", 1.0f / charPerSeconds);
    }

    void EffectEnd()
    {
        endCursor.SetActive(true);
        isTalking = false;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
