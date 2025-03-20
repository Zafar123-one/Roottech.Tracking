using System;
using Roottech.Tracking.WebApi.Controllers;

namespace Roottech.Tracking.WebApi.Infrastructure.Models
{
    public interface IUserTokenRepository : IDisposable
    {
        bool AddUserTokenIntoDB(UserTokenMaster userTokenMaster);
        Token GetTokenFromDB(string username, string password);
    }
}