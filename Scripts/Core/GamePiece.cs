using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MatchThree.Data;

namespace MatchThree.Core
{
    public class GamePiece : MonoBehaviour
    {
        [SerializeField] GamePieceShapes shape;
        [SerializeField] GamePieceType gamePieceType = GamePieceType.NONE;

        public bool IsMarked { get; private set; } = false;

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
                Debug.LogWarning($"WRN: Game piece local scale should not be changed. Change sprite's pixel per unit if it needs to be bigger.", this);
            }
#endif
        }

        private void Start()
        {
            GetComponent<SpriteRenderer>().sprite = shape.CurrentShape;
        }

        public bool IsSameAs(GamePiece targetBean)
        {
            return GetGamePieceType == targetBean.GetGamePieceType;
        }

        public void Mark()
        {
            IsMarked = true;
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

        public void PopVisual(float scale, float duration, int vibrato, float elasticity)
        {
            transform.DOPunchScale(Vector3.one * scale, duration, vibrato, elasticity);
        }
    }
}