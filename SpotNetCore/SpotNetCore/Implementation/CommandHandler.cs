using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using SpotNetCore.Models;
using SpotNetCore.Services;

namespace SpotNetCore.Implementation
{
    /// <summary>
    /// Main application loop, parses and executes commands
    /// </summary>
    public class CommandHandler
    {
        private readonly AuthenticationManager _authenticationManager;
        private readonly PlayerService _playerService;
        private readonly SearchService _searchService;
        private readonly AppSettings _appSettings;
        private static readonly List<string> ListOfCommands = Commands.GetCommandsList();
        private readonly CyclicLimitedList<String> _commandHistory;
     

        public CommandHandler(AuthenticationManager authenticationManager, AppSettings appSettings, PlayerService playerService, SearchService searchService)
        {
            _appSettings = appSettings;
            _authenticationManager = authenticationManager;
            _playerService = playerService;
            _searchService = searchService;
            _commandHistory = new();
        }

        public async Task HandleCommands()
        {
            while (true)
            {
                ClearCurrentLine();
                var input = GetUserInput();
                var command = ParseCommand(input);
                _commandHistory.Add(input);

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
                    "settings" => SpotifyCommand.Settings,
                    _ => SpotifyCommand.Invalid
                };

                if (spotifyCommand == SpotifyCommand.Invalid)
                {
                    Terminal.WriteRed($"{command.Command} is not a valid command.");
                    HelpManager.DisplayHelp();
                }

                if (spotifyCommand == SpotifyCommand.Exit)
                {
                    break;
                }

                if (spotifyCommand == SpotifyCommand.Help)
                {
                    Terminal.Startup();
                    HelpManager.DisplayHelp();
                }

                if (!_authenticationManager.IsAuthenticated)
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

                if (spotifyCommand == SpotifyCommand.Settings)
                {
                    var option = command.Parameters.FirstOrDefault(x => x.Parameter.ToLower() != "settings");
            
                    if (option == null || !Enum.TryParse(option.Parameter.ToLower().FirstCharToUpper(), out SettingOption settingOption))
                    {
                        HelpManager.DisplayHelp();
                        continue;
                    }
                    
                    switch (settingOption)
                    {
                      case SettingOption.Market:
                          if (!option.Query.IsNullOrEmpty())
                          {
                              _appSettings.Market = option.Query;
                              Terminal.WriteLine($"Market set to: {_appSettings.Market}");
                          }
                          else
                            Terminal.WriteLine($"Market: {_appSettings.Market}");
                          break;
                    }
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
                            album = (await _searchService.SearchForAlbum(parameter.Query))?.FirstOrDefault();
                        }
                        catch (NoSearchResultException)
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
                            playlist = (await _searchService.SearchForPlaylist(parameter.Query,1))?.FirstOrDefault();
                        }
                        catch (NoSearchResultException)
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
                            "d"           => ArtistOption.Discography,
                            "discography" => ArtistOption.Discography,
                            "p"           => ArtistOption.Popular,
                            "popular"     => ArtistOption.Popular,
                            "e"           => ArtistOption.Essential,
                            "essential"   => ArtistOption.Essential,
                            _             => ArtistOption.Essential
                        };

                        SpotifyArtist artist;

