using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;

namespace Training;

public partial class PlayerSelectionPage : ContentPage
{
    public ObservableCollection<PlayerProfile> Players = new ObservableCollection<PlayerProfile>();
    public CurrentPlayers selectedPlayers = new CurrentPlayers();
    PlayerProfile s0meBot;

    public PlayerSelectionPage()
	{
		InitializeComponent();
        

        SelectPlayerOne.ItemsSource = Players;
        SelectPlayerTwo.ItemsSource = Players;

        //Create s0meBot but add it only if it doesnt exist already
        s0meBot = new()
        {
            FirstName = "s0me",
            LastName = "Bot",
            DateOfBirth = "-_-_-_-",
            Wins = 0,
            Losses = 0,
            Draws = 0,
            MinutesPlayed = 0,
            SecondsPlayed = 0,
            ID = 0
        };
        
        ReadPlayers();
    }

    //Pass current (selected) players to gamepage
    public CurrentPlayers AssignedPlayers(out CurrentPlayers currentPlayers)
    {
        return currentPlayers = selectedPlayers.UpdatePlayers(selectedPlayers.PlayerOne, selectedPlayers.PlayerTwo);
    }

    //UI events
    private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        var listToPickFrom = sender as ListView;
        var updatePlayers = selectedPlayers;
        if (e.SelectedItem == null)
        {
            return;
        }
        //Multiple listviews, check which one raised the event and assign players based on that
        if (listToPickFrom == SelectPlayerOne)
        {
            updatePlayers.PlayerOne = (PlayerProfile)listToPickFrom.SelectedItem;
        }
        if (listToPickFrom == SelectPlayerTwo)
        {
            updatePlayers.PlayerTwo = (PlayerProfile)listToPickFrom.SelectedItem;
        }
        selectedPlayers = updatePlayers;

        //If s0meBot is selected, display text about that the game is played against a computer.
        if (selectedPlayers.PlayerOne.ID == 0)
        {
            IsBotGame.IsVisible = true;
        }
        if (selectedPlayers.PlayerTwo.ID == 0)
        {
            IsBotGame.IsVisible = true;
        }
        if (selectedPlayers.PlayerOne.ID != 0 && selectedPlayers.PlayerTwo.ID != 0)
        {
            IsBotGame.IsVisible = false;
        }

        //Binding context to display selected players
        DisplayPlayerOne.BindingContext = selectedPlayers.PlayerOne;
        DisplayPlayerTwo.BindingContext = selectedPlayers.PlayerTwo;
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

    //Read/Write
    private async void ReadPlayers()
    {
        try
        {
            //Attempt to fetch saved player files and add to list
            //After reading files add s0meBot (created in constructor) to list if it doesnt exist already
            //E.g. if list is empty after attempt to add all the saved players (including s0meBot)
            string jsonString;
            string filePath = Path.GetTempPath();
            DirectoryInfo d = new DirectoryInfo(filePath);

            foreach (var file in d.GetFiles("*.json"))
            {
                if (file.Name == "s0me_Bot.json")
                {
                    jsonString = File.ReadAllText(file.FullName);
                    s0meBot = JsonSerializer.Deserialize<PlayerProfile>(jsonString);
                    Players.Add(s0meBot);
                }
                else
                {
                    jsonString = File.ReadAllText(file.FullName);
                    PlayerProfile playerProfile = JsonSerializer.Deserialize<PlayerProfile>(jsonString);
                    Players.Add(playerProfile);
                }
            }
            if (Players.Count == 0)
            {
                Players.Add(s0meBot);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Huom", $"Virhe pelaajien hakemisessa: \n{ex.Message}", "Ok");
        }
    }
    public async void WritePlayers()
    {
        try
        {
            //Each player is saved in their own .json file in a predefined folder.
            string tempPath = Path.GetTempPath();
            string filePath;
            string fileName;
            string jsonString;

            await DisplayAlert("Huom", $"{Players.Count} pelaajaa tallennetaan.", "OK");
            foreach (var player in Players)
            {
                fileName = $"{player.FirstName}_{player.LastName}.json"; // E.g. s0me_Bot.json
                filePath = Path.Combine(tempPath, fileName);
                jsonString = JsonSerializer.Serialize(player);
                await File.WriteAllTextAsync(filePath, jsonString);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Huom", $"Virhe pelaajien tallentamisessa: \n{ex.Message}", "Ok");
        }

    }
    public async void SavePlayers()
    {
        try
        {
            //Save each updated player in their old .json file in temp
            string tempPath = Path.GetTempPath();
            string filePath;
            string fileName;
            string jsonString;

            await DisplayAlert("HUOM", $"Pelaajan {selectedPlayers.PlayerOne.FirstName} ja {selectedPlayers.PlayerTwo.FirstName} tiedot päivitetään.", "OK");


            foreach (var player in Players)
            {
                if (player.Equals(selectedPlayers.PlayerOne))
                {
                    fileName = $"{player.FirstName}_{player.LastName}.json";
                    filePath = Path.Combine(tempPath, fileName);
                    jsonString = JsonSerializer.Serialize(player);
                    await File.WriteAllTextAsync(filePath, jsonString);
                }
                else if (player.Equals(selectedPlayers.PlayerTwo))
                {
                    fileName = $"{player.FirstName}_{player.LastName}.json";
                    filePath = Path.Combine(tempPath, fileName);
                    jsonString = JsonSerializer.Serialize(player);
                    await File.WriteAllTextAsync(filePath, jsonString);
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Huom", $"Virhe pelaajien tietojen tallentamisessa: \n{ex.Message}", "Ok");
        }

    }
}