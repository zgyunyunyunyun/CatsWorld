using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberController : MonoBehaviour
{
    public static NumberController instance;
    private void Awake()
    {
        instance = this;
    }

    //将数字转化为字符串（万、亿）
    public string NumberToChinaString(int number)
    {
        if(number >= 100000000)
        {
            return (number / 100000000).ToString()+"亿";
        }else if (number >= 10000)
        {
            return (number / 10000).ToString()+"万";
        }
        else
        {
            return number.ToString();
        }
    }

}
