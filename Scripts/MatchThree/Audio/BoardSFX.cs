using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatchThree.Audio
{
    [CreateAssetMenu(menuName = "Match3/Audio/SFX")]
    public class BoardSFX : ScriptableObject
    {
        [Header("Matching")]
        [Space(5)]
        [SerializeField] AudioClip matchFoundSFX = null;
        [SerializeField] AudioClip matchComboOneSFX = null;
        [SerializeField] AudioClip matchComboTwoSFX = null;
        [SerializeField] AudioClip matchComboThreeSFX = null;

        [Header("")]

        [Space(5)]
        [SerializeField] AudioClip popupSFX = null;
        [SerializeField] AudioClip errorSFX = null;
        [SerializeField] AudioClip selectedSFX = null;
        [SerializeField] AudioClip poppingSFX = null;
        [SerializeField] AudioClip noMoreMatchesSFX = null;
        [SerializeField] AudioClip clearingBoardSFX = null;

        [Space(5)]
        [SerializeField] AudioClip[] swapSFX = null;

        public AudioClip MatchFoundSFX => matchFoundSFX;
        public AudioClip MatchComboOneSFX => matchComboOneSFX;
        public AudioClip MatchComboTwoSFX => matchComboTwoSFX;
        public AudioClip MatchComboThreeSFX => matchComboThreeSFX;


       public AudioClip PopupSFX => popupSFX;
       public AudioClip ErrorSFX => errorSFX;
       public AudioClip SelectedSFX => selectedSFX;
       public AudioClip PoppingSFX => poppingSFX;
       public AudioClip NoMoreMatchesSFX => noMoreMatchesSFX;
       public AudioClip ClearingBoardSFX => clearingBoardSFX;

        public AudioClip[] SwapSFX => swapSFX;

        public AudioClip RandomSwapSFX() => swapSFX[UnityEngine.Random.Range(0, swapSFX.Length - 1)];
    }
}