using System;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace JS.ECS.Materials
{
    public static class MaterialManager
    {
        private static ObjectMaterial[] materials;

        public static void LoadMaterials(bool loadFromResources = true)
        {
            if (loadFromResources) LoadFromResources();
            else LoadFromFiles();
        }

        private static void LoadFromResources()
        {
            var textAsset = Resources.Load("Blueprints/Materials") as TextAsset;
            StringReader reader = new StringReader(textAsset.text);
            string json = reader.ReadToEnd();
            ProcessMaterials(json);
        }

        private static void LoadFromFiles()
        {
            string[] files = Directory.GetFiles(Application.persistentDataPath);

            string file = string.Empty;
            foreach (string fileName in files)
            {
                if (fileName.Contains("Materials"))
                {
                    file = fileName;
                    break;
                }
            }
            if (file == string.Empty) throw new Exception("Materials file not found!");

            StreamReader reader = new StreamReader(file);
            string json = reader.ReadToEnd();
            ProcessMaterials(json);
        }

        private static void ProcessMaterials(string json)
        {
            MaterialBlueprints mats = JsonUtility.FromJson<MaterialBlueprints>(json);
            materials = new ObjectMaterial[mats.Materials.Length];

            for (int i = 0; i < materials.Length; i++)
            {
                materials[i] = CreateMaterial(mats.Materials[i]);
            }

            foreach(var mat in materials)
            {
                //Debug.Log(mat.Name + ", HP: " + mat.HP + ", Hard: " + mat.Hardness + ", Weight: " + mat.WeightModifier + ", Cost: " + mat.CostModifier);
            }
        }

        private static ObjectMaterial CreateMaterial(MaterialBlueprint blueprint)
        {
            var newMaterial = new ObjectMaterial();

            newMaterial.Name = blueprint.Name;

            foreach(var property in blueprint.Properties)
            {
                var values = property.Split('=');
                FieldInfo field = newMaterial.GetType().GetField(values[0]) ?? throw new Exception("Unknown field " + values[0] + " in ObjectMaterial!");

                if (field.FieldType == typeof(string))
                {
                    if (field.FieldType.IsArray) field.SetValue(newMaterial, values[1].Split(","));
                    else field.SetValue(newMaterial, values[1]);
                }
                else if (field.FieldType == typeof(int))
                {
                    field.SetValue(newMaterial, int.Parse(values[1]));
                }
                else if (field.FieldType == typeof(float))
                {
                    field.SetValue(newMaterial, float.Parse(values[1]));
                }
            }

            return newMaterial;
        }

        public static ObjectMaterial GetMaterial(string name)
        {
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i].Name == name) return materials[i];
            }
            return null;
        }
    }

    [Serializable]
    public class MaterialBlueprints
    {
        public MaterialBlueprint[] Materials;
    }

    [Serializable]
    public class MaterialBlueprint
    {
        public string Name;
        public string[] Properties;
    }
}
