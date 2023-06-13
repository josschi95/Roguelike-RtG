using UnityEngine;

namespace JS.ECS
{
    /// <summary>
    /// Component for all physical objects which can be interacted with
    /// </summary>
    public class Physics : ComponentBase
    {
        public Physics() { }

        public bool IsTakeable = true; //Can the object be picked up
        public bool IsSolid = false; //Does the object block gas from spreading and line of sight
        public bool IsCorporeal = true;
        public bool IsReal = true; //False for some magical effects, visual effects, etc.
        public float Weight = 1.0f; //The weight of the object

        public PhysicsCategory Category = PhysicsCategory.Miscellaneous; //The type of object

        #region - Position -
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
                    EntityManager.FireEvent(entity, new TransformChanged());
                }
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
        #endregion

        private void OnNodeChange(Vector2Int newPos)
        {
            CurrentNode.Entities.Remove(entity); //remove from previous node
            //if (!entity.GetTag<PlayerTag>()) Debug.Log("entity move to " + newPos);

            _localPosition = newPos;
            //var newNode = CurrentNode.grid.GetGridObject(newPos.x, newPos.y);

            _currentNode = CurrentNode.grid.GetGridObject(newPos.x, newPos.y);
            _currentNode.Entities.Add(entity);

            //if (!entity.GetTag<PlayerTag>()) Debug.Log("entity move to " + CurrentNode.x + "," + CurrentNode.y);

            EntityManager.FireEvent(entity, new TransformChanged());
        }

        public override void OnEvent(Event newEvent)
        {
            switch (newEvent)
            {
                case GetStat stat:
                    OnGetStat(stat);
                    break;
                case TakeDamage damage:
                    OnTakeDamage(damage);
                    break;
                case Death death:
                    OnDeath();
                    break;
            }
        }

        private void OnGetStat(GetStat stat)
        {
            if (EntityManager.TryGetStat(entity, stat.Name, out var value))
            {
                stat.Value += value.Value;
            }
        }

        private void OnTakeDamage(TakeDamage damage)
        {
            EntityManager.TryGetStat(entity, "HP", out var stat);

            foreach (var key in damage.Damage.Keys)
            {
                if (stat.CurrentValue <= 0) return;

                var E1 = new GetStat(key + "Resistance");
                EntityManager.FireEvent(entity, E1);

                int net = Mathf.RoundToInt(damage.Damage[key] * Resistance.GetModifier(E1.Value));
                stat.CurrentValue -= net;

                if (stat.CurrentValue <= 0) EntityManager.FireEvent(entity, new Death());
            }
        }

        private void OnDeath()
        {
            MessageSystem.NewMessage(entity.Name + " has been killed");
            CorpseManager.OnCreatureDeath(entity);
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
