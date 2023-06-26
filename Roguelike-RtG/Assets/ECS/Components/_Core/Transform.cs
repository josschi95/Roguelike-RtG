using UnityEngine;

namespace JS.ECS
{
    public class Transform : ComponentBase
    {
        #region - Position -
        public GridNode CurrentNode;

        public Vector3Int WorldPosition { get; set; }

        /// <summary>
        /// Regional position within the world, ranging from 0 to world map size * region map size
        /// </summary>
        public Vector2Int RegionPosition { get; set; }

        /// <summary>
        /// Position within current Local Map
        /// </summary>
        public Vector2Int LocalPosition { get; set; }
        #endregion

        public override void OnRegistered() => TransformSystem.Register(this);
        public override void Disassemble() => TransformSystem.Unregister(this);
    }
}