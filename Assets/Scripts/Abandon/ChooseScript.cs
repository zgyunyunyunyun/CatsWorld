using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ChooseScript : MonoBehaviour
{
    //筛选按钮的list
    private Button[] filterButtons;

    //选择面板的筛选按钮的父节点（根据等级筛选）
    public GameObject buttons;
    public Button all;

    //生产内容的父节点
    public Transform contentParent;


    // Start is called before the first frame update
    void Start()
    {
        
        //获得筛选按钮的数量并赋值到list
        int btnNumber = buttons.transform.childCount;
        filterButtons = new Button[btnNumber];

        for (int i=0; i< btnNumber; i++)
        {
            filterButtons[i] = buttons.transform.GetChild(i).GetComponent<Button>();
        }

        // 给每个筛选按钮添加点击事件
        for (int i = 0; i < filterButtons.Length; i++)
        {          
            //index：第n个筛选按钮
            int buttonIndex = i;
            filterButtons[i].onClick.AddListener(() => FilterContent(buttonIndex));
        }
    }

    void Update()
    {
        //修改被选中的按钮颜色
        if (CatListUI.instance.currChoose != null)
        {
            //Debug.Log("改变选项颜色");
            if (CatListUI.instance.currChoose == "全部")
            {
                filterButtons[0].GetComponent<Image>().color = new Color32(255, 128, 98, 255);
                filterButtons[1].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                filterButtons[2].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                filterButtons[3].GetComponent<Image>().color = new Color32(255, 255, 255, 255);

            }
            else if(CatListUI.instance.currChoose == "炼丹中")
            {
                
                filterButtons[1].GetComponent<Image>().color = new Color32(255, 128, 98, 255);
                filterButtons[0].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                filterButtons[2].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                filterButtons[3].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
            else if (CatListUI.instance.currChoose == "探索中")
            {
                filterButtons[2].GetComponent<Image>().color = new Color32(255, 128, 98, 255);
                filterButtons[0].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                filterButtons[1].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                filterButtons[3].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
            else if (CatListUI.instance.currChoose == "空闲")
            {
                filterButtons[3].GetComponent<Image>().color = new Color32(255, 128, 98, 255);
                filterButtons[0].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                filterButtons[1].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
                filterButtons[2].GetComponent<Image>().color = new Color32(255, 255, 255, 255);
            }
        }
    }

    void FilterContent(int filterIndex)
    {
        /*
        // 遍历所有内容项，根据筛选按钮索引来更改它们的激活状态
        foreach (Transform child in contentParent)
        {
            // 根据需要筛选的逻辑来设置这个变量
            bool shouldActivate = true; 

            // 例如，根据按钮索引筛选
            if ((child.GetSiblingIndex() % filterButtons.Length)  !=  filterIndex)
            {
                Debug.Log("筛选的结果：" + child.GetSiblingIndex() % filterButtons.Length);
                shouldActivate = false;
            }
            child.gameObject.SetActive(shouldActivate);
        }
        */


        //筛选条件：处于什么工作状态。获得点击按钮返回的值
        string workstate = filterButtons[filterIndex].GetComponentInChildren<TMP_Text>().text;

        Debug.Log("筛选的工作状态是：" + workstate);

        if(workstate != null)
        {
            //contentParent.GetComponent<CatListUI>().RefreshUI(workstate);
        }
              
    }
}
