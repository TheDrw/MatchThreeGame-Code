using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MatchThree.Core;
using System.Text;
using System.IO;
using UnityEditor;

namespace MatchThree.Data
{
    public class ResultsCVS : MonoBehaviour
    {
        [Tooltip("File name without the extension.")]
        [SerializeField] string fileName = "";

        private void Start()
        {
            if (fileName == "") Debug.LogError("ERR: fileName is not filled.");
            GameManager.OnGameFinishedWithFinalGameResults += SaveResultsToCVS;
        }

        private void OnDestroy()
        {
            GameManager.OnGameFinishedWithFinalGameResults -= SaveResultsToCVS;
        }

        void SaveResultsToCVS(List<FinishedGameResult> finishedResults)
        {
            if (finishedResults.Count == 0) return;

#if UNITY_EDITOR
            var folder = Application.streamingAssetsPath;
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
#else
             var folder = Application.persistentDataPath;
#endif

            var filePath = Path.Combine(folder, $"{fileName}.csv");

            var content = DataToCVS(filePath, finishedResults);
            if (!File.Exists(filePath))
            {
                File.WriteAllText(filePath, content);
                Debug.Log($"CSV file written to \"{filePath}\"");
            }
            else
            {
                File.AppendAllText(filePath, content);
                Debug.Log($"CSV file appended in \"{filePath}\"");
            }

#if UNITY_EDITOR
            AssetDatabase.Refresh();
#endif
        }

        string DataToCVS(string filePath, List<FinishedGameResult> finishedResults)
        {
            var data = !File.Exists(filePath) ?
                    new StringBuilder("Id,rank,time,turnCount") :
                    new StringBuilder();
            foreach (var result in finishedResults)
            {
                data.Append('\n').Append(result.BoardType.ToString()).Append(',')
                    .Append(result.Rank.ToString()).Append(',')
                    .Append(result.TimeFinished.ToString()).Append(',')
                    .Append(result.MoveCount.ToString());
            }

            return data.ToString();
        }
    }
}