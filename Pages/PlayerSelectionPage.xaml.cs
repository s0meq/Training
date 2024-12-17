using System.Collections.ObjectModel;
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
        string filepath = AppDomain.CurrentDomain.BaseDirectory;
        DirectoryInfo d = new DirectoryInfo(filepath);

        foreach (var file in d.GetFiles("*.json"))
        {
            
            jsonString = File.ReadAllText(file.FullName);
            PlayerProfile playerProfile = JsonSerializer.Deserialize<PlayerProfile>(jsonString);
        }
    }

    private async void NewButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NewPlayerCreationPage(this));
        WritePlayers();
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

    private async void WritePlayers()
    {
        string baseDir = AppDomain.CurrentDomain.BaseDirectory;
        string filePath;
        string fileName;
        string jsonString;


        try
        {
            foreach (var player in Players)
            {
                fileName = player.FirstName + ".json";
                filePath = Path.Combine(baseDir, fileName);
                jsonString = JsonSerializer.Serialize(player);
                File.WriteAllText(filePath, jsonString);
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Warning", "Could not write to file: \n{1}", "OK", ex.ToString());
        }

    }
}