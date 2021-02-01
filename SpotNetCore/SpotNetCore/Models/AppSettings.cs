using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using SpotNetCore.Models;

namespace SpotNetCore.Implementation
{
    public class AppSettings : INotifyPropertyChanged
    {
        private SpotifyAccessToken _spotifyTokens;
        private List<string> _requiredScopes;
        private string _clientId;
        private string _market;

        [JsonPropertyName("requiredScopes")]
        public List<String> RequiredScopes
        {
            get => _requiredScopes;
            set
            {
                if (_requiredScopes == value) return;
                
                _requiredScopes = value;
                OnPropertyChanged();
            }
        }

        [JsonPropertyName("clientId")]
        public String ClientId
        {
            get => _clientId;
            set
            {
                if (_clientId == value) return;
                
                _clientId = value;
                OnPropertyChanged();
            }
        }

        [JsonPropertyName("market")]
        public String Market
        {
            get => _market;
            set
            {
                if (_market == value) return;
                
                _market = value;
                OnPropertyChanged();
            }
        }

        [JsonPropertyName("spotifyTokens")]
        [DefaultValue(typeof(SpotifyAccessToken))]
        public SpotifyAccessToken SpotifyTokens
        {
            get => _spotifyTokens;
            set
            {
                if (_spotifyTokens == value) return;
                
                if (value != null)
                    value.PropertyChanged += (sender, args) => OnPropertyChanged();
                
                _spotifyTokens = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}