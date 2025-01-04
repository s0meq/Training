using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Timers;

namespace Training;

public partial class GamePage : ContentPage
{
	Move move;
    CurrentPlayers players;
    PlayerSelectionPage playerSelectionPage { get; set; }
    public ObservableCollection<Move> Moves { get; set; }
    //private DateTime startTime;
    private System.Timers.Timer aTimer = new System.Timers.Timer(1000);
    int minutesPlayed = 0;
    int secondsPlayed = 0;

	int playerTurn = 1;
    int playedTurns = 0;
    string sSelectedSquare;
    string player1Name, player2Name;
    public GamePage(PlayerSelectionPage sharedPlayerSelectionPage)
	{
		InitializeComponent();
        Moves = new ObservableCollection<Move>();
        playerSelectionPage = sharedPlayerSelectionPage;
        //get player names from PlayerSelectionPage and assign to player definitions
        //PlayerSelectionPage.AssignedPlayers(out player1Name, out player2Name);
        players = playerSelectionPage.AssignedPlayers(out CurrentPlayers currentPlayers);
        player1Name = players.PlayerOne.FirstName;
        player2Name = players.PlayerTwo.FirstName;
        Player1Definition.Text = player1Name;
        Player2Definition.Text = player2Name;
        TurnDefinition.Text = $"Pelaajan {player1Name} vuoro";

        aTimer.Elapsed += OnTimedEvent;
        aTimer.Enabled = true;
        aTimer.AutoReset = true;

        movesList.ItemsSource = Moves;
    }

