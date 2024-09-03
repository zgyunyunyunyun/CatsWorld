using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LingdanCat : MonoBehaviour
{
    public bool moveToFur = false;//是否移动到丹炉
    public bool moveToStay = false;//是否移动到丹炉
    public Vector3 target;//移动的目标
    public float speed;//移动的速度

    private int cType;//小猫类型
    private int fSite;//目的丹炉位置
    private GameObject furnace;//目的丹炉位置
    private int cSite;//目的小猫坑位
    private int catNumber;//小猫数据在列表中的位置

    private int sNumber;//小猫在stayBar的位置

    private Vector3 lastPosition;

    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;

        speed = 85.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //如果符合条件，移动小猫
        if (moveToStay || moveToFur)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);


            //transform.position = Vector3.Lerp(transform.position, target, speed * Time.deltaTime);

            //transform.Find("Particle").position = transform.position;
            
            if (lastPosition == transform.position && moveToFur)
            {
                //Debug.Log("小猫静止不动了，且被清理。序号为：" + catNumber);

                //替换丹炉上的小猫ui
                
                if (TaskController.instance.taskList.Count >= 1)
                {
                    if (TaskController.instance.taskList.Count == 1)
                    {
                        fSite = 0;
                    }
                    Debug.Log("清除丹炉，序号为：" + fSite);
                    GameObject tempTask = furnace;//TaskController.instance.taskList[fSite];
                    Image catIcon = tempTask.transform.Find("MainObject").Find("CatPos" + cSite.ToString()).GetComponent<Image>();
                    string path = "Materials/Cat/cat" + cType.ToString();
                    Sprite sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
                    catIcon.sprite = sprite;

                    Color color = catIcon.color; // 获取当前颜色
                    color.a = 1.0f; // 设置透明度
                    catIcon.color = color; // 应用新颜色

                    gameObject.SetActive(false);
                }


            }

            if (lastPosition == transform.position && moveToStay)
            {
                //Debug.Log("小猫静止不动了，且停留在stayBar");

            }

            lastPosition = transform.position;

        }
    }

    public void moveCat(Vector3 t, int type, int furnaceSite, GameObject furnaceObj, int catSite, int cNumber)
    {
        Debug.Log("小猫被移动到丹炉上，小猫序号为：" + cNumber);
        Debug.Log("小猫被移动到丹炉上，小猫类型为：" + type);
        //Debug.Log("小猫被移动到丹炉上，丹炉序号为：" + furnaceSite);
       // //Debug.Log("小猫被移动到判断完成任务的丹炉上，丹炉序号为：" + furnaceSite);

        target = t;
        moveToFur = true;
        cType = type;
        fSite = furnaceSite;
        furnace = furnaceObj;
        cSite = catSite;
        catNumber = cNumber;

        //transform.Find("Particle").gameObject.SetActive(true);
        //TaskController.instance.taskDataList[furnaceSite].cats.Add(LingDanCatController.instance.catList[cNumber]);
        TaskController.instance.taskDataList[furnaceSite].cats.Add(gameObject);
        //furnace.cats.Add(gameObject);

        //TaskController.instance.addTaskCat(furnaceSite, catNumber, cSite);
    }

    public void moveCatToStay(Vector3 t, int type, int stayNumber, int cNumber)
    {
        Debug.Log("小猫被移动到stayBar上，小猫序号为：" + cNumber);
        Debug.Log("小猫被移动到stayBar上，小猫类型为：" + type);
        target = t;
        moveToStay = true;
        cType = type;
        sNumber = stayNumber;
        catNumber = cNumber;

        //transform.Find("Particle").gameObject.SetActive(true);

        LingDanCatController.instance.addStayCat(cType, sNumber, catNumber);
    }

}
