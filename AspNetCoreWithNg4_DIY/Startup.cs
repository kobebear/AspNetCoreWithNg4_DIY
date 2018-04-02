using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCoreWithNg4_DIY
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //添加第一个中间件
            app.Use(async (context, next) => {
                await next();
                //如果系统返回404状态码
                if (context.Response.StatusCode == 404 &&
                   //且请求路径没有文件扩展名
                   !System.IO.Path.HasExtension(context.Request.Path.Value) &&
                      //且路径不是以/api/开头的
                      !context.Request.Path.Value.StartsWith("/api/", StringComparison.InvariantCultureIgnoreCase))
                {
                    //都跳转到index.html页面
                    context.Request.Path = "/index.html";
                    await next();
                }
            });

            //添加第二个中间件: 使用MVC的默认路由中间件
            app.UseMvcWithDefaultRoute();
            //添加第三个中间件: 启用默认文档提供器中间件。
            //  设置默认首页为default.html、default.htm、index.html或index.htm文件
            app.UseDefaultFiles();
            //添加第四个中间件: 启用静态文件支持
            //  允许直接访问wwwroot文件夹中的静态资源
            app.UseStaticFiles();
        }
    }
}
