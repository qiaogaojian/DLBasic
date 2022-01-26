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
        /// 存放excel表文件夹的的路径，之所以放在Editor是因为excel无需build到apk中
        /// </summary>
        public static readonly string excelsFolderPath = Application.dataPath + "/DLBasic/GameAbout/Config/Excel/Editor/Excels/";

        /// <summary>
        /// 存放Excel转化CS文件的文件夹路径 （默认存放，最好手动去移动位置）
        /// </summary>
        public static readonly string assetPath = "Assets/DLBasic/GameAbout/Config/Excel/Resources/";

        /// <summary>
        /// 读取excel文件内容
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="columnNum">行数</param>
        /// <param name="rowNum">列数</param>
        /// <returns></returns>
        public static DataRowCollection ReadExcel(string filePath, ref int columnNum, ref int rowNum)
        {
            FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.Read);
            IExcelDataReader excelReader = ExcelReaderFactory.CreateOpenXmlReader(stream);
            DataSet result = excelReader.AsDataSet();
            //Tables[0] 下标0表示excel文件中第一张表的数据
            columnNum = result.Tables[0].Columns.Count;
            rowNum = result.Tables[0].Rows.Count;
            return result.Tables[0].Rows;
        }

        public static LocalConfigItem[] CreateLocal(string filePath)
        {
            //获得表数据 9列  92行
            int columnNum = 0, rowNum = 0;
            DataRowCollection collect = ReadExcel(filePath, ref columnNum, ref rowNum);
            //根据excel的定义，第二行开始才是数据

            int langCount = int.Parse(collect[0][0].ToString()) + 1;//多少种语言
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
