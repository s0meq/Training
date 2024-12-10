using System.Collections.ObjectModel;

namespace Training;

public partial class GamePage : ContentPage
{
	Move move;
    public ObservableCollection<Move> Moves { get; set; }
	int playerTurn = 1;
    int playedTurns = 0;
    string sSelectedSquare;
    public GamePage()
	{
		InitializeComponent();
        BindingContext = this;
        Moves = new ObservableCollection<Move>();
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
        Console.WriteLine("Button clicked");
        
        foreach (Button button in squares)
        {
            if (button == selectedSquare)
            {
                sSelectedSquare = button.AutomationId;
            }
        }
        playedTurns++;
        move.PlayerName = $"Pelaaja: {playerTurn}";
        move.PlayedSquareName = $"Pelattu Ruutu: {sSelectedSquare}";
        move.PlayedTurn = $"Pelatut vuorot: {playedTurns}";
        Moves.Add(move);
        selectedSquare.Text = playerTurn == 1 ? "X" : "O";
        selectedSquare.IsEnabled = false;

        if (WinningConditionMet())
        {
            foreach (Button button in squares)
            {
                button.IsEnabled = false;
            }
            await DisplayAlert("Peli p‰‰ttyi", $"Pelaaja {playerTurn} voitti pelin!", "Lopeta");
        }
        else
        {
            playerTurn = playerTurn == 1 ? 2 : 1;
        }
    }

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
}