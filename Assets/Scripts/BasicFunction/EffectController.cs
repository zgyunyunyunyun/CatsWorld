using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectController : MonoBehaviour
{
    public float shrinkDuration = 0.3f; // 动画的总时长，单位秒
    public float shrinkInterval = 0.005f; // 每次的间隔时间，单位秒
    public bool isAnimating = false;
    public bool animationFinished = false;

    // Start is called before the first frame update
    void Start()
    {
        isAnimating = false;
        animationFinished = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool AnimationFinished()
    {
        return animationFinished;
    }

    public bool AnimationBeginning()
    {
        return isAnimating;
    }

    //放大或者缩小物体（1，放大；0，缩小）
    public void scaleObject(bool isScale)
    {
        isAnimating = true;

        // 开始缩小动画
        StartCoroutine(ShrinkToHidden(isScale));
    }

    IEnumerator ShrinkToHidden(bool isScale)
    {
        // 获取初始大小
        Vector3 initialScale = transform.localScale;

        // 通过指定时间内逐渐缩小到无限小来隐藏物体
        float elapsedTime = 0;
        while (elapsedTime < shrinkDuration)
        {
            // 计算缩小的比例
            float scaleRatio = 0;
            if (isScale)
            {
                scaleRatio = 1 + elapsedTime / shrinkDuration;
            }
            else
            {
                scaleRatio = 1 - elapsedTime / shrinkDuration;
            }
            
            transform.localScale = new Vector3(initialScale.x * scaleRatio, initialScale.y * scaleRatio, initialScale.z * scaleRatio);

            // 等待间隔时间
            yield return new WaitForSeconds(shrinkInterval);
            elapsedTime += shrinkInterval;
            //Debug.Log("动画播放结束，设置对应的值1" + animationFinished + "；值2" + isAnimating);
        }

        // 最后将物体大小设置为0来隐藏
        if (!isScale)
        {
            transform.localScale = Vector3.zero;
        }

        //结束动画
        animationFinished = true;
        isAnimating = false;
        Debug.Log("动画播放结束，设置对应的值1" + animationFinished + "；值2" + isAnimating);
    }
}
