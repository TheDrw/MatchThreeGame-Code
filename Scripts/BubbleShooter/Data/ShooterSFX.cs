using UnityEngine;

namespace BubbleShooter.Audio
{
    [CreateAssetMenu(menuName = "Bubble/Audio/Controller SFX")]
    public class ShooterSFX : ScriptableObject
    {
        [SerializeField] AudioClip fireSFX = null;
        [SerializeField] AudioClip rotateShooterSFX = null;

        public AudioClip FireSFX => fireSFX;
        public AudioClip RotateShooterSFX => rotateShooterSFX;
    }
}