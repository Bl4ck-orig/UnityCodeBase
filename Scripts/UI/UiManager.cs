using System;
using System.Collections.Generic;
using System.Linq;
using GameManaging;
using Inputs;
using UnityEngine;

namespace UI
{
    [RequireComponent(typeof(Animator))]
    public class UiManager : MonoBehaviour
    {
        public static Action<EView, bool> ShowView { get; set; }

        public static Action<EView> EnqueueView { get; set; }

        public static Action<bool> CanGoBackChanged { get; set; }

        public static Action Back { get; set; }

        public static EView CurrentView { get => (instance.views.Count == 0) ? EView.None : instance.views.Peek().ViewType; }

        public static bool CanGoBack
        {
            get
            {
                return cgb;
            }
            set
            {
                if (value == cgb)
                    return;

                cgb = value;
                CanGoBackChanged?.Invoke(cgb);
            }
        }
        private static bool cgb = false;

        private static UiManager instance;
        private Animator crossFadeAnimator;

        private Dictionary<EView, IView> viewDict;
        private Stack<IView> views;

        #region Initialization -----------------------------------------------------------------
        private void Awake() 
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
                return;
            }
            
            
            instance = this;
            views = new Stack<IView>();
            viewDict = new Dictionary<EView, IView>();

            crossFadeAnimator = GetComponent<Animator>();

            ShowView += OnShowView;
            EnqueueView += InvokeEnqueueView;
            Back += OnBack;

            GameManager.LoadingStarted += OnLoadingStarted;
            GameManager.EnterEnterState += OnEnterEnterState;
            GameManager.EnterExitState += OnEnterExitState;
            InputManager.OnEscapeStarted += OnEscape;
        }

        public static void RegisterView(IView _view, bool _isActive)
        {
            if (instance.viewDict.ContainsKey(_view.ViewType))
                return;

            instance.viewDict.Add(_view.ViewType, _view);
            if(_isActive)
                instance.OnShowView(_view.ViewType, false);
        }
        #endregion -----------------------------------------------------------------

        #region View Showing / Enqueueing -----------------------------------------------------------------
        private void OnShowView(EView _view, bool _goBack)
        {
            if (_goBack)
                GoBackToView(_view);
            else
                DisplayView(_view);

            EvaluateCanGoBack();
        }

        private void GoBackToView(EView _view)
        {
            while (views.Count > 0 && views.Peek().ViewType != _view)
            {
                views.Pop();
            }

            if (views.Peek().ViewType == _view)
                views.Peek().Show();
            else
                UnityEngine.Debug.LogError("View " + _view + " not in list while trying to go back!");
        }

        private void DisplayView(EView _view)
        {
            switch (_view)
            {
                case EView.None:
                    ClearViewStack();
                    break;
                case EView.Back:
                    views.Pop();
                    if (views.Count > 0)
                    {
                        views.Peek().Show();
                    }
                    break;
                default:
                    if(!viewDict.ContainsKey(_view))
                    {
                        Debug.LogError("View has not been registered!");
                        return;
                    }
                    views.Push(viewDict[_view]);
                    views.Peek().Show();
                    break;
            }
        }

        private void InvokeEnqueueView(EView _view)
        {
            if (!viewDict.ContainsKey(_view))
            {
                Debug.LogError("View has not been registered!");
                return;
            }

            views.Push(viewDict[_view]);
            EvaluateCanGoBack();
        }

        #endregion -----------------------------------------------------------

        #region Back -----------------------------------------------------------
        private void OnBack()
        {
            if (!CanGoBack)
                return;

            views.Peek().Back();
        }

        private void EvaluateCanGoBack()
        {
            if (views.Count < 1)
            {
                CanGoBack = false;
                return;
            }

            if (!views.Peek().CanGoBack)
            {
                CanGoBack = false;
                return;
            }

            CanGoBack = true;
        }
        #endregion -----------------------------------------------------------


        #region Hiding -----------------------------------------------------------------
        private void ClearViewStack()
        {
            while (views.Count > 0)
            {
                IView view = views.Pop();
                if (view.IsShown)
                    view.Hide();
            }
        }
        #endregion -----------------------------------------------------------------

        #region Events Reactions -----------------------------------------------------------------
        private void OnLoadingStarted()
        {
            views.Clear();
            viewDict = viewDict.Where(x => x.Value.IsSceneIndependent).ToDictionary(k => k.Key, v => v.Value);
        }

        private void OnEnterExitState() => crossFadeAnimator.SetTrigger("Exit");

        private void OnEnterEnterState() => crossFadeAnimator.SetTrigger("Enter");

        private void OnEscape()
        {
            if(CurrentView != EView.None)
                OnBack();
            else if (viewDict.ContainsKey(EView.Menu))
                OnShowView(EView.Menu, false);
        }
        #endregion -----------------------------------------------------------------


    }
}