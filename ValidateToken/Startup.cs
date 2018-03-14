using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace ValidateToken
{
    public class Startup
    {
        public ILogger Logger { get; set; }
        public IHostingEnvironment HostingEnvironment { get; set; }
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            HostingEnvironment = environment;
            Configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            ConfigureAuthentication(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            Logger = loggerFactory?.CreateLogger(GetType().Name);

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseAuthentication();
            app.UseMvc();
        }

        private void ConfigureAuthentication(IServiceCollection services)
        {
            var tokenValidationParameters = new TokenValidationParameters
            {
                NameClaimType = ClaimTypes.Name,

                ValidateAudience = true,
                ValidIssuer = "https://sts.windows.net/demo",

                ValidateIssuer = true,
                ValidAudience = "demo-audience",

                //DON'T OVERRIDE THE VALIDATOR LIKE THIS IN REAL LIFE!
                //(any signature will do in this demo)
                SignatureValidator = (token, parameters) => new JwtSecurityToken(token),
                //also, don't do this
                ValidateLifetime = false,
            };

            //create auth builder
            var authenticationBuilder = services.AddAuthentication(options =>
            {
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            });

            //set options and callbacks
            authenticationBuilder.AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = AuthenticationFailed,
                    OnTokenValidated = AuthenticationSucceeded
                };

                options.TokenValidationParameters = tokenValidationParameters;
            });
        }

        private Task AuthenticationSucceeded(TokenValidatedContext arg)
        {
            Logger?.LogError($"User authenticated: {arg.Principal.Identity.Name}");
            return Task.CompletedTask;
        }

        private Task AuthenticationFailed(AuthenticationFailedContext arg)
        {
            Logger?.LogError($"AuthenticationFailed: {arg.Exception.Message}", arg.Exception);
            return Task.CompletedTask;
        }
    }
}
