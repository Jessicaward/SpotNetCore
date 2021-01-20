using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using SpotNetCore.Models;
using SpotNetCore.Services;

namespace SpotNetCore.Implementation
{
    /// <summary>
    /// Main application loop, parses and executes commands
    /// </summary>
    public class CommandHandler : IDisposable
    {
        private readonly AuthenticationManager _authenticationManager;
        private readonly PlayerService _playerService;
        private readonly SearchService _searchService;
        private static readonly List<string> _listOfCommands;

        public CommandHandler(AuthenticationManager authenticationManager, PlayerService playerService, SearchService searchService)
        {
            _authenticationManager = authenticationManager;
            _playerService = playerService;
            _searchService = searchService;            
        }

        static CommandHandler()
        {
            // set of commands for autocomplete 
            _listOfCommands = new List<string>()
            {
                "play",
                "pause",
                "current",
                "next",
                "previous",
                "restart",
                "shuffle",
                "shuffle on",
                "shuffle off",
                "shuffle false",
                "shuffle true",
                "help",
                "exit",
                "close",
                "quit"
            };
            _listOfCommands.Sort();
        }

        ~CommandHandler()
        {
            Dispose(false);
        }

        public async Task HandleCommands()
        {
            var exit = false;
            while (!exit)
            {
                var command = ParseCommand(GetUserInput());

                var spotifyCommand = command.Command.Trim().ToLower() switch
                {
                    "play" => SpotifyCommand.PlayCurrentTrack,
                    "pause" => SpotifyCommand.PauseCurrentTrack,
                    "next" => SpotifyCommand.NextTrack,
                    "previous" => SpotifyCommand.PreviousTrack,
                    "restart" => SpotifyCommand.RestartTrack,
                    "artist" => SpotifyCommand.PlayArtist,
                    "track" => SpotifyCommand.PlayTrack,
                    "album" => SpotifyCommand.PlayAlbum,
                    "playlist" => SpotifyCommand.PlayPlaylist,
                    "shuffle" => SpotifyCommand.Shuffle,
                    "repeat" => SpotifyCommand.Repeat,
                    "volume" => SpotifyCommand.Volume,
                    "help" => SpotifyCommand.Help,
                    "exit" => SpotifyCommand.Exit,
                    "close" => SpotifyCommand.Exit,
                    "quit" => SpotifyCommand.Exit,
                    "queue" => SpotifyCommand.Queue,
                    "current" => SpotifyCommand.Current,
                    "clear" => SpotifyCommand.ClearQueue,
                    _ => SpotifyCommand.Invalid
                };

                if (spotifyCommand == SpotifyCommand.Invalid)
                {
                    Terminal.WriteRed($"{command.Command} is not a valid command.");
                    HelpManager.DisplayHelp();
                }

                if (spotifyCommand == SpotifyCommand.Exit)
                {
                    exit = true;
                    break;
                }

                if (spotifyCommand == SpotifyCommand.Help)
                {
                    HelpManager.DisplayHelp();
                    break;
                }

                if (!AuthenticationManager.IsAuthenticated)
                {
                    throw new NotAuthenticatedException();
                }

                //The rest of the commands require authentication
                if (_authenticationManager.Token == null || _authenticationManager.IsTokenAboutToExpire())
                {
                    await _authenticationManager.RequestRefreshedAccessToken();
                }

                if (spotifyCommand == SpotifyCommand.PlayCurrentTrack)
                {
                    await _playerService.PlayCurrentTrack();
                }

                if (spotifyCommand == SpotifyCommand.PauseCurrentTrack)
                {
                    await _playerService.PauseCurrentTrack();
                }

                if (spotifyCommand == SpotifyCommand.Current)
                {
                    Terminal.WriteCurrentSong(await _playerService.GetPlayerContext());
                }

                if (spotifyCommand == SpotifyCommand.NextTrack)
                {
                    Terminal.WriteCurrentSong(await _playerService.NextTrack());
                }

                if (spotifyCommand == SpotifyCommand.PreviousTrack)
                {
                    Terminal.WriteCurrentSong(await _playerService.PreviousTrack());
                }

                if (spotifyCommand == SpotifyCommand.RestartTrack)
                {
                    await _playerService.RestartTrack();

                    Terminal.WriteCurrentSong(await _playerService.GetPlayerContext());
                }

                if (spotifyCommand == SpotifyCommand.Shuffle)
                {
                    var toggle = command.Parameters.IsNullOrEmpty()
                        ? (bool?)null
                        : command.Parameters.First().Query == "on" || command.Parameters.First().Query == "true";

                    await _playerService.ShuffleToggle(toggle);
                }

                if (spotifyCommand == SpotifyCommand.Queue)
                {
                    //At least one parameter is required
                    if (command.Parameters.IsNullOrEmpty())
                    {
                        Terminal.WriteRed($"{command.Command} is not a valid command.");
                        HelpManager.DisplayHelp();
                        break;
                    }

                    if (command.Parameters.Any(x => x.Parameter.ToLower() == "track"))
                    {
                        var parameter = command.Parameters.First(x => x.Parameter.ToLower() == "track");
                        var tracks = (await _searchService.SearchForTrack(parameter.Query)).ToList();

                        if (tracks.IsNullOrEmpty())
                        {
                            Terminal.WriteRed($"Could not find track {parameter.Query}");
                            break;
                        }

                        var track = tracks.First();

                        await _playerService.QueueTrack(track.Uri);

                        Terminal.WriteYellow($"Queueing {track.Name}");
                    }

                    if (command.Parameters.Any(x => x.Parameter.ToLower() == "album"))
                    {
                        var parameter = command.Parameters.First(x => x.Parameter.ToLower() == "album");
                        SpotifyAlbum album;

                        try
                        {
                            album = await _searchService.SearchForAlbum(parameter.Query);
                        }
                        catch (NoSearchResultException e)
                        {
                            Terminal.WriteRed($"Could not find album {parameter.Query}");
                            break;
                        }

                        try
                        {
                            foreach (var track in album.Tracks)
                            {
                                await _playerService.QueueTrack(track.Uri);
                            }
                        }
                        catch (Exception e)
                        {
                            Terminal.WriteRed($"Could not queue album. Error: {e}");
                            break;
                        }

                        Terminal.WriteYellow($"Queueing {album.Name}");
                    }

                    if (command.Parameters.Any(x => x.Parameter.ToLower() == "playlist"))
                    {
                        var parameter = command.Parameters.First(x => x.Parameter.ToLower() == "playlist");

                        SpotifyPlaylist playlist;

                        try
                        {
                            playlist = await _searchService.SearchForPlaylist(parameter.Query);
                        }
                        catch (NoSearchResultException e)
                        {
                            Terminal.WriteYellow($"Could not find playlist {parameter.Query}");
                            break;
                        }

                        foreach (var track in playlist.Tracks)
                        {
                            await _playerService.QueueTrack(track.Uri);
                        }

                        Terminal.WriteYellow($"Queueing {playlist.Name}");
                    }

                    if (command.Parameters.Any(x => x.Parameter.ToLower() == "artist"))
                    {
                        var parameter = command.Parameters.First(x => x.Parameter.ToLower() == "artist");

                        var option = command.Parameters.FirstOrDefault(x => x.Parameter.ToLower() != "artist");
                        var artistOption = option?.Query switch
                        {
                            "d" => ArtistOption.Discography,
                            "discography" => ArtistOption.Discography,
                            "p" => ArtistOption.Popular,
                            "popular" => ArtistOption.Popular,
                            "e" => ArtistOption.Essential,
                            "essential" => ArtistOption.Essential,
                            _ => ArtistOption.Essential
                        };

                        SpotifyArtist artist;

                        try
                        {
                            artist = await _searchService.SearchForArtist(parameter.Query, artistOption);
                        }
                        catch (NoSearchResultException e)
                        {
                            Terminal.WriteRed($"Could not find album. Error: {e}");
                            break;
                        }

                        try
                        {
                            foreach (var track in artist.Tracks)
                            {
                                await _playerService.QueueTrack(track.Uri);
                            }
                        }
                        catch (Exception e)
                        {
                            Terminal.WriteRed($"Could not queue artist. Error: {e}");
                        }

                        switch (artistOption)
                        {
                            case ArtistOption.Discography:
                                Terminal.WriteYellow($"Queueing {artist.Name}'s Discography");
                                break;
                            case ArtistOption.Popular:
                                Terminal.WriteYellow($"Queueing {artist.Name}'s top tracks");
                                break;
                            case ArtistOption.Essential:
                                Terminal.WriteYellow($"Queueing 'This Is {artist.Name}'");
                                break;
                        }
                    }
                }
            }
        }

