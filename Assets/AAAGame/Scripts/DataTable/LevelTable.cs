//------------------------------------------------------------
//------------------------------------------------------------
// 此文件由工具自动生成，请勿直接修改。
// 生成时间：__DATA_TABLE_CREATE_TIME__
//------------------------------------------------------------

using GameFramework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityGameFramework.Runtime;
#if ENABLE_OBFUZ
[Obfuz.ObfuzIgnore(Obfuz.ObfuzScope.TypeName | Obfuz.ObfuzScope.MethodName)]
#endif
/// <summary>
/// 关卡表
/// </summary>
public class LevelTable : DataRowBase
{
	private int m_Id = 0;
	/// <summary>
    /// 
    /// </summary>
    public override int Id
    {
        get { return m_Id; }
    }

        /// <summary>
        /// 关卡prefab名
        /// </summary>
        public string LvPfbName
        {
            get;
            private set;
        }

        /// <summary>
        /// 初始槽数
        /// </summary>
        public int SlotCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 鱼数量
        /// </summary>
        public int FishCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 每种小猫卡片出现的次数
        /// </summary>
        public int RepeatCount
        {
            get;
            private set;
        }

        /// <summary>
        /// 小猫堆配置，每个数组表示一层，数组第一个元素为列数，第二个元素为行数，第三个元素为相对下面一层的水平偏移（0.5表示错开半格，0表示对齐，1表示偏移1格），第四个元素为相对下面一层的垂直偏移
        /// </summary>
        public float[][] Layers
        {
            get;
            private set;
        }

        public override bool ParseDataRow(string dataRowString, object userData)
        {
            string[] columnStrings = dataRowString.Split(DataTableExtension.DataSplitSeparators);
            for (int i = 0; i < columnStrings.Length; i++)
            {
                columnStrings[i] = columnStrings[i].Trim(DataTableExtension.DataTrimSeparators);
            }

            int index = 0;
            index++;
            m_Id = int.Parse(columnStrings[index++]);
            index++;
            LvPfbName = columnStrings[index++];
            SlotCount = int.Parse(columnStrings[index++]);
            FishCount = int.Parse(columnStrings[index++]);
            RepeatCount = int.Parse(columnStrings[index++]);
            Layers = DataTableExtension.Parse2DArray<float>(columnStrings[index++]);

            return true;
        }

        public override bool ParseDataRow(byte[] dataRowBytes, int startIndex, int length, object userData)
        {
            using (MemoryStream memoryStream = new MemoryStream(dataRowBytes, startIndex, length, false))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))
                {
                    m_Id = binaryReader.Read7BitEncodedInt32();
                    LvPfbName = binaryReader.ReadString();
                    SlotCount = binaryReader.Read7BitEncodedInt32();
                    FishCount = binaryReader.Read7BitEncodedInt32();
                    RepeatCount = binaryReader.Read7BitEncodedInt32();
                    Layers = binaryReader.Read2DArray<float>();
                }
            }

            return true;
        }
}
