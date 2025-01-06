using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Training;

public partial class GamePage : ContentPage
{
	Move move;
    CurrentPlayers players;
    PlayerSelectionPage playerSelectionPage { get; set; }
    public ObservableCollection<Move> Moves { get; set; }
    private System.Timers.Timer aTimer = new System.Timers.Timer(1000);
    int minutesPlayed = 0;
    int secondsPlayed = 0;

	int playerTurn = 1;
    int playedTurns = 0;
    string sSelectedSquare;
    string player1Name, player2Name;

    int randomNumber;
    public GamePage(PlayerSelectionPage sharedPlayerSelectionPage)
	{
		InitializeComponent();
        Moves = new ObservableCollection<Move>();
        playerSelectionPage = sharedPlayerSelectionPage;

        //get player names from PlayerSelectionPage and assign to player definitions
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

        if (players.PlayerOne.FirstName == "s0me")
        {
            BotTurnX();
        }
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

        playedTurns++;


        //Add move info to moves and display in list
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

                await DisplayAlert("Peli p��ttyi", $"Pelaaja {player1Name} voitti pelin!", "Lopeta");
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
                await DisplayAlert("Peli p��ttyi", $"Pelaaja {player2Name} voitti pelin!", "Lopeta");
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
            await DisplayAlert("Peli p��ttyi", $"Kumpikaan ei voittanut: \nTasapeli", "Lopeta");
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
                if (players.PlayerOne.FirstName == "s0me")
                {
                    BotTurnX();
                }
            }
            else if(playerTurn == 2)
            {
                TurnDefinition.Text = $"Pelaajan {player2Name} vuoro";
            }
        }
    }
    private void BotTurnX()
    {
        Random rnd = new Random();
        switch (playedTurns)
        {
            case 0:
                //Bot plays first turn, choose randomly
                randomNumber = rnd.Next(1, 10);
                switch (randomNumber)
                {
                    case 1:
                        SquareButton_Clicked(A1, null);
                        break;
                    case 2:
                        SquareButton_Clicked(A2, null);
                        break;
                    case 3:
                        SquareButton_Clicked(A3, null);
                        break;
                    case 4:
                        SquareButton_Clicked(B1, null);
                        break;
                    case 5:
                        SquareButton_Clicked(B2, null);
                        break;
                    case 6:
                        SquareButton_Clicked(B3, null);
                        break;
                    case 7:
                        SquareButton_Clicked(C1, null);
                        break;
                    case 8:
                        SquareButton_Clicked(C2, null);
                        break;
                    case 9:
                        SquareButton_Clicked(C3, null);
                        break;
                }
                break;
            case 2:
                if (randomNumber == 1 || randomNumber == 3 || randomNumber == 7 || randomNumber == 9)
                {
                    if (B2.IsEnabled)
                    {
                        SquareButton_Clicked(B2, null);
                    }
                    else
                    {
                        int r = rnd.Next(1, 5);
                        switch (r)
                        {
                            case 1:
                                SquareButton_Clicked(A2, null);
                                break;
                            case 2:
                                SquareButton_Clicked(B1, null);
                                break;
                            case 3:
                                SquareButton_Clicked(B3, null);
                                break;
                            case 4:
                                SquareButton_Clicked(C2, null);
                                break;
                        }
                    }
                }
                else if (randomNumber == 2 || randomNumber == 4 || randomNumber == 6 || randomNumber == 8)
                {
                    int r = rnd.Next(1, 3);
                    if (r + 1 == 3 && B2.IsEnabled)
                    {
                        SquareButton_Clicked(B2, null);
                    }
                    else
                    {
                        switch (randomNumber)
                        {
                            case 2:

                                if (r == 1 && A1.IsEnabled)
                                {
                                    SquareButton_Clicked(A1, null);
                                }
                                else if (r == 1 && !A1.IsEnabled)
                                {
                                    SquareButton_Clicked(A3, null);
                                }
                                else if (r == 2 && A3.IsEnabled)
                                {
                                    SquareButton_Clicked(A3, null);
                                }
                                else if (r == 2 && !A3.IsEnabled)
                                {
                                    SquareButton_Clicked(A1, null);
                                }
                                break;
                            case 4:
                                if (r == 1 && A1.IsEnabled)
                                {
                                    SquareButton_Clicked(A1, null);
                                }
                                else if (r == 1 && !A1.IsEnabled)
                                {
                                    SquareButton_Clicked(C1, null);
                                }
                                else if (r == 2 && C1.IsEnabled)
                                {
                                    SquareButton_Clicked(C1, null);
                                }
                                else if (r == 2 && !C1.IsEnabled)
                                {
                                    SquareButton_Clicked(A1, null);
                                }
                                break;
                            case 6:
                                if (r == 1 && A3.IsEnabled)
                                {
                                    SquareButton_Clicked(A3, null);
                                }
                                else if (r == 1 && !A3.IsEnabled)
                                {
                                    SquareButton_Clicked(C3, null);
                                }
                                else if (r == 2 && C3.IsEnabled)
                                {
                                    SquareButton_Clicked(C3, null);
                                }
                                else if (r == 2 && !C3.IsEnabled)
                                {
                                    SquareButton_Clicked(A3, null);
                                }
                                break;
                            case 8:
                                if (r == 1 && C1.IsEnabled)
                                {
                                    SquareButton_Clicked(C1, null);
                                }
                                else if (r == 1 && !C1.IsEnabled)
                                {
                                    SquareButton_Clicked(C3, null);
                                }
                                else if (r == 2 && C3.IsEnabled)
                                {
                                    SquareButton_Clicked(C3, null);
                                }
                                else if (r == 2 && !C3.IsEnabled)
                                {
                                    SquareButton_Clicked(C1, null);
                                }
                                break;
                        }
                    }
                }
                else
                {
                    if (randomNumber == 5)
                    {
                        int r = rnd.Next(1, 9);
                        switch (r)
                        {
                            case 1:
                                if (A1.IsEnabled)
                                {
                                    SquareButton_Clicked(A1, null);
                                }
                                else
                                {
                                    SquareButton_Clicked(A2, null);
                                }
                                break;
                            case 2:
                                if (A2.IsEnabled)
                                {
                                    SquareButton_Clicked(A2, null);
                                }
                                else
                                {
                                    SquareButton_Clicked(A3, null);
                                }
                                break;
                            case 3:
                                if (A3.IsEnabled)
                                {
                                    SquareButton_Clicked(A3, null);
                                }
                                else
                                {
                                    SquareButton_Clicked(B1, null);
                                }
                                break;
                            case 4:
                                if (B1.IsEnabled)
                                {
                                    SquareButton_Clicked(B1, null);
                                }
                                else
                                {
                                    SquareButton_Clicked(B3, null);
                                }
                                break;
                            case 5:
                                if (B3.IsEnabled)
                                {
                                    SquareButton_Clicked(B3, null);
                                }
                                else
                                {
                                    SquareButton_Clicked(C1, null);
                                }
                                break;
                            case 6:
                                if (C1.IsEnabled)
                                {
                                    SquareButton_Clicked(C1, null);
                                }
                                else
                                {
                                    SquareButton_Clicked(C2, null);
                                }
                                break;
                            case 7:
                                if (C2.IsEnabled)
                                {
                                    SquareButton_Clicked(C2, null);
                                }
                                else
                                {
                                    SquareButton_Clicked(C3, null);
                                }
                                break;
                            case 8:
                                if (C3.IsEnabled)
                                {
                                    SquareButton_Clicked(C3, null);
                                }
                                else
                                {
                                    SquareButton_Clicked(A1, null);
                                }
                                break;
                        }
                    }
                }
                break;
            case 4:
                // Game winning moves from corner plays
                //Win from playing C3
                if (!A1.IsEnabled && A1.Text == "X" && !B2.IsEnabled && B2.Text == "X" && C3.IsEnabled
                    || !A3.IsEnabled && A3.Text == "X" && !B3.IsEnabled && B3.Text == "X" && C3.IsEnabled
                    || !C1.IsEnabled && C1.Text == "X" && !C2.IsEnabled && C2.Text == "X" && C3.IsEnabled)
                {
                    SquareButton_Clicked(C3, null);
                }
                //Win from playing C1
                else if (!A3.IsEnabled && A3.Text == "X" && !B2.IsEnabled && B2.Text == "X" && C1.IsEnabled
                        || !A1.IsEnabled && A1.Text == "X" && !B1.IsEnabled && B1.Text == "X" && C1.IsEnabled
                        || !C3.IsEnabled && C3.Text == "X" && !C2.IsEnabled && C2.Text == "X" && C1.IsEnabled)
                {
                    SquareButton_Clicked(C1, null);
                }
                //win from playing A3
                else if (!C1.IsEnabled && C1.Text == "X" && !B2.IsEnabled && B2.Text == "X" && A3.IsEnabled
                        || !C3.IsEnabled && C3.Text == "X" && !B3.IsEnabled && B3.Text == "X" && A3.IsEnabled
                        || !A1.IsEnabled && A1.Text == "X" && !A2.IsEnabled && A2.Text == "X" && A3.IsEnabled)
                {
                    SquareButton_Clicked(A3, null);
                }
                //Win from playing A1
                else if (!C3.IsEnabled && C3.Text == "X" && !B2.IsEnabled && B2.Text == "X" && A1.IsEnabled
                        || !C1.IsEnabled && C1.Text == "X" && !B1.IsEnabled && B1.Text == "X" && A1.IsEnabled
                        || !A3.IsEnabled && A3.Text == "X" && !A2.IsEnabled && A2.Text == "X" && A1.IsEnabled)
                {
                    SquareButton_Clicked(A1, null);
                }
                else
                {
                    // when X's are separated from each other
                    // .V.
                    if (!A1.IsEnabled && A1.Text == "X" && !B3.IsEnabled && B3.Text == "X")
                    {
                        if (A3.IsEnabled)
                        {
                            SquareButton_Clicked(A3, null);
                        }
                        else if (C1.IsEnabled)
                        {
                            SquareButton_Clicked(C1, null);
                        }
                        else
                        {
                            SquareButton_Clicked(B2, null);
                        }
                    }
                    else if (!C1.IsEnabled && C1.Text == "X" && !B3.IsEnabled && B3.Text == "X")
                    {
                        if (C3.IsEnabled)
                        {
                            SquareButton_Clicked(C3, null);
                        }
                        else if (B1.IsEnabled)
                        {
                            SquareButton_Clicked(B1, null);
                        }
                        else
                        {
                            SquareButton_Clicked(B2, null);
                        }
                    }
                    // >
                    else if (!A1.IsEnabled && A1.Text == "X" && !C2.IsEnabled && C2.Text == "X")
                    {
                        if (C1.IsEnabled)
                        {
                            SquareButton_Clicked(C1, null);
                        }
                        else if (C3.IsEnabled)
                        {
                            SquareButton_Clicked(C3, null);
                        }
                        else
                        {
                            SquareButton_Clicked(B2, null);
                        }
                    }
                    else if (!A3.IsEnabled && A3.Text == "X" && !C2.IsEnabled && C2.Text == "X")
                    {
                        if (C1.IsEnabled)
                        {
                            SquareButton_Clicked(C1, null);
                        }
                        else if (A1.IsEnabled)
                        {
                            SquareButton_Clicked(A1, null);
                        }
                        else
                        {
                            SquareButton_Clicked(B2, null);
                        }
                    }
                    // A
                    else if (!A3.IsEnabled && A3.Text == "X" && !B1.IsEnabled && B1.Text == "X")
                    {
                        if (A1.IsEnabled)
                        {
                            SquareButton_Clicked(A1, null);
                        }
                        else if (C3.IsEnabled)
                        {
                            SquareButton_Clicked(C3, null);
                        }
                        else
                        {
                            SquareButton_Clicked(B2, null);
                        }
                    }
                    else if (!C3.IsEnabled && C3.Text == "X" && !B1.IsEnabled && B1.Text == "X")
                    {
                        if (C1.IsEnabled)
                        {
                            SquareButton_Clicked(C1, null);
                        }
                        else if (A3.IsEnabled)
                        {
                            SquareButton_Clicked(A3, null);
                        }
                        else
                        {
                            SquareButton_Clicked(B2, null);
                        }
                    }
                    // <
                    else if (!C1.IsEnabled && C1.Text == "X" && !A2.IsEnabled && A2.Text == "X")
                    {
                        if (A1.IsEnabled)
                        {
                            SquareButton_Clicked(A1, null);
                        }
                        else if (C2.IsEnabled)
                        {
                            SquareButton_Clicked(C2, null);
                        }
                        else
                        {
                            SquareButton_Clicked(B2, null);
                        }
                    }
                    else if (!C3.IsEnabled && C3.Text == "X" && !A2.IsEnabled && A2.Text == "X")
                    {
                        if (A3.IsEnabled)
                        {
                            SquareButton_Clicked(A3, null);
                        }
                        else if (C2.IsEnabled)
                        {
                            SquareButton_Clicked(C2, null);
                        }
                        else
                        {
                            SquareButton_Clicked(B2, null);
                        }
                    }
                    // X
                    else if (!A3.IsEnabled && A3.Text == "X" && !C1.IsEnabled && C1.Text == "X")
                    {
                        if (A2.IsEnabled)
                        {
                            SquareButton_Clicked(A2, null);
                        }
                        else if (C2.IsEnabled)
                        {
                            SquareButton_Clicked(C2, null);
                        }
                        else
                        {
                            SquareButton_Clicked(B2, null);
                        }
                    }
                    else if (!A1.IsEnabled && A1.Text == "X" && !C3.IsEnabled && C3.Text == "X")
                    {
                        if (A2.IsEnabled)
                        {
                            SquareButton_Clicked(A2, null);
                        }
                        else if (C2.IsEnabled)
                        {
                            SquareButton_Clicked(C2, null);
                        }
                        else
                        {
                            SquareButton_Clicked(B2, null);
                        }
                    }
                    // +
                    else if (!B3.IsEnabled && B3.Text == "X" && !B1.IsEnabled && B1.Text == "X")
                    {
                        if (A1.IsEnabled)
                        {
                            SquareButton_Clicked(A1, null);
                        }
                        else if (C1.IsEnabled)
                        {
                            SquareButton_Clicked(C1, null);
                        }
                        else
                        {
                            SquareButton_Clicked(B2, null);
                        }
                    }
                    else if (!A2.IsEnabled && A2.Text == "X" && !C2.IsEnabled && C2.Text == "X")
                    {
                        if (C1.IsEnabled)
                        {
                            SquareButton_Clicked(C1, null);
                        }
                        else if (C3.IsEnabled)
                        {
                            SquareButton_Clicked(C3, null);
                        }
                        else
                        {
                            SquareButton_Clicked(B2, null);
                        }
                    }
                    else
                    {
                        // When 0 Blocks from a corner or side
                        //Corners:
                        // O at C3
                        if (A3.Text == "X" && B3.Text == "X" && C3.Text == "O")
                        {
                            if (B2.IsEnabled)
                            {
                                SquareButton_Clicked(B2, null);
                            }
                            else
                            {
                                SquareButton_Clicked(A2, null);
                            }

                        }
                        else if (A1.Text == "X" && B2.Text == "X" && C3.Text == "O")
                        {
                            if (B1.IsEnabled)
                            {
                                SquareButton_Clicked(B1, null);
                            }
                            else
                            {
                                SquareButton_Clicked(A2, null);
                            }
                        }
                        else if (C1.Text == "X" && C2.Text == "X" && C3.Text == "O")
                        {
                            if (B2.IsEnabled)
                            {
                                SquareButton_Clicked(B2, null);
                            }
                            else
                            {
                                SquareButton_Clicked(A1, null);
                            }
                        }
                        // O at C1
                        if (A3.Text == "X" && B2.Text == "X" && C1.Text == "O")
                        {
                            if (B3.IsEnabled)
                            {
                                SquareButton_Clicked(B3, null);
                            }
                            else
                            {
                                SquareButton_Clicked(A2, null);
                            }
                        }
                        else if (A1.Text == "X" && B1.Text == "X" && C1.Text == "O")
                        {
                            if (B2.IsEnabled)
                            {
                                SquareButton_Clicked(B2, null);
                            }
                            else
                            {
                                SquareButton_Clicked(A3, null);
                            }
                        }
                        else if (C3.Text == "X" && C2.Text == "X" && C1.Text == "O")
                        {
                            if (B2.IsEnabled)
                            {
                                SquareButton_Clicked(B2, null);
                            }
                            else
                            {
                                SquareButton_Clicked(A3, null);
                            }
                        }
                        // O at A3
                        if (A1.Text == "X" && A2.Text == "X" && A3.Text == "O")
                        {
                            if (B2.IsEnabled)
                            {
                                SquareButton_Clicked(B2, null);
                            }
                            else
                            {
                                SquareButton_Clicked(C1, null);
                            }
                        }
                        else if (C1.Text == "X" && B2.Text == "X" && A3.Text == "O")
                        {
                            if (B1.IsEnabled)
                            {
                                SquareButton_Clicked(B1, null);
                            }
                            else
                            {
                                SquareButton_Clicked(C2, null);
                            }
                        }
                        else if (C3.Text == "X" && B3.Text == "X" && A3.Text == "O")
                        {
                            if (B2.IsEnabled)
                            {
                                SquareButton_Clicked(B2, null);
                            }
                            else
                            {
                                SquareButton_Clicked(C1, null);
                            }
                        }
                        // O at A1
                        if (C1.Text == "X" && B1.Text == "X" && A1.Text == "O")
                        {
                            if (B2.IsEnabled)
                            {
                                SquareButton_Clicked(B2, null);
                            }
                            else
                            {
                                SquareButton_Clicked(C3, null);
                            }
                        }
                        else if (C3.Text == "X" && B2.Text == "X" && A1.Text == "O")
                        {
                            if (C2.IsEnabled)
                            {
                                SquareButton_Clicked(C2, null);
                            }
                            else
                            {
                                SquareButton_Clicked(B3, null);
                            }
                        }
                        else if (A3.Text == "X" && A2.Text == "X" && A1.Text == "O")
                        {
                            if (B2.IsEnabled)
                            {
                                SquareButton_Clicked(B2, null);
                            }
                            else
                            {
                                SquareButton_Clicked(C3, null);
                            }
                        }
                        //Sides:
                        // O at A2
                        else if (C2.Text == "X" && B2.Text == "X" && A2.Text == "O")
                        {
                            if (B3.IsEnabled)
                            {
                                SquareButton_Clicked(B3, null);
                            }
                            else
                            {
                                SquareButton_Clicked(A1, null);
                            }
                        }
                        // O at B3
                        else if (B1.Text == "X" && B2.Text == "X" && B3.Text == "O")
                        {
                            if (A2.IsEnabled)
                            {
                                SquareButton_Clicked(A2, null);
                            }
                            else
                            {
                                SquareButton_Clicked(C2, null);
                            }
                        }
                        // O at B1
                        else if (B3.Text == "X" && B2.Text == "X" && B1.Text == "O")
                        {
                            if (A3.IsEnabled)
                            {
                                SquareButton_Clicked(A3, null);
                            }
                            else
                            {
                                SquareButton_Clicked(C3, null);
                            }
                        }
                        // O at C2
                        else if (A2.Text == "X" && B2.Text == "X" && C2.Text == "O")
                        {
                            if (B1.IsEnabled)
                            {
                                SquareButton_Clicked(B1, null);
                            }
                            else
                            {
                                SquareButton_Clicked(C3, null);
                            }
                        }
                    }
                }
                break;
            case 6:
                // Game winning moves
                //Win from playing C3
                if (!A1.IsEnabled && A1.Text == "X" && !B2.IsEnabled && B2.Text == "X" && C3.IsEnabled
                    || !A3.IsEnabled && A3.Text == "X" && !B3.IsEnabled && B3.Text == "X" && C3.IsEnabled
                    || !C1.IsEnabled && C1.Text == "X" && !C2.IsEnabled && C2.Text == "X" && C3.IsEnabled)
                {
                    SquareButton_Clicked(C3, null);
                }
                //Win from playing C1
                else if (!A3.IsEnabled && A3.Text == "X" && !B2.IsEnabled && B2.Text == "X" && C1.IsEnabled
                        || !A1.IsEnabled && A1.Text == "X" && !B1.IsEnabled && B1.Text == "X" && C1.IsEnabled
                        || !C3.IsEnabled && C3.Text == "X" && !C2.IsEnabled && C2.Text == "X" && C1.IsEnabled)
                {
                    SquareButton_Clicked(C1, null);
                }
                //win from playing A3
                else if (!C1.IsEnabled && C1.Text == "X" && !B2.IsEnabled && B2.Text == "X" && A3.IsEnabled
                        || !C3.IsEnabled && C3.Text == "X" && !B3.IsEnabled && B3.Text == "X" && A3.IsEnabled
                        || !A1.IsEnabled && A1.Text == "X" && !A2.IsEnabled && A2.Text == "X" && A3.IsEnabled)
                {
                    SquareButton_Clicked(A3, null);
                }
                //Win from playing A1
                else if (!C3.IsEnabled && C3.Text == "X" && !B2.IsEnabled && B2.Text == "X" && A1.IsEnabled
                        || !C1.IsEnabled && C1.Text == "X" && !B1.IsEnabled && B1.Text == "X" && A1.IsEnabled
                        || !A3.IsEnabled && A3.Text == "X" && !A2.IsEnabled && A2.Text == "X" && A1.IsEnabled)
                {
                    SquareButton_Clicked(A1, null);
                }
                //Win from playing A2
                else if (C2.Text == "X" && B2.Text == "X" && A2.IsEnabled
                        || A1.Text == "X" && A3.Text == "X" && A2.IsEnabled)
                {
                    SquareButton_Clicked(A2, null);
                }
                //Win from playing B1
                else if (B3.Text == "X" && B2.Text == "X" && B1.IsEnabled
                        || A1.Text == "X" && C1.Text == "X" && B1.IsEnabled)
                {
                    SquareButton_Clicked(B1, null);
                }
                //Win from playing B3
                else if (B1.Text == "X" && B2.Text == "X" && B3.IsEnabled
                        || A3.Text == "X" && C3.Text == "X" && B3.IsEnabled)
                {
                    SquareButton_Clicked(B3, null);
                }
                //Win from playing C2
                else if (A2.Text == "X" && B2.Text == "X" && C2.IsEnabled
                        || C1.Text == "X" && C3.Text == "X" && C2.IsEnabled)
                {
                    SquareButton_Clicked(C2, null);
                }
                break;
            case 8:
                break;
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