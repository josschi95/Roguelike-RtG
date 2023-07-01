using JS.WorldMap.Generation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
