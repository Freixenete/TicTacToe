using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Net.Http.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;


namespace TicTacToe;

class Program
{
    private static readonly HttpClient cliente = new HttpClient { BaseAddress = new Uri("http://localhost:8080/") };
    
    private static Dictionary<string, string> players = new();
    private static Dictionary<string, int> wins = new();
    //private static List<string> listaJugadores = new List<string>();

    static async Task Main(string[] args)
    {
        await ListaJugadores();
        //Task.Delay(500);
        await Partidas();
        ShowWiner();
    }

    private static async Task ListaJugadores()
    {
        //var listaJugadores = await cliente.GetFromJsonAsync<List<string>>($"/jugadors");
        string participantes = await cliente.GetStringAsync("/jugadors");
        var listaJugadores = JsonSerializer.Deserialize<List<string>>(participantes);

        Regex regex = new Regex(@"participant ([A-Z]+\w+ [A-Z-'-a-z]+\w+).*representa(nt)? (a |de )([A-Z-a-z]+\w+)");
        
        foreach (var respuesta in listaJugadores)
        {
            Match match = regex.Match(respuesta);
            if (match.Success)
            {
                string name = match.Groups[1].Value;
                string country = match.Groups[4].Value;
                if (country != "Espanya")
                {
                    players[name] = country;
                    if (wins.ContainsKey(name))
                    {
                        wins[name] = wins[name] + 1;
                    }
                    else
                    {
                        wins[name] = 0;
                    }
                    Console.WriteLine("- " + name + " from " + country);

                }

            }
        }
        

    }

    private static async Task Partidas()
    {
        for (int i = 1; i <= 10000; i++)
        {
            try
            {
                var match = await cliente.GetFromJsonAsync<Partida>($"/partida/{i}");
                
                if (players.ContainsKey(match.jugador1) && players.ContainsKey(match.jugador2))
                {
                    string winer = VerGanador(match.Tauler);
                    if (winer == "O" && wins.ContainsKey(match.jugador1))
                    {
                        wins[match.jugador1]++;
                    }
                    else if (winer == "X" && wins.ContainsKey(match.jugador2))
                    {
                        wins[match.jugador2]++;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error procesing the game {i}: {ex.Message}");
            }
        }
    }

    private static string VerGanador(List<string> board)
    {
        string[] lines = new string[8]
        {
            board[0], board[1], board[2],  // Filas
            "" + board[0][0] + board[1][0] + board[2][0], // Columnas
            "" + board[0][1] + board[1][1] + board[2][1],
            "" + board[0][2] + board[1][2] + board[2][2],
            "" + board[0][0] + board[1][1] + board[2][2], // Diagonales
            "" + board[0][2] + board[1][1] + board[2][0]
        };

        if (Array.Exists(lines, a => a == "XXX"))
        {
            return "X";
        }
        if (Array.Exists(lines, a => a == "OOO"))
        {
            return "O";
        }
        return "";
    }

    private static void ShowWiner()
    {
        int maxWins = 0;
        List<string> winers = new();

        foreach (var player in wins)
        {
            if (player.Value > maxWins)
            {
                maxWins = player.Value;
                winers.Clear();
                winers.Add(player.Key);
            }
            else if (player.Value == maxWins)
            {
                winers.Add(player.Key);
            }
        }
        
        Console.WriteLine($"The winner of the tournament is: ");
        foreach (var winer in winers)
        {
            Console.WriteLine($"{winer} from {players[winer]} won {maxWins} games!!");
        }
    }
}