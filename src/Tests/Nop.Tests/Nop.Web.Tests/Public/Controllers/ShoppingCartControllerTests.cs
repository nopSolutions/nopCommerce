using NUnit.Framework;

using Nop.Web.Controllers;
using NUnit.Framework.Legacy;

namespace Nop.Tests.Nop.Web.Tests.Public.Controllers

{

    [TestFixture]

    public class ShoppingCartControllerTests

    {

        [Test]

        public void GetCartStatusMessage_Should_Return_Updated_When_Id_Is_Positive()

        {

            // Arrange

            int id = 14;
            string addedText = "Added logic";

            string updatedText = "Updated logic";

            // Act 
            string result = ShoppingCartController.GetCartStatusMessage(id, addedText, updatedText);

            // Assert

            ClassicAssert.AreEqual("Updated logic", result, "If ID is 14 we shall get the Updated-text");

        }

        [Test]

        public void GetCartStatusMessage_Should_Return_Added_When_Id_Is_Zero()

        {

            // Arrange

            int id = 0;

            string addedText = "Added logic";

            string updatedText = "Updated logic";

            // Act

            string result = ShoppingCartController.GetCartStatusMessage(id, addedText, updatedText);

            // Assert

            ClassicAssert.AreEqual("Added logic", result, "If ID is 0 we shall get the Added-text");

        }

    }

}