using UnityEngine;

namespace JS.ECS
{
    public class Transform : ComponentBase
    {
        public delegate void OnTransformChanged();
        public OnTransformChanged onTransformChanged;

        public OnTransformChanged onWorldTileChanged;
        public OnTransformChanged onRegionTileChanged;
        public OnTransformChanged onLocalTileChanged;

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
                onTransformChanged?.Invoke();
                onWorldTileChanged?.Invoke();
            }
        }

        public Vector2Int RegionPosition
        {
            get => _regionPosition;
            set
            {
                _regionPosition = value;
                onTransformChanged?.Invoke();
                onRegionTileChanged?.Invoke();
            }
        }

        public Vector2Int LocalPosition
        {
            get => _localPosition; 
            set
            {
                _localPosition = value;
                onTransformChanged?.Invoke();
                onLocalTileChanged?.Invoke();
            }
        }

        public int Depth
        {
            get => _depth;
            set
            {
                _depth = Mathf.Clamp(value, -int.MaxValue, 1);
                onTransformChanged?.Invoke();
            }
        }

        public override void Disassemble()
        {
            base.Disassemble();
            onTransformChanged = null;
        }

        public override void FireEvent(Event newEvent)
        {
            //
        }
    }
}