namespace UI
{ 
    public interface IView
    {
        public bool IsShown { get; }

        public EView ViewType { get; }

        public bool CanGoBack { get; }

        public bool IsSceneIndependent { get; }

        public void Show();

        public void Back();

        public void Hide(bool _invokeNextView = false, EView _nextView = EView.None, bool _goBack = false);
    }
}