using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LingdanFadeAnimation : MonoBehaviour
{
    public bool isAnimate = false;//�Ƿ񲥷Ŷ���
    public float moveSpeed = 50;//�ƶ��ٶ�
    public float sizeSpeed = 0.2f;//��С�ٶ�
    float size = 1;
    Color newalph;

    public TMP_Text numberTextObj;
    private TMP_Text plusText;

    // Start is called before the first frame update
    void Start()
    {
        newalph = GetComponent<Image>().color;
        //��ʼ�����֣������ƶ�����
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
            //�ƶ���ʯ
            size -= sizeSpeed;
            transform.Translate(0, moveSpeed * Time.deltaTime, 0);
            //transform.localScale = new Vector3(size, size , 0);
            newalph.a = size;
            GetComponent<Image>().color = newalph;
            plusText.gameObject.SetActive(true);

            //��ʼ�����ƶ�����
            if (plusText != null)
            {
                plusText.transform.localPosition = new Vector3(200, 10, 0);

            }

            //������ʯ������
            if (size <= 0)
            {
                gameObject.SetActive(false);
                plusText.gameObject.SetActive(false);
            }
        }
    }
}
