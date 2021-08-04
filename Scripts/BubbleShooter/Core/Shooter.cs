using BubbleShooter.Audio;
using BubbleShooter.Controller;
using BubbleShooter.Effects;
using MatchThree.Core;
using MatchThree.Settings;
using UnityEngine;

namespace BubbleShooter.Core
{
    public class Shooter : MonoBehaviour
    {
        [SerializeField] GameSettings settings;
        [SerializeField] Transform loadLocation;
        [SerializeField] BubbleTrailParticles bubbleTrailParticles;

        [Header("Setting")]
        [Tooltip("The speed the bubble fires. If over 15, collision has to be checked for corners " +
        "because collisions can be very incosistent."),
        SerializeField] float shootSpeed = 15;

        [SerializeField] float rotateSpeed = 50;
        [SerializeField] float limitAngle = 88f;

        [Space(5)]
        [SerializeField] ShooterSFX shooterSFX;
        [SerializeField] AudioSource fireAudioSource;
        [SerializeField] AudioSource rotatorAudioSource;

        public float ShootSpeed => shootSpeed;
        public float RotateSpeed => rotateSpeed;
        public float LimitAngle => limitAngle;
        public Transform LoadLocation => loadLocation;
        public Transform ShooterTransform => transform;
        public AudioSource FireAudioSource => fireAudioSource;
        public AudioSource RotatorAudioSource => RotatorAudioSource;

        public bool IsLoaded => loadedBubble != null;

        Bubble loadedBubble = null;
        float currAngle = 0f;

        private void OnValidate()
        {
            limitAngle = Mathf.Clamp(limitAngle, 0, 88);
            shootSpeed = Mathf.Clamp(shootSpeed, 0.1f, 100);
            rotateSpeed = Mathf.Clamp(rotateSpeed, 0.1f, 500);
        }

        private void Start()
        {
            fireAudioSource.clip = shooterSFX.FireSFX;
            rotatorAudioSource.clip = shooterSFX.RotateShooterSFX;
        }

        public void LoadBubble(Bubble bubble)
        {
            loadedBubble = bubble;

            if(settings.EnableVFX)
            {
                loadedBubble.AttachTrail(bubbleTrailParticles);
            }
        }

        public void Rotate(ShootDirection dir)
        {
            if (dir == ShootDirection.Idle) return;

            float diff = currAngle;
            currAngle += (float)dir * rotateSpeed * Time.deltaTime;
            currAngle = Mathf.Clamp(currAngle, -limitAngle, limitAngle);
            transform.rotation = Quaternion.Euler(0f, 0f, currAngle);

            bool hasChanged = Mathf.Abs(currAngle - diff) > 0;
            if (hasChanged)
            {
                if (!rotatorAudioSource.isPlaying)
                {
                    rotatorAudioSource.pitch = UnityEngine.Random.Range(0.98f, 1.02f);
                    rotatorAudioSource.Play();
                }
            }
            else if (rotatorAudioSource.isPlaying)
            {
                rotatorAudioSource.Stop();
            }
        }

        public void Shoot()
        {
            if (loadedBubble &&
                !loadedBubble.IsShooting)
            {
                loadedBubble.Shoot(transform.up, shootSpeed);
                fireAudioSource.Play();
                loadedBubble = null;
            }
        }
    }
}