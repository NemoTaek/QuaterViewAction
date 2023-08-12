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

        // ��ȭ�� �� ����� ���� ��������
        ObjectData objectData = scanObject.GetComponent<ObjectData>();

        // ��ȭ �õ�
        Talk(objectData.id, objectData.isNPC);

        talkPanel.SetBool("isShow", isTalkAction);
    }

    void Talk(int id, bool isNPC)
    {
        // ����Ʈ ����
        int questId = questManager.GetQuestTalkIndex(id);
        int talkId = id;
        string talkData;

        if(talkText.isTalking)
        {
            talkText.SetMessage("");
            return;
        }

        // ���� ����Ʈ�� ��ȭ �������� �ش� NPC�� ������ ����Ʈ�� �´� ��ȭ ���
        // ����Ʈ�� �ް� ��Ծ��� ���� ����ؼ� �ٽ� ���ɾ��� ���� �ش� ����Ʈ�� ��ȭ ���
        if (questManager.questList[questId].npcId[questManager.questActionIndex] == id || 
            (questManager.questActionIndex > 0 && questManager.questList[questId].npcId[questManager.questActionIndex-1] == id))
        {
            talkId = id + questId;
        }

        // ��ȭ ����Ʈ�� �����ͼ� ��ȭ�� �����Ѵ�.
        // null�� �������� ��� ��ȭ�� �����ٴ� �ǹ�
        talkData = talkManager.GetTalk(talkId, talkIndex);

        // ��� ��ȭ�� ��ġ�� ��ȭâ�� �ݰ�, �ε����� �̹����� �ʱ�ȭ�ϰ�, ����Ʈ �̸��� ����
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
            // NPC ��ȭ���� �ʻ�ȭ�� �����Ƿ� ":"���� ������
            // 0�� �ε����� �ؽ�Ʈ, 1�� �ε����� �ʻ�ȭ �ε����̴�.
            talkText.SetMessage(talkData.Split(":")[0]);
            portraitImage.sprite = talkManager.GetPortrait(id, int.Parse(talkData.Split(":")[1]));
            portraitImage.color = new Color(1, 1, 1, 1);

            // �ʻ�ȭ�� �޶����� �ִϸ��̼� �߰�
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
            // ���� ������Ʈ�� Ȱ���������� üũ
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

    // ����Ʈ id, ����Ʈ action index, �÷��̾� ��ġ ����
    public void GameSave()
    {
        // PlayerPrefs: ������ ������ ���� ����� �����ϴ� Ŭ����
        // ��δ� Build Setting - Player Settings... �� �ִ� Company�� Product Name���� ������Ʈ���� �����
        PlayerPrefs.SetFloat("PlayerX", player.transform.position.x);
        PlayerPrefs.SetFloat("PlayerY", player.transform.position.y);
        PlayerPrefs.SetInt("QuestId", questManager.questId);
        PlayerPrefs.SetInt("QuestActionIndex", questManager.questActionIndex);
        PlayerPrefs.Save();

        GameContinue();
    }

    public void GameLoad()
    {
        // ���� ���� ���� �ÿ��� �����Ͱ� �����Ƿ� ����ó�� 
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
        // ���ø����̼��� ����
        Application.Quit();
    }
}
