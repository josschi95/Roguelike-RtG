using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace JS.CharacterSystem.Creation
{
    public class UICharacterCreation_Class : MonoBehaviour
    {
        [SerializeField] private CreatureCatalog catalog;
        [SerializeField] private CharacterBuilder characterBuilder;
        [SerializeField] private UICharacterCreation characterCreator;

        [Header("Categories")]
        [SerializeField] private Button warriorButton;
        [SerializeField] private Button mageButton;
        [SerializeField] private Button artisanButton;

        [Space]

        [SerializeField] private RectTransform classButtonParent;
        [SerializeField] private UISelectionElement selectionElement;
        [SerializeField] private ClassTier baseTier;

        private void OnEnable()
        {
            DisplayClassOptions(ClassArchetype.Martial);
            warriorButton.onClick.AddListener(delegate 
            { 
                DisplayClassOptions(ClassArchetype.Martial); 
            });
            mageButton.onClick.AddListener(delegate
            {
                DisplayClassOptions(ClassArchetype.Mage);
            });
            artisanButton.onClick.AddListener(delegate
            {
                DisplayClassOptions(ClassArchetype.Artisan);
            });
        }

        private void OnDisable()
        {
            warriorButton.onClick.RemoveAllListeners();
            mageButton.onClick.RemoveAllListeners();
            artisanButton.onClick.RemoveAllListeners();
        }

        private void ResetButtons()
        {
            int count = classButtonParent.transform.childCount;
            for (int i = count - 1; i >= 0; i--)
            {

                Destroy(classButtonParent.transform.GetChild(i).gameObject);
            }
        }

        private void DisplayClassOptions(ClassArchetype archetype)
        {
            ResetButtons();
            foreach (var charClass in catalog.Classes)
            {
                if (charClass.Archetype != archetype) continue;
                if (!IsValidClass(charClass)) continue;

                var element = Instantiate(selectionElement);
                element.transform.SetParent(classButtonParent.transform, false);
                element.Text.text = charClass.ClassName;

                element.onHoverEnter += delegate
                {
                    characterCreator.OnClassHover(charClass);
                };
                element.onHoverExit += characterCreator.OnHoverExit;

                element.Button.onClick.AddListener(delegate
                {
                    OnConfirmClass(charClass);
                });
            }
        }

        private bool IsValidClass(CharacterClass characterClass)
        {
            if (characterClass.Tier != baseTier) return false;
            return true;
        }

        private void OnConfirmClass(CharacterClass characterClass)
        {
            characterBuilder.Class = characterClass;
            characterCreator.Next();
        }
    }
}