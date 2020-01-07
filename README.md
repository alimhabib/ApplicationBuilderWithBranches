ApplicationBuilderWithBranches
===============


I built it as base on this nice article [Running multiple independent ASP.NET Core pipelines](https://www.strathweb.com/2017/04/running-multiple-independent-asp-net-core-pipelines-side-by-side-in-the-same-application/) but the problem was that all in one project , and  i wanted to make it more moduler, and more generic 

## Get Started

you can use the nuget or add refrence to the class library 

```

            app.UseBranchWithServices("/admin" ,
                a => {
                    a.Use(async (c, next) => {
                        if (c.Request.Path.ToString().Contains("foo")) {
                            await c.Response.WriteAsync("bar!");
                        } else {
                            await next();
                        }
                    });

                    a.UseMvc();
                } , typeof( AdminWebApplication.Startup));

```

`app.UseBranchWithServices `  takes three parameters:

*  the first is the path routes. 
* `Action<IApplicationBuilder>`  which you use to apply your middlewares.
`UseMVC`, .... 
* Type, which represent the Startup type you need to branch it 
