using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CreateAssetMenu(fileName = "TableConfig",menuName = "CustomTable/Create TableConfig")]
public class Config : ScriptableObject
{
    public string TableFolderPath;
    public string TableFolderRelativePath;
}

