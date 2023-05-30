using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Inputs
{
    /// <summary>
    /// Managing Inputs
    /// 
    /// - Left the "Up" input as example, as well as some left mouse button functionality.
    /// - Remove Commandline Control section if no developer console present.
    /// </summary>
    public class InputManager : MonoBehaviour
    {
        public const float DOUBLE_CLICK_TIME = 0.2f;

        public static Action OnEnableInputs { get; set; }

        public static Action OnDisableInputs { get; set; }

        #region Mouse Input Events -----------------------------------------------------------------
        public static Action OnLeftMouseButtonClicked { get; set; }
        public static Action OnLeftMouseButtonHold { get; set; }
        public static Action OnLeftMouseButtonStarted { get; set; }
        public static Action OnLeftMouseButtonCanceled { get; set; }
        #endregion -----------------------------------------------------------------

        #region KeyBoard Input Events -----------------------------------------------------------------
        public static Action OnUpStarted { get; set; }
        public static Action OnUpCanceled { get; set; }
        #endregion -----------------------------------------------------------------

        public static bool IsLeftMouseButtonDown { get => instance.playerInputs.Mouse.LeftMouseButton.IsPressed(); }
        public static bool IsUpDown  { get => instance.playerInputs.KeyBoard.Up.IsPressed(); }

        private static InputManager instance;
        private PlayerInputs playerInputs;
        private bool leftMouseWasPerformed = false;

        #region Cleanup -----------------------------------------------------------------
        private void UnsetAllInputs()
        {
            leftMouseWasPerformed = false;
        }
        #endregion -----------------------------------------------------------------

        #region Initialization -----------------------------------------------------------------
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
                return;
            }

            instance = this;
            playerInputs = new PlayerInputs();

            AssignGlobalEvents();
            AssignMouseEvents();
            AssignKeyBoardEvents();

        }

        private void AssignGlobalEvents()
        {
            OnEnableInputs += EnableInputs;
            OnDisableInputs += DisableInputs;
        }

        private void AssignMouseEvents()
        {
            playerInputs.Mouse.LeftMouseButton.started += LeftMouseButtonStarted;
            playerInputs.Mouse.LeftMouseButton.performed += LeftMouseButtonHold;
            playerInputs.Mouse.LeftMouseButton.canceled += LeftMouseButtonCanceled;
        }

        private void AssignKeyBoardEvents()
        {
            playerInputs.KeyBoard.Up.started += UpStarted;
            playerInputs.KeyBoard.Up.canceled += UpCanceled;
        }
        #endregion -----------------------------------------------------------------

        #region Enable / Disable -----------------------------------------------------------------
        private void DisableInputs()
        {
            UnsetAllInputs();
            playerInputs.Disable();
        }

        private void EnableInputs() => playerInputs.Enable();

        public static void SetCommandLineControls()
        {
            instance.SetCommandLineControlsInstance();
        }

        private void SetCommandLineControlsInstance()
        {
            playerInputs.KeyBoard.Up.started -= UpStarted;
        }

        public static void UnsetCommandLineControls()
        {
            instance.UnsetCommandLineControlsInstance();
        }

        private void UnsetCommandLineControlsInstance()
        {
            playerInputs.KeyBoard.Up.started += UpStarted;
        }
        #endregion -----------------------------------------------------------------


        #region Input Started -----------------------------------------------------------------
        public void LeftMouseButtonStarted(InputAction.CallbackContext _context) => OnLeftMouseButtonStarted?.Invoke();


        public void LeftMouseButtonHold(InputAction.CallbackContext _context)
        {
            leftMouseWasPerformed = true;
            OnLeftMouseButtonHold?.Invoke();
        }

        public void LeftMouseButtonCanceled(InputAction.CallbackContext _context)
        {
            if (!leftMouseWasPerformed)
                OnLeftMouseButtonClicked?.Invoke();
            
            leftMouseWasPerformed = false;
            OnLeftMouseButtonCanceled?.Invoke();
        }

        public  void UpStarted (InputAction.CallbackContext _context) => OnUpStarted?.Invoke();

        public  void UpCanceled (InputAction.CallbackContext _context) => OnUpCanceled?.Invoke();
        #endregion -----------------------------------------------------------------
    }
}
