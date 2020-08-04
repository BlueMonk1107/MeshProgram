using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class ExportUtil
{
    private static ExportObjTable _exportTable;
    private static Config _config;

    private const string DEFAULT_TABLE_PATH = "Assets/Scripts/Test_8/Table/";
    private const string DEFAULT_CONFIG_NAME = "Config.asset";
    private const string DEFAULT_EXPORT_TABLE_NAME = "ExportObjTable.asset";
    private const string EXPORT_FOLDER_NAME = "ExportObj";

    public static ExportObjTable Table
    {
        get
        {
            if (_config == null)
            {
                if (!InitConfig())
                {
                    EditorUtility.DisplayDialog("错误", "Config数据资源加载失败", "关闭");
                    return null;
                }
            }

            if (_exportTable == null)
            {
                string path = _config.TableFolderPath + DEFAULT_EXPORT_TABLE_NAME;
                string relative = _config.TableFolderRelativePath + DEFAULT_EXPORT_TABLE_NAME;
                if (!File.Exists(path))
                {
                    ExportObjTable table = ScriptableObject.CreateInstance<ExportObjTable>();
                    
                    string root = Application.dataPath.Remove(Application.dataPath.Length - 6);
                    table.ExportPath = root + EXPORT_FOLDER_NAME + "/";
                    
                    AssetDatabase.CreateAsset(table,relative);
                    _exportTable = table;
                }
                else
                {
                    _exportTable = AssetDatabase.LoadAssetAtPath<ExportObjTable>(relative);
                }
            }

            if (_exportTable == null)
            {
                EditorUtility.DisplayDialog("错误", "ExportObjTable数据资源加载失败", "关闭");
            }

            return _exportTable;
        }
    }

    private static bool InitConfig()
    {
        string root = Application.dataPath.Remove(Application.dataPath.Length - 6);
        string configFolderPath = root + DEFAULT_TABLE_PATH;
        if (!Directory.Exists(configFolderPath))
            Directory.CreateDirectory(configFolderPath);

        string configPath = configFolderPath + DEFAULT_CONFIG_NAME;
        string relativePath = DEFAULT_TABLE_PATH + DEFAULT_CONFIG_NAME;
        if (!File.Exists(configPath))
        {
            Config config = ScriptableObject.CreateInstance<Config>();
            config.TableFolderPath = configFolderPath;
            config.TableFolderRelativePath = DEFAULT_TABLE_PATH;
            AssetDatabase.CreateAsset(config,relativePath);
            _config = config;
        }
        else
        {
            _config = AssetDatabase.LoadAssetAtPath<Config>(relativePath);
        }

        return _config != null;
    }

    public static void ExportObjToOne(MeshFilter filter,string folderPath,string objName)
    {
        var filters = new[] {filter};
        CreateObj(filters, folderPath, objName);
        CreateMtl(filters, folderPath, objName);
    }

    public static void ExportObjsToOne(MeshFilter[] filters,string folderPath,string objName)
    {
        CreateObj(filters, folderPath, objName);
        CreateMtl(filters, folderPath, objName);
    }

    private static void CreateObj(MeshFilter[] filters,string folderPath,string objName)
    {
        MeshData data = new MeshData(filters);

        using (StreamWriter sw = new StreamWriter(string.Format("{0}{1}.obj",folderPath,objName)))
        {
            sw.Write("mtllib {0}.mtl\n",objName);
            sw.Write(data.ToString());
        }
    }

    private static void CreateMtl(MeshFilter[] filters,string folderPath,string objName)
    {
        MaterialData data = new MaterialData(filters);
        using (StreamWriter sw = new StreamWriter(string.Format("{0}{1}.mtl",folderPath,objName)))
        {
            sw.Write(data.ToString());
        }

        foreach (var pathPair in data.Paths)
        {
            if(!File.Exists(folderPath+pathPair.Key))
                File.Copy(pathPair.Value,folderPath+pathPair.Key);
        }
    }


    public static bool CreateExportFolder()
    {
        if (string.IsNullOrEmpty(Table.ExportPath))
        {
            EditorUtility.DisplayDialog("错误", "导出目录路径为空", "关闭");
            return false;
        }
        
        try
        {
            Directory.CreateDirectory(Table.ExportPath);
        }
        catch (Exception e)
        {
            EditorUtility.DisplayDialog("错误", "创建导出文件夹失败", "关闭");
            return false;
        }

        return true;
    }
}
