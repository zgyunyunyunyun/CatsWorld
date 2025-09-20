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
/// LayerTable
/// </summary>
public class LayerTable : DataRowBase
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
        /// 各层每行小猫数量，4,4,4表示该层有三行，每行4只小猫
        /// </summary>
        public int[] CatNum
        {
            get;
            private set;
        }

        /// <summary>
        /// 每行小猫间距
        /// </summary>
        public float[] RowGap
        {
            get;
            private set;
        }

        /// <summary>
        /// 每行小猫起始位置
        /// </summary>
        public Vector3[] StartPos
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
            CatNum = DataTableExtension.ParseArray<int>(columnStrings[index++]);
            RowGap = DataTableExtension.ParseArray<float>(columnStrings[index++]);
            StartPos = DataTableExtension.ParseVector3Array(columnStrings[index++]);

            return true;
        }

        public override bool ParseDataRow(byte[] dataRowBytes, int startIndex, int length, object userData)
        {
            using (MemoryStream memoryStream = new MemoryStream(dataRowBytes, startIndex, length, false))
            {
                using (BinaryReader binaryReader = new BinaryReader(memoryStream, Encoding.UTF8))
                {
                    m_Id = binaryReader.Read7BitEncodedInt32();
                    CatNum = binaryReader.ReadArray<int>();
                    RowGap = binaryReader.ReadArray<float>();
                    StartPos = binaryReader.ReadVector3Array();
                }
            }

            return true;
        }
}
