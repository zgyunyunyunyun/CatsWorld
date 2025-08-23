using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneTransferData : MonoBehaviour
{
    public int getLingshiNumber;//����Ϸ���õ���ʯ

    public int maxCatLevel;//Сè��ߵȼ�
    private float putTimer = 1;//ÿ1s����һ�εȼ�����


    public float price = 1;//��ǰ�鵤�۸�ϵ��

    public float priceRestTimer = -1;//�۸���Ҫ���е���ʱ��ʱ��
    private float priceRestGap = 900;//�۸�ʣ��ʱ��ĵ���ʱ

    public bool isHighPirce = false;//��ʱ�Ƿ�߼۸�

    public float expendRestTimer = -1;//��չ������Ҫ���е���ʱ��ʱ��
    private float expendRestGap = 900;//����ʣ��ʱ��ĵ���ʱ

    public bool hasAttackEnemy = false;//��ʱ�Ƿ�����ĵ���


    public int outCatNumber = 0;//ÿ�δ���Ϸ���������Сè����

    public bool isConsumeStone = false;//�ж��Ƿ��Ѿ���������ʯ


    public static SceneTransferData instance;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;

            // ����������󲻱�����
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        outCatNumber = Random.Range(20, 100);
    }

    // Update is called once per frame
    void Update()
    {
        if(NewCatController.instance != null && NewCatController.instance.outCatNumber <= 0)
        {
            NewCatController.instance.outCatNumber = outCatNumber;
        }

        //չʾ�۸�UIʱ������ʱչʾ����
        if (isHighPirce && priceRestTimer >= 0)
        {
            priceRestTimer -= Time.deltaTime;

            if (priceRestTimer <= 0)
            {
                priceRestTimer = priceRestGap;

                isHighPirce = false;

                price = 1;
            }
        }

        //չʾ����UIʱ������ʱչʾ����
        if (hasAttackEnemy && expendRestTimer >= 0)
        {
            expendRestTimer -= Time.deltaTime;

            if (expendRestTimer <= 0)
            {
                expendRestTimer = expendRestGap;

                hasAttackEnemy = false;
            }
        }

        putTimer -= Time.deltaTime;
        if (putTimer <= 0)
        {
            putCatMaxLevel();
        }
    }

    public void putCatMaxLevel()
    {
        maxCatLevel = CatController.instance.getCatMaxLevel();
    }
}
