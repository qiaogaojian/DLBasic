using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static ExcelEditorTools;

public class ExcelEditorWindow : EditorWindow
{
    [MenuItem("Tools/Excel Editor")]
    static public void OpenExcelEditorWindow()
    {
        GetWindow<ExcelEditorWindow>(false, "Excel Editor", true).Show();
    }

    public ExcleCfgDatas excleCfgDatas;


    void OnGUI()
    {
        excleCfgDatas = EditorGUILayout.ObjectField("excleCfgDatas", excleCfgDatas, typeof(ExcleCfgDatas), false) as ExcleCfgDatas;

        if (GUILayout.Button("AddLocalExcelData"))
        {
            excleCfgDatas.localConfigItems = ExcelConfig.CreateLocal(ExcelConfig.excelsFolderPath + "LocalConfig.xlsx");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Close();
        }
    }


}
