using Hellang.Middleware.ProblemDetails;
using HirdetoRendszer.Api.Extensions;
using HirdetoRendszer.Bll.Extensions;
using HirdetoRendszer.Bll.Mapping;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace HirdetoRendszer
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddMvc().AddJsonOptions(options => options.JsonSerializerOptions.IgnoreNullValues = true);

            services.AddHttpContextAccessor();

            services.AddSwagger();

            services.AddBllServices(_configuration);

            services.ConfigureValidators(_configuration);

            services.AddAutoMapper(typeof(MappingProfile));

            services.AddProblemDetailsForExceptions(_configuration);

            services.AddCorsPolicies(_configuration);

            services.ConfigureDatabase(_configuration);

            services.ConfigureIdentity(_configuration);

            services.ConfigureAuthentication(_configuration);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Hirdeto rendszer v1"));
            }

            app.UseProblemDetails();

            app.UseHttpsRedirection();

            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
