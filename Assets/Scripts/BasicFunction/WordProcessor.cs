using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WordProcessor : MonoBehaviour
{
    public TMP_Text backgroundText;//��������
    public GameObject backgroundTipObj;//��������ʾ���������Ժ���չʾ

    private float charsPerSecond = 0.07f;//����ʱ����
    private int currentPos = 0;//��ǰ����λ��

    // Start is called before the first frame update
    void Start()
    {
        ShowBackGround();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowBackGround()
    {
        //string content = "���ɽ���������������������\n\nСè��ӵ�м��ѵ������츳���������츳�ͣ����������������츳\n\n����ǰ������һ��������Ĵ��£�" +
        //    "Сè���ķ����ѣ�����Сè�޼ҿɹ�\n\n����Ѱ��Сè������Сè�����ɳ������ܼ�԰";
        string content = "���ɽ���\n\n����Сè�Ĵ�����\n\n��Ҫ��\n����Сè\nָ��Сè�ɳ�";
        backgroundText.text = content;
        //StartCoroutine(DisplayWordOneByOne(backgroundText, content));

        //StartCoroutine(WaitDisplay(backgroundTipObj, 8.5f));
    }


    IEnumerator DisplayWordOneByOne(TMP_Text text, string texts)
    {
        text.text = "";
        while (currentPos < texts.Length)
        {
            yield return new WaitForSeconds(charsPerSecond);
            currentPos++;
            text.text = texts.Substring(0, currentPos);//ˢ���ı���ʾ����
        }
    }

    IEnumerator WaitDisplay(GameObject text, float time)
    {
        yield return new WaitForSeconds(time);

        text.SetActive(true);
    }
}
