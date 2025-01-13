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
    private System.Timers.Timer bTimer; // = new System.Timers.Timer();
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

        //Check if button text should be X or O based on player turn (1 = X, 2 = O)
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
            //if playing against s0meBot, start a timer of 0,5 - 2 seconds before the bot makes it's move
            //so that it feels more like a person rather than a computer making the moves.
            if (players.PlayerOne.FirstName == "s0me" && playerTurn == 1)
            {
                Random randomThinkingTime = new Random();
                int thinkingTime = randomThinkingTime.Next(500, 2100);
                bTimer = new System.Timers.Timer(thinkingTime);
                bTimer.Elapsed += OnTimedThinking;
                bTimer.Enabled = true;
                bTimer.AutoReset = false;

            }
            else if (players.PlayerTwo.FirstName == "s0me" && playerTurn == 2)
            {
                Random randomThinkingTime = new Random();
                int thinkingTime = randomThinkingTime.Next(500, 2100);
                bTimer = new System.Timers.Timer(thinkingTime);
                bTimer.Elapsed += OnTimedThinking;
                bTimer.Enabled = true;
                bTimer.AutoReset = false;
            }
            if (playerTurn == 1)
            {
                TurnDefinition.Text = $"Pelaajan {player1Name} vuoro";

            }
            else if(playerTurn == 2)
            {
                TurnDefinition.Text = $"Pelaajan {player2Name} vuoro";
                
            }
        }
    }
    private void BotTurnX()
    {
        //I could've used somekind of algorithm that makes the most "optimal move" 
        //but figured it would feel more like playing against a person rather than feeling like a computer just throws the moves at you
        // making the game impossible to win
        //While bot plays as X, use this as moves for bot
        Random rnd = new Random();
        Button checkWin, checkLoss, checkBlock;
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
                checkWin = CheckForWin("X");
                if (checkWin != null)
                {
                    SquareButton_Clicked(checkWin, null);
                }
                else
                {
                    checkLoss = CheckForLoss("O");
                    if (checkLoss != null)
                    {
                        SquareButton_Clicked(checkLoss, null);
                    }
                    else
                    {
                        // When 0 Blocks from a corner or side
                        checkBlock = CheckForBlock("X", "O");
                        if (checkBlock != null)
                        {
                            SquareButton_Clicked(checkBlock, null);
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
                        }
                    }
                }
                break;
            case 6:

                checkWin = CheckForWin("X");
                if (checkWin != null)
                {
                    SquareButton_Clicked(checkWin, null);
                }
                else
                {
                    checkLoss = CheckForLoss("O");
                    if (checkLoss != null)
                    {
                        SquareButton_Clicked(checkLoss, null);
                    }
                    else
                    {
                        Button[] squares = { A1, A2, A3, B1, B2, B3, C1, C2, C3 };
                        foreach (Button sqr in squares)
                        {
                            if (sqr.IsEnabled)
                            {
                                SquareButton_Clicked(sqr, null);
                                break;
                            }
                        }
                    }
                }
                break;
            case 8:
                checkWin = CheckForWin("X");
                if (checkWin != null)
                {
                    SquareButton_Clicked(checkWin, null);
                }
                else
                {
                    //Game not winnable, end the game
                    Button[] squares = { A1, A2, A3, B1, B2, B3, C1, C2, C3 };
                    foreach (Button sqr in squares)
                    {
                        if (sqr.IsEnabled)
                        {
                            SquareButton_Clicked(sqr, null);
                            break;
                        }
                    }
                }
                break;
        }
    }
    private void BotTurnO()
    {
        //When s0meBot plays as O it's an easier opponent than if it plays as X
        // while bot plays as O, use this as instructions for moves
        Random rnd = new Random();
        Button checkWin, checkLoss, checkBlock;
        switch (playedTurns)
        {
            case 1:
                //Bot plays second turn, choose randomly from remaining spots
                randomNumber = rnd.Next(1, 10);
                switch (randomNumber)
                {
                    case 1:
                        if (A1.IsEnabled)
                        {
                            SquareButton_Clicked(A1, null);
                        }
                        else
                        {
                            SquareButton_Clicked(C3, null);
                        }
                        break;
                    case 2:
                        if (A2.IsEnabled)
                        {
                            SquareButton_Clicked(A2, null);
                        }
                        else
                        {
                            SquareButton_Clicked(B2, null);
                        }
                        break;
                    case 3:
                        if (A3.IsEnabled)
                        {
                            SquareButton_Clicked(A3, null);
                        }
                        else
                        {
                            SquareButton_Clicked(C1, null);
                        }
                        break;
                    case 4:
                        if (B1.IsEnabled)
                        {
                            SquareButton_Clicked(B1, null);
                        }
                        else
                        {
                            SquareButton_Clicked(B2, null);
                        }
                        break;
                    case 5:
                        if (B2.IsEnabled)
                        {
                            SquareButton_Clicked(B2, null);
                        }
                        else
                        {
                            Button[] squares = { A1, A2, A3, B1, B3, C1, C2, C3 };
                            int rand = rnd.Next(9);
                            if (squares[rand].IsEnabled)
                            {
                                SquareButton_Clicked(squares[rand], null);
                                break;
                            }
                            
                        }
                        break;
                    case 6:
                        if (B3.IsEnabled)
                        {
                            SquareButton_Clicked(B3, null);
                        }
                        else
                        {
                            SquareButton_Clicked(B1, null);
                        }
                        break;
                    case 7:
                        if (C1.IsEnabled)
                        {
                            SquareButton_Clicked(C1, null);
                        }
                        else
                        {
                            SquareButton_Clicked(B2, null);
                        }
                        break;
                    case 8:
                        if (C2.IsEnabled)
                        {
                            SquareButton_Clicked(C2, null);
                        }
                        else
                        {
                            SquareButton_Clicked(A2, null);
                        }
                        break;
                    case 9:
                        if (C3.IsEnabled)
                        {
                            SquareButton_Clicked(C3, null);
                        }
                        else
                        {
                            SquareButton_Clicked(B2, null);
                        }
                        break;
                }
                break;
            case 3:
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
                                if (A2.IsEnabled)
                                {
                                    SquareButton_Clicked(A2, null);
                                }
                                else
                                {
                                    SquareButton_Clicked(C2, null);
                                }
                                break;
                            case 2:
                                if (B1.IsEnabled)
                                {
                                    SquareButton_Clicked(B1, null);
                                }
                                else
                                {
                                    SquareButton_Clicked(B3, null);
                                }
                                break;
                            case 3:
                                if (B3.IsEnabled)
                                {
                                    SquareButton_Clicked(B3, null);
                                }
                                else
                                {
                                    SquareButton_Clicked(B1, null);
                                }
                                break;
                            case 4:
                                if (C2.IsEnabled)
                                {
                                    SquareButton_Clicked(C2, null);
                                }
                                else
                                {
                                    SquareButton_Clicked(A2, null);
                                }
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
                    else if (r + 1 == 3 && !B2.IsEnabled)
                    {
                        checkLoss = CheckForLoss("X");
                        if (checkLoss != null)
                        {
                            SquareButton_Clicked(checkLoss, null);
                        }
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
            case 5:
                checkWin = CheckForWin("X");
                if (checkWin != null)
                {
                    SquareButton_Clicked(checkWin, null);
                }
                else
                {
                    checkBlock = CheckForBlock("O", "X");
                    if (checkBlock != null)
                    {
                        SquareButton_Clicked(checkBlock, null);
                    }
                    else
                    {
                        Button[] squares = { A1, A2, A3, B1, B2, B3, C1, C2, C3 };
                        foreach (Button sqr in squares)
                        {
                            if (sqr.IsEnabled)
                            {
                                SquareButton_Clicked(sqr, null);
                                break;
                            }
                        }
                    }
                }
                break;
            case 7:
                // Game winning moves
                checkWin = CheckForWin("O");
                if (checkWin != null)
                {
                    SquareButton_Clicked(checkWin, null);
                }
                else
                {
                    checkLoss = CheckForLoss("X");
                    if (checkLoss != null)
                    {
                        SquareButton_Clicked(checkLoss, null);
                    }
                    else
                    {
                        Button[] squares = { A1, A2, A3, B1, B2, B3, C1, C2, C3 };
                        foreach (Button sqr in squares)
                        {
                            if (sqr.IsEnabled)
                            {
                                SquareButton_Clicked(sqr, null);
                                break;
                            }
                        }
                    }
                }
                break;
            case 9:
                // Game winning moves
                checkWin = CheckForWin("O");
                if (checkWin != null)
                {
                    SquareButton_Clicked(checkWin, null);
                }
                else
                {
                    //Game not winnable, end the game
                    Button[] squares = { A1, A2, A3, B1, B2, B3, C1, C2, C3 };
                    foreach (Button sqr in squares)
                    {
                        if (sqr.IsEnabled)
                        {
                            SquareButton_Clicked(sqr, null);
                            break;
                        }
                    }
                }
                break;
        }
    }
    private Button CheckForLoss(string botInitial)
    {
        //This method checks for possible lost game by checking any predefined 2 consecutive squares in following order
        //No winning move available, check for opponents chance of winning
        if (B1.Text == botInitial && B2.Text == botInitial && B3.IsEnabled
            || A3.Text == botInitial && C3.Text == botInitial && B3.IsEnabled)
        {
            return B3;
        }
        else if (B3.Text == botInitial && B2.Text == botInitial && B1.IsEnabled
                || A1.Text == botInitial && C1.Text == botInitial && B1.IsEnabled)
        {
            return B1;
        }
        else if (B2.Text == botInitial && C2.Text == botInitial && A2.IsEnabled
                || A1.Text == botInitial && A3.Text == botInitial && A2.IsEnabled)
        {
            return A2;
        }
        else if (B2.Text == botInitial && A2.Text == botInitial && C2.IsEnabled
                || C1.Text == botInitial && C3.Text == botInitial && C2.IsEnabled)
        {
            return C2;
        }
        //No wins from opponent when playing any of the sides either, check for corners
        else
        {
            if (C1.Text == botInitial && B2.Text == botInitial && A3.IsEnabled
                || A1.Text == botInitial && A2.Text == botInitial && A3.IsEnabled
                || C3.Text == botInitial && B3.Text == botInitial && A3.IsEnabled)
            {
                return A3;
            }
            else if (A1.Text == botInitial && B2.Text == botInitial && C3.IsEnabled
                    || C1.Text == botInitial && C2.Text == botInitial && C3.IsEnabled
                    || A3.Text == botInitial && B3.Text == botInitial && C3.IsEnabled)
            {
                return C3;
            }
            else if (C1.Text == botInitial && B1.Text == botInitial && A1.IsEnabled
                    || C3.Text == botInitial && B2.Text == botInitial && A1.IsEnabled
                    || A3.Text == botInitial && A2.Text == botInitial && A1.IsEnabled)
            {
                return A1;
            }
            else if (A1.Text == botInitial && B1.Text == botInitial && C1.IsEnabled
                    || C3.Text == botInitial && C2.Text == botInitial && C1.IsEnabled
                    || A3.Text == botInitial && B2.Text == botInitial && C1.IsEnabled)
            {
                return C1;
            }
            else
            {
                return null;
            }
        }
    }
    private Button CheckForWin(string botInitial)
    {
        //This method checks possible winning moves that result in winning the game
        // Game winning moves
        //Win from playing C3
        if (!A1.IsEnabled && A1.Text == botInitial && !B2.IsEnabled && B2.Text == botInitial && C3.IsEnabled
            || !A3.IsEnabled && A3.Text == botInitial && !B3.IsEnabled && B3.Text == botInitial && C3.IsEnabled
            || !C1.IsEnabled && C1.Text == botInitial && !C2.IsEnabled && C2.Text == botInitial && C3.IsEnabled)
        {
            return C3;
        }
        //Win from playing C1
        else if (!A3.IsEnabled && A3.Text == botInitial && !B2.IsEnabled && B2.Text == botInitial && C1.IsEnabled
                || !A1.IsEnabled && A1.Text == botInitial && !B1.IsEnabled && B1.Text == botInitial && C1.IsEnabled
                || !C3.IsEnabled && C3.Text == botInitial && !C2.IsEnabled && C2.Text == botInitial && C1.IsEnabled)
        {
            return C1;
        }
        //win from playing A3
        else if (!C1.IsEnabled && C1.Text == botInitial && !B2.IsEnabled && B2.Text == botInitial && A3.IsEnabled
                || !C3.IsEnabled && C3.Text == botInitial && !B3.IsEnabled && B3.Text == botInitial && A3.IsEnabled
                || !A1.IsEnabled && A1.Text == botInitial && !A2.IsEnabled && A2.Text == botInitial && A3.IsEnabled)
        {
            return A3;
        }
        //Win from playing A1
        else if (!C3.IsEnabled && C3.Text == botInitial && !B2.IsEnabled && B2.Text == botInitial && A1.IsEnabled
                || !C1.IsEnabled && C1.Text == botInitial && !B1.IsEnabled && B1.Text == botInitial && A1.IsEnabled
                || !A3.IsEnabled && A3.Text == botInitial && !A2.IsEnabled && A2.Text == botInitial && A1.IsEnabled)
        {
            return A1;
        }
        else if (playedTurns > 4)
        {
                    //Win from playing A2
            if (C2.Text == botInitial && B2.Text == botInitial && A2.IsEnabled
                || A1.Text == botInitial && A3.Text == botInitial && A2.IsEnabled)
            {
                return A2;
            }
            //Win from playing B1
            else if (B3.Text == botInitial && B2.Text == botInitial && B1.IsEnabled
                    || A1.Text == botInitial && C1.Text == botInitial && B1.IsEnabled)
            {
                return B1;
            }
            //Win from playing B3
            else if (B1.Text == botInitial && B2.Text == botInitial && B3.IsEnabled
                    || A3.Text == botInitial && C3.Text == botInitial && B3.IsEnabled)
            {
                return B3;
            }
            //Win from playing C2
            else if (A2.Text == botInitial && B2.Text == botInitial && C2.IsEnabled
                    || C1.Text == botInitial && C3.Text == botInitial && C2.IsEnabled)
            {
                return C2;
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }
    private Button CheckForBlock(string botInitial, string opponentInitial)
    {
        // When opponent Blocks from a corner or side
        //Corners:
        // opponent at C3
        if (A3.Text == botInitial && B3.Text == botInitial && C3.Text == opponentInitial)
        {
            if (B2.IsEnabled)
            {
                return B2;
            }
            else
            {
                return A2;
            }

        }
        else if (A1.Text == botInitial && B2.Text == botInitial && C3.Text == opponentInitial)
        {
            if (B1.IsEnabled)
            {
                return B1;
            }
            else
            {
                return A2;
            }
        }
        else if (C1.Text == botInitial && C2.Text == botInitial && C3.Text == opponentInitial)
        {
            if (B2.IsEnabled)
            {
                return B2;
            }
            else
            {
                return A1;
            }
        }
        // opponent at C1
        else if (A3.Text == botInitial && B2.Text == botInitial && C1.Text == opponentInitial)
        {
            if (B3.IsEnabled)
            {
                return B3;
            }
            else
            {
                return A2;
            }
        }
        else if (A1.Text == botInitial && B1.Text == botInitial && C1.Text == opponentInitial)
        {
            if (B2.IsEnabled)
            {
                return B2;
            }
            else
            {
                return A3;
            }
        }
        else if (C3.Text == botInitial && C2.Text == botInitial && C1.Text == opponentInitial)
        {
            if (B2.IsEnabled)
            {
                return B2;
            }
            else
            {
                return A3;
            }
        }
        // opponent at A3
        else if (A1.Text == botInitial && A2.Text == botInitial && A3.Text == opponentInitial)
        {
            if (B2.IsEnabled)
            {
                return B2;
            }
            else
            {
                return C1;
            }
        }
        else if (C1.Text == botInitial && B2.Text == botInitial && A3.Text == opponentInitial)
        {
            if (B1.IsEnabled)
            {
                return B1;
            }
            else
            {
                return C2;
            }
        }
        else if (C3.Text == botInitial && B3.Text == botInitial && A3.Text == opponentInitial)
        {
            if (B2.IsEnabled)
            {
                return B2;
            }
            else
            {
                return C1;
            }
        }
        // opponent at A1
        else if (C1.Text == botInitial && B1.Text == botInitial && A1.Text == opponentInitial)
        {
            if (B2.IsEnabled)
            {
                return B2;
            }
            else
            {
                return C3;
            }
        }
        else if (C3.Text == botInitial && B2.Text == botInitial && A1.Text == opponentInitial)
        {
            if (C2.IsEnabled)
            {
                return C2;
            }
            else
            {
                return B3;
            }
        }
        else if (A3.Text == botInitial && A2.Text == botInitial && A1.Text == opponentInitial)
        {
            if (B2.IsEnabled)
            {
                return B2;
            }
            else
            {
                return C3;
            }
        }
        //Sides:
        // opponent at A2
        else if (C2.Text == botInitial && B2.Text == botInitial && A2.Text == opponentInitial)
        {
            if (B3.IsEnabled)
            {
                return B3;
            }
            else
            {
                return A1;
            }
        }
        // opponent at B3
        else if (B1.Text == botInitial && B2.Text == botInitial && B3.Text == opponentInitial)
        {
            if (A2.IsEnabled)
            {
                return A2;
            }
            else
            {
                return C2;
            }
        }
        // opponent at B1
        else if (B3.Text == botInitial && B2.Text == botInitial && B1.Text == opponentInitial)
        {
            if (A3.IsEnabled)
            {
                return A3;
            }
            else
            {
                return C3;
            }
        }
        // opponent at C2
        else if (A2.Text == botInitial && B2.Text == botInitial && C2.Text == opponentInitial)
        {
            if (B1.IsEnabled)
            {
                return B1;
            }
            else
            {
                return C3;
            }
        }
        else
        {
            return null;
        }
    }
    private bool WinningConditionMet()
	{
        //All the possible winning conditions below
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
    private void OnTimedThinking(object sender, System.Timers.ElapsedEventArgs e)
    {
        //Method for executing a "thinking time" for the bot before it makes the move
        bTimer.Close();
        if (playerTurn == 1)
        {
            Dispatcher.Dispatch(() =>
            {
                BotTurnX();
            });
        }
        else if (playerTurn == 2)
        {
            Dispatcher.Dispatch(() =>
            {
                BotTurnO();
            });

        }
    }
    private void OnTimedEvent(object sender, System.Timers.ElapsedEventArgs e)
    {
        //Timer runs from the moment gamepage opens, and every 60 "events" (seconds) manually add minutes 
        //to measure played time in minutes, but also add seconds to playerprofile to keep track of actual played time
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