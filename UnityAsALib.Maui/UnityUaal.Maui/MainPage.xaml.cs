namespace UnityUaal.Maui
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void OnShowMainClicked(object? sender, EventArgs e)
        {
            UnityUaalMaui.Unity.UnityBridge.ShowMainWindow();
        }

        private void OnShowUnityClicked(object? sender, EventArgs e)
        {
            UnityUaalMaui.Unity.UnityBridge.ShowUnityWindow();
        }
    }
}
