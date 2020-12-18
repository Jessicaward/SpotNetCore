using System;
using System.Linq;
using SpotNetCore.Models;

namespace SpotNetCore.Implementation
{
    /// <summary>
    /// Main application loop, parses and executes commands
    /// </summary>
    public class CommandParser
    {
        public async void HandleCommands()
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
                    "queue" => SpotifyCommand.Queue,
                    "current" => SpotifyCommand.Current
                };

                if (spotifyCommand == SpotifyCommand.Exit)
                {
                    exit = true;
                    break;
                }
                else if (spotifyCommand == SpotifyCommand.Help)
                {
                    
                }
            }
        }

        private string GetUserInput()
        {
            return Console.ReadLine();
        }

        private ParsedCommand ParseCommand(string input)
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