using System.Web.Http;
using Owin;

namespace CGRPG_TournamentLib 
{ 
    public class Startup
    {
        // This code configures Web API. The Startup class is specified as a type
        // parameter in the WebApp.Start method.
        public void Configuration(IAppBuilder app)
        {
            app.UseErrorPage();
            // Configure Web API for self-host. 
            HttpConfiguration config = new HttpConfiguration(); 
            config.Routes.MapHttpRoute( 
                name: "DefaultApi", 
                routeTemplate: "api/{controller}/{action}/{id}", 
                defaults: new { id = RouteParameter.Optional } 
            );
            //app.CreatePerOwinContext(MyDbContext.Create);
            //app.CreatePerOwinContext(ApplicationUserManager.Create);
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            app.UseWebApi(config);
            config.EnsureInitialized();
        }
    }
}