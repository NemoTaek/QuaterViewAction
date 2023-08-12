using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalkEffect : MonoBehaviour
{
    public string targetMessage;    // 텍스트 애니메이션 대상 메세지
    public int charPerSeconds;  // 1초에 몇글자 보여줄것이냐
    Text text;  // 애니메이션으로 보여줄 텍스트 변수
    int index;  // 메세지의 글자 하나하나의 인덱스
    public GameObject endCursor;
    public bool isTalking;   // 말하는 중인가?

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

        // 글자 하나씩 cps 시간동안 나오도록 설정
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
