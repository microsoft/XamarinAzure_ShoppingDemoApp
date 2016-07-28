using Microsoft.Azure.Mobile.Server;
using Microsoft.Azure.Mobile.Server.Authentication;
using Microsoft.Azure.Mobile.Server.Config;
using Newtonsoft.Json;
using Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.IO;
using System.Web.Hosting;
using System.Web.Http;
using Xamarin.Azure.Backend.DataObjects;
using Xamarin.Azure.Backend.Models;

namespace Xamarin.Azure.Backend
{
    public partial class Startup
    {
        public static void ConfigureMobileApp(IAppBuilder app)
        {
            HttpConfiguration config = new HttpConfiguration();
            config.MapHttpAttributeRoutes();

            new MobileAppConfiguration()
                .UseDefaultConfiguration()
                .ApplyTo(config);

            // Use Entity Framework Code First to create database tables based on your DbContext
            Database.SetInitializer(new MobileServiceInitializer());

            MobileAppSettingsDictionary settings = config.GetMobileAppSettingsProvider().GetMobileAppSettings();

            if (string.IsNullOrEmpty(settings.HostName))
            {
                app.UseAppServiceAuthentication(new AppServiceAuthenticationOptions
                {
                    // This middleware is intended to be used locally for debugging. By default, HostName will
                    // only have a value when running in an App Service application.
                    SigningKey = ConfigurationManager.AppSettings["SigningKey"],
                    ValidAudiences = new[] { ConfigurationManager.AppSettings["ValidAudience"] },
                    ValidIssuers = new[] { ConfigurationManager.AppSettings["ValidIssuer"] },
                    TokenHandler = config.GetAppServiceTokenHandler()
                });
            }

            app.UseWebApi(config);
        }
    }

    public class MobileServiceInitializer : CreateDatabaseIfNotExists<MobileServiceContext>
    {
        protected override void Seed(MobileServiceContext context)
        {
            string dataPath = HostingEnvironment.MapPath(@"~/App_Data/items.json");
            string rawData = File.ReadAllText(dataPath);
            var saleItems = JsonConvert.DeserializeObject<IEnumerable<SaleItem>>(rawData);

            Uri currentRequest = System.Web.HttpContext.Current.Request.Url;

            foreach (var item in saleItems)
            {
                // Update default image urls using current domain name
                item.ImageUrl = $"https://{currentRequest.Authority}/Images/{item.Id}.png";
            }

            context.Set<SaleItem>().AddRange(saleItems);

            base.Seed(context);
        }
    }
}

