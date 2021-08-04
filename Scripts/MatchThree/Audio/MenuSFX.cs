using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MatchThree.Audio
{
    public interface IMenuSFX
    {
        void ButtonSelected();
        void ButtonPressed();

        void ButtonReleased();

        void ButtonDenied();

        void MenuIn();

        void MenuOut();

        void Pop();
    }


    [RequireComponent(typeof(AudioSource))]
    public class MenuSFX : MonoBehaviour, IMenuSFX
    {
        static public IMenuSFX Play { get; private set; }

        [SerializeField] AudioClip selected;
        [SerializeField] AudioClip pressed;
        [SerializeField] AudioClip released;
        [SerializeField] AudioClip denied;
        [SerializeField] AudioClip menuIn;
        [SerializeField] AudioClip menuOut;
        [SerializeField] AudioClip pop;

        AudioSource audioSource;

        private void Awake()
        {
            if (Play == null)
            {
                Play = this;
            }

            audioSource = GetComponent<AudioSource>();
        }

        private void OnDestroy()
        {
            Play = null;
        }

        public void ButtonSelected()
        {
            audioSource.PlayOneShot(selected);
        }

        public void ButtonPressed()
        {
            audioSource.PlayOneShot(pressed);
        }

        public void ButtonReleased()
        {
            audioSource.PlayOneShot(released);
        }

        public void ButtonDenied()
        {
            audioSource.PlayOneShot(denied);
        }

        public void MenuIn()
        {
            audioSource.PlayOneShot(menuIn);
        }

        public void MenuOut()
        {
            audioSource.PlayOneShot(menuOut);
        }

        public void Pop()
        {
            audioSource.PlayOneShot(pop);
        }
    }
}