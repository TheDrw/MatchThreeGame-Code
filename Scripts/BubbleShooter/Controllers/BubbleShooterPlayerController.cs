using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using MatchThree.Controllers;


namespace BubbleShooter.Controller
{
    public class BubbleShooterPlayerController : BubbleShooterController, 
        InputActions.IBubbleControlsActions, IPlayer
    {
        InputActions.BubbleControlsActions bubbleControls;

        protected override void Awake()
        {
            base.Awake();

            var input = new InputActions();
            bubbleControls = input.BubbleControls;
            bubbleControls.SetCallbacks(this);
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            bubbleControls.Enable();
        }


#if UNITY_EDITOR
        private void TestingInputs()
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                SwitchBubble();
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }

            if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.LeftShift))
            {
                board.UndoLastMove();
            }
        }
#endif

        protected override void OnDisable()
        {
            base.OnDisable();

            bubbleControls.Disable();
        }

        void SwitchBubble()
        {
            //Destroy(loadedBubble.gameObject);
            LoadBubble();
        }

        public void OnRotateLeft(InputAction.CallbackContext context)
        {
            //if (!isActive) return;

            if (context.performed)
            {
                int val = Mathf.Clamp((int)shooterDirection + 1, -1, 1);
                SetShootDirection(val);
            }
            else if (context.canceled)
            {
                int val = Mathf.Clamp((int)shooterDirection - 1, -1, 1);
                SetShootDirection(val);
            }
        }

        public void OnRotateRight(InputAction.CallbackContext context)
        {
            //if (!isActive) return;

            if (context.performed)
            {
                int val = Mathf.Clamp((int)shooterDirection - 1, -1, 1);
                SetShootDirection(val);
            }
            else if(context.canceled)
            {
                int val = Mathf.Clamp((int)shooterDirection + 1, -1, 1);
                SetShootDirection(val);
            }
        }

        public void OnFire(InputAction.CallbackContext context)
        {
            if (context.performed)
            {
                ShootBubble();
            }
        }
    }
}