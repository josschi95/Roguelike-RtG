using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using JS.AssetOrganization;

namespace JS.Architecture.Primitives
{
    [CreateAssetMenu(menuName = AssetMenuSortOrders.PrimitivesPath + "Bool", order = AssetMenuSortOrders.PrimitivesOrder + 1)]
    public class BoolVariable : ScriptableObject
    {
        [FormerlySerializedAs("isTrue")]
        [SerializeField, ReadOnly] private bool _value;
        private bool _lastValue;

#if UNITY_EDITOR
        [SerializeField, Multiline]
        private string _developerNotes;
#endif
        public UnityEvent OnValueChangeCallbackEvent;
        public UnityEvent OnValueTrueCallbackEvent;
        public UnityEvent OnValueFalseCallbackEvent;

        public bool Value
        {
            get { return _value; }
            set
            {
                _value = value;
                if (_value != _lastValue)
                {
                    _lastValue = _value;
                    OnValueChangeCallbackEvent?.Invoke();

                    if (_value)
                        OnValueTrueCallbackEvent?.Invoke();
                    else
                        OnValueFalseCallbackEvent?.Invoke();
                }
            }
        }

#if UNITY_EDITOR
        public void Toggle()
        {
            Value = !Value;
        }
#endif
    }
}