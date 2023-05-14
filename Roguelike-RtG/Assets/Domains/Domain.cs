using UnityEngine;

namespace JS.DomainSystem
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Domains/New Domain")]
    public class Domain : ScriptableObject
    {
        [ReadOnly] public int ID;

        [SerializeField] private Domain[] parentDomains;
        [SerializeField] private Domain[] childDomains;

        [Space]

        [SerializeField] private Domain[] alignedDomains;
        [SerializeField] private Domain[] opposedDomains;

        [Space]
        [Space]

        [SerializeField] private string[] domainAdjectives;
        [SerializeField] private string[] domainNouns;


        public Domain[] ParentDomains => parentDomains;
        public Domain[] ChildDomains => childDomains;
        public Domain[] AlignedDomains => alignedDomains;
        public Domain[] OpposedDomains => opposedDomains;
        public string[] DomainAdjectives => domainAdjectives;
        public string[] DomainNouns => domainNouns;

        public void SetProperties(Domain[] parents, Domain[] children, Domain[] allies, Domain[] opponents, string[] adjectives, string[] nouns)
        {
            parentDomains = parents;
            childDomains = children;

            alignedDomains = allies;
            opposedDomains = opponents;

            domainAdjectives = adjectives;
            domainNouns = nouns;
        }
    }
}
