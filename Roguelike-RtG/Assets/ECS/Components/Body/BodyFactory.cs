using System;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using UnityEngine;
using JS.ECS.Tags;
using System.Data.Common;

//*****************************************************************************//
//Note that I only really need to check the fields I plan on using in BodyPart //
//*****************************************************************************//

namespace JS.ECS
{
    public static class BodyFactory
    {
        private static Dictionary<string, BodyPartBlueprint> _bodyParts;
        private static Dictionary<string, BodyPartLayout[]> _anatomies;

        #region - Load Bodies -
        public static void LoadBodies(bool loadFromResources = true)
        {
            _bodyParts = new Dictionary<string, BodyPartBlueprint>();
            _anatomies = new Dictionary<string, BodyPartLayout[]>();

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

            foreach (var anatomy in bodies.Anatomies)
            {
                var partArray = new BodyPartLayout[anatomy.BodyParts.Length];

                for (int i = 0; i < partArray.Length; i++)
                {
                    partArray[i] = new BodyPartLayout();
                    partArray[i].Type = anatomy.BodyParts[i].Type;
                    partArray[i].Laterality = anatomy.BodyParts[i].Laterality;
                    partArray[i].AttachedTo = anatomy.BodyParts[i].AttachedTo;
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

            foreach (var anatomy in bodies.Anatomies)
            {
                var partArray = new BodyPartLayout[anatomy.BodyParts.Length];

                for (int i = 0; i < partArray.Length; i++)
                {
                    partArray[i] = new BodyPartLayout();
                    partArray[i].Type = anatomy.BodyParts[i].Type;
                    partArray[i].Laterality =  anatomy.BodyParts[i].Laterality;
                    partArray[i].AttachedTo = anatomy.BodyParts[i].AttachedTo;
                }

                _anatomies[anatomy.Name] = partArray;
            }
        }
        #endregion

        #region - Body Construction
        public static BodyPart GetNewBodyPart(string name)
        {
            if (_bodyParts.ContainsKey(name))
                return CreateNewBodyPart(_bodyParts[name]);
            return null;
        }

        private static BodyPart CreateNewBodyPart(BodyPartBlueprint blueprint)
        {
            BodyPart newPart;
            if (blueprint.Inherits != null && _bodyParts.ContainsKey(blueprint.Inherits))
            {
                newPart = CreateNewBodyPart(_bodyParts[blueprint.Inherits]);
                newPart.Type = (BodyPartType)Enum.Parse(typeof(BodyPartType), blueprint.Inherits);
            }
            else
            {
                newPart = new BodyPart();
                newPart.Type = (BodyPartType)Enum.Parse(typeof(BodyPartType), blueprint.Type);
            }
            newPart.Name = blueprint.Type;

            if (blueprint.Parameters == null) return newPart;
            foreach(var parameter in blueprint.Parameters)
            {
                var values = parameter.Split('=');

                if (values[0].Equals("Armor"))
                {
                    var slots = values[1].Split(',');
                    newPart.Armor = new ArmorSlot[slots.Length];
                    for (int i = 0; i < slots.Length; i++)
                    {
                        newPart.Armor[i] = new ArmorSlot(Enum.Parse<EquipmentSlot>(slots[i]), newPart);
                    }
                    continue;
                }
                else if (values[0].Equals("DefaultBehavior"))
                {
                    var entity = EntityFactory.GetEntity(values[1]) 
                        ?? throw new Exception("DefaultBehavior entity not found!");
                    
                    newPart.DefaultBehavior = entity;
                    continue;
                }

                FieldInfo info = newPart.GetType().GetField(values[0]) 
                    ?? throw new Exception("Unknown field " + values[0] + " in BodyPart!");

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
            }
            return newPart;
        }

        public static List<BodyPart> GetNewBody(string anatomy)
        {
            var parts = new List<BodyPart>();

            if (!_anatomies.ContainsKey(anatomy)) throw new Exception("Anatomy: " + anatomy + " not found!");

            foreach(var part in _anatomies[anatomy])
            {
                var newPart = CreateNewBodyPart(_bodyParts[part.Type]);
                if (part.Laterality != null) newPart.Laterality = (Laterality)Enum.Parse(typeof(Laterality), part.Laterality);
                parts.Add(newPart);

                if (part.AttachedTo == null) continue;

                //Attach the parts to its parent
                for (int i = 0; i < parts.Count; i++)
                {
                    if (parts[i].Type.ToString() != part.AttachedTo) continue;

                    if (parts[i].Laterality == Laterality.None)
                    {
                        newPart.AttachedTo = parts[i];
                        break;
                    }
                    else if (newPart.Laterality == parts[i].Laterality)
                    {
                        newPart.AttachedTo = parts[i];
                        break;
                    }
                }
            }

            return parts;
        }

        /// <summary>
        /// Builds the body for a creature based on its assigned anatomy.
        /// </summary>
        /// <param name="body"></param>
        public static void CreateAnatomy(Body body)
        {
            body.BodyParts = new List<BodyPart>();

            var parts = GetNewBody(body.Anatomy);
            body.BodyParts.AddRange(parts);

            //Set primary limb
            EntityManager.TryGetTag<PrimaryLimb>(body.entity, out var limb);
            for (int i = 0; i < parts.Count; i++)
            {
                if (parts[i].Type.ToString() == limb.Value)
                {
                    body.PrimaryLimb = parts[i];
                    break;
                }
            }

            body.ArmorSlots = new List<ArmorSlot>();
            foreach (var part in parts)
            {
                body.ArmorSlots.AddRange(part.Armor);
            }
            /*for (int i = 0; i < body.ArmorSlots.Count; i++)
            {
                Debug.Log(body.ArmorSlots[i].BodySlot.ToString() + " attached to " + body.ArmorSlots[i].Attachedto.Name);
            }*/

            BodySystem.Register(body);
        }
        #endregion

        private static bool StringToBool(string value)
        {
            if (value == null) return false;
            if (value.Equals("true")) return true;
            else if (value.Equals("True")) return true;
            return false;
        }
    }
}

/// <summary>
/// A class needed to serialize from JSON
/// </summary>
[Serializable]
public class Bodies
{
    public BodyPartBlueprint[] BodyParts;
    public AnatomyBlueprint[] Anatomies;
}

/// <summary>
/// A class which defines the body parts within a single anatomy.
/// </summary>
[Serializable]
public class AnatomyBlueprint
{
    public string Name;
    public BodyPartLayout[] BodyParts;
}

/// <summary>
/// A class that defines how each body part is connected within an anatomy.
/// </summary>
[Serializable]
public class BodyPartLayout
{
    public string Type; //the name of the body part blueprint
    public string Laterality; //laterality
    public string AttachedTo; //what it is attached to
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