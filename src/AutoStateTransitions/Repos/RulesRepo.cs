using AutoStateTransitions.Misc;
using AutoStateTransitions.Models;
using AutoStateTransitions.Repos.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace AutoStateTransitions.Repos
{
    public class RulesRepo : IRulesRepo, IDisposable
    {
        private IOptions<AppSettings> _appSettings;
        private IHelper _helper;
        private HttpClient _httpClient;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RulesRepo(IOptions<AppSettings> appSettings, IHelper helper, HttpClient httpClient, IHttpContextAccessor httpContextAccessor)
        {
            _appSettings = appSettings;
            _helper = helper;
            _httpClient = httpClient;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<RulesModel> ListRules(string wit)
        {
            string src = _appSettings.Value.SourceForRules;

            // set baseUrl to current context if SourceForRule is a relative path, i.e, not starting with http
            if (!src.ToLower().StartsWith("http"))
            {
                src = $"{_httpContextAccessor.HttpContext.Request.Scheme}://{_httpContextAccessor.HttpContext.Request.Host}/{src}";
            }
            
            System.Console.WriteLine( DateTime.Now + " src: " + src);
            var json = await _httpClient.GetStringAsync($"{src}/rules.{wit.ToLower()}.json");
            System.Console.WriteLine( DateTime.Now + " wit: " + wit);
  
            System.Console.WriteLine( DateTime.Now + " json: " + json);
            RulesModel rules = JsonConvert.DeserializeObject<RulesModel>(json);

            return rules;
            
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~RulesRepo()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _appSettings = null;
                _helper = null;
            }
        }
    }

}
