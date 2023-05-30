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

        public MapLevel MapLevel { get; private set; } = MapLevel.Local;

        private Vector2Int _worldPosition;
        private Vector2Int _regionPosition;
        private Vector2Int _localPosition;

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

public enum MapLevel
{
    Local,
    World,
}