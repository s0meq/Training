using System.Collections.ObjectModel;

namespace Training;

public partial class PlayerSelectionPage : ContentPage
{
    public ObservableCollection<PlayerProfile> Players { get; set; }
    public PlayerSelectionPage()
	{
		InitializeComponent();
        Players = new ObservableCollection<PlayerProfile>();
        SelectPlayerOne.ItemsSource = Players;
        SelectPlayerTwo.ItemsSource = Players;
	}

    private void ListView_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {

    }

    private async void NewButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NewPlayerCreationPage());
    }

    private async void StartButton_Clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new GamePage());
    }

    private void BackButton_Clicked(object sender, EventArgs e)
    {
        Navigation.PopAsync();
    }
}