        private static string GetUserInput()
        {
            return GetInputWithAutoComplete();
        }

        /// <summary>
        /// Gets User Input By Suggesting the Autocomplete
        /// </summary>
        /// <returns> User input </returns>
        private static string GetInputWithAutoComplete()
        {
            // to store the user input till now
            StringBuilder stringBuilder = new StringBuilder();

            // to store the current key entered by the user
            var userInput = Console.ReadKey(intercept: true);

            // Process input until user presses Enter
            while (ConsoleKey.Enter != userInput.Key)
            {
                // If Tab
                if (ConsoleKey.Tab == userInput.Key)
                {
                    HandleTabInput(stringBuilder);
                }
                // Non Tab Key
                else
                {
                    HandleKeyInput(stringBuilder, userInput);
                }
                
                // read next key entered by user
                userInput = Console.ReadKey(intercept: true);
            }

            // when user presses enter, move the cursor to the next line
            Console.Write("\n");

            // return the user input
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Processes the user input which is not a tab
        /// </summary>
        /// <param name="stringBuilder"> string builder which stores the user input till now </param>
        /// <param name="userInput"> the current user input </param>
        private static void HandleKeyInput(StringBuilder stringBuilder, ConsoleKeyInfo userInput)
        {
            // current input
            string currentInput = stringBuilder.ToString();

            // Handle backspace
            if(ConsoleKey.Backspace == userInput.Key )
            {
                // if user has pressed backspace, remove the last character from the console output and current input
                if(currentInput.Length > 0)
                {
                    // remove from current input
                    stringBuilder.Remove(stringBuilder.Length - 1, 1);
                    
                    // clear the line
                    ClearCurrentLine();

                    // remove from string and print on console
                    currentInput = currentInput.Remove(currentInput.Length - 1);
                    Console.Write(currentInput);
                }
            }

            // if key is space bar, add " " to the input
            else if(ConsoleKey.Spacebar == userInput.Key)
            {
                stringBuilder.Append(" ");
                Console.Write(" ");
            }

            // all other keys
            else
            {
                // To Lower is done because when we read a key using Console.ReadKey(),
                // the uppercase is returned irrespective of the case of the user input.
                var key = userInput.Key;
                stringBuilder.Append(key.ToString().ToLower());
                Console.Write(key.ToString().ToLower());
            }
        }

        /// <summary>
        /// Handle Tab Input
        /// </summary>
        /// <param name="stringBuilder"> String Builder </param>
        private static void HandleTabInput(StringBuilder stringBuilder)
        {
            // current input
            string currentInput = stringBuilder.ToString();

            // check if input is already a part of the commands list
            int indexOfInput = _listOfCommands.IndexOf(currentInput);

            // match
            string match = "";

            // if input is a part of the commands list : 
            // then display the next command in alphabetical order
            if(-1 != indexOfInput)
            {
                match = indexOfInput + 1 < _listOfCommands.Count() ? _listOfCommands[indexOfInput + 1] : "";
            }
            // if input isnt in the commands list, find the first match
            else
            {
                match = _listOfCommands.FirstOrDefault(m => m.StartsWith(currentInput, true, CultureInfo.InvariantCulture));
            }

            // no match
            if(string.IsNullOrEmpty(match))
            {
                return;
            }

            // clear line and current input
            ClearCurrentLine();
            stringBuilder.Clear();

            // set line and current input to the current match
            Console.Write(match);
            stringBuilder.Append(match);
        }

        /// <summary>
        /// Clear Current Line on the Console Output
        /// </summary>
        private static void ClearCurrentLine()
        {
            var currentLine = Console.CursorTop;
            Console.SetCursorPosition(0, Console.CursorTop);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(0, currentLine);
        }

        private static ParsedCommand ParseCommand(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Input must contain command");
            }
            
            var split = input.Split(new[] { "-", "--" }, StringSplitOptions.RemoveEmptyEntries);

            return new ParsedCommand
            {
                Command = split[0].Trim(),
                Parameters = split.Skip(1).Select(x => new ParsedParameter()
                {
                    Parameter = x.Substring(0, x.IndexOf(' ') + 1).Trim(),
                    Query = x.Substring(x.IndexOf(' ') + 1).Trim()
                })
            };
        }
        
        private void Dispose(bool disposing)
        {
            if (!disposing) return;

            _playerService?.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}