    public async void SquareButton_Clicked(object sender, EventArgs e)
    {
        //Check for winning conditions each turn
        //Add played turn and its info to 'Moves' to display it in the "siirrot" list
        //Array only for referencing to a played square to display in the "siirrot" list
        Button[,] squares = { { A1, A2, A3 }, { B1, B2, B3 }, { C1, C2, C3 } };
        Button selectedSquare = sender as Button;
        move = new Move();
        Debug.WriteLine("Button clicked");
        
        foreach (Button button in squares)
        {
            if (button == selectedSquare)
            {
                sSelectedSquare = button.AutomationId;
            }
        }

        //start timer from the first played turn to measure played time
        playedTurns++;
        //if (!aTimer.Enabled)
        //{
        //    aTimer.Start();
        //}


        if (playerTurn == 1)
        {
            move.PlayerName = $"Pelaaja: {player1Name}";
        }
        else if (playerTurn == 2)
        {
            move.PlayerName = $"Pelaaja: {player2Name}";
        }
        move.PlayedSquareName = $"Pelattu Ruutu: {sSelectedSquare}";
        move.PlayedTurn = $"Pelatut vuorot: {playedTurns}";
        Moves.Add(move);
        selectedSquare.Text = playerTurn == 1 ? "X" : "O";
        selectedSquare.IsEnabled = false;

        if (WinningConditionMet())
        {
            aTimer.Stop();
            foreach (Button button in squares)
            {
                button.IsEnabled = false;
            }
            if (playerTurn == 1)
            {
                //Current players are removed from the list on PlayerSelectionPage
                //and the instance which has the updated wins, losses or draws
                //is added to the list
                //This is done for following situations:
                //player1 win
                //player2 win
                //draw

                await DisplayAlert("Peli päättyi", $"Pelaaja {player1Name} voitti pelin!", "Lopeta");
                var updatedPlayer = playerSelectionPage.selectedPlayers.PlayerOne.AddWin(minutesPlayed, secondsPlayed);
                var updatedPlayer2 = playerSelectionPage.selectedPlayers.PlayerTwo.AddLoss(minutesPlayed, secondsPlayed);

                if (playerSelectionPage.Players.Contains(playerSelectionPage.selectedPlayers.PlayerOne))
                {
                    playerSelectionPage.Players.Remove(playerSelectionPage.selectedPlayers.PlayerOne);
                    playerSelectionPage.selectedPlayers.PlayerOne = updatedPlayer;
                    playerSelectionPage.Players.Add(playerSelectionPage.selectedPlayers.PlayerOne);
                }
                if (playerSelectionPage.Players.Contains(playerSelectionPage.selectedPlayers.PlayerTwo))
                {
                    playerSelectionPage.Players.Remove(playerSelectionPage.selectedPlayers.PlayerTwo);
                    playerSelectionPage.selectedPlayers.PlayerTwo = updatedPlayer2;
                    playerSelectionPage.Players.Add(playerSelectionPage.selectedPlayers.PlayerTwo);
                }

            }
            else if (playerTurn == 2)
            {
                await DisplayAlert("Peli päättyi", $"Pelaaja {player2Name} voitti pelin!", "Lopeta");
                var updatedPlayer = playerSelectionPage.selectedPlayers.PlayerOne.AddLoss(minutesPlayed, secondsPlayed);
                var updatedPlayer2 = playerSelectionPage.selectedPlayers.PlayerTwo.AddWin(minutesPlayed, secondsPlayed);

                if (playerSelectionPage.Players.Contains(playerSelectionPage.selectedPlayers.PlayerOne))
                {
                    playerSelectionPage.Players.Remove(playerSelectionPage.selectedPlayers.PlayerOne);
                    playerSelectionPage.selectedPlayers.PlayerOne = updatedPlayer;
                    playerSelectionPage.Players.Add(playerSelectionPage.selectedPlayers.PlayerOne);
                }
                if (playerSelectionPage.Players.Contains(playerSelectionPage.selectedPlayers.PlayerTwo))
                {
                    playerSelectionPage.Players.Remove(playerSelectionPage.selectedPlayers.PlayerTwo);
                    playerSelectionPage.selectedPlayers.PlayerTwo = updatedPlayer2;
                    playerSelectionPage.Players.Add(playerSelectionPage.selectedPlayers.PlayerTwo);
                }

            }
            playerSelectionPage.SavePlayers();
            
            await Navigation.PopAsync();
        }
        //
        else if (!WinningConditionMet() && playedTurns == 9)
        {
            aTimer.Stop();
            await DisplayAlert("Peli päättyi", $"Kumpikaan ei voittanut: \nTasapeli", "Lopeta");
            var updatedPlayer = playerSelectionPage.selectedPlayers.PlayerOne.AddDraw(minutesPlayed, secondsPlayed);
            var updatedPlayer2 = playerSelectionPage.selectedPlayers.PlayerTwo.AddDraw(minutesPlayed, secondsPlayed);

            if (playerSelectionPage.Players.Contains(playerSelectionPage.selectedPlayers.PlayerOne))
            {
                playerSelectionPage.Players.Remove(playerSelectionPage.selectedPlayers.PlayerOne);
                playerSelectionPage.selectedPlayers.PlayerOne = updatedPlayer;
                playerSelectionPage.Players.Add(playerSelectionPage.selectedPlayers.PlayerOne);
            }
            if (playerSelectionPage.Players.Contains(playerSelectionPage.selectedPlayers.PlayerTwo))
            {
                playerSelectionPage.Players.Remove(playerSelectionPage.selectedPlayers.PlayerTwo);
                playerSelectionPage.selectedPlayers.PlayerTwo = updatedPlayer2;
                playerSelectionPage.Players.Add(playerSelectionPage.selectedPlayers.PlayerTwo);
            }
            playerSelectionPage.SavePlayers();
            
            await Navigation.PopAsync();
        }
        else
        {
            //no winning conditions or draw yet, NEXT!!!
            playerTurn = playerTurn == 1 ? 2 : 1;
            if(playerTurn == 1)
            {
                TurnDefinition.Text = $"Pelaajan {player1Name} vuoro";
            }
            else if(playerTurn == 2)
            {
                TurnDefinition.Text = $"Pelaajan {player2Name} vuoro";
            }
        }
    }
    //All the possible winning conditions below
	private bool WinningConditionMet()
	{
        //Check for Vertical winning condition
		if (A1.Text.Equals("X") && A2.Text.Equals("X") && A3.Text.Equals("X") ||
            A1.Text.Equals("O") && A2.Text.Equals("O") && A3.Text.Equals("O"))
		{
			return true;
		}
        if (B1.Text.Equals("X") && B2.Text.Equals("X") && B3.Text.Equals("X") ||
            B1.Text.Equals("O") && B2.Text.Equals("O") && B3.Text.Equals("O"))
        {
            return true;
        }
        if (C1.Text.Equals("X") && C2.Text.Equals("X") && C3.Text.Equals("X") ||
            C1.Text.Equals("O") && C2.Text.Equals("O") && C3.Text.Equals("O"))
        {
            return true;
        }
        //Check for Horizontal winning condition
        if (A1.Text.Equals("X") && B1.Text.Equals("X") && C1.Text.Equals("X") ||
            A1.Text.Equals("O") && B1.Text.Equals("O") && C1.Text.Equals("O"))
        {
            return true;
        }
        if (A2.Text.Equals("X") && B2.Text.Equals("X") && C2.Text.Equals("X") ||
            A2.Text.Equals("O") && B2.Text.Equals("O") && C2.Text.Equals("O"))
        {
            return true;
        }
        if (A3.Text.Equals("X") && B3.Text.Equals("X") && C3.Text.Equals("X") ||
            A3.Text.Equals("O") && B3.Text.Equals("O") && C3.Text.Equals("O"))
        {
            return true;
        }
        //Check for diagonal winning condition
        if (A1.Text.Equals("X") && B2.Text.Equals("X") && C3.Text.Equals("X") ||
            A1.Text.Equals("O") && B2.Text.Equals("O") && C3.Text.Equals("O"))
        {
            return true;
        }
        if (A3.Text.Equals("X") && B2.Text.Equals("X") && C1.Text.Equals("X") ||
            A3.Text.Equals("O") && B2.Text.Equals("O") && C1.Text.Equals("O"))
        {
            return true;
        }
        //return false if no winning conditions are met
        return false;
	}
    private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
    {
        secondsPlayed++;
        if (secondsPlayed == 60)
        {
            minutesPlayed++;
            secondsPlayed = 0;
        }
        Dispatcher.Dispatch(() =>
        {
            ElapsedTime.Text = $"{minutesPlayed:D2}:{secondsPlayed:D2}";
        });
    }
}