using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

public class MaterialData  
{
	private StringBuilder _data;
	private Dictionary<string, MaterialModel> _materialMap;
	public Dictionary<string, string> Paths { get; private set; }

	public MaterialData(MeshFilter[] filters)
	{
		_data = new StringBuilder();
		_materialMap = new Dictionary<string, MaterialModel>();
		Paths = new Dictionary<string, string>();

		foreach (MeshFilter filter in filters)
		{
			SaveMaterialsAndFace(filter, _materialMap);
			SaveMaterials(_data, _materialMap,Paths);
		}
	}
	
	private void SaveMaterialsAndFace(MeshFilter filter,Dictionary<string, MaterialModel> materialMap)
	{
		Material[] materials = filter.GetComponent<Renderer>().materials;
		for (int i = 0; i < filter.mesh.subMeshCount; i++)
		{
			string materialName = materials[i].name;

			if (!materialMap.ContainsKey(materialName))
			{
				MaterialModel mData = new MaterialModel();
				mData.Name = materialName;
				mData.TexturePath =
					materials[i].mainTexture ? AssetDatabase.GetAssetPath(materials[i].mainTexture) : null;
                
				materialMap.Add(mData.Name,mData);
			}
		}
	}

	private void SaveMaterials(StringBuilder data,Dictionary<string, MaterialModel> materialMap,Dictionary<string, string> paths)
	{
		foreach (var model in materialMap)
		{
			data.Append("newmtl ")
				.Append(model.Key)
				.Append("\n")
				.Append("Ka 1 1 1\n")
				.Append("Kd 1 1 1\n")
				.Append("Ks 1 1 1\n")
				.Append("Ni 1\n")
				.Append("Ns 500\n")
				.Append("Tf 1 1 1\n")
				.Append("d 1\n");
			
			if (model.Value.TexturePath != null)
			{
				string name = Path.GetFileName(model.Value.TexturePath);
			
				data.Append("map_Kd ")
					.Append(name)
					.Append("\n");
				
				if(!paths.ContainsKey(name))
					paths.Add(name,model.Value.TexturePath);
			}
		}
	}
	
	public override string ToString()
	{
		return _data.ToString();
	}
}

public class MaterialModel
{
	public string Name;
	public string TexturePath;
}
