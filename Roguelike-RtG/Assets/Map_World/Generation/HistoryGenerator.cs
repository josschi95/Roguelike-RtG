using System.Collections;
using UnityEngine;

namespace JS.World.Map.Generation
{
    public class HistoryGenerator : MonoBehaviour
    {
        [SerializeField] private SettlementGenerator settlementGenerator;

        public IEnumerator RunHistory(int years)
        {
            int currentYear = 0;
            while (currentYear < years)
            {
                settlementGenerator.RunSettlementHistory(currentYear);
                currentYear++;
                yield return null;
            }

            settlementGenerator.FinalizeSettlements();
        }
    }
}