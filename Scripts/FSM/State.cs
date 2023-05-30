using System;
using Debugging;
using UnityEngine;

namespace FSM
{
    public abstract class State<T> where T : struct, IConvertible
    {
        public float StartTime { get; set; }

        public T StateType { get; set; }

        public bool IsExitingState { get; set; }

        private int id;
        private EScriptGroup scriptGroup;
        private IDebugInformation debugInfo;
        private string debugString;

        protected State(T _stateType, EScriptGroup _scriptGroup, int _id, IDebugInformation _debugInfo = null)
        {
            StateType = _stateType;
            id = _id;
            scriptGroup = _scriptGroup;
            debugInfo = _debugInfo;
        }

        #region Entering -----------------------------------------------------------------
        public void Enter()
        {
            PrintDebuggingInformation("Enter state " + StateType);

            StartTime = Time.time;
            IsExitingState = false;
            Entering();
        }

        protected abstract void Entering();

        #endregion -----------------------------------------------------------------

        #region Updating -----------------------------------------------------------------
        public abstract void LogicUpdate();

        public abstract void PhysicsUpdate();
        #endregion -----------------------------------------------------------------

        #region Exiting -----------------------------------------------------------------
        public void Exit()
        {
            PrintDebuggingInformation("Exit state " + StateType);

            IsExitingState = true;
            Exiting();
        }

        protected abstract void Exiting();
        #endregion -----------------------------------------------------------------

        #region Debugging -----------------------------------------------------------------
        protected void PrintDebuggingInformation(string _message)
        {
            debugString = (debugInfo == null) ? "Id = " + id : debugInfo.GetInfo();
            DebugManager.OutputDetailed(scriptGroup, this, debugString + " - " + _message);
        }
        #endregion -----------------------------------------------------------------
    }
}
