using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    // �� ������ �� ������ �Ѿ�ö� �����Ǿ�� �� ��
    // �÷��̾ �ִ� ����, ���ӸŴ����� �ִ� ����, UI
    // 3���ۿ� ����̴µ�.. ���� ���ϱ� �� ����� �� �̾����� �� �� �����ϴ�...

    public static T instance = null;

    protected virtual void Awake()
    {
        //if (instance == null)
        //{
        instance = (T)this;

        //    // �ٸ� ������ �ε��Ǿ ������Ʈ�� �ı����� �ʴ´�
        //    DontDestroyOnLoad(gameObject);
        //}
        //// this: ���� ����Ǹ鼭 ��� ���� ������ �̱���
        //// instance: ���� ����Ǳ� ���� �����ߴ� �̱���
        //else if (instance != this)
        //{
        //    // DontDestroyOnLoad �޼ҵ尡 �ִ� ���ӿ�����Ʈ�� DontDestroyOnLoad �������� �̵��ϰ� �ȴ�.
        //    // �� �Ŀ� ���� ������Ʈ�� ������ �� �����Ǿ� �ش� ������Ʈ�� �ߺ��� �Ǿ������.
        //    // �׷��Ƿ� ������ �ִ� ������Ʈ�� ��������, ���翡 ���� ����� ������Ʈ�� �����Ѵ�.
        //    Destroy(gameObject);
        //}
    }
}
