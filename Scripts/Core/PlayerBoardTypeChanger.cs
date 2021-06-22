using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MatchThree.Controllers;
using MatchThree.Data;

namespace MatchThree.Core
{
    public class PlayerBoardTypeChanger : MonoBehaviour
    {
        [SerializeField] PlayerController playerController;
        [SerializeField] PlayerChoice playerChoice;

        private void Awake()
        {
            if(playerController == null)
            {
                playerController = FindObjectOfType<PlayerController>();
                Debug.LogWarning($"WRN : Looking through heirarchy for player controller.", this);
            }
        }

        private void Start()
        {
            playerController.OverrideBoardType(playerChoice.CurrentChoice);
        }
    }
}