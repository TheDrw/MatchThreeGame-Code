using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BubbleShooter.Audio
{
    [CreateAssetMenu(menuName = "Bubble/Audio/Board SFX")]
    public class BubbleBoardSFX : ScriptableObject
    {
        [Header("Matching SFX")]
        [SerializeField] AudioClip matchThreeSFX;
        [SerializeField] AudioClip matchFourSFX;
        [SerializeField] AudioClip matchFiveSFX;
        [SerializeField] AudioClip matchManySFX;

        [Space(5)]
        [SerializeField] AudioClip[] bounceSFX;
        [SerializeField] AudioClip[] dropSFX;
        [SerializeField] AudioClip parkedSFX;

        public AudioClip MatchThreeSFX => matchThreeSFX;
        public AudioClip MatchFourSFX => matchFourSFX;
        public AudioClip MatchFiveSFX => matchFiveSFX;
        public AudioClip MatchManySFX => matchManySFX;


        public AudioClip BounceSFX => bounceSFX[UnityEngine.Random.Range(0, bounceSFX.Length)];
        public AudioClip DropSFX => dropSFX[UnityEngine.Random.Range(0, bounceSFX.Length)];
        public AudioClip ParkedSFX => parkedSFX;
    }
}