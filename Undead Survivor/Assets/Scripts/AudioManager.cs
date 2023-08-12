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
        // bgm 초기화
        GameObject bgmObject = new GameObject("BgmPlayer");
        bgmObject.transform.parent = transform;
        bgmPlayer = bgmObject.AddComponent<AudioSource>();
        bgmPlayer.playOnAwake = false;  // playOnAwake: 시작할 때 플레이 할 것인가
        bgmPlayer.loop = true;
        bgmPlayer.volume = bgmVolume;
        bgmPlayer.clip = bgmClip;
        bgmHighPassFilter = Camera.main.GetComponent<AudioHighPassFilter>();

        // sfx 초기화
        GameObject sfxObject = new GameObject("SfxPlayer");
        sfxObject.transform.parent = transform;
        sfxPlayers = new AudioSource[channels];
        for(int i=0; i<sfxPlayers.Length; i++)
        {
            sfxPlayers[i] = sfxObject.AddComponent<AudioSource>();
            sfxPlayers[i].playOnAwake = false;
            sfxPlayers[i].bypassListenerEffects = true;     // 효과음 사운드에 필터를 먹지 않도록 설정
            sfxPlayers[i].volume = sfxVolume;
        }
    }

    public void PlaySfx(Sfx sfx)
    {
        for(int i=0; i< sfxPlayers.Length; i++)
        {
            int loopIndex = (i + channelIndex) % sfxPlayers.Length;
            // 재생중이 아닌 채널에서 재생
            if (!sfxPlayers[loopIndex].isPlaying) {
                int random = 0;
                // 효과음이 2개일 경우에는 둘 중 랜덤으로 출력
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

    // BGM에 필터를 걸어 효과음과 겹칠 때는 BGM을 온전히 계속 재생하면 이질감이 들 수 있다
    // 그래서 주파수가 설정한 기준보다 높은 음역대만 통과시키는 필터(Aduio HighPass Filter)를 사용
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
