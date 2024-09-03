using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LingdanFadeAnimation : MonoBehaviour
{
    public bool isAnimate = false;//是否播放动画
    public float moveSpeed = 50;//移动速度
    public float sizeSpeed = 0.2f;//缩小速度
    float size = 1;
    Color newalph;

    public TMP_Text numberTextObj;
    private TMP_Text plusText;

    // Start is called before the first frame update
    void Start()
    {
        newalph = GetComponent<Image>().color;
        //初始化数字；后续移动数字
        if (plusText == null)
        {
            plusText = Instantiate(numberTextObj);

            plusText.transform.SetParent(transform, false);
            plusText.transform.localPosition = new Vector3(70, 10, 0);
            plusText.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isAnimate)
        {           
            //移动灵石
            size -= sizeSpeed;
            transform.Translate(0, moveSpeed * Time.deltaTime, 0);
            //transform.localScale = new Vector3(size, size , 0);
            newalph.a = size;
            GetComponent<Image>().color = newalph;
            plusText.gameObject.SetActive(true);

            //初始化后移动数字
            if (plusText != null)
            {
                plusText.transform.localPosition = new Vector3(200, 10, 0);

            }

            //隐藏灵石和数字
            if (size <= 0)
            {
                gameObject.SetActive(false);
                plusText.gameObject.SetActive(false);
            }
        }
    }
}
