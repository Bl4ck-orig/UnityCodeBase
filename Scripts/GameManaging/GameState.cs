using Debugging;
using FSM;

namespace GameManaging
{
    public abstract class GameState : State<EGameState>
    {
   
        protected GameManager GameManager;

        protected GameState(GameManager _gameManager, EGameState _stateType, EScriptGroup _scriptGroup, int _id) : base(_stateType, _scriptGroup, _id)
        {
            GameManager = _gameManager;
        }


        protected override void Entering() { }

        protected override void Exiting() { }

        public override void LogicUpdate() { }

        public override void PhysicsUpdate() { }
    }
}