using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FLRC.ChallengeDashboard.Controllers
{
    public class AboutController : Controller
    {
        private readonly string _url;

        public AboutController(IConfiguration configuration) => _url = configuration.GetValue<string>("About");
    
        public RedirectResult Index() => Redirect(_url); 

        public RedirectResult Rules() => Redirect(_url + "#rules"); 
        
        public RedirectResult Timing() => Redirect(_url + "#timing"); 
    }
}