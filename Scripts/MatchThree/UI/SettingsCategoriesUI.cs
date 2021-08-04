using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MatchThree.Audio;

namespace MatchThree.UI
{
    public class SettingsCategoriesUI : MonoBehaviour
    {
        [SerializeField] CategorySelectableUI initialFocusCategory;
        CategorySelectableUI prevCategory = null;

        private void Start()
        {
            prevCategory = initialFocusCategory;
            initialFocusCategory.Activate();
        }

        public void SwitchCategory(CategorySelectableUI newCategory)
        {
            if (prevCategory == newCategory) return;

            MenuSFX.Play?.Pop();
            prevCategory.Deactivate();

            newCategory.Activate();
            prevCategory = newCategory;
        }
    }
}