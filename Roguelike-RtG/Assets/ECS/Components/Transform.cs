using UnityEngine;

namespace JS.ECS
{
    public class Transform : ComponentBase
    {
        public GridNode currentNode;
        private Vector2Int _worldPosition;
        private Vector2Int _regionPosition;
        private Vector2Int _localPosition;
        private int _depth;

        public Vector2Int WorldPosition
        {
            get => _worldPosition;
            set
            {
                _worldPosition = value;
                entity.FireEvent(new TransformChanged());
            }
        }

        public Vector2Int RegionPosition
        {
            get => _regionPosition;
            set
            {
                _regionPosition = value;
                entity.FireEvent(new TransformChanged());
            }
        }

        public Vector2Int LocalPosition
        {
            get => _localPosition; 
            set
            {
                _localPosition = value;
                entity.FireEvent(new TransformChanged());
            }
        }

        public int Depth
        {
            get => _depth;
            set
            {
                _depth = Mathf.Clamp(value, -int.MaxValue, 1);
                entity.FireEvent(new TransformChanged());
            }
        }

        public override void OnEvent(Event newEvent)
        {
            //
        }
    }
}