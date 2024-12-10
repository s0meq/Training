namespace Training
{
    public partial class MainPage : ContentPage
    {

        public MainPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.Window.MinimumHeight = 1000;
            this.Window.MinimumWidth = 1600;
            this.Window.MaximumHeight = 1000;
            this.Window.MaximumWidth = 1600;
        }

        private async void StartButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new GamePage());
        }

        private void QuitButton_Clicked(object sender, EventArgs e)
        {
            System.Environment.Exit(0);
        }
    }

}
