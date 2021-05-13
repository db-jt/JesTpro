// This file is part of JesTpro project.
//
// JesTpro is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (if needed) any later version.
//
// JesTpro has NO WARRANTY!! It is distributed for test, study or 
// personal environments. Any commercial distribution
// has no warranty! 
// See the GNU General Public License in root project folder  
// for more details or  see <http://www.gnu.org/licenses/>

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using RazorLight;

namespace jt.jestpro.Services
{
    public interface ITemplateHelperService
    {
        Task<string> GetTemplateHtmlAsStringAsync<T>(
                              string viewName, T model);
    }
    public class TemplateHelperService : ITemplateHelperService
    {
        private IRazorViewEngine _razorViewEngine;
        private IServiceProvider _serviceProvider;
        private ITempDataProvider _tempDataProvider;

        public TemplateHelperService(
            IRazorViewEngine engine,
            IServiceProvider serviceProvider,
            ITempDataProvider tempDataProvider)
        {
            this._razorViewEngine = engine;
            this._serviceProvider = serviceProvider;
            this._tempDataProvider = tempDataProvider;
        }

        public async Task<string> GetTemplateHtmlAsStringAsync<T>(string viewName, T model)
        {
            var engine = GetRazorPage("Views/Templates");
            string result = await engine.CompileRenderAsync(viewName, model);
            return result;
        }

        //private ActionContext GetActionContext()
        //{
        //    var httpContext = new DefaultHttpContext();
        //    httpContext.RequestServices = _serviceProvider;
        //    return new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        //}



        //public async Task<string> GetTemplateHtmlAsStringAsync<T>(string viewName, T model)
        //{
        //    var httpContext = new DefaultHttpContext() { RequestServices = _serviceProvider };
        //    //var actionContext = new ActionContext(httpContext, new RouteData(), new ActionDescriptor());
        //    var actionContext = GetActionContext();
        //    //var dir = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
        //    //var getViewResult = _viewEngine.GetView(executingFilePath: dir, viewPath: viewName, isMainPage: true)
        //    using (StringWriter sw = new StringWriter())
        //    {
        //        //var viewResult = _razorViewEngine.FindView(actionContext, viewName, false);
        //        var viewResult = FindView(actionContext, viewName);

        //        if (viewResult == null)
        //        {
        //            return string.Empty;
        //        }

        //        var viewDataDictionary = new ViewDataDictionary(
        //            new EmptyModelMetadataProvider(),
        //            new ModelStateDictionary()
        //        )
        //        {
        //            Model = model
        //        };

        //        var viewContext = new ViewContext(
        //            actionContext,
        //            viewResult,
        //            viewDataDictionary,
        //            new TempDataDictionary(actionContext.HttpContext, _tempDataProvider),
        //            sw,
        //            new HtmlHelperOptions()
        //        );

        //        await viewResult.RenderAsync(viewContext);

        //        return sw.ToString();
        //    }
        //}

        //private IView FindView(ActionContext actionContext, string viewName)
        //{
        //    var getViewResult = _razorViewEngine.GetView(executingFilePath: null, viewPath: viewName, isMainPage: true);
        //    if (getViewResult.Success)
        //    {
        //        return getViewResult.View;
        //    }
        //    getViewResult = _razorViewEngine.GetView(viewName,viewName,true);
        //    if (getViewResult.Success)
        //    {
        //        return getViewResult.View;
        //    }

        //    var findViewResult = _razorViewEngine.FindView(actionContext, viewName, isMainPage: true);
        //    if (findViewResult.Success)
        //    {
        //        return findViewResult.View;
        //    }

        //    var searchedLocations = getViewResult.SearchedLocations.Concat(findViewResult.SearchedLocations);
        //    var errorMessage = string.Join(
        //        Environment.NewLine,
        //        new[] { $"Unable to find view '{viewName}'. The following locations were searched:" }.Concat(searchedLocations)); ;

        //    throw new InvalidOperationException(errorMessage);
        //}

        private RazorLightEngine GetRazorPage(string view)
        {
            string currentDirectory = Directory.GetCurrentDirectory();
            var fullPath = Path.Combine(currentDirectory, view);
            return new RazorLightEngineBuilder()
                .UseFileSystemProject(fullPath)
                .UseMemoryCachingProvider()
                .Build();
        }

    }
  
}
