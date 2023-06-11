using JS.ECS.Tags;
using System;
using UnityEngine;

namespace JS.ECS
{
    /// <summary>
    /// Component for all physical objects which can be interacted with
    /// </summary>
    public class Physics : ComponentBase
    {
        public Physics() { }

        public Physics(bool takeable = true, bool solid = false, float weight = 1.0f, PhysicsCategory category = PhysicsCategory.Miscellaneous)
        {
            IsTakeable = takeable;
            IsSolid = solid;
            Weight = weight;
            Category = category;
        }

        public bool IsTakeable = true; //Can the object be picked up
        public bool IsSolid = false; //Does the object block gas from spreading and line of sight
        public bool IsCorporeal = true;
        public bool IsReal = true; //False for some magical effects, visual effects, etc.
        public float Weight = 1.0f; //The weight of the object
        public PhysicsCategory Category = PhysicsCategory.Miscellaneous; //The type of object

        private GridNode _currentNode;
        public GridNode CurrentNode
        {
            get
            {
                if (_currentNode == null)
                {
                    var grid = GridManager.GetGrid(Position);
                    _currentNode = grid.Grid.GetGridObject(_localPosition.x, _localPosition.y);
                }
                return _currentNode;
            }
        }
        private Vector3Int _position;
        /// <summary>
        /// Regional position within the world, ranging from 0 to world map size * region map size
        /// </summary>
        public Vector3Int Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    entity.FireEvent(new TransformChanged());
                }
            }
        }

        /// <summary>
        /// Position as displayed on the world map
        /// </summary>
        public Vector3Int WorldMapPosition
        {
            get
            {
                int x = Mathf.FloorToInt(_position.x / 3);
                int y = Mathf.FloorToInt(_position.y / 3);
                return new Vector3Int(x, y, _position.z);
            }
        }

        private Vector2Int _localPosition;
        /// <summary>
        /// Position within current Local Map
        /// </summary>
        public Vector2Int LocalPosition
        {
            get => _localPosition;
            set
            {
                if (_localPosition != value) OnNodeChange(value);
            }
        }

        private void OnNodeChange(Vector2Int newPos)
        {
            CurrentNode.Entities.Remove(entity); //remove from previous node
            //if (!entity.GetTag<PlayerTag>()) Debug.Log("entity move to " + newPos);

            _localPosition = newPos;
            var newNode = CurrentNode.grid.GetGridObject(newPos.x, newPos.y);

            _currentNode = newNode;
            _currentNode.Entities.Add(entity);

            //if (!entity.GetTag<PlayerTag>()) Debug.Log("entity move to " + CurrentNode.x + "," + CurrentNode.y);

            entity.FireEvent(new TransformChanged());
        }

        public override void OnEvent(Event newEvent)
        {

        }
    }
}

public enum PhysicsCategory
{
    Miscellaneous,
    Creature,
    Item,
    Wall,
    MeleeWeapon,
    MissileWeapon,
    Armor,
    NaturalWeapon,
    NaturalMissileWeapon,
    NaturalArmor,
}

public enum DamageTypes
{
    Blunt,
    Pierce,
    Slash,
    Bleed,

    Fire,
    Frost,
    Lightning,

    Poison,
    Acid,

    Sonic,
    Psychic,
    Mystic,

    Positive,
    Negative,
}
