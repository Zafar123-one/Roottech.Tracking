using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using NHibernate;
using Roottech.Tracking.Common.Repository.Interfaces;
using Roottech.Tracking.Domain.SMIM.Entities;
using Roottech.Tracking.WebApi.Infrastructure.Models;

namespace Roottech.Tracking.WebApi.Infrastructure.Provider
{
    public class AuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {
            string clientId = string.Empty;
            string clientSecret = string.Empty;
            // The TryGetBasicCredentials method checks the Authorization header and
            // Return the ClientId and clientSecret
            if (!context.TryGetBasicCredentials(out clientId, out clientSecret))
            {
                context.SetError("invalid_client", "Client credentials could not be retrieved through the Authorization header.");
                return Task.FromResult<object>(null);
            }
            //Check the existence of by calling the ValidateClient method
            ClientMaster client = context.OwinContext.Get<IClientMasterRepository>().ValidateClient(clientId, clientSecret);
            if (client == null)
            {
                // Client could not be validated.
                context.SetError("invalid_client", "Client credentials could not be retrieved from the Authorization header");
                return Task.FromResult<object>(null);
            }
            else
            {
                if (!client.Active)
                {
                    context.SetError("invalid_client", "Client is inactive.");
                    return Task.FromResult<object>(null);
                }
                // Client has been verified.
                context.OwinContext.Set<ClientMaster>("ta:client", client);
                context.OwinContext.Set<string>("ta:clientAllowedOrigin", client.AllowedOrigin);
                context.OwinContext.Set<string>("ta:clientRefreshTokenLifeTime", client.RefreshTokenLifeTime.ToString());
                context.Validated();
                return Task.FromResult<object>(null);
            }
            //await Task.Run(() => context.Validated());
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            ClientMaster client = context.OwinContext.Get<ClientMaster>("ta:client");
            var allowedOrigin = context.OwinContext.Get<string>("ta:clientAllowedOrigin");
            if (allowedOrigin == null)
                allowedOrigin = "*";

            if (context.OwinContext.Response.Headers.ContainsKey("Access-Control-Allow-Origin"))
                context.OwinContext.Response.Headers.Remove("Access-Control-Allow-Origin");
            context.OwinContext.Response.Headers.Add("Access-Control-Allow-Origin", new[] {allowedOrigin});

            var loginInfo = Login(context.UserName, context.Password, context.OwinContext.Get<ISessionFactory>().GetCurrentSession());
            if (!string.IsNullOrEmpty(loginInfo.Msg))
            {
                context.SetError("Wrong Crendtials", "Provided username and password is incorrect " + loginInfo.Msg);
                return;
            }

            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Role, loginInfo.AppType.ToString()));
            identity.AddClaim(new Claim(ClaimTypes.Name, loginInfo.Id.ToString()));
            //identity.AddClaim(new Claim("Email", loginInfo.User_Name));// user.UserEmailID));
            identity.AddClaim(new Claim("loginOrgCode", loginInfo.GroupCount > 0 ? "%" : loginInfo.Id.ToString()));
            identity.AddClaim(new Claim("User_Code", loginInfo.Id.ToString()));
            identity.AddClaim(new Claim("UserOrgCode", loginInfo.OrgCode));// user.UserEmailID));
            identity.AddClaim(new Claim("AppType", loginInfo.AppType.ToString()));
            identity.AddClaim(new Claim("OrgName", loginInfo.OrgName));
            identity.AddClaim(new Claim("User_Name", loginInfo.User_Name));
            identity.AddClaim(new Claim("LoggedOn", DateTime.Now.ToString()));
            //await Task.Run(() => context.Validated(identity));

            var props = new AuthenticationProperties(new Dictionary<string, string>
            {
                {
                    "client_id", (context.ClientId == null) ? string.Empty : context.ClientId
                },
                {
                    "userName", context.UserName
                }
            });
            var ticket = new AuthenticationTicket(identity, props);
            context.Validated(ticket);
            return;
        }
        public override Task TokenEndpoint(OAuthTokenEndpointContext context)
        {
            foreach (KeyValuePair<string, string> property in context.Properties.Dictionary)
            {
                context.AdditionalResponseParameters.Add(property.Key, property.Value);
            }
            return Task.FromResult<object>(null);
        }
        public override Task GrantRefreshToken(OAuthGrantRefreshTokenContext context)
        {
            var originalClient = context.Ticket.Properties.Dictionary["client_id"];
            var currentClient = context.ClientId;
            if (originalClient != currentClient)
            {
                context.SetError("invalid_clientId", "Refresh token is issued to a different clientId.");
                return Task.FromResult<object>(null);
            }
            // Change auth ticket for refresh token requests
            var newIdentity = new ClaimsIdentity(context.Ticket.Identity);
            newIdentity.AddClaim(new Claim("newClaim", "newValue"));
            var newTicket = new AuthenticationTicket(newIdentity, context.Ticket.Properties);
            context.Validated(newTicket);
            return Task.FromResult<object>(null);
        }
        public Login Login(string userId, string password, ISession session)
        {
            return session.GetNamedQuery("GetLogin")
                .SetString("Id", userId)
                .SetString("Password", CommonFunctions.HashPass(password))
                .SetString("UserIP", CommonFunctions.LocalIPAddress())
                .UniqueResult<Login>();
            //var loginInfo = session.GetNamedQuery("GetLogin").SetString("Id", "103").SetString("Password", "F219EDF143A2E5B416B1D28F5758B80B160FA113").SetString("UserIP", "1").UniqueResult<Login>();
        }
    }
}