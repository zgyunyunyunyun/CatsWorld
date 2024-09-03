using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ObjectMove : MonoBehaviour
{
    public float speed = 0.1f;//移动速度
    private Rigidbody2D rb;
    private Vector3 movement;//移动方向

    public float timer = 0f;//每个x秒改变移动方向
    public float activity = 5f;//移动方向的距离，决定了移动的幅度

    private RectTransform rectTransform;

    public int objectType = -1;//0是灵液，1是杂质

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        //GetComponent<Button>().onClick.AddListener(() => addPointAndDestroy());


        //rectTransform = GetComponent<RectTransform>();//实现触摸效果

    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            //Debug.Log("Touch");
            //检测鼠标左键是否点击
            //Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            var onScreenPosition = Input.mousePosition;
            var ray = Camera.main.ScreenPointToRay(onScreenPosition);

            var hit = Physics2D.Raycast(new Vector2(ray.origin.x, ray.origin.y), Vector2.zero, Mathf.Infinity);
            if (hit.collider != null && gameObject == hit.collider.gameObject)
            {               
                Debug.Log("Touch");
                Debug.Log(hit.collider.gameObject.name);
                addPointAndDestroy();
                
                //hit.collider.gameObject.transform.position = ray.origin;
            }
        }

        //实现触摸效果，暂时屏蔽
        /*
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            Debug.Log("Touch");
            Touch touch = Input.GetTouch(0);
            Vector3 worldPosition;

            if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, touch.position, Camera.main, out worldPosition))
            {
                Vector2 localPoint;
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, touch.position, Camera.main, out localPoint))
                {
                    Vector2 localMousePos = new Vector2(localPoint.x, localPoint.y);
                    if (rectTransform.rect.Contains(localMousePos))
                    {
                        Debug.Log("Clicked on 2D Object!");
                    }
                }
            }
        }*/
    }

    private void FixedUpdate()
    {
        
        //计时器，每个xs，改变一次移动方向
        timer -= Time.deltaTime;
        if(timer <= 0)
        {
            movement = new Vector3(Random.Range(-0.7f * activity, 0.7f * activity), Random.Range(-1 * activity, 1 * activity), 0);
            timer = 5f;
        }
       
        rb.AddForce(movement * speed);
    }

    void addPointAndDestroy()
    {
        if(objectType == 0)
        {
            Debug.Log("收集灵液1滴!");
            UpLianDanController.instance.addPurityPoint(1f);
            Destroy(gameObject);
        }else if(objectType ==1)
        {
            Debug.Log("收集杂质1滴!");
            UpLianDanController.instance.addImpurityPoint(1f);
            Destroy(gameObject);
        }
        
    }
}
