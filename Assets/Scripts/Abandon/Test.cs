using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Test : MonoBehaviour
{

    public TMP_Text areaText;
    public GameObject beginBG;

    public float tipsTimer = 1.0f;//tips��ʱ��

    public static Test instance;
    private void Awake()
    {
        instance = this;


    }

    // Start is called before the first frame update
    void Start()
    {
        // ����������󲻱�����
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
