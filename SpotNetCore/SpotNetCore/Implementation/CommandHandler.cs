using System;
using System.Linq;
using System.Threading.Tasks;
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

        public CommandHandler(AuthenticationManager authenticationManager, PlayerService playerService, SearchService searchService)
        {
            _authenticationManager = authenticationManager;
            _playerService = playerService;
            _searchService = searchService;
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
                        ? (bool?) null
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
                        var album = await _searchService.SearchForAlbum(parameter.Query);

                        if (album == null)
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
                        }
                        
                        Terminal.WriteYellow($"Queueing {album.Name}");
                    }
                }
            }
        }
        
        private static string GetUserInput()
        {
            return Console.ReadLine();
        }

        private static ParsedCommand ParseCommand(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentException("Input must contain command");
            }
            
            var split = input.Split(new []{"--"}, StringSplitOptions.None);
		
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