using System.Runtime.InteropServices;

namespace Training;

public partial class NewPlayerCreationPage : ContentPage
{
    PlayerSelectionPage playerSelectionPage;
	public NewPlayerCreationPage(PlayerSelectionPage sharedPlayerSelectionPage)
	{
		InitializeComponent();
        playerSelectionPage = sharedPlayerSelectionPage;
        DateOfBirth.MaximumDate = DateTime.Now;
	}

    private void CancelButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    private async void SaveNewButton_Clicked(object sender, EventArgs e)
    {
        //Create new playerprofile when save button is clicked
        PlayerProfile player = new PlayerProfile();
        try
        {
            player.FirstName = FirstName.Text;
            player.LastName = LastName.Text;
            
            player.DateOfBirth = DateOfBirth.Date.ToShortDateString();
        
            player.Wins = 0;
            player.Losses = 0;
            player.Draws = 0;
            player.MinutesPlayed = 0;
            player.SecondsPlayed = 0;
            player.ID = playerSelectionPage.Players.Count + 1;
            playerSelectionPage.Players.Add(player);
            playerSelectionPage.WritePlayers();
            await Navigation.PopAsync();
        } catch (Exception ex)
        {
            await DisplayAlert("Virhe", $"Virhe pelaajaprofiilin luonnissa: \n{ex.Message}", "Ok");
        }
    }
}