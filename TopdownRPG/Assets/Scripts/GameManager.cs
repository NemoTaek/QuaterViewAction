using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Animator talkPanel;
    public TalkEffect talkText;
    public Image portraitImage;
    public Animator portraitAnimator;
    public Sprite prevPortrait;

    public GameObject scanObject;
    public bool isTalkAction;
    public TalkManager talkManager;
    public int talkIndex;

    public QuestManager questManager;
    public Text questTitle;
    public GameObject gameMenu;
    public GameObject player;

    void Start()
    {
        GameLoad();
    }
    public void Action(GameObject scanObj)
    {
        scanObject = scanObj;

        // 대화를 할 대상의 정보 가져오기
        ObjectData objectData = scanObject.GetComponent<ObjectData>();

        // 대화 시도
        Talk(objectData.id, objectData.isNPC);

        talkPanel.SetBool("isShow", isTalkAction);
    }

    void Talk(int id, bool isNPC)
    {
        // 퀘스트 세팅
        int questId = questManager.GetQuestTalkIndex(id);
        int talkId = id;
        string talkData;

        if(talkText.isTalking)
        {
            talkText.SetMessage("");
            return;
        }

        // 현재 퀘스트의 대화 순서에서 해당 NPC가 맞으면 퀘스트에 맞는 대화 출력
        // 퀘스트를 받고 까먹었을 때를 대비해서 다시 말걸었을 때도 해당 퀘스트의 대화 출력
        if (questManager.questList[questId].npcId[questManager.questActionIndex] == id || 
            (questManager.questActionIndex > 0 && questManager.questList[questId].npcId[questManager.questActionIndex-1] == id))
        {
            talkId = id + questId;
        }

        // 대화 리스트를 가져와서 대화를 진행한다.
        // null을 가져오면 모든 대화가 끝났다는 의미
        talkData = talkManager.GetTalk(talkId, talkIndex);

        // 모든 대화를 마치면 대화창을 닫고, 인덱스와 이미지를 초기화하고, 퀘스트 이름을 세팅
        if (talkData == null)
        {
            isTalkAction = false;
            talkIndex = 0;
            prevPortrait = null;
            questTitle.text = questManager.CheckQuest(talkId);
            return;
        }

        if(isNPC)
        {
            // NPC 대화에는 초상화가 있으므로 ":"으로 나누고
            // 0번 인덱스는 텍스트, 1번 인덱스는 초상화 인덱스이다.
            talkText.SetMessage(talkData.Split(":")[0]);
            portraitImage.sprite = talkManager.GetPortrait(id, int.Parse(talkData.Split(":")[1]));
            portraitImage.color = new Color(1, 1, 1, 1);

            // 초상화가 달라지면 애니메이션 추가
            if(prevPortrait != null && prevPortrait != portraitImage.sprite)
            {
                portraitAnimator.SetTrigger("doEffect");
            }
            prevPortrait = portraitImage.sprite;
        }
        else
        {
            talkText.SetMessage(talkData);
            portraitImage.color = new Color(1, 1, 1, 0);
        }

        isTalkAction = true;
        talkIndex++;
    }

    void Update()
    {
        if(Input.GetButtonDown("Cancel"))
        {
            Time.timeScale = 0;
            // 게임 오브젝트가 활성상태인지 체크
            if (gameMenu.activeSelf)
            {
                GameContinue();
            }
            else
            {
                gameMenu.SetActive(true);
            }
        }
    }

    void GameContinue()
    {
        gameMenu.SetActive(false);
        Time.timeScale = 1;
    }

    // 퀘스트 id, 퀘스트 action index, 플레이어 위치 저장
    public void GameSave()
    {
        // PlayerPrefs: 간단한 데이터 저장 기능을 지원하는 클래스
        // 경로는 Build Setting - Player Settings... 에 있는 Company와 Product Name으로 레지스트리에 저장됨
        PlayerPrefs.SetFloat("PlayerX", player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerY", player.transform.position.y);
        PlayerPrefs.SetInt("QuestId", questManager.questId);
        PlayerPrefs.SetInt("QuestActionIndex", questManager.questActionIndex);
        PlayerPrefs.Save();

        GameContinue();
    }

    public void GameLoad()
    {
        // 최초 게임 실행 시에는 데이터가 없으므로 예외처리 
        if (!PlayerPrefs.HasKey("PlayerX")) return;

        float x = PlayerPrefs.GetFloat("PlayerX");
        float y = PlayerPrefs.GetFloat("PlayerY");
        int questId = PlayerPrefs.GetInt("QuestId");
        int questActionIndex = PlayerPrefs.GetInt("QuestActionIndex");

        player.transform.position = new Vector3(x, y, 0);
        questManager.questId = questId;
        questManager.questActionIndex = questActionIndex;
        questManager.ControlObject();
        questTitle.text = questManager.CheckQuest();

        if (gameMenu.activeSelf)
        {
            GameContinue();
        }
    }

    public void GameExit()
    {
        // 어플리케이션을 종료
        Application.Quit();
    }
}
