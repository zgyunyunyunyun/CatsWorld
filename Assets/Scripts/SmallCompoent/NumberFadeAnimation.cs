using TMPro;
using UnityEngine;

public class NumberFadeAnimation : MonoBehaviour
{
    public bool isAnimate = false;//�Ƿ񲥷Ŷ���
    public float moveSpeed = 50;//�ƶ��ٶ�
    public float sizeSpeed = 0.2f;//��С�ٶ�
    float size = 1;

    public TMP_Text text;
    private TMP_Text plusText;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isAnimate)
        {
            //�ƶ�
            size -= sizeSpeed;


            //��ʼ�����֣������ƶ�����
            if (plusText == null)
            {
                plusText = Instantiate(text);

                plusText.transform.SetParent(transform, false);
                plusText.transform.localPosition = new Vector3(120, 10, 0);
            }
            else
            {
                plusText.transform.Translate(0, moveSpeed * Time.deltaTime, 0);

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
