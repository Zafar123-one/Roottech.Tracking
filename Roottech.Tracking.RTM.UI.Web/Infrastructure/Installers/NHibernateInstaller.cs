using System.Collections.Generic;
using System.Configuration;
using System.Web;
using System.Web.Configuration;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using NHibernate;
using Roottech.Tracking.Common.Session;
using Roottech.Tracking.RTM.UI.Web.Infrastructure.SessionManagement;

namespace Roottech.Tracking.RTM.UI.Web.Infrastructure.Installers
{
    public class NHibernateInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var connectionString = ConfigurationManager.AppSettings["Live"];

            //var toBeSaved = false;
            //var clientCount = 3; // 3 added for DEV01, DEV05, WEB-DB

            // Write the section name of web.config file (connectionStrings)
            //var config = WebConfigurationManager.OpenWebConfiguration(HttpContext.Current.Request.ApplicationPath);
            //var connectionStringsSection = config.GetSection("connectionStrings") as ConnectionStringsSection;
            //var appSettingsSection = config.GetSection("appSettings") as AppSettingsSection;

            //DecryptConfig(new ConfigurationSection[] { appSettingsSection });

            // Get SessionFactory of centraldb(clients list) on Application Start not HttpSesion
            container.Register(Component.For<ISessionFactory>().UsingFactoryMethod(k =>
                new NHibernateHelper(connectionString, "").SessionFactory).LifestyleSingleton());

            //EncryptConfig(new ConfigurationSection[] { appSettingsSection });
            //if (toBeSaved) if (ConfigurationManager.ConnectionStrings.Count != clientCount) config.Save(ConfigurationSaveMode.Full);
            container.Register(
                Component.For<NHibernateSessionModule>(),
                Component.For<ISessionFactoryProvider>().AsFactory(),
                Component.For<IEnumerable<ISessionFactory>>().UsingFactoryMethod(k => k.ResolveAll<ISessionFactory>()).LifestyleSingleton());
            HttpContext.Current.Application[SessionFactoryProvider.Key] = container.Resolve<ISessionFactoryProvider>();
        }

        /// <summary>
        /// Encrypts sections of config file in common apps
        /// </summary>
        public void EncryptConfig(ConfigurationSection[] configurationSections)
        {
            foreach (var section in configurationSections) ProtectSection(section, "DataProtectionConfigurationProvider", "E");
        }

        /// <summary>
        /// Decrypt sections of config file in common apps
        /// </summary>
        public void DecryptConfig(ConfigurationSection[] configurationSections)
        {
            foreach (var section in configurationSections) ProtectSection(section, "", "D");
        }

        /// <summary>
        /// Encrypt / Decrypt a web.config section
        /// </summary>
        /// <param name="section">Configuration Section</param>
        /// <param name="provider">Provider Name</param>
        /// <param name="mode">E=Encrypt D=Decrypt</param>
        private void ProtectSection(ConfigurationSection section, string provider, string mode)
        {
            switch (mode)
            {
                case "E":
                    if (section != null && !section.SectionInformation.IsProtected)
                    {
                        section.SectionInformation.ProtectSection(provider);
                        section.SectionInformation.ForceSave = true;
                        //config.Save(ConfigurationSaveMode.Full);
                    }
                    break;
                case "D":
                    if (section != null && section.SectionInformation.IsProtected)
                    {
                        section.SectionInformation.UnprotectSection();
                        section.SectionInformation.ForceSave = true;
                        //config.Save(ConfigurationSaveMode.Full);
                    }
                    break;
            }
        }
    }
}