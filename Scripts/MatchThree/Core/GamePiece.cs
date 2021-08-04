using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MatchThree.Data;

namespace MatchThree.Core
{
    interface IPunchScale
    {
        void PunchScaleVFX(float scale, float duration, int vibrato, float elasticity);
    }

    interface IPopScale
    {
        void PopScaleVFX(float scale, float duration);
    }

    [SelectionBase]
    public class GamePiece : MonoBehaviour, IPunchScale, IPopScale
    {
        [SerializeField] GamePieceShapes shape;
        [SerializeField] GamePieceType gamePieceType = GamePieceType.NONE;

        public bool IsVisited { get; private set; } = false;

        public GamePieceType GetGamePieceType => gamePieceType;

        private void Awake()
        {
#if UNITY_EDITOR
            if (gamePieceType == GamePieceType.NONE)
            {
                Debug.LogError($"ERR: Game piece type is not initialized in inspector.", this);
                Debug.Break();
            }

            if(transform.localScale != Vector3.one)
            {
                Debug.LogWarning($"WRN: Game piece local scale should not be changed. Change sprite's pixel per unit if it needs to be bigger. " +
                    $"But if this is for the ceiling, ignore this.", this);
            }
#endif
        }

        protected virtual void Start()
        {
            GetComponent<SpriteRenderer>().sprite = shape.CurrentShape;
        }

        public bool IsSameAs(GamePiece target) => GetGamePieceType == target.GetGamePieceType;

        public bool IsSameAs(GamePieceType targetType) => gamePieceType == targetType;

        public bool IsPlayableGamePiece() => gamePieceType != GamePieceType.Ceiling && gamePieceType != GamePieceType.Wall;

        public void Visit()
        {
            IsVisited = true;
        }

        public void Unvisit()
        {
            IsVisited = false;
        }

        public void HintOn()
        {
            transform.GetChild(1).gameObject.SetActive(true);
        }

        public void HintOff()
        {
            transform.GetChild(1).gameObject.SetActive(false);
        }

        public void Highlight()
        {
            HintOff();
            transform.GetChild(0).gameObject.SetActive(true);
            transform.DOScale(Vector3.one * 1.1f, .1f);
        }

        public void Unhighlight()
        {
            HintOff();
            transform.GetChild(0).gameObject.SetActive(false);
            transform.DOScale(Vector3.one, .1f);
        }

        public void PunchScaleVFX(float scale, float duration, int vibrato, float elasticity)
        {
            // reset in case if it gets called multiple times, it will take its previous size.
            // like in the bubble shooter when it bouces the wall straight to the ceiling/top piece
            transform
                .DOPunchScale(Vector3.one * scale, duration, vibrato, elasticity)
                .OnComplete(() => transform.localScale = Vector3.one);
        }

        public void PopScaleVFX(float scale, float duration)
        {
            transform.DOScale(1 * scale, duration);
        }
    }
}