using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text.Json;

namespace Training;

public partial class PlayerSelectionPage : ContentPage
{
    public ObservableCollection<PlayerProfile> Players = new ObservableCollection<PlayerProfile>();
    public CurrentPlayers selectedPlayers = new CurrentPlayers();


    public PlayerSelectionPage()
	{
		InitializeComponent();
        ReadPlayers();

        SelectPlayerOne.ItemsSource = Players;
        SelectPlayerTwo.ItemsSource = Players;

    }

    private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        var listToPickFrom = sender as ListView;
        var updatePlayers = selectedPlayers;
        if (e.SelectedItem == null)
        {
            return; // Do nothing if no item is selected
        }

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

    public CurrentPlayers AssignedPlayers(out CurrentPlayers currentPlayers)
    {
        return currentPlayers = selectedPlayers.UpdatePlayers(selectedPlayers.PlayerOne, selectedPlayers.PlayerTwo);
    }

    private void ReadPlayers()
    {
        string jsonString;
        string filePath = Path.GetTempPath();
        DirectoryInfo d = new DirectoryInfo(filePath);

        foreach (var file in d.GetFiles("*.json"))
        {
            
            jsonString = File.ReadAllText(file.FullName);
            PlayerProfile playerProfile = JsonSerializer.Deserialize<PlayerProfile>(jsonString);
            Players.Add(playerProfile);
        }
    }

    private async void NewButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NewPlayerCreationPage(this));
    }

    private async void StartButton_Clicked(object sender, EventArgs e)
    {
        SelectPlayerOne.SelectedItem = null;
        SelectPlayerTwo.SelectedItem = null;
        await Navigation.PushAsync(new GamePage(this));
    }

    private void BackButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }

    public async void WritePlayers()
    {
        try
        {
            //Get path to temp to save each player in their own .json file in temp
            string tempPath = Path.GetTempPath();
            string filePath;
            string fileName;
            string jsonString;

            await DisplayAlert("HUOM", $"{Players.Count} players to be written", "OK");
            foreach (var player in Players)
            {
                fileName = $"{player.FirstName}_{player.LastName}.json";
                filePath = Path.Combine(tempPath, fileName);
                jsonString = JsonSerializer.Serialize(player);
                await File.WriteAllTextAsync(filePath, jsonString);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Warning", "Could not write to file: \n{0}", "OK", ex.ToString());
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

            await DisplayAlert("HUOM", $"{selectedPlayers.PlayerOne.FirstName} and {selectedPlayers.PlayerTwo.FirstName} to be updated", "OK");


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
            await DisplayAlert("Warning", "Could not write to file: \n{0}", "OK", ex.ToString());
        }

    }
}