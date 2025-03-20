using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using NHibernate;
using NHibernate.Criterion;
using NHibernate.Transform;
using OfficeOpenXml.FormulaParsing.Excel.Functions.DateTime;
using Roottech.Tracking.Common.Repository.Interfaces;
using Roottech.Tracking.Domain.RSMT.Entities;
using Roottech.Tracking.Domain.SMSA.Entities;
using Roottech.Tracking.Library.Models;
using Roottech.Tracking.Library.Utils;

namespace Roottech.Tracking.RTM.UI.Web.Areas.Stationary.Controllers
{
    public class ContractMstApiController : ApiController
    {
        private readonly IKeyedRepository<double, ContractMst> _repositoryContractMst;
        private readonly IKeyedRepository<string, Company> _repositoryCompany;
        private readonly IKeyedRepository<double, Vendor> _repositoryVendor;
        private readonly IKeyedRepository<string, ContrType> _repositoryContrType;
        private readonly IKeyedRepository<double, ContrStatus> _repositoryContrStatus;
        // GET api/contractmstapi
        public ContractMstApiController(IKeyedRepository<double, ContractMst> repositoryContractMst,
            IKeyedRepository<string, Company> repositoryCompany,
            IKeyedRepository<double, Vendor> repositoryVendor,
            IKeyedRepository<string, ContrType> repositoryContrType,
            IKeyedRepository<double, ContrStatus> repositoryContrStatus)
        {
            _repositoryContractMst = repositoryContractMst;
            _repositoryCompany = repositoryCompany;
            _repositoryVendor = repositoryVendor;
            _repositoryContrType = repositoryContrType;
            _repositoryContrStatus = repositoryContrStatus;
        }

        public dynamic GetContractMsts()
        {
            ContractMst contractMst = null;
            var orgCode = SystemLibrary.GetCookies("UserOrgCode");
            var contractMsts = _repositoryContractMst.QueryOver()
                .Where(x => x.OrgCode == orgCode)
                .SelectList(list => list
                    .Select(x => x.Id).WithAlias(() => contractMst.Id)
                    .Select(x => x.ContractDate).WithAlias(() => contractMst.ContractDate)
                    .Select(x => x.MasterContractNo).WithAlias(() => contractMst.MasterContractNo)
                    .Select(x => x.Title).WithAlias(() => contractMst.Title)
                    .Select(x => x.ContrStatusNo).WithAlias(() => contractMst.ContrStatusNo)
                    .Select(x => x.ContDscr).WithAlias(() => contractMst.ContDscr)
                    .Select(x => x.DateFrom).WithAlias(() => contractMst.DateFrom)
                    .Select(x => x.DateTo).WithAlias(() => contractMst.DateTo)
                    .Select(x => x.VendorCode).WithAlias(() => contractMst.VendorCode)
                    .Select(x => x.ToVendorCode).WithAlias(() => contractMst.ToVendorCode)
                    .Select(x => x.ContrType).WithAlias(() => contractMst.ContrType)
                    .Select(x => x.CompanyCode).WithAlias(() => contractMst.CompanyCode)
                    .Select(x => x.ExpiryAlertB4).WithAlias(() => contractMst.ExpiryAlertB4)
                    .Select(x => x.ExpiryAlertDt).WithAlias(() => contractMst.ExpiryAlertDt)
                ).TransformUsing(Transformers.AliasToBean<ContractMst>()).List();
            return new
            {
                DateTime.Today, 
                contractMsts
            };
        }
        // POST api/sitedirectoryapi
        public HttpResponseMessage Post(ContractMst contractMst)
        {
            var orgCode = SystemLibrary.GetCookies("UserOrgCode");
            contractMst.OrgCode = orgCode;
            contractMst.UserCode = SystemLibrary.GetCookies("User_Code");
            contractMst.DmlType = "I";
            contractMst.DmlDate = DateTime.Now;
            _repositoryContractMst.Add(contractMst);

            return Request.CreateResponse(HttpStatusCode.Created, contractMst); //return siteDirectory;
        }

