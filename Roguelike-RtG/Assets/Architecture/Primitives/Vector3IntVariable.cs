using UnityEngine;
using UnityEngine.Events;
using JS.AssetOrganization;

namespace JS.Architecture.Primitives
{
    [CreateAssetMenu(menuName = AssetMenuSortOrders.PrimitivesPath + "Vector3Int", fileName = "Vector3Int", order = AssetMenuSortOrders.PrimitivesOrder + 7)]
    public class Vector3IntVariable : ScriptableObject
    {
        public Vector3Int Value;
        public Vector3Int DefaultValue;
        private Vector3Int _lastValue;

        public UnityEvent OnValueChangeCallbackEvent;

#if UNITY_EDITOR
#pragma warning disable 0414
        [SerializeField, TextArea]
        private string _developerNotes = "";
#pragma warning restore 0414
#endif

        public void ResetValue()
        {
            SetValue(DefaultValue);
        }

        public void SetValue(Vector3Int value)
        {
            Value = value;
            ValueChanged();
        }

        public void SetValue(Vector3IntVariable value)
        {
            Value = value.Value;
            ValueChanged();
        }

        void ValueChanged()
        {
            if (Value != _lastValue)
            {
                _lastValue = Value;
                OnValueChangeCallbackEvent?.Invoke();
            }
        }
    }
}

