using MySql.Data.MySqlClient;
using RJLG.LauncherAuthServer.Controllers;
using RJLG.LauncherAuthServer.Models;
using SNS.Data.DataSerializer;
using SNS.Data.DataSerializer.DataExtensions;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace RJLG.LauncherAuthServer
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            ConnectionStringSettings conStr = ConfigurationManager.ConnectionStrings["AuthenticationServer"];
            Settings.RegisterConnector<MySqlConnection>(conStr.Name, conStr.ConnectionString, ConnectorSettings.GetMySqlSettings(), true, 10, 10, 2000);

            VersionController.Versions = Data<IntelliSEMVersion>.LoadAll().ToList();
            CustomersController.Customers = CustomerAccount.LoadAll();
            HomeController.UsageStatistics = UsageStatistic.LoadAll(VersionController.Versions.Count, CustomersController.Customers.Count, CustomersController.Customers.Sum(c => c.LoginKeys.Count));
        }
    }
}
