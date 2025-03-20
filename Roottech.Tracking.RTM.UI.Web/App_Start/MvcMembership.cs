using System.Web.Mvc;
using MvcMembership;

//[assembly: WebActivator.PreApplicationStartMethod(typeof(MvcMembership), "Start")]

namespace Roottech.Tracking.RTM.UI.Web.App_Start
{
    public static class MvcMembership
    {
        public static void Start()
        {
            GlobalFilters.Filters.Add(new TouchUserOnEachVisitFilter());
        }
    }
}