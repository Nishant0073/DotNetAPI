using DotnetAPI.Data;
using Microsoft.AspNetCore.Mvc;

namespace DotnetAPI.Controller
{
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly IConfiguration _config;
        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _config = config;
        }
    }
}