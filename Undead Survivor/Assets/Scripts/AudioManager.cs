using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("------- BGM -------")]
    public AudioClip bgmClip;
    public float bgmVolume;
    AudioSource bgmPlayer;
    AudioHighPassFilter bgmHighPassFilter;

    [Header("------- SFX -------")]
    public AudioClip[] sfxClips;
    public float sfxVolume;
    AudioSource[] sfxPlayers;
    public int channels;
    int channelIndex;

    public enum Sfx { Dead, Hit, LevelUp=3, Lose, Melee, Range=7, Select, Win };


    void Awake()
    {
        instance = this;
        Init();
    }

    void Init()
    {
        // bgm �ʱ�ȭ
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;  // playOnAwake: ������ �� �÷��� �� ���ΰ�
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;
        bgmHighPassFilter = Camera.main.GetComponent<AudioHighPassFilter>();

        // sfx �ʱ�ȭ
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];
        for(int i=0; i<sfxPlayers.Length; i++)
        {
            sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[i].playOnAwake = false;
            sfxPlayers[i].bypassListenerEffects = true;     // ȿ���� ���忡 ���͸� ���� �ʵ��� ����
            sfxPlayers[i].volume = sfxVolume;
        }
    }

    public void PlaySfx(Sfx sfx)
    {
        for(int i=0; i< sfxPlayers.Length; i++)
        {
            int loopIndex = (i + channelIndex) % sfxPlayers.Length;
            // ������� �ƴ� ä�ο��� ���
            if (!sfxPlayers[loopIndex].isPlaying) {
                int random = 0;
                // ȿ������ 2���� ��쿡�� �� �� �������� ���
                if(sfx == Sfx.Hit || sfx == Sfx.Melee)
                {
                    random = Random.Range(0, 2);
                }

                channelIndex = loopIndex;
                sfxPlayers[channelIndex].clip = sfxClips[(int)sfx + random];
                sfxPlayers[channelIndex].Play();
                break;
            }
        }
    }

    public void PlayBgm(bool isPlay)
    {
        if(isPlay)
        {
            bgmPlayer.Play();
        }
        else
        {
            bgmPlayer.Stop();
        }
    }

    // BGM�� ���͸� �ɾ� ȿ������ ��ĥ ���� BGM�� ������ ��� ����ϸ� �������� �� �� �ִ�
    // �׷��� ���ļ��� ������ ���غ��� ���� �����븸 �����Ű�� ����(Aduio HighPass Filter)�� ���
    public void HighPassBgm(bool isPlay)
    {
        bgmHighPassFilter.enabled = isPlay;
    }

    void Start()
    {
        
    }


    void Update()
    {
        
    }
}
