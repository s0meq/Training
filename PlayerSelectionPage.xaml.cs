namespace Training;

public partial class PlayerSelectionPage : ContentPage
{
	public PlayerSelectionPage()
	{
		InitializeComponent();
	}
    private async void StartButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GamePage());
    }
}