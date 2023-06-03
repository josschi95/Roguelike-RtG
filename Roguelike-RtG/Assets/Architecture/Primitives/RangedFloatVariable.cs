using UnityEngine;
using JS.AssetOrganization;

namespace JS.Primitives
{
	[CreateAssetMenu(menuName = AssetMenuSortOrders.PrimitivesPath + "Ranged Float", fileName = "RangedFloat", order = AssetMenuSortOrders.PrimitivesOrder + 5)]
	public class RangedFloatVariable : ScriptableObject
	{
		public FloatReference MinValue;
		public FloatReference MaxValue;
	}
}