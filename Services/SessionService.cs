using ManaHub.Contracts;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ManaHub.Services
{
    internal sealed class SessionService : ISessionService
    {
        private string _username = string.Empty;

        public string Username
        {
            get => _username;
            set
            {
                if (_username == value)
                    return;

                _username = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public void Clear() => Username = string.Empty;

        private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
