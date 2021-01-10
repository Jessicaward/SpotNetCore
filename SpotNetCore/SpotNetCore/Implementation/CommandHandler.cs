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
    public class CommandHandler
    {
        private readonly AuthenticationManager _authenticationManager;
        private readonly PlayerService _playerService;

        public CommandHandler(AuthenticationManager authenticationManager, PlayerService playerService)
        {
            _authenticationManager = authenticationManager;
            _playerService = playerService;
        }
        
        public async Task HandleCommands()
        {
            var exit = false;
            while (!exit)
            {
                var command = ParseCommand(GetUserInput());

                var spotifyCommand = command.Command.ToLower() switch
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
                    "clear" => SpotifyCommand.ClearQueue
                };

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
                    await AuthenticationManager.RequestRefreshedAccessToken();
                }

                if (spotifyCommand == SpotifyCommand.PlayCurrentTrack)
                {
                    _playerService.PlayCurrentTrack();
                }

                if (spotifyCommand == SpotifyCommand.PauseCurrentTrack)
                {
                    _playerService.PauseCurrentTrack();
                }

                if (spotifyCommand == SpotifyCommand.Current)
                {
                    Terminal.WriteCurrentSong(await _playerService.GetCurrentlyPlaying());
                }
                
                if (spotifyCommand == SpotifyCommand.NextTrack)
                {
                    await _playerService.NextTrack();
                    Terminal.WriteCurrentSong(await _playerService.GetCurrentlyPlaying());
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
            
            var split = input.Split(" ");
            return new ParsedCommand
            {
                Command = split[0],
                Parameters = split.Skip(1).Take(split.Length - 1)
            };
        }
    }
}