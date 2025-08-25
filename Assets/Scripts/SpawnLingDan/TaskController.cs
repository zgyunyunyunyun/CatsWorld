using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using TMPro;

public class TaskController : MonoBehaviour
{
    /*基本数据存储
     * 
     */

    public List<GameObject> taskList = new List<GameObject>();//任务对象list
    public List<Task> taskDataList = new List<Task>();//任务数据list

    public List<GameObject> taskToDestoryList = new List<GameObject>();//存储未销毁的任务对象list

    //待实例的对象
    public GameObject taskToSpawn;
    public GameObject taskParent;

    public int fType;//丹炉的类型数量
    public int cType;//小猫的类型数量


    public AudioClip finishTaskClip;//点击完成任务的音效

    public TMP_Text finishPercent;//完成度
    public TMP_Text stoneNumber;//完成任务兑换的灵石
    public TMP_Text stonePassPanelNumber;//完成任务兑换的灵石
    public TMP_Text stoneFinishPanelNumber;//完成任务兑换的灵石

    private float fPercent;//完成度
    public int sNumber;//灵石数

    private float maxCatNumber;//当前关卡的小猫数量，用户展示任务完成的比例
    private float curCatNumber;//当前关卡的小猫数量，用户展示任务完成的比例
    private int currLingdanLevel;//当前关卡灵石的阶数，用户生产不同灵石等级的任务

    private int furnaceNumber;//总共需要产生的丹炉数量
    public int currFurnaceNumber;//当前产生的丹炉数量

    public float changeCatUITime = 0.8f;//收集小猫的间隙，需小于完成任务的时间
    public float finishTaskTime = 0.85f;//收集的任务完成后，留个完成任务动效的时间间隔
    public float clearTaskTime = 1f;//完成任务后，多久清理一次任务

    float clearTimer;//计时器，用于避免对象被提前摧毁，无法获得展示的位置


