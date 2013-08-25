//Contributor: MvcContrib.TestHelper

using System;
using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Nop.Core.Fakes;
using Nop.Tests;

namespace Nop.Web.MVC.Tests.Public.Infrastructure
{
    /// <summary>
    /// Used to simplify testing routes and restful testing routes 
    /// <example>
    /// This tests that incoming PUT on resource is redirected to Update
    ///			 "~/banner/1"
    ///			   .WithMethod(HttpVerbs.Put)
    ///			   .ShouldMapTo&lt;BannerController>(action => action.Update(1));
    ///
    /// This tests that incoming POST was a faux PUT using the _method=PUT form parameter
    ///			 "~/banner/1"
    ///			   .WithMethod(HttpVerbs.Post, HttpVerbs.Put)
    ///			   .ShouldMapTo&lt;BannerController>(action => action.Update(1));
    /// </example>
    /// </summary>
    public static class RouteTestingExtensions
    {
        #region Utilities

        private static HttpContextBase FakeHttpContext(string url, HttpVerbs? httpMethod, HttpVerbs? formMethod)
        {
            NameValueCollection form = null;

            if (formMethod.HasValue)
                form = new NameValueCollection { { "_method", formMethod.Value.ToString().ToUpper() } };

            if (!httpMethod.HasValue)
                httpMethod = HttpVerbs.Get;

            var httpMethodStr = httpMethod.Value.ToString().ToUpper();
            var context = new FakeHttpContext(url, httpMethodStr, null, form, null, null, null, null);
            return context;
        }

        private static HttpContextBase FakeHttpContext(string url, string method)
        {
            var httpMethod = (HttpVerbs)Enum.Parse(typeof(HttpVerbs), method);
            return FakeHttpContext(url, httpMethod, null);
        }

        private static HttpContextBase FakeHttpContext(string url, HttpVerbs? httpMethod)
        {
            return FakeHttpContext(url, httpMethod, null);
        }
        
        private static HttpContextBase FakeHttpContext(string url)
        {
            return FakeHttpContext(url, null, null);
        }

        #endregion 
        
        #region Methods

        /// <summary>
        /// Returns the result of <paramref name="func"/> if <paramref name="obj"/> is not null.
        /// <example>
        /// <code>
        /// Request.Url.ReadValue(x => x.Query)
        /// </code>
        /// </example>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult">The type of the result.</typeparam>
        /// <param name="obj">The obj.</param>
        /// <param name="func">The func.</param>
        /// <returns></returns>
        private static TResult ReadValue<T, TResult>(this T obj, Func<T, TResult> func) where T : class
        {
            return ReadValue(obj, func, default(TResult));
        }

        /// <summary>
        /// Returns the result of <paramref name="func"/> if <paramref name="obj"/> is not null.
        /// Otherwise, <paramref name="defaultValue"/> is returned.
        /// <example>
        /// <code>
        /// Request.Url.ReadValue(x => x.Query, "default")
        /// </code>
        /// </example>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="obj"></param>
        /// <param name="func"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        private static TResult ReadValue<T, TResult>(this T obj, Func<T, TResult> func, TResult defaultValue) where T : class
        {
            return obj != null ? func(obj) : defaultValue;
        }

        /// <summary>
        /// Will return the name of the action specified in the ActionNameAttribute for a method if it has an ActionNameAttribute.
        /// Will return the name of the method otherwise.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private static string ActionName(this MethodInfo method)
        {
            if (method.IsDecoratedWith<ActionNameAttribute>()) return method.GetAttribute<ActionNameAttribute>().Name;

            return method.Name;
        }
        
        /// <summary>
        /// A way to start the fluent interface and and which method to use
        /// since you have a method constraint in the route.
        /// </summary>
        /// <param name="url"></param>
        /// <param name="httpMethod"></param>
        /// <returns></returns>
        public static RouteData WithMethod(this string url, string httpMethod)
        {
            return Route(url, httpMethod);
        }

        public static RouteData WithMethod(this string url, HttpVerbs verb)
        {
            return WithMethod(url, verb.ToString("g"));
        }

