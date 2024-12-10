namespace Training;

public partial class NewPlayerCreationPage : ContentPage
{
	public NewPlayerCreationPage()
	{
		InitializeComponent();
	}

    private void CancelButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private async void SaveNewButton_Clicked(object sender, EventArgs e)
    {
        PlayerProfile player = new PlayerProfile();
        try
        {
            player.FirstName = FirstName.Text;
            player.LastName = LastName.Text;
            player.DateOfBirth = DateOfBirth.Date.ToString();
            player.Wins = 0;
            player.Losses = 0;
            player.Draws = 0;
            player.MinutesPlayed = 0;
            await Navigation.PopAsync();
        } catch (Exception ex)
        {
            await DisplayAlert("Virhe", $"Jotain meni pieleen: { ex.Message}", "OK");
        }
    }
}