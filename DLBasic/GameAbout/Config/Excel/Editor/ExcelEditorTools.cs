using DLBasic.Common;
using Excel;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
using System;
using EXECLConfig;

public class ExcelEditorTools
{
    public class ExcelConfig
    {
        #region ExcelBaseFun

        /// <summary>
        /// ���excel���ļ��еĵ�·����֮���Է���Editor����Ϊexcel����build��apk��
        /// </summary>
        public static readonly string excelsFolderPath = Application.dataPath + "/DLBasic/GameAbout/Config/Excel/Editor/Excels/";

        /// <summary>
        /// ���Excelת��CS�ļ����ļ���·�� ��Ĭ�ϴ�ţ�����ֶ�ȥ�ƶ�λ�ã�
        /// </summary>
        public static readonly string assetPath = "Assets/DLBasic/GameAbout/Config/Excel/Resources/";

        /// <summary>
        /// ��ȡexcel�ļ�����
        /// </summary>
        /// <param name="filePath">�ļ�·��</param>
        /// <param name="columnNum">����</param>
        /// <param name="rowNum">����</param>
        /// <returns></returns>
        public static DataRowCollection ReadExcel(string filePath, ref int columnNum, ref int rowNum)
        {
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet result = excelReader.AsDataSet();
            //Tables[0] �±�0��ʾexcel�ļ��е�һ�ű������
            columnNum = result.Tables[0].Columns.Count;
            rowNum = result.Tables[0].Rows.Count;
            return result.Tables[0].Rows;
        }

        public static LocalConfigItem[] CreateLocal(string filePath)
        {
            //��ñ����� 9��  92��
            int columnNum = 0, rowNum = 0;
            DataRowCollection collect = ReadExcel(filePath, ref columnNum, ref rowNum);
            //����excel�Ķ��壬�ڶ��п�ʼ��������

            int langCount = int.Parse(collect[0][0].ToString()) + 1;//����������
            List<LocalConfigItem> langs = new List<LocalConfigItem>();

            for (int i = 1; i < langCount; i++)
            {
                LocalConfigItem item = new LocalConfigItem();
                item.key = collect[0][i].ToString();

                for (int j = 1; j < rowNum; j++)
                {
                    item.AddStructs(collect[j][0].ToString(), collect[j][i].ToString());
                }

                langs.Add(item);
            }

            return langs.ToArray();
        }
        #endregion

    }
}
