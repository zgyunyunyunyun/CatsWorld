using UnityEngine;

public class ResultData : MonoBehaviour
{

    public float upLianDan;//炼丹加成
    public float duration;//持续时间

    public bool isFinish;//当前游戏是否完成

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
