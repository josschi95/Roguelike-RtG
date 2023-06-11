using System;
using System.Collections.Generic;
using System.Data.Common;
using System.IO;
using System.Reflection;
using UnityEngine;

//*****************************************************************************//
//Note that I only really need to check the fields I plan on using in BodyPart //
//*****************************************************************************//

namespace JS.ECS
{
    public static class BodyFactory
    {
        private static Dictionary<string, BodyPartBlueprint> _bodyParts;
        private static Dictionary<string, BodyPartBlueprint[]> _anatomies;

        #region - Load Bodies -
        public static void LoadBodies(bool loadFromResources = true)
        {
            _bodyParts = new Dictionary<string, BodyPartBlueprint>();
            _anatomies = new Dictionary<string, BodyPartBlueprint[]>();

            if (loadFromResources) LoadFromResources();
            else LoadFromStreamingAssets();
        }

        private static void LoadFromResources()
        {
            var file = Resources.Load("Blueprints/Bodies") as TextAsset;

            StringReader reader = new StringReader(file.text);
            string json = reader.ReadToEnd();

            Bodies bodies = JsonUtility.FromJson<Bodies>(json);

            foreach(var part in bodies.BodyParts)
            {
                _bodyParts[part.Type] = part;
            }

            foreach (var anatomy in bodies.AllAnatomies)
            {
                var partArray = new BodyPartBlueprint[anatomy.BodyParts.Length];

                for (int i = 0; i < partArray.Length; i++)
                {
                    partArray[i] = _bodyParts[anatomy.BodyParts[i].Type];
                }

                _anatomies[anatomy.Name] = partArray;
            }
        }

        private static void LoadFromStreamingAssets()
        {
            string[] files = Directory.GetFiles(Application.persistentDataPath);
            foreach (string fileName in files)
            {
                if (fileName.Contains("Bodies"))
                {
                    ProcessBodies(fileName);
                    return;
                }
            }
            throw new Exception("Bodies not found!");
        }

        private static void ProcessBodies(string fileName)
        {
            StreamReader reader = new StreamReader(fileName);
            string json = reader.ReadToEnd();
            Bodies bodies = JsonUtility.FromJson<Bodies>(json);

            foreach (var part in bodies.BodyParts)
            {
                _bodyParts[part.Type] = part;
            }

            foreach (var anatomy in bodies.AllAnatomies)
            {
                var partArray = new BodyPartBlueprint[anatomy.BodyParts.Length];

                for (int i = 0; i < partArray.Length; i++)
                {
                    partArray[i] = _bodyParts[anatomy.BodyParts[i].Type];
                }

                _anatomies[anatomy.Name] = partArray;
            }
        }
        #endregion

