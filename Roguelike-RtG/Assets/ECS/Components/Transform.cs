using UnityEngine;

namespace JS.ECS
{
    public class Transform : ComponentBase
    {
        public delegate void OnTransformChanged();
        public OnTransformChanged onTransformChanged;

        private Vector2 _worldPosition;
        private Vector2 _regionPosition;
        private Vector2 _localPosition;

        public Vector2 WorldPosition
        {
            get => _worldPosition;
            set
            {
                _worldPosition = value;
                onTransformChanged?.Invoke();
            }
        }

        public Vector2 RegionPosition
        {
            get => _regionPosition;
            set
            {
                _regionPosition = value;
                onTransformChanged?.Invoke();
            }
        }

        public Vector2 LocalPosition
        {
            get => _localPosition; 
            set
            {
                _localPosition = value;
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

public enum PositionLevel
{
    Local,
    Region,
    World,
}