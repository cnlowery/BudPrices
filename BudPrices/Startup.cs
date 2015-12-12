using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BudPrices.Startup))]
namespace BudPrices
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
