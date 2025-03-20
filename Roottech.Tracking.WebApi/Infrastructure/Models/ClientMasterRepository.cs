using System;
using System.Linq;
using Roottech.Tracking.Common.Repository.Interfaces;
using Roottech.Tracking.Domain.SMIM.Entities;

namespace Roottech.Tracking.WebApi.Infrastructure.Models
{
    public class ClientMasterRepository : IClientMasterRepository
    {
        private readonly IKeyedRepository<int, ClientMaster> _repositoryClientMaster;
        public ClientMasterRepository(IKeyedRepository<int, ClientMaster> repositoryClientMaster)
        {
            _repositoryClientMaster = repositoryClientMaster;
        }
        //This method is used to check and validate the Client credentials
        public ClientMaster ValidateClient(string ClientID, string ClientSecret)
        {
            return _repositoryClientMaster.FilterBy(x => x.ClientId == ClientID && x.ClientSecret == ClientSecret)
                .SingleOrDefault();
        }
        public void Dispose()
        {
            _repositoryClientMaster.Dispose();
            //context.Dispose();
        }
    }

}