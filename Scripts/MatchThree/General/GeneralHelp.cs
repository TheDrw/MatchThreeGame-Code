using UnityEngine.SceneManagement;
using MatchThree.Core;
using UnityEngine;
using Game;

namespace MatchThree.Helpers
{
    public class GeneralHelp
    {
        /// <summary>
        /// Scans through all the base objects of the scene to find the game object 
        /// with the GameManager class attached to it.
        /// </summary>
        /// <returns></returns>
        public static GameManager GetGameManagerFromActiveScene()
        {
            var activeScene = SceneManager.GetActiveScene();
            foreach (var go in activeScene.GetRootGameObjects())
            {
                if (go.TryGetComponent(out GameManager gm))
                {
                    return gm;
                }
            }

#if UNITY_EDITOR

            Debug.LogError("ERR: Missing GameManager in active scene. GameManager object " +
                "must be one of the root game objects i.e not a child of a game object in the heirarchy. " +
                "If testing scene, disregard this message.");
#endif
            return null;
        }

        public static IGameManager GetIGameManagerFromActiveScene()
        {
            var activeScene = SceneManager.GetActiveScene();
            foreach (var go in activeScene.GetRootGameObjects())
            {
                if (go.TryGetComponent(out IGameManager gm))
                {
                    return gm;
                }
            }

            return null;
        }

        // set for when slider max scale is 10f. 
        public static float ValueToDecibelConversion(float val)
        {
            float sliderMaxVal = 10f; // obviously changes if max val is different
            return val < 1 ? -80f : 20f * Mathf.Log10(val / sliderMaxVal);
        }
    }
}
