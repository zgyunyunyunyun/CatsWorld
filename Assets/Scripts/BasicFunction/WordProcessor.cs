using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WordProcessor : MonoBehaviour
{
    public TMP_Text backgroundText;//背景文字
    public GameObject backgroundTipObj;//背景的提示，播放完以后再展示

    private float charsPerSecond = 0.07f;//打字时间间隔
    private int currentPos = 0;//当前打字位置

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
        //string content = "修仙界广大无垠，生存着世间万物\n\n小猫族拥有极佳的炼丹天赋，但修炼天赋低，不懂得运用炼丹天赋\n\n万年前发生了一件震惊世界的大事，" +
        //    "小猫族四分五裂，大量小猫无家可归\n\n快来寻找小猫，帮助小猫修炼成长、重塑家园";
        string content = "修仙界中\n\n大量小猫四处流浪\n\n需要你\n收养小猫\n指引小猫成长";
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
            text.text = texts.Substring(0, currentPos);//刷新文本显示内容
        }
    }

    IEnumerator WaitDisplay(GameObject text, float time)
    {
        yield return new WaitForSeconds(time);

        text.SetActive(true);
    }
}
