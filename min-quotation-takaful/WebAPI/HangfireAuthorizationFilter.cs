using Hangfire.Annotations;
using Hangfire.Dashboard;

namespace WebAPI
{
    public class HangfireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        //public bool Authorize(DashboardContext context)
        //{
        //    // Allow all local requests or implement your logic
        //    var httpContext = context.GetHttpContext();
        //    return httpContext.User.Identity.IsAuthenticated;
        //}

        public bool Authorize(DashboardContext context) => true;
    }
}
