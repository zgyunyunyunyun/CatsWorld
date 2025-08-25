using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WordProcessor : MonoBehaviour
{
    public TMP_Text backgroundText;//背景文字
    public GameObject backgroundTipObj;//背景的提示，播放完以后再展示

    private float charsPerSecond = 0.07f;//打字时间间隔
    private int currentPos = 0;//当前打字位置
    public GameObject backgroundPanel;
    public GameObject newCatUI;

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
        string content = "有大量的小猫在流浪\n\n需要你收养小猫，帮助小猫成长";
        backgroundText.text = content;
        Coroutine v = StartCoroutine(DisplayWordOneByOne(backgroundText, content));

        backgroundPanel.GetComponent<Button>().onClick.AddListener(() =>
        {
            StopCoroutine(v);
            backgroundText.text = content;
            LastClickWord();
        });
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
        LastClickWord();

    }

    private void LastClickWord()
    {
        //给背景按钮添加点击事件
        backgroundPanel.GetComponent<Button>().onClick.AddListener(() =>
        {
            Debug.Log("背景按钮被点击");

            backgroundPanel.SetActive(false);
            MainController.instance.setBGIsOpened();
            newCatUI.SetActive(true);
        });

    }
}
