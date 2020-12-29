﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using PollBall.Services;

namespace PollBall
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<IPollResultsService, PollResultsService>();
            services.AddMvc();
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IPollResultsService pollResults)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Query.ContainsKey("favorite"))
                {
                    string selectedValue = context.Request.Query["favorite"];
                    SelectedGame selectedGame = (SelectedGame)Enum.Parse(typeof(SelectedGame), selectedValue, true);
                    pollResults.AddVote(selectedGame);

                    context.Response.Headers.Add("content-type", "text/html");
                    await context.Response.WriteAsync("Thank you for submitting the poll. You may look at the poll results <a href='/?submitted=true'>Here</a>.");
                }
                else
                {
                    
                    await next.Invoke();
                }
            });
            
            app.UseStaticFiles();

            app.UseMvcWithDefaultRoute();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("This text was generated by the app.Run middleware. wwwroot folder path: " + env.WebRootPath);
            });
        }
    }
}
