using Microsoft.Extensions.Logging;

namespace SpotNetCore.Implementation
{
    public class AuthorisationManager
    {
        private readonly ILogger<AuthorisationManager> _logger;

        public AuthorisationManager(ILogger<AuthorisationManager> logger)
        {
            _logger = logger;
        }
        
        
    }
}