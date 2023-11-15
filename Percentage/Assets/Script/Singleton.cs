using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    // 전 씬에서 새 씬으로 넘어올때 유지되어야 할 것
    // 플레이어에 있는 모든것, 게임매니저에 있는 모든것, UI
    // 3개밖에 없어보이는데.. 막상 보니까 맵 빼고는 다 이어져야 할 것 같습니다...

    public static T instance = null;

    protected virtual void Awake()
    {
        //if (instance == null)
        //{
        instance = (T)this;

        //    // 다른 씬으로 로딩되어도 오브젝트가 파괴되지 않는다
        //    DontDestroyOnLoad(gameObject);
        //}
        //// this: 씬이 변경되면서 방금 새로 생성된 싱글톤
        //// instance: 씬이 변경되기 전에 저장했던 싱글톤
        //else if (instance != this)
        //{
        //    // DontDestroyOnLoad 메소드가 있는 게임오브젝트는 DontDestroyOnLoad 영역으로 이동하게 된다.
        //    // 그 후에 같은 오브젝트가 있으면 또 생성되어 해당 오브젝트가 중복이 되어버린다.
        //    // 그러므로 이전에 있던 오브젝트를 가져오고, 현재에 새로 생기는 오브젝트는 삭제한다.
        //    Destroy(gameObject);
        //}
    }
}