    public static TaskController instance;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        fPercent = 0;
        sNumber = 0;
        fPercent = 100;
        currLingdanLevel = 100;
        furnaceNumber = 0;
        currFurnaceNumber = 0;
        clearTimer = clearTaskTime;
    }


    // Update is called once per frame
    void Update()
    {

        //完成当前关卡所有小猫炼丹任务，游戏结束
        if (curCatNumber <= 0)
        {
            TaskLevelController.instance.passedCurrentGame();
        }

        //如果灵丹价格处于高位，则高亮展示比例，否则正常展示 
        if (PriceController.instance != null && PriceController.instance.isHighPirce)
        {
            stoneNumber.text = sNumber.ToString() + "<color=#FF0000> (+" + ((int)((PriceController.instance.price - 1) * 100)).ToString() + "%)</color>";
            stonePassPanelNumber.text = sNumber.ToString() + "<color=#FF0000> (+" + ((int)((PriceController.instance.price - 1) * 100)).ToString() + "%)</color>";
            stoneFinishPanelNumber.text = sNumber.ToString() + "<color=#FF0000> (+" + ((int)((PriceController.instance.price - 1) * 100)).ToString() + "%)</color>";
        }
        else
        {
            stoneNumber.text = sNumber.ToString();
            stonePassPanelNumber.text = sNumber.ToString();
            stoneFinishPanelNumber.text = sNumber.ToString();
        }

        finishPercent.text = fPercent.ToString() + "%";


        //每隔clearTaskTime清理一次任务列表
        clearTimer -= Time.deltaTime;
        if (clearTimer < 0)
        {
            if (taskToDestoryList.Count > 0)
            {
                GameObject temp = taskToDestoryList[taskToDestoryList.Count - 1];
                //taskToDestoryList.RemoveAt(taskToDestoryList.Count - 1);
                //Destroy(temp);
            }
            clearTimer = clearTaskTime;
        }

        //遍历任务列表，判断任务是否完成
        for (int i = taskDataList.Count - 1; i >= 0; i--)
        {
            GameObject tempTaskObj = taskList[i];//获得处理对象

            //判断任务完成、且未开始丹炉播放动画，则播放1次动画
            if (taskDataList[i].catchNumber >= 3
                && !tempTaskObj.transform.Find("MainObject").GetComponent<EffectController>().AnimationBeginning()
                && !tempTaskObj.transform.Find("MainObject").GetComponent<EffectController>().AnimationFinished()
                && !taskDataList[i].cats[0].activeSelf
                && !taskDataList[i].cats[1].activeSelf
                && !taskDataList[i].cats[2].activeSelf)
            {
                Debug.Log("开始播放任务完成动画，丹炉序号为：" + i);

                //播放任务完成的丹炉动画
                tempTaskObj.transform.Find("MainObject").GetComponent<EffectController>().scaleObject(false);
            }


            //判断任务完成、且完成丹炉动画播放、且未开始播放灵石数字动画，则处理1次数据，开始播放1次灵石数字动画
            if (taskDataList[i].catchNumber >= 3
                && tempTaskObj.transform.Find("MainObject").GetComponent<EffectController>().AnimationFinished()
                && !tempTaskObj.transform.Find("EffectObject").Find("Lingdan").GetComponent<EffectController>().AnimationBeginning()
                && !tempTaskObj.transform.Find("EffectObject").Find("Lingdan").GetComponent<EffectController>().AnimationFinished()
                && !taskDataList[i].cats[0].activeSelf
                && !taskDataList[i].cats[1].activeSelf
                && !taskDataList[i].cats[2].activeSelf)
            {
                Debug.Log("丹炉任务完成动画播放完毕，丹炉序号为：" + i);

                //播放小猫音效
                AudioManager.instance.PlayAudio(finishTaskClip);

                //将灵石数字可见，同时播放动画、展示数字
                tempTaskObj.transform.Find("EffectObject").Find("Lingdan").gameObject.SetActive(true);
                tempTaskObj.transform.Find("EffectObject").Find("Lingdan").GetComponent<EffectController>().scaleObject(true);

                //开始处理完成任务后的数据变化
                if (tempTaskObj != null)
                {

                    //开始完成一个炼丹任务，并计算获得的灵石
                    Debug.Log("完成1个炼丹的任务");

                    fPercent = (int)(curCatNumber / maxCatNumber * 100);

                    Debug.Log("增加灵丹：" + (int)Mathf.Pow(10, taskDataList[i].lingdanLevel + 2));
                    Debug.Log("灵丹等级：" + taskDataList[i].lingdanLevel);

                    float factor = 0;//品质系数
                    if (taskDataList[i].lingdanQuality == 0)
                    {
                        factor = 1.0f;
                    }
                    else if (taskDataList[i].lingdanQuality == 1)
                    {
                        factor = 1.2f;
                    }
                    else if (taskDataList[i].lingdanQuality == 2)
                    {
                        factor = 1.5f;
                    }
                    else if (taskDataList[i].lingdanQuality == 3)
                    {
                        factor = 2.0f;
                    }
                    Debug.Log("灵丹品质：" + taskDataList[i].lingdanQuality);

                    float price = 0;

                    //含价格飙升系数
                    if (PriceController.instance != null)
                    {
                        price = (int)(Mathf.Pow(5, taskDataList[i].lingdanLevel + 1) * factor * PriceController.instance.price * 10) * (TaskLevelController.instance.currLevel + 1);//提高该玩法的可玩性和意愿
                        sNumber += (int)price / 2;
                    }
                    else
                    {
                        price = (int)(Mathf.Pow(5, taskDataList[i].lingdanLevel + 1) * factor * 10) * (TaskLevelController.instance.currLevel + 1);//提高该玩法的可玩性和意愿
                        sNumber += (int)price / 2;
                    }


                    tempTaskObj.transform.Find("EffectObject").Find("Lingdan").Find("Text").GetComponent<TMP_Text>().text = "+" + ((int)price).ToString();


                }
            }


            //判断任务完成、且完成丹炉动画播放、且完成灵石数字动画播放、且所有小猫停止运动了，则隐藏当前UI并生成下一个UI
            if (taskDataList[i].catchNumber >= 3
                && tempTaskObj.transform.Find("MainObject").GetComponent<EffectController>().AnimationFinished()
                && tempTaskObj.transform.Find("EffectObject").Find("Lingdan").GetComponent<EffectController>().AnimationFinished()
                && !taskDataList[i].cats[0].activeSelf
                && !taskDataList[i].cats[1].activeSelf
                && !taskDataList[i].cats[2].activeSelf)
            {

                Debug.Log("开始清理完成任务的数据，其序号为：" + i);
                Debug.Log("三只小猫的active状态为：" + taskDataList[i].cats[0].activeSelf);
                Debug.Log("三只小猫的active状态为：" + taskDataList[i].cats[1].activeSelf);
                Debug.Log("三只小猫的active状态为：" + taskDataList[i].cats[2].activeSelf);

                //判断是否该清理小猫数据
                if (taskDataList[i].cats.Count >= 3)
                {

                    int catNumber = taskDataList[i].cats.Count;
                    for (int j = catNumber - 1; j > 0; j--)
                    {
                        Debug.Log("清空完成任务的小猫序号" + j);

                        taskDataList[i].cats.RemoveAt(j);
                    }

                }

                //将完成任务的数据从list中移除

                taskDataList.RemoveAt(i);
                taskList.RemoveAt(i);
                //问题，有可能存在部分丹炉重叠的情况，主要是因为点击清除任务较快，2个不同的丹炉重叠在了一个index上，后面新增的任务还没有生成，就往同一个位置叠加了丹炉。因此，挪到后面来

                curCatNumber -= 3;//把清理小猫数据和控制生成任务分离。原因见产生的新任务处


                //toDestory.SetActive(false);
                //taskToDestoryList.Add(toDestory);

                Destroy(tempTaskObj);

                //产生新的任务
                spawnTask(i);//问题，有可能遗留了3个小猫没有去处，是因为点击清理任务速度较快，还没有倒计时接触，就已经把当前小猫数量降到了12以下，导致无法产生更多小猫。改变判断条件即可

            }
        }
    }

    //开始关卡
    public void startGame(int catNumber, int level)
    {
        //总共要产生的丹炉数量
        furnaceNumber = catNumber / 3;
        Debug.Log("需要产生的丹炉数量：" + furnaceNumber);

        //根据参数赋值
        fPercent = 0;
        curCatNumber = catNumber;
        maxCatNumber = catNumber;
        currLingdanLevel = level;
        Debug.Log("catNumber" + catNumber);
        Debug.Log("curCatNumber" + curCatNumber);
        Debug.Log("maxCatNumber" + maxCatNumber);

        //清空数据（任务对象，任务数据，未销毁的任务）
        if (taskList.Count > 0)
        {
            for (int i = 0; i < taskList.Count; i++)
            {
                Destroy(taskList[i]);
            }
            taskList.Clear();
        }
        if (taskDataList.Count > 0)
        {
            taskDataList.Clear();
        }
        if (taskToDestoryList.Count > 0)
        {
            for (int i = 0; i < taskToDestoryList.Count; i++)
            {
                Destroy(taskToDestoryList[i]);
            }
            taskToDestoryList.Clear();
        }

        //生产4个丹炉
        for (int i = 0; i < 4; i++)
        {
            int site = i;
            spawnTask(i);
            currFurnaceNumber++;
        }


    }


    //产生任务（需要产生第i个丹炉，灵丹等级）
    void spawnTask(int i)
    {
        Debug.Log("产生了丹炉，序号为：" + i);
        Debug.Log("当前产生的丹炉数量为：" + currFurnaceNumber);
        Debug.Log("curCatNumber" + curCatNumber);
        //如果已产生的丹炉大于最大数量，则不再产生丹炉及任务
        if (curCatNumber >= 12)
        {

            //初始化任务状态
            Task task = new Task();
            task.lingdanLevel = currLingdanLevel;
            task.catchNumber = 0;
            task.cats = new List<GameObject>();

            //随机产生不同品质丹药，按概率
            int pro = Random.Range(0, 100);
            if (pro < 5)
            {
                task.lingdanQuality = 3;
            }
            else if (pro >= 5 && pro < 15)
            {
                task.lingdanQuality = 2;
            }
            else if (pro >= 15 && pro < 40)
            {
                task.lingdanQuality = 1;
            }
            else if (pro >= 40)
            {
                task.lingdanQuality = 0;
            }



            //产生对象并添加任务列表
            GameObject temp = Instantiate(taskToSpawn);

            //设置任务位置
            temp.transform.SetParent(taskParent.transform, false);
            temp.transform.localPosition = new Vector3(-412 + i * 260, 0, 0);

            //展示丹炉图片
            Image furnaceIcon = temp.transform.Find("MainObject").Find("furnaceImage").GetComponent<Image>();
            //int furnaceType = Random.Range(0, 4);
            string path = "Materials/Logo/鱼群-" + task.lingdanQuality.ToString();
            Sprite sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
            furnaceIcon.sprite = sprite;

            //不同品质的灵丹，输出效果
            if (task.lingdanQuality == 0)
            {
                //temp.transform.Find("MainObject").Find("Panel").GetComponent<Image>().color = new Color32(45, 235, 0, 150);
                //temp.transform.Find("MainObject").Find("Name").GetComponent<TMP_Text>().text = "中品灵丹";
            }
            else if (task.lingdanQuality == 1)
            {
                //temp.transform.Find("MainObject").Find("Panel").GetComponent<Image>().color = new Color32(45, 235, 0, 150);
                //temp.transform.Find("MainObject").Find("Name").GetComponent<TMP_Text>().text = "中品灵丹";
            }
            else if (task.lingdanQuality == 2)
            {
                //temp.transform.Find("MainObject").Find("Panel").GetComponent<Image>().color = new Color32(253, 107, 0, 230);
                //temp.transform.Find("MainObject").Find("Name").GetComponent<TMP_Text>().text = "上品灵丹";
            }
            else if (task.lingdanQuality == 3)
            {
                //temp.transform.Find("MainObject").Find("Panel").GetComponent<Image>().color = new Color32(194, 0, 253, 150);
                //temp.transform.Find("MainObject").Find("Name").GetComponent<TMP_Text>().text = "极品灵丹";
            }

            //展示小猫图片
            Image catIcon = temp.transform.Find("MainObject").Find("Cat").GetComponent<Image>();

            //保证生成的小猫任务和小猫呼应
            int catType = Random.Range(0, cType);

            int flag = 0;
            while (LingDanCatController.instance.catTypeAmount[catType] <= 0)
            {
                catType = Random.Range(0, cType);
                flag++;
                if (flag > 10)
                {
                    for (int x = 0; x < 4; x++)
                    {
                        if (LingDanCatController.instance.catTypeAmount[x] > 0)
                        {
                            catType = x;
                        }
                    }
                    break;
                }
            }


            LingDanCatController.instance.catTypeAmount[catType]--;


            string catPath = "Materials/Cat/cat" + catType.ToString();
            Sprite catSprite = Resources.Load(catPath, typeof(Sprite)) as Sprite;
            catIcon.sprite = catSprite;

            taskList.Insert(i, temp);
            //furTimerList.Insert(i, furSecondes);

            //记录icon数据
            task.catType = catType;
            //task.furnaceIcon = furnaceType;
            task.lingdanLevel = SceneTransferData.instance.maxCatLevel;


            taskDataList.Insert(i, task);
            Debug.Log("新生产的灵丹任务，序号为：" + i);
            Debug.Log("新生产的灵丹任务的小猫类型为：" + catType);


            //判断stayBar上是否已有新生成任务的小猫
            List<StayBarCat> stayCatList = LingDanCatController.instance.stayCatList;

            int cSite = 0;//丹炉内没有小猫的位置。在新增丹炉的情况下，没有小猫的位置是第一个
            Debug.Log("StayBar总共小猫数量为：" + stayCatList.Count);
            for (int j = 0; j < stayCatList.Count; j++)
            {
                //如果有小猫，则判断与新生产的任务是否一致
                /* 将小猫移动到该去的位置（丹炉；丹炉内的位置）；移除stayBar的小猫
                 * 
                 */
                //Debug.Log("StayBar小猫遍历序号为：" + j);
                //Debug.Log("StayBar小猫遍历bool为：" + stayCatList[j].hasCat);
                // Debug.Log("StayBar小猫遍历type为：" + stayCatList[j].catType);
                //Debug.Log("StayBar小猫遍历type2为：" + catType);
                //Debug.Log("StayBar小猫遍历位置为：" + cSite);

                if (stayCatList[j].hasCat && stayCatList[j].catType == catType)
                {
                    Transform tran = temp.transform.Find("MainObject").Find("CatPos" + cSite.ToString());
                    //移动小猫
                    Debug.Log("移动的小猫类型为：" + stayCatList[j].catType);
                    // &&  && taskDataList[i] != null && stayCatList[j] != null
                    if (temp == null)
                    {
                        Debug.Log("temp is null");
                    }
                    if (tran == null)
                    {
                        Debug.Log("tran is null");
                    }
                    if (taskDataList[i] == null)
                    {
                        Debug.Log("taskDataList is null");
                    }
                    if (stayCatList[j] == null)
                    {
                        Debug.Log("stayCatList is null");
                    }

                    if (stayCatList[j].cat != null && tran != null)
                    {
                        stayCatList[j].cat.GetComponent<LingdanCat>().moveCat(tran.position, catType, i, temp, cSite, stayCatList[j].catNumber);//目标位置、小猫类型、丹炉位置、丹炉内位置、小猫序号

                        //stayCatList[j].cat.GetComponent<LingdanCat>().moveCat(tran.position, catType,  cSite, stayCatList[j].catNumber);//目标位置、小猫类型、丹炉位置、丹炉内位置、小猫序号


                    }

                    //增加该丹炉里小猫的数量
                    taskDataList[i].catchNumber++;
                    cSite++;


                    //清空该位置的小猫数据（保留空对象——底图）
                    stayCatList[j].hasCat = false;
                    stayCatList[j].catType = -1;
                    stayCatList[j].catNumber = -1;
                    stayCatList[j].cat = null;

                    if (taskDataList[i].catchNumber >= 3)
                    {
                        break;
                    }
                }
            }

            //如果stay的小猫数量大于0，说明仍然有小猫没有命中任务，因此，需要把后面的小猫往前平移
            if (stayCatList.Count > 0)
            {
                //遍历stayBar
                for (int j = 0; j < stayCatList.Count; j++)
                {
                    //找到有小猫的位置
                    if (stayCatList[j].hasCat)
                    {
                        //当前看，应该把小猫放在哪个位置

                        for (int k = 0; k < j; k++)
                        {
                            //如果从头看，哪个位置为空，则把小猫放到该位置上
                            if (!stayCatList[k].hasCat)
                            {
                                Transform catTran = stayCatList[k].emptyObj.transform;

                                //移动小猫，并隐藏后续移除小猫在当前列表的对象
                                stayCatList[j].cat.GetComponent<LingdanCat>().moveCatToStay(catTran.position, stayCatList[j].catType, k, stayCatList[j].catNumber);//目标位置、小猫类型、停留位置、小猫序号

                                //清空移动了位置的小猫数据（保留空对象——底图）
                                stayCatList[j].hasCat = false;
                                stayCatList[j].catType = -1;
                                stayCatList[j].catNumber = -1;
                                stayCatList[j].cat = null;

                                break;
                            }
                        }



                    }
                }
            }
        }

    }

    /*添加了小猫后，可能有几种情况：
     * 1、炼丹任务未完成，需要继续添加小猫。但要记录哪些小猫被点击了，用于后续做清空处理——搞定
     * 2、炼丹任务刚好完成，需要判断哪些小猫应该被清空——搞定
     * 2.1、完成炼丹任务后，新增的炼丹任务的小猫，是否在目前stayBar有了，需要将小猫移动到任务上
     * 2.2、平移了stayBar后，仍然需要在stayBar内平移小猫，把前面的空位补齐
     * 
     * 
     * 备注：这里的诸多问题，始终还是时机和顺序的问题
     */

    //增加完成任务的小猫（丹炉，小猫序号，丹炉内第n只小猫）
    public void addTaskCat(Task fur, int cNumber, int cSite)
    {
        /*
        Debug.Log("完成任务的小猫序号：" + cNumber);
        Debug.Log("完成任务的小猫，所在丹炉内的序号：" + cSite);        

        //丹炉的位置
        int index = taskDataList.IndexOf(fur);
        //taskDataList[index].cats.Add(LingDanCatController.instance.catList[cNumber]);
        Debug.Log("完成任务的小猫，所在丹炉的序号：" + index);

        //改变丹炉内小猫的样式，同时清理原来的UI样式。如果小猫数量到达3个了，就不需要展示这个UI流程
        if (taskDataList[index].catchNumber < 2)
        {
            //StartCoroutine(changeCatUI(fur, index, cSite, cNumber));
        }else if (taskDataList[index].catchNumber >= 2)
        {
            Debug.Log("第"+ index + "个丹炉完成了收集任务");
            StartCoroutine(finishTask(fur, cNumber));
            
        }
        */
    }
    /*
     //改变丹炉上小猫的UI
     IEnumerator changeCatUI(Task fur,int fSite, int cSite, int cNumber)
     {
         Image catIcon = null;
         //把丹炉上小猫UI展示出来（避免边界条件）
         if (taskList.Count >= fSite + 1)
         {
              catIcon = taskList[fSite].transform.Find("MainObject").Find("CatPos" + cSite.ToString()).GetComponent<Image>();
             string path = "Materials/Cat/cat" + fur.catType.ToString();
             Sprite sprite = Resources.Load(path, typeof(Sprite)) as Sprite;
             catIcon.sprite = sprite;

             Color color = catIcon.color; // 获取当前颜色
             color.a = 1.0f; // 设置透明度
             catIcon.color = color; // 应用新颜色
         }

         if (catIcon != null)
         {
             catIcon.gameObject.SetActive(false);
         }

         yield return new WaitForSeconds(changeCatUITime); // 等待时间是为了：灵丹移动后才展示新的UI

         if (catIcon != null)
         {
             catIcon.gameObject.SetActive(true);
         }

        // StartCoroutine(destoryCatUI(cNumber));
     }


     IEnumerator destoryCatUI(int cNumber)
     {
         yield return new WaitForSeconds(0.1f);//抵达后才清理掉原本的灵丹UI。其实也不必

         //清理原本的小猫UI
         GameObject temp = LingDanCatController.instance.catList[cNumber];
         //Destroy(temp);
     }
    */

    //判断是否完成1个任务（完成任务的丹炉，需要清理的最后一个小猫）
    IEnumerator finishTask(Task fSite, int cNumber)
    {
        Debug.Log("进入完成任务等待时间");
        yield return new WaitForSeconds(finishTaskTime); //最后一个小猫移动到丹炉后，隐藏并销毁数据。随后开始播放动画并生产新的小猫UI

        //丹炉的位置
        int index = taskDataList.IndexOf(fSite);
        Debug.Log("判断完成任务：" + fSite);
        Debug.Log("判断完成任务的位置：" + index);
        Debug.Log("判断完成任务的数量：" + taskDataList.Count);
        Debug.Log("应该被清理的小猫数量：" + taskDataList[index].cats.Count);

        //播放清理最后一个小猫的UI
        //StartCoroutine(destoryCatUI(cNumber));



        //判断是否完成任务
        if (taskDataList[index].catchNumber >= 2)
        {
            Debug.Log("完成1个炼丹的任务");

            fPercent = (int)(curCatNumber / maxCatNumber * 100);

            Debug.Log("增加灵丹：" + (int)Mathf.Pow(10, taskDataList[index].lingdanLevel + 2));
            Debug.Log("灵丹等级：" + taskDataList[index].lingdanLevel);

            float factor = 0;//品质系数
            if (taskDataList[index].lingdanQuality == 0)
            {
                factor = 1.0f;
            }
            else if (taskDataList[index].lingdanQuality == 1)
            {
                factor = 1.2f;
            }
            else if (taskDataList[index].lingdanQuality == 2)
            {
                factor = 1.5f;
            }
            else if (taskDataList[index].lingdanQuality == 3)
            {
                factor = 2.0f;
            }

            float price = 0;
            //含价格飙升系数
            if (PriceController.instance != null)
            {
                price = (int)(Mathf.Pow(10, taskDataList[index].lingdanLevel + 1) * factor * PriceController.instance.price);
                sNumber += (int)price;
            }
            else
            {
                price = (int)(Mathf.Pow(10, taskDataList[index].lingdanLevel + 1) * factor);
                sNumber += (int)price;
            }

            //播放任务完成动画
            //StartCoroutine(PlayAniAndAudio(index, price));

        }


    }

    //播放完成任务隐藏丹炉的动效和音乐（序号，灵丹价格）
    IEnumerator PlayAniAndAudio(int index, float price)
    {
        //销毁该位置的丹炉，重新随机生成一个
        GameObject toDestory = taskList[index];
        if (toDestory != null)
        {

            //播放完成任务的动效
            toDestory.transform.Find("MainObject").GetComponent<EffectController>().scaleObject(false);
            toDestory.transform.Find("EffectObject").Find("Lingdan").gameObject.SetActive(true);
            toDestory.transform.Find("EffectObject").Find("Lingdan").GetComponent<EffectController>().scaleObject(true);
            toDestory.transform.Find("EffectObject").Find("Lingdan").Find("Text").GetComponent<TMP_Text>().text = "+" + ((int)price).ToString();

            //播放小猫音效
            AudioManager.instance.PlayAudio(finishTaskClip);

            //判断是否该清理小猫数据
            if (taskDataList[index].cats.Count >= 2)
            {
                Debug.Log("应该被清理的小猫数量：" + taskDataList[index].cats.Count);

                int catNumber = taskDataList[index].cats.Count;
                for (int i = catNumber - 1; i > 0; i--)
                {
                    Debug.Log("清空完成任务的小猫序号" + i);
                    //GameObject toDestory = taskDataList[index].cats[i];
                    taskDataList[index].cats.RemoveAt(i);
                    //curCatNumber--;
                }

            }

            //将完成任务的数据从list中移除

            taskDataList.RemoveAt(index);


            //等待1.5s后，删除UI对象数据
            yield return new WaitForSeconds(1.2f);


            curCatNumber -= 3;//把清理小猫数据和控制生成任务分离。原因见产生的新任务处
            taskList.RemoveAt(index);//问题，有可能存在部分丹炉重叠的情况，主要是因为点击清除任务较快，2个不同的丹炉重叠在了一个index上，后面新增的任务还没有生成，就往同一个位置叠加了丹炉。因此，挪到后面来

            //遗留问题，仍然无法解决

            Debug.Log("正在播放的灵丹序号为：" + index);
            toDestory.SetActive(false);
            taskToDestoryList.Add(toDestory);

            Resources.UnloadUnusedAssets();
            System.GC.Collect();

            //产生新的任务
            spawnTask(index);//问题，有可能遗留了3个小猫没有去处，是因为点击清理任务速度较快，还没有倒计时接触，就已经把当前小猫数量降到了12以下，导致无法产生更多小猫。改变判断条件即可
        }
    }
}


//最大的教训是，想把很多能力做的通用，却又不是各个地方都能通用，就做得很复杂。

[Serializable]
public class Task
{
    public int furnaceIcon;//丹炉icon
    public int lingdanLevel;//丹药阶数
    public int lingdanQuality;//丹药品质
    public int catType;//小猫icon类型
    public int catchNumber;//完成任务的数量
    public List<GameObject> cats;//已完成任务的三只小猫对象
}