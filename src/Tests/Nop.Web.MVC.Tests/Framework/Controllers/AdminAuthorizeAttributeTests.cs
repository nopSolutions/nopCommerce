using System.Web.Mvc;
using System.Web.Routing;
using Nop.Core.Fakes;
using Nop.Web.Framework.Controllers;
using NUnit.Framework;
using Rhino.Mocks;

namespace Nop.Web.MVC.Tests.Framework.Controllers
{
    [TestFixture]
    public class AdminAuthorizeAttributeTests
    {
        private AuthorizationContext GetAuthorizationContext<TController>() where TController : ControllerBase, new()
        {
            var controllerDescriptor = new ReflectedControllerDescriptor(typeof(TController));
            var controllerContext = new ControllerContext(new FakeHttpContext("~/"), new RouteData(), new TController());
            return new AuthorizationContext(controllerContext, controllerDescriptor.FindAction(controllerContext, "Index"));
        }

        private AdminAuthorizeAttribute GetAdminAuthorizeAttribute(bool result)
        {
            var attribute = MockRepository.GeneratePartialMock<AdminAuthorizeAttribute>();
            //by the way, HasAdminAccess should method be virtual in order to be overridden
            attribute.Expect(x => x.HasAdminAccess(Arg<AuthorizationContext>.Is.Anything)).Return(result);
            return attribute;
        }
        private void TestActionThatShouldRequirePermission<TController>() where TController : ControllerBase, new()
        {
            var authorizationContext = GetAuthorizationContext<TController>();
            var attribute = GetAdminAuthorizeAttribute(false);
            attribute.OnAuthorization(authorizationContext);
            Assert.That(authorizationContext.Result, Is.InstanceOf<HttpUnauthorizedResult>());

            var authorizationContext2 = GetAuthorizationContext<TController>();
            var attribute2 = GetAdminAuthorizeAttribute(true);
            attribute2.OnAuthorization(authorizationContext2);
            Assert.That(authorizationContext2.Result, Is.Null);
        }

        [Test]
        public void Normal_request_should_not_be_affected()
        {
            var authorizationContext = GetAuthorizationContext<NormalController>();

            var attribute = GetAdminAuthorizeAttribute(false);
            attribute.OnAuthorization(authorizationContext);

            Assert.That(authorizationContext.Result, Is.Null);
        }

        
        [Test]
        public void Normal_with_attribute_request_should_require_permission()
        {
            TestActionThatShouldRequirePermission<NormalWithAttribController>();
        }

        [Test]
        public void Normal_with_action_attribute_request_should_require_permission()
        {
            TestActionThatShouldRequirePermission<NormalWithActionAttribController>();
        }

        [Test]
        public void Inherited_attribute_request_should_require_permission()
        {
            TestActionThatShouldRequirePermission<InheritedAttribController>();
        }
    }

    public class NormalController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }
    
    [AdminAuthorize]
    public class NormalWithAttribController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
    }

    public class NormalWithActionAttribController : Controller
    {
        [AdminAuthorize]
        public ActionResult Index()
        {
            return View();
        }
    }

    [AdminAuthorize]
    public class BaseWithAttribController : Controller
    {
        public ActionResult Something()
        {
            return View();
        }
    }

    public class InheritedAttribController : BaseWithAttribController
    {
        public ActionResult Index()
        {
            return View();
        }
    }
}
