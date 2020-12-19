using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FLRC.ChallengeDashboard
{
    public class RulesController : Controller
    {
        private readonly string _url;

        public RulesController(IConfiguration configuration) => _url = configuration.GetValue<string>("Rules");
    
        public RedirectResult Index() => Redirect(_url); 
    }
}