        private static BodyPart CreateNewBodyPart(BodyPartBlueprint blueprint)
        {
            BodyPart newPart;
            if (blueprint.Inherits != null && _bodyParts.ContainsKey(blueprint.Inherits))
            {
                newPart = CreateNewBodyPart(_bodyParts[blueprint.Inherits]);
            }
            else newPart = new BodyPart();

            if (blueprint.Parameters == null) return newPart;

            var dict = new Dictionary<string, string>();
            foreach (var part in blueprint.Parameters)
            {
                var values = part.Split('=');
                 dict[values[0]] = values[1];
            }

            

            foreach(var parameter in blueprint.Parameters)
            {
                var values = parameter.Split('=');


                if (values[0].Equals("Armor"))
                {
                    var slots = values[1].Split(',');
                    newPart.Armor = new ArmorSlot[slots.Length];
                    for (int i = 0; i < slots.Length; i++)
                    {
                        newPart.Armor[i] = new ArmorSlot(Enum.Parse<BodySlot>(slots[i]));
                    }
                    continue;
                }

                FieldInfo info = newPart.GetType().GetField(values[0]);

                if (info == null) throw new Exception("Unknown field " + values[0] + " in BodyPart!");

                if (info.FieldType.IsArray)
                {
                    var elements = values[1].Split(",");
                    //info.SetValue(newPart, elements);

                    if (info.FieldType == typeof(string))
                    {
                        //Debug.Log(info.Name + " set to string " + values[1]);
                        info.SetValue(newPart, elements);
                    }
                    else if (info.FieldType == typeof(bool))
                    {
                        //Debug.Log(info.Name + " set to bool " + values[1]);
                        info.SetValue(newPart, StringsToBools(elements));
                    }
                    else if (info.FieldType == typeof(int))
                    {
                        //Debug.Log(info.Name + " set to int" + values[1]);
                        info.SetValue(newPart, int.Parse(values[1]));
                    }
                    else if (info.FieldType == typeof(float))
                    {
                        //Debug.Log(info.Name + " set to float " + values[1]);
                        info.SetValue(newPart, float.Parse(values[1]));
                    }
                    else if (info.FieldType.IsEnum)
                    {
                        //Debug.Log(info.Name + " set to enum " + values[1]);
                        var e = (Enum)info.GetValue(newPart);
                        info.SetValue(newPart, Enum.Parse(e.GetType(), values[1]));
                    }
                }
                else
                {
                    if (info.FieldType == typeof(string))
                    {
                        //Debug.Log(info.Name + " set to string " + values[1]);
                        info.SetValue(newPart, values[1]);
                    }
                    else if (info.FieldType == typeof(bool))
                    {
                        //Debug.Log(info.Name + " set to bool " + values[1]);
                        info.SetValue(newPart, StringToBool(values[1]));
                    }
                    else if (info.FieldType == typeof(int))
                    {
                        //Debug.Log(info.Name + " set to int" + values[1]);
                        info.SetValue(newPart, int.Parse(values[1]));
                    }
                    else if (info.FieldType == typeof(float))
                    {
                        //Debug.Log(info.Name + " set to float " + values[1]);
                        info.SetValue(newPart, float.Parse(values[1]));
                    }
                    else if (info.FieldType.IsEnum)
                    {
                        //Debug.Log(info.Name + " set to enum " + values[1]);
                        var e = (Enum)info.GetValue(newPart);
                        info.SetValue(newPart, Enum.Parse(e.GetType(), values[1]));
                    }
                    else if (info.FieldType == typeof(Entity))
                    {
                        info.SetValue(newPart, EntityFactory.GetEntity(values[1]));
                    }
                }
            }
            return newPart;
        }

        private static bool StringToBool(string value)
        {
            if (value == null) return false;
            if (value.Equals("true")) return true;
            else if (value.Equals("True")) return true;
            return false;
        }

        private static bool[] StringsToBools(string[] values)
        {
            var myBools = new bool[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                myBools[i] = StringToBool(values[i]);
            }
            return myBools;
        }

        public static BodyPart GetNewBodyPart(string name)
        {
            if (_bodyParts.ContainsKey(name))
                return CreateNewBodyPart(_bodyParts[name]);
            return null;
        }
    }
}

[Serializable]
public class Bodies
{
    public BodyPartBlueprint[] BodyParts;
    public AnatomyBlueprint[] AllAnatomies;
}

/// <summary>
/// A class which defines the body parts within a single anatomy.
/// </summary>
[Serializable]
public class AnatomyBlueprint
{
    public string Name;
    public BodyPartBlueprint[] BodyParts;
}

[Serializable]
public class BodyPartBlueprint
{
    public string Type;
    public string Inherits;
    public string[] Parameters;
}

/*
public enum Anatomy
{
    Humanoid,           //1 head, 2 arms and legs
    TailedHumanoid,     //1 head, 2 arms and legs, 1 tail
    Quadruped,          //1 head, 4 legs
    Centauroid,         //1 head, 2 arms, 4 legs
    Avian,              //1 head, 2 legs (wings are back slot)
    Ooze,               //literally nothing
    Insectoid,          //this can be so many things I have no idea yet
    Arachnid,           //1 head, 8 legs
    Gastropod,          //1 head, 1 tail
    ArmedGastropod,     //1 head, 2 arms, 1 tail
    Eyeball,            //1 head? is that even separate?
    Dragon,             //1 head, 4 legs... wings go on back... so how is this different from Quadruped? I guess it would add by default?
}*/