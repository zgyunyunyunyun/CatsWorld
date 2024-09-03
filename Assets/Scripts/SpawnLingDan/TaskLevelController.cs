using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TaskLevelController : MonoBehaviour
{
    /*�ؿ�����
    * 
    */
    public int initMaxLevel = 5;//��ʼ���ؿ���
    public int spawnCatNumber = 10;//Сè�ĳ�ʼ������30,60��120,240,480(1,2,4,8,16)
    private int currLevel;//��ǰ�ؿ�����0��ʼ

    private int maxLevelCat;//Сè��ߵȼ�0-5
    private int currCatNumber;//��ǰ�ؿ�Сè������


    public GameObject gameFailedPanel;//��Ϸʧ����塪��δ���
    public GameObject gamePassPanel;//������Ϸͨ�����
    public GameObject gameFinishedPanel;//��Ϸȫ��ͨ�����

    public float judgeTime = 1f;//�ж���Ϸ�Ƿ����ʱ��


    public static TaskLevelController instance;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        currLevel = 0;
        currCatNumber = spawnCatNumber * 3 * (int)Mathf.Pow( 2, currLevel);
        maxLevelCat = 0;

        //��ʼ���Ծ֡���������Ҫ������Сè��������һ�ֵ�Сèδ�������ܱ���ӽ���������
        LingDanCatController.instance.SpawnCat(currCatNumber);
        TaskController.instance.startGame(currCatNumber, maxLevelCat);

        SceneTransferData.instance.getLingshiNumber = 0;//���ɴ��ݵ���ʯ��Ϊ0.

    }

    // Update is called once per frame
    void Update()
    {

    }

    //ͨ�ص�ǰ�ؿ�
    public void passedCurrentGame()
    {
        //�Ƿ�������йؿ�������Ϸ���������򣬵�����Ϸ����
        if (currLevel + 1 >= initMaxLevel)
        {
            //Debug.Log("������йؿ���Сè����������Ϸ��������");
            gameFinishedPanel.SetActive(true);
            //Time.timeScale = 0;
        }
        else
        {
            //Debug.Log("��ɵ�ǰ�ؿ�����Сè����������Ϸ��������");
            gamePassPanel.SetActive(true);
            //Time.timeScale = 0;
        }
        
    }

    //��һ��
    public void nextLevel()
    {
        Debug.Log("������һ��");

        Time.timeScale = 1;

        currLevel++;
        currCatNumber = spawnCatNumber * 3 * (int)Mathf.Pow(2, currLevel);

        //��ʼ����һ�Ծ�
        LingDanCatController.instance.SpawnCat(currCatNumber);
        TaskController.instance.startGame(currCatNumber, maxLevelCat);
        

    }

    //���¿�ʼ
    public void restartGame()
    {
        Debug.Log("���¿�ʼ��Ϸ");

        Time.timeScale = 1;

        //�ж��Ƿ������һ�ֵ�ͨ������
        if (gameFailedPanel.activeSelf)
        {
            gameFailedPanel.SetActive(false);
        }



        //��ʼ���Ծ�
        currCatNumber = spawnCatNumber * 3 * (int)Mathf.Pow(2, currLevel);
        Debug.Log("��ʼ��Ϸ��Ҫ������Сè������" + currCatNumber);
        LingDanCatController.instance.SpawnCat(currCatNumber);
        TaskController.instance.startGame(currCatNumber, maxLevelCat);
        

    }

    //��Ϸʧ�ܣ�����
    public void gameFailed()
    {
        Debug.Log("��Ϸ��������");

        StartCoroutine(waitEnd(judgeTime));
    }

    IEnumerator waitEnd(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        gameFailedPanel.SetActive(true);
        //Time.timeScale = 0;
    }

    //����ʯ������ӵ��ʲ���
    public void getLingshiAward()
    {
        Debug.Log("�����ʯ������������ϵ������" + TaskController.instance.sNumber);
        SceneTransferData.instance.getLingshiNumber += TaskController.instance.sNumber;
    }
}
