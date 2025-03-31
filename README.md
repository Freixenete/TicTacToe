# TicTacToe

Program that analyzes a Tic Tac Toe 10000 matches from a Docker Container. Fist touch with Json serialization

## Code

```csharp
// Path: TicTacToe/Program.cs
 private static async Task PlayersList()
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
