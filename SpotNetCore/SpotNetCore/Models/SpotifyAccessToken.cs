using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;

namespace SpotNetCore.Models
{
    public class SpotifyAccessToken : INotifyPropertyChanged
    {
        private string _accessToken;
        private string _tokenType;
        private int _expiresInSeconds;
        private string _refreshToken;
        private DateTime _expiresAt;
        private string _scope;

        [JsonPropertyName("access_token")]
        public string AccessToken
        {
            get => _accessToken;
            set
            {
                if (_accessToken == value) return;
                
                _accessToken = value;
                OnPropertyChanged();
            }
        }

        [JsonPropertyName("token_type")]
        public string TokenType
        {
            get => _tokenType;
            set
            {
                if (_tokenType == value) return;
                
                _tokenType = value;
                OnPropertyChanged();
            }
        }

        [JsonPropertyName("scope")]
        public string Scope
        {
            get => _scope;
            set
            {
                if (_scope == value) return;
                
                _scope = value;
                OnPropertyChanged();
            }
        }

        [JsonPropertyName("expires_in")]
        public int ExpiresInSeconds
        {
            get => _expiresInSeconds;
            set
            {
                if (_expiresInSeconds == value) return;
                
                _expiresInSeconds = value;
                OnPropertyChanged();
            }
        }

        [JsonPropertyName("refresh_token")]
        public string RefreshToken
        {
            get => _refreshToken;
            set
            {
                if (_refreshToken == value) return;
                
                _refreshToken = value;
                OnPropertyChanged();
            }
        }

        [JsonPropertyName("expires_at")]
        public DateTime ExpiresAt
        {
            get => _expiresAt;
            set
            {
                if (_expiresAt == value) return;
                
                _expiresAt = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}