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

    //将数字转化为字符串（万、亿），保留以为小数，如101000转换为10.1万
    public string NumberToChinaString(int number)
    {
        string str;
        if (number >= 100000000)
        {
            str = (number / 100000000.0f).ToString("F1") + "亿";
        }
        else if (number >= 10000)
        {
            str = (number / 10000.0f).ToString("F1") + "万";
        }
        else
        {
            str = number.ToString();
        }

        return str;
    }
}
