using UnityEngine;

namespace JS.ECS
{
    public class Transform : ComponentBase
    {
        #region - Position -
        public GridNode CurrentNode;

        /// <summary>
        /// Regional position within the world, ranging from 0 to world map size * region map size
        /// </summary>
        public Vector3Int Position;

        /// <summary>
        /// Position within current Local Map
        /// </summary>
        public Vector2Int LocalPosition;
        #endregion

        public override void OnRegistered() => TransformSystem.Register(this);
        public override void Disassemble() => TransformSystem.Unregister(this);
    }
}