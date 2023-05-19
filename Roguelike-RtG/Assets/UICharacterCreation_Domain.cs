using UnityEngine;
using JS.DomainSystem;

namespace JS.CharacterSystem.Creation
{
    public class UICharacterCreation_Domain : MonoBehaviour
    {
        [SerializeField] private CreatureCatalog catalog;
        [SerializeField] private CharacterBuilder builder;
        [SerializeField] private UICharacterCreation characterCreation;

        [SerializeField] private RectTransform buttonParent;
        [SerializeField] private UISelectionElement selectionElement;

        private void OnEnable()
        {
            AssignButtons();
        }

        private void OnDisable()
        {
            RemoveButtons();
        }

        private void AssignButtons()
        {
            RemoveButtons();

            foreach(var domain in catalog.Domains)
            {
                var element = Instantiate(selectionElement);
                element.transform.SetParent(buttonParent.transform, false);
                element.Text.text = domain.name;

                element.onHoverEnter += delegate
                {
                    characterCreation.OnDomainHover(domain);
                };

                element.onHoverExit += characterCreation.OnHoverExit;

                element.Button.onClick.AddListener(delegate
                {
                    OnConfirmDomain(domain);
                });
            }
        }

        private void RemoveButtons()
        {
            for (int i = buttonParent.transform.childCount - 1; i >= 0; i--)
            {
                Destroy(buttonParent.transform.GetChild(i).gameObject);
            }
        }

        private void OnConfirmDomain(Domain domain)
        {
            builder.Domain = domain;
            characterCreation.Next();
        }
    }
}