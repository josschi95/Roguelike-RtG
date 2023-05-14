using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Xml;
using System.Linq;

namespace JS.DomainSystem
{
    [CreateAssetMenu(menuName = "Scriptable Objects/Domains/Domain Database")]
    public class DomainDatabase : ScriptableObject
    {
        [HideInInspector] public TextAsset XMLRawFile;

        [SerializeField] private Domain[] domains;

        private Dictionary<int, Domain> domainDictionary_ID;
        private Dictionary<string, Domain> domainDictionary_Name;

        private void UpdateDictionaries()
        {
            //order alphabetically
            domains = domains.OrderBy(x => x.name).ToArray();

            //assign IDs alphabetically
            for (int i = 0; i < domains.Length; i++)
            {
                domains[i].ID = i;
            }

            domainDictionary_ID = new Dictionary<int, Domain>();
            domainDictionary_Name = new Dictionary<string, Domain>();
            for (int i = 0; i < domains.Length; i++)
            {
                domainDictionary_ID.Add(i, domains[i]);
                domainDictionary_Name.Add(domains[i].name, domains[i]);
            }
        }

        public void LoadXMLFile()
        {
            UpdateDictionaries();

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(new StringReader(XMLRawFile.text));

            string xmlPathPattern = "//domain-data/record";
            XmlNodeList nodeList = xmlDoc.SelectNodes(xmlPathPattern);

            for (int i = 0; i < nodeList.Count; i++)
            {
                ProcessNode(domains[i], nodeList[i]);
            }

            Debug.Log("Domains Updated.");
        }

        private void ProcessNode(Domain domain, XmlNode node)
        {
            XmlNode name = node.FirstChild;

            if (domain.name != name.InnerXml)
            {
                throw new UnityException("XML File Data Inconsistent with existing Scriptable Objects.");
            }

            XmlNode parents = name.NextSibling;
            XmlNode children = parents.NextSibling;

            XmlNode allies = children.NextSibling;
            XmlNode opponents = allies.NextSibling;

            XmlNode adjectives = opponents.NextSibling;
            XmlNode nouns = adjectives.NextSibling;

            domain.SetProperties(GetDomains(parents), GetDomains(children), GetDomains(allies),
                GetDomains(opponents), GetStrings(adjectives), GetStrings(nouns));
        }

        private Domain[] GetDomains(XmlNode node)
        {
            string s = node.InnerXml;
            s = s.Replace(" ", string.Empty);
            if (s == string.Empty) return null;

            var domainNames = s.Split(',');

            Domain[] domains = new Domain[domainNames.Length];
            for (int i = 0; i < domains.Length; i++)
            {
                domains[i] = domainDictionary_Name[domainNames[i]];
            }
            return domains;
        }

        private string[] GetStrings(XmlNode node)
        {
            string s = node.InnerXml;
            s = s.Replace(" ", string.Empty);

            if (s == string.Empty) return null;

            return s.Split(',');
        }
    }
}