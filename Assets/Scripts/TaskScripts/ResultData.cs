using UnityEngine;

public class ResultData : MonoBehaviour
{

    public float upLianDan;//�����ӳ�
    public float duration;//����ʱ��

    public bool isFinish;//��ǰ��Ϸ�Ƿ����

    public static ResultData instance;
    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
        isFinish = false;
    }

}