        // PUT api/sitedirectoryapi/5
        public HttpResponseMessage Put(ContractMst contractMst)
        {
            var foundContractMst = _repositoryContractMst.FindBy(contractMst.Id);
            if (foundContractMst == null)
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound));
            foundContractMst.ContractDate = contractMst.ContractDate;
            foundContractMst.MasterContractNo = contractMst.MasterContractNo;
            foundContractMst.Title = contractMst.Title;
            foundContractMst.ContrStatusNo = contractMst.ContrStatusNo;
            foundContractMst.ContDscr = contractMst.ContDscr;
            foundContractMst.DateFrom = contractMst.DateFrom;
            foundContractMst.DateTo = contractMst.DateTo;
            foundContractMst.VendorCode = contractMst.VendorCode;
            foundContractMst.ToVendorCode = contractMst.ToVendorCode;
            foundContractMst.ContrType = contractMst.ContrType;
            foundContractMst.ExpiryAlertB4 = contractMst.ExpiryAlertB4;
            foundContractMst.ExpiryAlertDt = contractMst.ExpiryAlertDt;
            foundContractMst.CompanyCode = contractMst.CompanyCode;
            foundContractMst.UserCode = SystemLibrary.GetCookies("User_Code");
            foundContractMst.DmlType = "U";
            foundContractMst.DmlDate = DateTime.Now;
            _repositoryContractMst.Merge(foundContractMst);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        // DELETE api/sitedirectoryapi/5
        public HttpResponseMessage Delete(int id)
        {
            //_repositoryContractMst.Delete(id);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        public IList<KeyValue<string, string>> GetCompanies()
        {
            var orgCode = SystemLibrary.GetCookies("UserOrgCode");
            KeyValue<string, string> keyValue = null;
            return _repositoryCompany.QueryOver()
                .Where(x => x.OrgCode == orgCode)
                .OrderBy(x => x.CompanyName).Asc
                .SelectList(list => list
                    .Select(x => x.Id).WithAlias(() => keyValue.Id)
                    .Select(x => x.CompanyName).WithAlias(() => keyValue.Name)
                    ).TransformUsing(Transformers.AliasToBean<KeyValue<string, string>>())
                .List<KeyValue<string, string>>();
        }
        public IList<KeyValue<string, string>> GetVendors()
        {
            var orgCode = SystemLibrary.GetCookies("UserOrgCode");
            KeyValue<string, string> keyValue = null;
            return _repositoryVendor.QueryOver()
                .Where(x => x.OrgCode == orgCode)
                .OrderBy(x => x.VendorName).Asc
                .SelectList(list => list
                    .Select(Projections.Cast(NHibernateUtil.String, Projections.Property<Vendor>(c => c.Id))).WithAlias(() => keyValue.Id)
                    .Select(x => x.VendorName).WithAlias(() => keyValue.Name)
                    ).TransformUsing(Transformers.AliasToBean<KeyValue<string, string>>())
                .List<KeyValue<string, string>>();
        }
        public IList<KeyValue<string, string>> GetContrTypes()
        {
            var orgCode = SystemLibrary.GetCookies("UserOrgCode");
            KeyValue<string, string> keyValue = null;
            return _repositoryContrType.QueryOver()
                .Where(x => x.OrgCode == orgCode)
                .OrderBy(x => x.Name).Asc
                .SelectList(list => list
                    .Select(x => x.Id).WithAlias(() => keyValue.Id)
                    .Select(x => x.Name).WithAlias(() => keyValue.Name)
                    ).TransformUsing(Transformers.AliasToBean<KeyValue<string, string>>())
                .List<KeyValue<string, string>>();
        }
        public IList<KeyValue<string, string>> GetContrStatuses()
        {
            var orgCode = SystemLibrary.GetCookies("UserOrgCode");
            KeyValue<string, string> keyValue = null;
            return _repositoryContrStatus.QueryOver()
                .Where(x => x.OrgCode == orgCode)
                .OrderBy(x => x.Name).Asc
                .SelectList(list => list
                    .Select(Projections.Cast(NHibernateUtil.String, Projections.Property<ContrStatus>(c => c.Id))).WithAlias(() => keyValue.Id)
                    .Select(x => x.Name).WithAlias(() => keyValue.Name)
                    ).TransformUsing(Transformers.AliasToBean<KeyValue<string, string>>())
                .List<KeyValue<string, string>>();
        }
    }
}
