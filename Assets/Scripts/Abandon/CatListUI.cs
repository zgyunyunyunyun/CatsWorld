using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CatListUI : MonoBehaviour
{
    public GameObject workUI;
    public GameObject chooseWork;
    public GameObject greyBG;
    public Button disLiandanWork;
    public Button disTansuoWork;

    private List<Cat> cats = new List<Cat>();

    //private int disCatId;//所分配工作小猫的id，初始化未-1
    public string currChoose = "全部";//当前所选择的小猫分类，默认是全部

    public static CatListUI instance;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //disCatId = -1;
        //RefreshUI("全部");

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /*
    //清空当前子节点后重新初始化列表
    public void RefreshUI(string work_state)
    {
        currChoose = work_state;
        //清空子节点
        if (transform.childCount > 0)
        {
            for (int j = 0; j < transform.childCount; j++)
            {
                GameObject.Destroy(transform.GetChild(j).gameObject);

                Debug.Log("已清空原父节点的所有子节点");
            }
        }
        

        if (cats != null)
        {
            cats.Clear();
        }


        //获得小猫list并重新排序
        List<Cat> tempcats = CatController.instance.cats;

        Debug.Log("小猫的数量：" + tempcats.Count);

        for (int j = 0; j < tempcats.Count; j++)
        {
            if(tempcats[j].work == "空闲")
            {
                cats.Insert(0, tempcats[j]);
            }
            else
            {
                cats.Add(tempcats[j]);
            }
        }


        int i;//遍历小猫
        int t = 0;//实际展示小猫的位置

        for (i = 0; i < cats.Count; i++)
        {
            //小猫不符合展示的条件，跳过并进入下一次循环
            if(work_state != "全部" && cats[i].work != work_state)
            {
                Debug.Log("不符合条件的小猫序号：" + i);
                continue;             
            }

            Debug.Log("当前小猫数量：" + cats.Count);

            //获得实例化工作项的数据，用于后面赋值
            GameObject temp = Instantiate(workUI);

            //将小猫属性列表挂着父节点上
            temp.transform.SetParent(this.transform, false);

            temp.transform.localPosition = new Vector3(410, -60 - t * 236, 0);

            Image catIcon = temp.transform.Find("catIcon").GetComponent<Image>();
            TMP_Text catName = temp.transform.Find("catName").GetComponent<TMP_Text>();
            TMP_Text workState = temp.transform.Find("workState").GetComponent<TMP_Text>();
            TMP_Text getThing = temp.transform.Find("getThing").GetComponent<TMP_Text>();
            Button button = temp.transform.Find("Button").GetComponent<Button>();
            TMP_Text contentBtn = button.transform.Find("Text (TMP)").GetComponent<TMP_Text>();


            //对获得的UI进行赋值
            string path = "Materials/" + cats[i].cat_icon;
            Debug.Log("成功诞生小猫，小猫头像 path:" + path);
            Sprite sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
            catIcon.sprite = sprite;

            catName.text = cats[i].cat_name;
            workState.text = cats[i].work;
            if (cats[i].work == "炼丹中")
            {
                getThing.text = "收获：灵丹（待计算）";
                contentBtn.text = "停止";

                //点击按钮时，把当前需要分配工作的小猫的id记录下来
                int catid = cats[i].cat_id;
                button.onClick.AddListener(() => clickWorkBtn(catid));
                button.onClick.AddListener(() => this.DistributeWork("空闲"));
            }
            else if (cats[i].work == "探索中")
            {
                getThing.text = "收获：灵草（待计算）";
                contentBtn.text = "停止";

                //点击按钮时，把当前需要分配工作的小猫的id记录下来
                int catid = cats[i].cat_id;
                button.onClick.AddListener(() => clickWorkBtn(catid));
                button.onClick.AddListener(() => this.DistributeWork("空闲"));
            }
            else
            {
                getThing.text = "收获：无";
                contentBtn.text = "工作";

                button.onClick.AddListener(() => chooseWork.gameObject.SetActive(true));
                button.onClick.AddListener(() => greyBG.gameObject.SetActive(false));

                //点击按钮时，把当前需要分配工作的小猫的id记录下来
                int catid = cats[i].cat_id;
                button.onClick.AddListener(() => clickWorkBtn(catid));

                //绑定按钮点击分配工作
                disLiandanWork.onClick.AddListener(() => this.DistributeWork("炼丹中"));
                disTansuoWork.onClick.AddListener(() => this.DistributeWork("探索中"));
                disLiandanWork.onClick.AddListener(() => chooseWork.gameObject.SetActive(false));
                disLiandanWork.onClick.AddListener(() => greyBG.gameObject.SetActive(true));
                disTansuoWork.onClick.AddListener(() => chooseWork.gameObject.SetActive(false));
                disTansuoWork.onClick.AddListener(() => greyBG.gameObject.SetActive(true));
            }

            t++;//展示了1只小猫并做累计

        }
    }
    */
    /*
    void clickWorkBtn(int catID)
    {
        if (disCatId == -1)
        {
            disCatId = catID;
        }
        else
        {
            Debug.LogError("小猫工作分配出错，未正确初始化");
        }    
    }*/

    /*
    void DistributeWork(string workState)
    {
        List<Cat> cats = CatController.instance.cats;
        //Debug.Log("准备分配工作:");
        if (cats != null && cats.Count > 0)
        {
            for(int i =0; i < cats.Count; i++)
            {
                //判断是否需要开始分配工作或停止工作
                if (disCatId != -1 && cats[i].cat_id == disCatId)
                {
                    CatController.instance.cats[i].work = workState;

                    Debug.Log("小猫开始工作了，该小猫id为：" + cats[i].cat_id);

                    disCatId = -1;
                    RefreshUI(currChoose);
                }else
                {
                    Debug.LogError("分配工作失败");
                }
            }
        }
        
    }
    */

}