                        try
                        {
                            artist = (await _searchService.SearchForArtist(parameter.Query, artistOption, 1))?.FirstOrDefault();
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

        private string GetUserInput()
        {
            return GetInputWithAutoComplete();
        }

        /// <summary>
        /// Gets User Input By Suggesting the Autocomplete
        /// </summary>
        /// <returns> User input </returns>
        private string GetInputWithAutoComplete()
        {
            // to store the user input till now
            ConsoleInput consoleInput = new ConsoleInput();

            // to store the current key entered by the user
            ConsoleKeyInfo userInput;
            do
            {
                userInput = Console.ReadKey(intercept: true);
                // If Tab
                if (ConsoleKey.Tab == userInput.Key)
                {
                    HandleTabInput(consoleInput);
                }
                else if (ConsoleKey.Enter == userInput.Key)
                {
                    break;
                }
                else
                {
                    HandleKeyInput(consoleInput, userInput);
                }
                
            } while (ConsoleKey.Enter != userInput.Key);
            // when user presses enter, move the cursor to the next line
            Console.Write("\n");

            // return the user input
            return consoleInput;
        }

        /// <summary>
        /// Processes the user input which is not a tab
        /// </summary>
        /// <param name="consoleInput"> Console Input which stores the user input till now </param>
        /// <param name="userInput"> the current user input </param>
        private void HandleKeyInput(ConsoleInput consoleInput, ConsoleKeyInfo userInput)
        {
            // current input
            string currentInput = consoleInput;

            // Handle backspace
            if(ConsoleKey.Backspace == userInput.Key)
            {
                // if user has pressed backspace, remove the last character from the console output and current input
                if(currentInput.Length > 0 && consoleInput.CurrentIndex > 0)
                {
                    var number = consoleInput.CurrentIndex > 0 ? consoleInput.CurrentIndex - 1 : 0;
                    consoleInput.Remove(number, 1);
                    
                    // clear the line
                    ClearCurrentLine();

                    // remove from string and print on console
                    currentInput = consoleInput;
                    Console.Write(currentInput);
                    Terminal.SetCursorPosition(number,Console.CursorTop);
                }
            }
            else if (ConsoleKey.UpArrow == userInput.Key)
            {
                ClearCurrentLine();
                consoleInput.Clear();
                currentInput = _commandHistory.GetPrevious();
                consoleInput.Append(currentInput);
                
                Console.Write($"{currentInput}");
            } 
            else if (ConsoleKey.DownArrow == userInput.Key)
            {
                ClearCurrentLine();
                consoleInput.Clear();
                currentInput = _commandHistory.GetNext();
                consoleInput.Append(currentInput);
                
                Console.Write($"{currentInput}");
            }
            else if (ConsoleKey.LeftArrow == userInput.Key)
            {
                var number = consoleInput.CurrentIndex > 0 ? --consoleInput.CurrentIndex : 0;
                
                Terminal.SetCursorPosition(number,Console.CursorTop);
            }
            else if (ConsoleKey.RightArrow == userInput.Key)
            {
                var number = consoleInput.CurrentIndex < consoleInput.Length? ++consoleInput.CurrentIndex : consoleInput.Length;
                Terminal.SetCursorPosition(number,Console.CursorTop);
            }
            // all other keys
            else
            {
                // To Lower is done because when we read a key using Console.ReadKey(),
                // the uppercase is returned irrespective of the case of the user input.
                var key = userInput.KeyChar;
                consoleInput.Insert(key);
                ClearCurrentLine();
                Console.Write(consoleInput);
                Terminal.SetCursorPosition(consoleInput.CurrentIndex,Console.CursorTop);
            }
        }

        /// <summary>
        /// Handle Tab Input
        /// </summary>
        /// <param name="consoleInput"> Console Input </param>
        private void HandleTabInput(ConsoleInput consoleInput)
        {
            // current input
            string currentInput = consoleInput;

            // check if input is already a part of the commands list
            int indexOfInput = ListOfCommands.IndexOf(currentInput);

            // match
            string match;

            // if input is a part of the commands list : 
            // then display the next command in alphabetical order
            if(-1 != indexOfInput)
            {
                match = indexOfInput + 1 < ListOfCommands.Count() ? ListOfCommands[indexOfInput + 1] : "";
            }
            // if input isnt in the commands list, find the first match
            else
            {
                match = ListOfCommands.FirstOrDefault(m => m.StartsWith(currentInput, true, CultureInfo.InvariantCulture));
            }

            // no match
            if(string.IsNullOrEmpty(match))
            {
                return;
            }

            // clear line and current input
            ClearCurrentLine();
            consoleInput.Clear();

            // set line and current input to the current match
            Console.Write(match);
            consoleInput.Append(match);
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
            Console.Write(AppConstants.Prompt);
        }

        private static ParsedCommand ParseCommand(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Input must contain command");
            }
            
            var split = input.Split(new[] { "-", "--" }, StringSplitOptions.RemoveEmptyEntries);
            //var split = input.Split(" ");
            var p =new ParsedCommand
            {
                Command = split[0].Trim(),
                Parameters = split.Skip(1).Select(x =>
                {
                    var match = Regex.Match(x.Trim(), "^(?'Key'[A-z0-9]+)(?:[= ](?'Value'.+))?$");
                    return new ParsedParameter()
                    {
                        Parameter = match.Groups["Key"].Value.Trim(),
                        Query = match.Groups["Value"].Value.Trim()
                    };
                })
            };

            return p;
        }

        public class ConsoleInput
        {
            private StringBuilder Buffer { get; } = new StringBuilder();
            public int CurrentIndex { get; set; }

            public ConsoleInput Append(String str)
            {
                Buffer.Append(str);
                CurrentIndex += str.Length;
                return this;
            }
            public ConsoleInput Append(Char c)
            {
                Buffer.Append(c);
                CurrentIndex++;
                return this;
            }

            public ConsoleInput Insert(char c, int index = -1) => Insert(c.ToString(), index);

            public ConsoleInput Insert(String str, int index = -1)
            {
                if (index == -1)
                {
                    index = CurrentIndex;
                }
                Buffer.Insert(index, str);
                CurrentIndex = index + str.Length;
                return this;
            }

            public ConsoleInput Clear()
            {
                Buffer.Clear();
                CurrentIndex = 0; 
                return this;
            }

            public ConsoleInput Remove(int offset, int count)
            {
                Buffer.Remove(offset, count);
                CurrentIndex = offset - count + 1;
                return this;
            }

            public override string ToString()
            {
                return Buffer.ToString();
            }

            public static implicit operator string(ConsoleInput consoleInput)
            {
                return consoleInput.ToString();}

            public int Length => Buffer.Length;
        }
    }
}