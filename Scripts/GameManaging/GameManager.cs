using System;
using System.Collections.Generic;
using Debugging;
using FSM;
using ObjectSpawning;
using SceneManagement;
using UnityEngine;

namespace GameManaging
{
    public class GameManager : MonoBehaviour
    {
        public static Action LoadingFinished { get; set; }

        public static Action LoadingStarted { get; set; }

        public static Action EnterEnterState { get; set; }

        public static Action EnterExitState { get; set; }


        #region State Properties -----------------------------------------------------------------
        public static EGameState State
        {
            get
            {
                if (instance == null)
                    return 0;

                return instance.stateMachine.CurrentStateType;
            }
        }

        public bool HasSceneBeenSet { get; set; } = false;

        public int SceneToLoad { get; set; } = 0;

        public EScriptGroup ScriptGroup { get => EScriptGroup.GameManager; }
        #endregion -----------------------------------------------------------------

        #region Time Properties -----------------------------------------------------------------
        public float TimeScale
        {
            get
            {
                return timeScale;
            }
            set
            {
                timeScale = Mathf.Max(0.001f, value);
                if (timeScale != Time.timeScale)
                {
                    Time.timeScale = timeScale;
                }
            }
        }

        public float TimeScaleFactor { get; set; } = 0.05f;
        #endregion -----------------------------------------------------------------

        [SerializeField] private EGameState standardState;
        [SerializeField] private List<EGameState> states;
        [SerializeField] private EGameState startState;

        [SerializeField, Header("Config")] private SO_SceneConfig sceneConfig;

        private static GameManager instance;
        private float timeScale = 1f;
        private StateMachineHandler<EGameState> stateMachine;
        private GameManagerStateFactory stateFactory = new GameManagerStateFactory();
        private int id;

        #region Initialization -----------------------------------------------------------------
    
        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(transform.parent.gameObject);
                return;
            }

            instance = this;

            id = ObjectSpawner.GetNextId();
            SO_SceneConfig.SetSceneConfig(sceneConfig);

            stateMachine = new StateMachineHandler<EGameState>(stateFactory, this, ScriptGroup, id, standardState, states, startState);
            AssignEvents();

            HandlePlayerData();

            DontDestroyOnLoad(transform.root.gameObject);
        }

        private void Start()
        {
            stateMachine.StartStateMachine();
        }

        private void AssignEvents()
        {
        }

        private void HandlePlayerData()
        {
         /* if (SaveSystem<SerializedPlayerStats>.Exists())
                stats = new PlayerStats(SaveSystem<SerializedPlayerStats>.TryLoading().GetPlayerStats());
            else
                stats = new PlayerStats(); */
        }
        #endregion -----------------------------------------------------------------

        #region Updating -----------------------------------------------------------------
        private void Update() => stateMachine.Update();

        private void FixedUpdate() => stateMachine.FixedUpdate();

        public void ChangeState(EGameState _state) => stateMachine.ChangeState(_state);
        #endregion -----------------------------------------------------------------

        #region Resume / Pause -----------------------------------------------------------------
        public void OnResume() => ChangeState(EGameState.Run);

        public void OnPause() => ChangeState(EGameState.Pause);
        #endregion -----------------------------------------------------------------

        #region Scene Loading -----------------------------------------------------------------
        private void LoadMainMenu() => OnLoadScene(EScene.MainMenu, SO_SceneConfig.FirstScene(EScene.MainMenu).SceneName);

        public void OnLoadScene(EScene _sceneType, string _sceneName)
        {
            try
            {
                LoadScene(SO_SceneConfig.BakedSceneId(_sceneType, _sceneName));
            }
            catch(Exception ex)
            {
                Debug.LogError(ex);
                Debug.LogError("Could not load scene " + _sceneName);
            }
        }

        private void LoadScene(int _sceneId)
        {
            HasSceneBeenSet = true;
            SceneToLoad = _sceneId;
            ChangeState(EGameState.Exit);
        }


        #endregion -----------------------------------------------------------------

        private void OnSavePlayerStats()
        {
         // SaveSystem<SerializedPlayerStats>.TrySaving(new SerializedPlayerStats(stats));
        }

        #region Debugging-----------------------------------------------------------------
        private void OnSetTimeScale(float _value) => TimeScale = _value;
        #endregion -----------------------------------------------------------------

        #region Animation Events -----------------------------------------------------------------
        public void OnEnterFinished() => ChangeState(EGameState.Run);

        public void OnExitFinished() => ChangeState(EGameState.Load);
        #endregion -----------------------------------------------------------------
    }
}





 
