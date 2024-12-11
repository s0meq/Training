using System.Collections.ObjectModel;

namespace Training;

public partial class PlayerSelectionPage : ContentPage
{
    public ObservableCollection<PlayerProfile> Players { get; set; }
    public CurrentPlayers selectedPlayers { get; set; }
    public string Player1;
    public string Player2;

    public PlayerSelectionPage()
	{
		InitializeComponent();
        ReadPlayers();
        Players = new ObservableCollection<PlayerProfile>();
        selectedPlayers = new CurrentPlayers();
        SelectPlayerOne.ItemsSource = Players;
        SelectPlayerTwo.ItemsSource = Players;
	}

    private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        var listToPickFrom = sender as ListView;
        var updatePlayers = selectedPlayers;

        if (listToPickFrom == SelectPlayerOne)
        {
            updatePlayers.PlayerOne = (PlayerProfile)listToPickFrom.SelectedItem;
        }
        if (listToPickFrom == SelectPlayerTwo)
        {
            updatePlayers.PlayerTwo = (PlayerProfile)listToPickFrom.SelectedItem;
        }
        selectedPlayers = updatePlayers;
    }

    public void AssignedPlayers(out string player1, out string player2)
    {
        player1 = selectedPlayers.PlayerOne.FirstName;
        player2 = selectedPlayers.PlayerTwo.FirstName;
    }

    private void ReadPlayers()
    {
        
    }

    private async void NewButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NewPlayerCreationPage(this));
    }

    private async void StartButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GamePage(this));
    }

    private void BackButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
}