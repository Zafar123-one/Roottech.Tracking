using System;
using Roottech.Tracking.Domain.SMIM.Entities;

namespace Roottech.Tracking.WebApi.Infrastructure.Models
{
    public interface IClientMasterRepository : IDisposable
    {
        ClientMaster ValidateClient(string ClientID, string ClientSecret);
    }
}