using TMPro;
using UnityEngine;

public class NumberFadeAnimation : MonoBehaviour
{
    public bool isAnimate = false;//是否播放动画
    public float moveSpeed = 50;//移动速度
    public float sizeSpeed = 0.2f;//缩小速度
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
            //移动
            size -= sizeSpeed;


            //初始化数字；后续移动数字
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

            //隐藏灵石和数字
            if (size <= 0)
            {
                gameObject.SetActive(false);
                plusText.gameObject.SetActive(false);
            }
        }
    }
}
