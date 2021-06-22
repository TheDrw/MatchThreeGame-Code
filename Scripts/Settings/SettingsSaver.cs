using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MatchThree.Data;

namespace MatchThree.Settings
{
    public class SettingsSaver : MonoBehaviour
    {
        [SerializeField] GameSettings settings;

        private void OnDestroy()
        {
            settings.Save();
        }
    }
}