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
        private static string[] paths = { "BaseObjects", "Items", "Creatures" };

        private static Dictionary<string, ObjectBlueprint> _objectBlueprints;

        public static Entity GetEntity(string blueprint)
        {
            if (blueprint == null) return null;
            if (!_objectBlueprints.ContainsKey(blueprint)) return null;
            return CreateEntity(_objectBlueprints[blueprint]);
        }

        #region - Loading Blueprints -
        public static void LoadBlueprints(bool loadFromResources = true)
        {
            _objectBlueprints = new Dictionary<string, ObjectBlueprint>();

            if (loadFromResources )
            {
                for (int i = 0; i < paths.Length; i++)
                {
                    LoadFromResources(paths[i]);
                }
                return;
            }

            string[] files = Directory.GetFiles(Application.persistentDataPath);

            for (int i = 0; i < paths.Length; i++)
            {
                foreach (string fileName in files)
                {
                    if (fileName.Contains(paths[i]))
                    {
                        ProcessBlueprints(fileName);
                        break;
                    }
                }
            }
        }

        private static void LoadFromResources(string pathName)
        {
            var textAsset = Resources.Load("Blueprints/" + pathName) as TextAsset;

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
                //Debug.Log(blueprint.Name);
                _objectBlueprints[blueprint.Name] = blueprint;
            }
        }
        #endregion

        #region - Entity Construction -
        private static Entity CreateEntity(ObjectBlueprint blueprint)
        {
            Entity entity;
            if (_objectBlueprints.ContainsKey(blueprint.Inherits))
            {
                entity = CreateEntity(_objectBlueprints[blueprint.Inherits]);
            }
            else entity = EntityManager.NewEntity(blueprint.Name);
            entity.Name = blueprint.Name;
            //Debug.Log(blueprint.Name);

            //Tags go first because they have the fewest dependencies
            if (blueprint.Tags != null) AddTags(entity, blueprint.Tags);

            AddComponents(entity, blueprint.Components);

            #region - Stats -
            if (blueprint.Stats != null)
            {
                foreach (var stat in blueprint.Stats)
                {
                    if (EntityManager.TryGetStat(entity, stat.Name, out var existingStat))
                    {

                    }
                    else
                    {
                        var newStat = new StatBase(stat.Name, stat.ShortName, stat.Value, stat.Potential, stat.MinValue, stat.MaxValue);
                        EntityManager.AddStat(entity, newStat);
                    }
                }
            }
            #endregion

            return entity;
        }

        /// <summary>
        /// Adds all tags listed in the blueprints to the entity.
        /// </summary>
        private static void AddTags(Entity entity, TagBlueprint[] tags)
        {
            foreach (var tag in tags)
            {
                //Check if the entity already has a tag of this type
                EntityManager.TryGetTagByName(entity, tag.Name, out var oldTag);

                //Remove the tag if marked to do so
                if (tag.Value != null && tag.Value == "removeTag")
                {
                    //Debug.Log("Removing Tag: " + tag.Name);
                    if (oldTag != null) EntityManager.RemoveTag(entity, oldTag);
                    continue;
                }
                //Else override the existing value
                else if (oldTag != null && tag.Value != null)
                {
                    //Debug.Log("Modifying tag: " + tag.Name);
                    oldTag.Value = tag.Value;
                    continue;
                }
                //Otherwise create a new tag and add it
                Type newTagType = Type.GetType("JS.ECS.Tags." + tag.Name) ?? throw new Exception("Unknown tag " + tag.Name + "!");
                object newTag = Activator.CreateInstance(newTagType);
                if (tag.Value != null)
                {
                    var value = newTagType.GetField("Value");
                    value.SetValue(newTag, tag.Value);
                }
                //Debug.Log("Adding new tag: " + tag.Name);
                EntityManager.AddTag(entity, (TagBase)newTag);
            }
        }

        /// <summary>
        /// Adds all components listed in the blueprints to the entity.
        /// </summary>
        private static void AddComponents(Entity entity, ComponentBlueprint[] components)
        {
            foreach (var component in components)
            {
                #region - Class -
                string ClassName = "JS.ECS." + component.Name;
                Type newComponentType = Type.GetType(ClassName) ?? 
                    throw new Exception("Unknown part " + ClassName + "!");

                object NewComponent = Activator.CreateInstance(newComponentType);

                bool isOverride = false;
                if (EntityManager.TryFindComponent(entity, (ComponentBase)NewComponent, out var existingComp))
                {
                    //Debug.Log("Existing component of type " + NewComponent.ToString() + " found");
                    NewComponent = existingComp;
                    isOverride = true;
                }
                #endregion

                //Continue early if parameters are empty, use base values
                if (component.Parameters == null)
                {
                    if (!isOverride) EntityManager.AddComponent(entity, (ComponentBase)NewComponent);
                    continue;
                }

                foreach (var parameter in component.Parameters)
                {
                    var values = parameter.Split('=');
                    FieldInfo info = newComponentType.GetField(values[0]) ?? throw new Exception("Unknown field " +
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
                        //Debug.Log(info.Name + " set to enum " + values[1]);
                        var e = (Enum)info.GetValue(NewComponent);
                        info.SetValue(NewComponent, Enum.Parse(e.GetType(), values[1]));
                    }
                }
                if (!isOverride) EntityManager.AddComponent(entity, (ComponentBase)NewComponent);
            }
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
}