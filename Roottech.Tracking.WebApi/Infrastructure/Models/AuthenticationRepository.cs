using System;
using System.Linq;
using System.Threading.Tasks;
using Roottech.Tracking.Common.Repository.Interfaces;
using Roottech.Tracking.Domain.SMIM.Entities;

namespace Roottech.Tracking.WebApi.Infrastructure.Models
{
    public class AuthenticationRepository : IAuthenticationRepository
    {
        private readonly IKeyedRepository<string, RefreshToken> _repositoryRefreshToken;
        //SECURITY_DBEntities context = new SECURITY_DBEntities();

        public AuthenticationRepository(IKeyedRepository<string, RefreshToken> repositoryRefreshToken)
        {
            _repositoryRefreshToken = repositoryRefreshToken;
        }
        //Add the Refresh token
        public async Task<bool> AddRefreshToken(RefreshToken token)
        {
            var existingToken = _repositoryRefreshToken.FilterBy(r => r.UserName == token.UserName
                                                                      && r.ClientId == token.ClientId)
                .SingleOrDefault();
            //context.RefreshTokens.FirstOrDefault(r => r.UserName == token.UserName && r.ClientID == token.ClientID);

            if (existingToken != null)
            {
                var result = await RemoveRefreshToken(existingToken);
            }
            _repositoryRefreshToken.Add(token);//context.RefreshTokens.Add(token);
            return true; //context.SaveChangesAsync() > 0;
        }

        //Remove the Refesh Token by id
        public async Task<bool> RemoveRefreshTokenById(string refreshTokenId)
        {
            var refreshToken = _repositoryRefreshToken.FindBy(refreshTokenId);// await context.RefreshTokens.FindAsync(refreshTokenId);

            if (refreshToken != null)
            {
                _repositoryRefreshToken.Delete(refreshToken);//context.RefreshTokens.Remove(refreshToken);
                return true; //await context.SaveChangesAsync() > 0;
            }

            return false;
        }

        //Remove the Refresh Token
        public async Task<bool> RemoveRefreshToken(RefreshToken refreshToken)
        {
            _repositoryRefreshToken.Delete(refreshToken);//context.RefreshTokens.Remove(refreshToken);
            return true; //await context.SaveChangesAsync() > 0;
        }

        //Find the Refresh Token by token ID
        public async Task<RefreshToken> FindRefreshToken(string refreshTokenId)
        {
            return _repositoryRefreshToken.FindBy(refreshTokenId);// await context.RefreshTokens.FindAsync(refreshTokenId);
        }

        //Get All Refresh Tokens
        public IQueryable<RefreshToken> GetAllRefreshTokens()
        {
            return _repositoryRefreshToken.All();// context.RefreshTokens.ToList();
        }

        public void Dispose()
        {
            _repositoryRefreshToken.Dispose(); // context.Dispose();
        }
    }
}