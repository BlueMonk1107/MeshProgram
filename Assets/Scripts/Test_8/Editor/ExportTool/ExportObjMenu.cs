using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExportObjMenu  
{
    [MenuItem("Tools/导出模型/打开导出文件夹")]
    private static void OpenExportFolder()
    {
        if(!ExportUtil.CreateExportFolder())
            return;

        OpenFolder();
    }

    private static void OpenFolder()
    {
        System.Diagnostics.Process.Start("explorer.exe", ExportUtil.Table.ExportPath.Replace("/", "\\"));
    }

    private static bool Export(Func<int> logicFun)
    {
        if(!ExportUtil.CreateExportFolder())
            return false;

        Transform[] selectedTrans = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);
        if (selectedTrans.Length == 0)
        {
            EditorUtility.DisplayDialog("未选中模型", "请选中一个或多个模型", "关闭");
            return false;
        }

        int index = 0;
        if (logicFun != null)
        {
            index = logicFun();
        }

        if (index > 0)
        {
            if (EditorUtility.DisplayDialog("导出成功", "成功导出" + index + "个模型", "打开导出目录", "关闭"))
            {
                OpenFolder();
            }

            return true;
        }
        else
        {
            EditorUtility.DisplayDialog("错误", "导出失败", "关闭");
            return false;
        }
    }

    [MenuItem("Tools/导出模型/将选中模型分别导出")]
    private static void ExportAll()
    {
        bool sucess = Export(() =>
        {
            int index = 0;
            Transform[] selectedTrans = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);
            foreach (Transform selectedTran in selectedTrans)
            {
                MeshFilter[] filters = selectedTran.GetComponentsInChildren<MeshFilter>();

                string name = string.Format("{0}_{1}", selectedTran.name, index);

                ExportUtil.ExportObjsToOne(filters, ExportUtil.Table.ExportPath, name);
                index++;
            }

            return index;
        });
        
        if(!sucess)
            return;
    }
    
    [MenuItem("Tools/导出模型/将选中模型分别导出(子物体拆分导出)")]
    private static void ExportAllChild()
    {
        bool sucess = Export(() =>
        {
            int index = 0;
            Transform[] selectedTrans = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);
            foreach (Transform selectedTran in selectedTrans)
            {
                MeshFilter[] filters = selectedTran.GetComponentsInChildren<MeshFilter>();

                foreach (MeshFilter filter in filters)
                {
                    string name = string.Format("{0}_{1}_{2}", selectedTran.name,filter.name,index);

                    ExportUtil.ExportObjToOne(filter, ExportUtil.Table.ExportPath, name);
                    index++;
                }
            }

            return index;
        });
        
        if(!sucess)
            return;
    }
    
    [MenuItem("Tools/导出模型/将所有选中模型导出成一个obj")]
    private static void ExportToOne()
    {
        bool sucess = Export(() =>
        {
            int index = 0;
            Transform[] selectedTrans = Selection.GetTransforms(SelectionMode.Editable | SelectionMode.ExcludePrefab);
            List<MeshFilter> allFilters = new List<MeshFilter>();
            
            foreach (Transform selectedTran in selectedTrans)
            {
                MeshFilter[] filters = selectedTran.GetComponentsInChildren<MeshFilter>();

                allFilters.AddRange(filters);
            }
            
            string name = string.Format("{0}_{1}", SceneManager.GetActiveScene().name, index);

            ExportUtil.ExportObjsToOne(allFilters.ToArray(), ExportUtil.Table.ExportPath, name);
            index++;

            return index;
        });
        
        if(!sucess)
            return;
    }
}
