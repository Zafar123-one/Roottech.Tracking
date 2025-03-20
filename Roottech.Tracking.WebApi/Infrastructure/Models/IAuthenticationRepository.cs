using System;
using System.Linq;
using System.Threading.Tasks;
using Roottech.Tracking.Domain.SMIM.Entities;

namespace Roottech.Tracking.WebApi.Infrastructure.Models
{
    public interface IAuthenticationRepository : IDisposable
    {
        Task<bool> AddRefreshToken(RefreshToken token);
        Task<RefreshToken> FindRefreshToken(string refreshTokenId);
        IQueryable<RefreshToken> GetAllRefreshTokens();
        Task<bool> RemoveRefreshToken(RefreshToken refreshToken);
        Task<bool> RemoveRefreshTokenById(string refreshTokenId);
    }
}