        /// <summary>
        /// Find the route for a URL and an Http Method
        /// because you have a method contraint on the route 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="httpMethod"></param>
        /// <returns></returns>
        public static RouteData Route(string url, string httpMethod)
        {
            var context = FakeHttpContext(url, httpMethod);
            return RouteTable.Routes.GetRouteData(context);
        }

        /// <summary>
        /// Returns the corresponding route for the URL.  Returns null if no route was found.
        /// </summary>
        /// <param name="url">The app relative url to test.</param>
        /// <returns>A matching <see cref="RouteData" />, or null.</returns>
        public static RouteData Route(this string url)
        {
            var context = FakeHttpContext(url);
            return RouteTable.Routes.GetRouteData(context);
        }

        /// <summary>
        /// Returns the corresponding route for the URL.  Returns null if no route was found.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="formMethod">The form method.</param>
        /// <returns></returns>
        public static RouteData Route(this string url, HttpVerbs httpMethod, HttpVerbs formMethod)
        {
            var context = FakeHttpContext(url, httpMethod, formMethod);
            var route = RouteTable.Routes.GetRouteData(context);

            // cater for SimplyRestful methods and others
            // adding values during the GetHttpHandler method
            route.ReadValue(x => x.RouteHandler).ReadValue(x => x.GetHttpHandler(new RequestContext(context, route)));
            return route;
        }

        /// <summary>
        /// Returns the corresponding route for the URL.  Returns null if no route was found.
        /// </summary>
        /// <param name="url">The URL.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <returns></returns>
        public static RouteData Route(this string url, HttpVerbs httpMethod)
        {
            var context = FakeHttpContext(url, httpMethod);
            var route = RouteTable.Routes.GetRouteData(context);

            // cater for SimplyRestful methods and others
            // adding values during the GetHttpHandler method
            route.ReadValue(x => x.RouteHandler).ReadValue(x => x.GetHttpHandler(new RequestContext(context, route)));
            return route;
        }

        /// <summary>
        /// Asserts that the route matches the expression specified based on the incoming HttpMethod and FormMethod for Simply Restful routing.  Checks controller, action, and any method arguments
        /// into the action as route values.
        /// </summary>
        /// <param name="relativeUrl">The relative URL.</param>
        /// <param name="httpMethod">The HTTP method.</param>
        /// <param name="formMethod">The form method.</param>
        /// <returns></returns>
        public static RouteData WithMethod(this string relativeUrl, HttpVerbs httpMethod, HttpVerbs formMethod)
        {
            return relativeUrl.Route(httpMethod, formMethod);
        }

