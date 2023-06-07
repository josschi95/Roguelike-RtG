using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using JS.CharacterSystem;
using JS.ECS.Tags;
using UnityEngine;

namespace JS.ECS
{
    public static class EntityFactory
    {
        private static Dictionary<string, ObjectBlueprint> _blueprints;

        public static void LoadBlueprints()
        {
            _blueprints = new Dictionary<string, ObjectBlueprint>();

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

        private static void ProcessBlueprints(string fileName)
        {
            StreamReader reader = new StreamReader(fileName);
            string json = reader.ReadToEnd();
            Blueprint blueprints = JsonUtility.FromJson<Blueprint>(json);

            foreach(var blueprint in blueprints.Blueprints)
            {
                _blueprints[blueprint.Name] = blueprint;
                //CreateEntity(blueprint);
            }
        }

        private static Entity CreateEntity(ObjectBlueprint blueprint)
        {
            Entity entity;
            if (_blueprints.ContainsKey(blueprint.Inherits))
            {
                entity = CreateEntity(_blueprints[blueprint.Inherits]);
            }
            else entity = new Entity(blueprint.Name);

            //Debug.Log(blueprint.Name);
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
            if (!_blueprints.ContainsKey(blueprint)) return null;
            return CreateEntity(_blueprints[blueprint]);
        }
    }

    public class Blueprint
    {
        public ObjectBlueprint[] Blueprints;
    }

    public class ObjectBlueprint
    {
        public string Name;
        public string Inherits;
        public ComponentBlueprint[] Components;
        public StatBlueprint[] Stats;
        public TagBlueprint[] Tags;
    }

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

    public class TagBlueprint
    {
        public string Name;
        public string Value;
    }
}

