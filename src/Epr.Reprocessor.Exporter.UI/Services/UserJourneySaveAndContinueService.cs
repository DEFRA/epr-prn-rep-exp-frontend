﻿using Epr.Reprocessor.Exporter.UI.Services.Interfaces;

namespace Epr.Reprocessor.Exporter.UI.Services
{
    public class UserJourneySaveAndContinueService : IUserJourneySaveAndContinueService
    {
        public Task SaveAndContinueAsync(string action, string controller, string data)
        {
            // add call to facade endpoint to save user journey in the database
            throw new NotImplementedException();
        }
    }
}