        /// <summary>
        /// Asserts that the route matches the expression specified.  Checks controller, action, and any method arguments
        /// into the action as route values.
        /// </summary>
        /// <typeparam name="TController">The controller.</typeparam>
        /// <param name="routeData">The routeData to check</param>
        /// <param name="action">The action to call on TController.</param>
        public static RouteData ShouldMapTo<TController>(this RouteData routeData, Expression<Func<TController, ActionResult>> action)
            where TController : Controller
        {
            routeData.ShouldNotBeNull("The URL did not match any route");

            //check controller
            routeData.ShouldMapTo<TController>();

            //check action
            var methodCall = (MethodCallExpression)action.Body;
            string actualAction = routeData.Values.GetValue("action").ToString();

            string expectedAction = methodCall.Method.ActionName();
            actualAction.AssertSameStringAs(expectedAction);

            //check parameters
            for (int i = 0; i < methodCall.Arguments.Count; i++)
            {
                ParameterInfo param = methodCall.Method.GetParameters()[i];
                bool isReferenceType = !param.ParameterType.IsValueType;
                bool isNullable = isReferenceType ||
                    (param.ParameterType.UnderlyingSystemType.IsGenericType && param.ParameterType.UnderlyingSystemType.GetGenericTypeDefinition() == typeof(Nullable<>));

                string controllerParameterName = param.Name;
                bool routeDataContainsValueForParameterName = routeData.Values.ContainsKey(controllerParameterName);
                object actualValue = routeData.Values.GetValue(controllerParameterName);
                object expectedValue = null;
                Expression expressionToEvaluate = methodCall.Arguments[i];

                // If the parameter is nullable and the expression is a Convert UnaryExpression, 
                // we actually want to test against the value of the expression's operand.
                if (expressionToEvaluate.NodeType == ExpressionType.Convert
                    && expressionToEvaluate is UnaryExpression)
                {
                    expressionToEvaluate = ((UnaryExpression)expressionToEvaluate).Operand;
                }

                switch (expressionToEvaluate.NodeType)
                {
                    case ExpressionType.Constant:
                        expectedValue = ((ConstantExpression)expressionToEvaluate).Value;
                        break;

                    case ExpressionType.New:
                    case ExpressionType.MemberAccess:
                        expectedValue = Expression.Lambda(expressionToEvaluate).Compile().DynamicInvoke();
                        break;
                }

                if (isNullable && (string)actualValue == String.Empty && expectedValue == null)
                {
                    // The parameter is nullable so an expected value of '' is equivalent to null;
                    continue;
                }

                // HACK: this is only sufficient while System.Web.Mvc.UrlParameter has only a single value.
                if (actualValue == UrlParameter.Optional ||
                    (actualValue != null && actualValue.ToString().Equals("System.Web.Mvc.UrlParameter")))
                {
                    actualValue = null;
                }

                if (expectedValue is DateTime)
                {
                    actualValue = Convert.ToDateTime(actualValue);
                }
                else
                {
                    expectedValue = (expectedValue == null ? expectedValue : expectedValue.ToString());
                }

                string errorMsgFmt = "Value for parameter '{0}' did not match: expected '{1}' but was '{2}'";
                if (routeDataContainsValueForParameterName)
                {
                    errorMsgFmt += ".";
                }
                else
                {
                    errorMsgFmt += "; no value found in the route context action parameter named '{0}' - does your matching route contain a token called '{0}'?";
                }
                actualValue.ShouldEqual(expectedValue, String.Format(errorMsgFmt, controllerParameterName, expectedValue, actualValue));
            }

            return routeData;
        }

        /// <summary>
        /// Converts the URL to matching RouteData and verifies that it will match a route with the values specified by the expression.
        /// </summary>
        /// <typeparam name="TController">The type of controller</typeparam>
        /// <param name="relativeUrl">The ~/ based url</param>
        /// <param name="action">The expression that defines what action gets called (and with which parameters)</param>
        /// <returns></returns>
        public static RouteData ShouldMapTo<TController>(this string relativeUrl, Expression<Func<TController, ActionResult>> action) where TController : Controller
        {
            return relativeUrl.Route().ShouldMapTo(action);
        }

        /// <summary>
        /// Verifies the <see cref="RouteData">routeData</see> maps to the controller type specified.
        /// </summary>
        /// <typeparam name="TController"></typeparam>
        /// <param name="routeData"></param>
        /// <returns></returns>
        public static RouteData ShouldMapTo<TController>(this RouteData routeData) where TController : Controller
        {
            //strip out the word 'Controller' from the type
            string expected = typeof(TController).Name.Replace("Controller", "");

            //get the key (case insensitive)
            string actual = routeData.Values.GetValue("controller").ToString();

            actual.AssertSameStringAs(expected);
            return routeData;
        }

        /// <summary>
        /// Verifies the <see cref="RouteData">routeData</see> will instruct the routing engine to ignore the route.
        /// </summary>
        /// <param name="relativeUrl"></param>
        /// <returns></returns>
        public static RouteData ShouldBeIgnored(this string relativeUrl)
        {
            RouteData routeData = relativeUrl.Route();

            routeData.RouteHandler.ShouldBe<StopRoutingHandler>();
            return routeData;
        }

        /// <summary>
        /// Gets a value from the <see cref="RouteValueDictionary" /> by key.  Does a
        /// case-insensitive search on the keys.
        /// </summary>
        /// <param name="routeValues"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static object GetValue(this RouteValueDictionary routeValues, string key)
        {
            foreach (var routeValueKey in routeValues.Keys)
            {
                if (string.Equals(routeValueKey, key, StringComparison.InvariantCultureIgnoreCase))
                {
                    if (routeValues[routeValueKey] == null)
                        return null;
                    return routeValues[routeValueKey].ToString();
                }
            }

            return null;
        }
        
        #endregion 
    }
}
