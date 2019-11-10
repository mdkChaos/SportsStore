using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Globalization;
using System.Threading.Tasks;

namespace SportsStore.Middleware
{
    public class CultureMiddleware
    {
        private readonly RequestDelegate _next;

        public CultureMiddleware(RequestDelegate next)
        {
            this._next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            string lang = "en-US";
            CultureInfo.CurrentCulture = new CultureInfo(lang);
            CultureInfo.CurrentUICulture = new CultureInfo(lang);
            await _next.Invoke(context);
        }
    }

    public static class CultureExtensions
    {
        public static IApplicationBuilder UseCulture(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CultureMiddleware>();
        }
    }
}
