using System;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security.Infrastructure;
using Roottech.Tracking.Domain.SMIM.Entities;
using Roottech.Tracking.WebApi.Infrastructure.Models;

namespace Roottech.Tracking.WebApi.Infrastructure.Provider
{
    public class RefreshTokenProvider : IAuthenticationTokenProvider
    {
        public async Task CreateAsync(AuthenticationTokenCreateContext context)
        {
            //Get the client ID from the Ticket properties
            var clientid = context.Ticket.Properties.Dictionary["client_id"];

            if (string.IsNullOrEmpty(clientid))
                return;

            //Generating a Uniqure Refresh Token ID
            var refreshTokenId = Guid.NewGuid().ToString("n");

            using (IAuthenticationRepository _repo = context.OwinContext.Get<IAuthenticationRepository>())
            {
                // Getting the Refesh Token Life Time From the Owin Context
                var refreshTokenLifeTime = context.OwinContext.Get<string>("ta:clientRefreshTokenLifeTime");

                //Creating the Refresh Token object
                var token = new RefreshToken()
                {
                    //storing the RefreshTokenId in hash format
                    Id = Helper.GetHash(refreshTokenId),
                    ClientId = clientid,
                    UserName = context.Ticket.Identity.Name,
                    IssuedTime = DateTime.UtcNow,
                    ExpiredTime = DateTime.UtcNow.AddMinutes(Convert.ToDouble(refreshTokenLifeTime))
                };

                //Setting the Issued and Expired time of the Refresh Token
                context.Ticket.Properties.IssuedUtc = token.IssuedTime;
                context.Ticket.Properties.ExpiresUtc = token.ExpiredTime;

                token.ProtectedTicket = context.SerializeTicket();

                var result = await _repo.AddRefreshToken(token);

                if (result)
                    context.SetToken(refreshTokenId);
            }
        }

        public async Task ReceiveAsync(AuthenticationTokenReceiveContext context)
        {
            var allowedOrigin = context.OwinContext.Get<string>("ta:clientAllowedOrigin");
            if (context.OwinContext.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
                context.OwinContext.Response.Headers.Remove("Access-Control-Allow-Origin");
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] { allowedOrigin });
            string hashedTokenId = Helper.GetHash(context.Token);
            using (IAuthenticationRepository _repo = context.OwinContext.Get<IAuthenticationRepository>())
            {
                var refreshToken = await _repo.FindRefreshToken(hashedTokenId);
                if (refreshToken != null)
                {
                    //Get protectedTicket from refreshToken class
                    context.DeserializeTicket(refreshToken.ProtectedTicket);
                    var result = await _repo.RemoveRefreshTokenById(hashedTokenId);
                }
            }
        }

        public void Create(AuthenticationTokenCreateContext context)
        {
            throw new NotImplementedException();
        }

        public void Receive(AuthenticationTokenReceiveContext context)
        {
            throw new NotImplementedException();
        }
    }
}