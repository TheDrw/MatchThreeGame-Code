using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;
using TMPro.Examples;

namespace MatchThree.UI
{
    public class CategorySelectableUI : MonoBehaviour, IPointerClickHandler
    {
        SettingsCategoriesUI settingsCategory;
        [SerializeField] GameObject categoryMenu;

        public GameObject CategoryMenu => categoryMenu;

        private void Start()
        {
            settingsCategory = GetComponentInParent<SettingsCategoriesUI>();
        }

        public void Activate()
        {
            categoryMenu.SetActive(true);

            GetComponent<RectTransform>().DOScale(1.2f, .15f);
            GetComponent<TMP_Text>().fontStyle = FontStyles.Underline;
            GetComponent<VertexJitter>().enabled = true;
        }

        public void Deactivate()
        {
            categoryMenu.SetActive(false);

            GetComponent<RectTransform>().DOScale(1f, .15f);
            GetComponent<TMP_Text>().fontStyle = FontStyles.Normal;
            GetComponent<VertexJitter>().enabled = false;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            settingsCategory.SwitchCategory(this);
        }
    }
}