using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using JS.CharacterSystem;
using UnityEngine;

namespace JS.ECS
{
    public static class EntityFactory
    {
        private static Dictionary<string, ObjectBlueprint> _objectBlueprints;

        public static void LoadBlueprints(bool loadFromResources = true)
        {
            _objectBlueprints = new Dictionary<string, ObjectBlueprint>();

            if (loadFromResources )
            {
                LoadFromResources();
                return;
            }

            string[] files = Directory.GetFiles(Application.persistentDataPath);
            foreach (string fileName in files)
            {
                if (fileName.Contains("EntityBlueprints"))
                {
                    ProcessBlueprints(fileName);
                    return;
                }
            }

            throw new Exception("Blueprints not found!");
        }

        private static void LoadFromResources()
        {
            var textAsset = Resources.Load("Blueprints/EntityBlueprints") as TextAsset;

            StringReader reader = new StringReader(textAsset.text);
            string json = reader.ReadToEnd();

            Blueprint blueprints = JsonUtility.FromJson<Blueprint>(json);

            foreach (var blueprint in blueprints.Blueprints)
            {
                //Debug.Log(blueprint.Name);
                _objectBlueprints[blueprint.Name] = blueprint;
            }
        }

        private static void ProcessBlueprints(string fileName)
        {
            StreamReader reader = new StreamReader(fileName);

            string json = reader.ReadToEnd();
            Blueprint blueprints = JsonUtility.FromJson<Blueprint>(json);

            foreach(var blueprint in blueprints.Blueprints)
            {
                Debug.Log(blueprint.Name);
                _objectBlueprints[blueprint.Name] = blueprint;
            }
        }

        private static Entity CreateEntity(ObjectBlueprint blueprint)
        {
            Entity entity;
            if (_objectBlueprints.ContainsKey(blueprint.Inherits))
            {
                entity = CreateEntity(_objectBlueprints[blueprint.Inherits]);
            }
            else entity = new Entity(blueprint.Name);
            //Debug.Log(blueprint.Name);

            #region - Components -
            foreach (ComponentBlueprint component in blueprint.Components)
            {
                #region - Class -
                string ClassName = "JS.ECS." + component.Name;
                Type newComponentType = Type.GetType(ClassName);

                if (newComponentType == null) throw new Exception("Unknown part " + ClassName + "!");

                object NewComponent = Activator.CreateInstance(newComponentType);

                bool isOverride = false;
                if (entity.HasComponent((ComponentBase)NewComponent, out var existingComp))
                {
                    Debug.Log("Existing component of type " + NewComponent.ToString() + " found");
                    NewComponent = existingComp;
                    isOverride = true;
                }
                #endregion
                
                //Continue early if parameters are empty, use base values
                if (component.Parameters == null)
                {
                    if (!isOverride) entity.AddComponent((ComponentBase)NewComponent);
                    continue;
                }

                foreach (var parameter in component.Parameters)
                {
                    var values = parameter.Split('=');
                    FieldInfo info = newComponentType.GetField(values[0]);
                    if (info == null) throw new Exception("Unknown field " + 
                        values[0] + " in " + newComponentType + "!");

                    if (info.FieldType == typeof(string))
                    {
                        //Debug.Log(info.Name + " set to string " + values[1]);
                        info.SetValue(NewComponent, values[1]);
                    }
                    else if (info.FieldType == typeof(bool))
                    {
                        //Debug.Log(info.Name + " set to bool " + values[1]);
                        info.SetValue(NewComponent, StringToBool(values[1]));
                    }
                    else if (info.FieldType == typeof(int))
                    {
                        //Debug.Log(info.Name + " set to int" + values[1]);
                        info.SetValue(NewComponent, int.Parse(values[1]));
                    }
                    else if (info.FieldType == typeof(float))
                    {

                        //Debug.Log(info.Name + " set to float " + values[1]);
                        info.SetValue(NewComponent, float.Parse(values[1]));
                    }
                    else if (info.FieldType.IsEnum)
                    {
                        var e = (Enum)info.GetValue(NewComponent);

                    }
                }
                if (!isOverride) entity.AddComponent((ComponentBase)NewComponent);
            }
            #endregion

            #region - Stats -
            if (blueprint.Stats != null)
            {
                foreach (var stat in blueprint.Stats)
                {
                    if (entity.TryGetStat(stat.Name, out var existingStat))
                    {

                    }
                    else
                    {
                        var newStat = new StatBase(stat.Name, stat.ShortName, stat.Value, stat.Potential, stat.MinValue, stat.MaxValue);
                        entity.AddStat(newStat);
                    }
                }
            }
            #endregion

            #region - Tags -
            if (blueprint.Tags != null)
            {
                foreach (var tag in blueprint.Tags)
                {
                    if (tag.Value != null && tag.Value == "removeTag")
                    {

                    }
                    else
                    {

                    }
                }
            }
            #endregion

            return entity;
        }

        private static bool StringToBool(string value)
        {
            if (value == null) return false;
            if (value.Equals("true")) return true;
            else if (value.Equals("True")) return true;
            return false;
        }

        public static Entity GetEntity(string blueprint)
        {
            if (blueprint == null) return null;
            if (!_objectBlueprints.ContainsKey(blueprint)) return null;
            return CreateEntity(_objectBlueprints[blueprint]);
        }
    }

    [Serializable]
    public class Blueprint
    {
        public ObjectBlueprint[] Blueprints;
    }

    [Serializable]
    public class ObjectBlueprint
    {
        public string Name;
        public string Inherits;
        public ComponentBlueprint[] Components;
        public StatBlueprint[] Stats;
        public TagBlueprint[] Tags;
    }

    [Serializable]
    public class ComponentBlueprint
    {
        public string Name;
        public string[] Parameters;

        public ComponentBlueprint(string name, string[] parameters)
        {
            Name = name;
            Parameters = parameters;
        }
    }

    [Serializable]
    public class StatBlueprint
    {
        public string Name;

        public string ShortName;
        public int Value;
        public int MinValue;
        public int Potential;
        public int MaxValue;

        public StatBlueprint(string name, string shortName, int value, int minValue, int potential, int maxValue)
        {
            Name = name;
            ShortName = shortName;
            Value = value;
            MinValue = minValue;
            Potential = potential;
            MaxValue = maxValue;
        }
    }

    [Serializable]
    public class TagBlueprint
    {
        public string Name;
        public string Value;
    }

    [Serializable]
    public class Anatomies
    {
        public AnatomyBlueprint[] anatomies;
    }

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
        public string AttachedTo;
        public string Laterality;
    }
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