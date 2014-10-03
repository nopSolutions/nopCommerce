using System;
using System.Reflection;
using NUnit.Framework;

namespace Nop.Tests
{
    /// <summary>
    /// Contains exception assertions used for unit testing.
    /// </summary>
    public class ExceptionAssert
    {
        /// <summary>A method delegate that is invoked by <see cref="ExceptionAssert"/>.</summary>
        public delegate void ExceptionDelegate();

        /// <summary>Executes a method and asserts that the specified exception is thrown.</summary>
        /// <param name="exceptionType">The type of exception to expect.</param>
        /// <param name="method">The method to execute.</param>
        /// <returns>The thrown exception.</returns>
        public static Exception Throws(Type exceptionType, ExceptionDelegate method)
        {
            try
            {
                method.Invoke();
            }
            catch (Exception ex)
            {
                Assert.AreEqual(exceptionType, ex.GetType());
                return ex;
            }
            Assert.Fail("Expected exception '" + exceptionType.FullName + "' wasn't thrown.");
            return null;
        }

        /// <summary>Executes a method and asserts that the specified exception is thrown.</summary>
        /// <typeparam name="T">The type of exception to expect.</typeparam>
        /// <param name="method">The method to execute.</param>
        /// <returns>The thrown exception.</returns>
        public static T Throws<T>(ExceptionDelegate method)
            where T : Exception
        {
            try
            {
                method.Invoke();
            }
            catch (TargetInvocationException ex)
            {
                Assert.That(ex.InnerException, Is.TypeOf(typeof(T)));
            }
            catch (T ex)
            {
                return ex;
            }
            catch (Exception ex)
            {
                Assert.Fail("Expected exception '" + typeof(T).FullName + "' but got exception '" + ex.GetType() + "'.");
                return null;
            }
            Assert.Fail("Expected exception '" + typeof(T).FullName + "' wasn't thrown.");
            return null;
        }

        /// <summary>Executes a method and asserts that the specified exception is thrown.</summary>
        /// <typeparam name="T">The type of exception to expect.</typeparam>
        /// <param name="method">The method to execute.</param>
        /// <returns>The thrown exception.</returns>
        public static void InnerException<T>(ExceptionDelegate method)
            where T : Exception
        {
            try
            {
                method.Invoke();
            }
            catch (Exception ex)
            {
                TypeAssert.AreEqual(typeof(T), ex.InnerException);
                return;
            }
            Assert.Fail("Expected exception '" + typeof(T).FullName + "' wasn't thrown.");
        }
    }

    [TestFixture]
    public class ExceptionAssertTests
    {
        //[Test, ExpectedException(typeof(MbUnit.Core.Exceptions.AssertionException))]
        //public void FailsOnExceptionNotTrown()
        //{
        //    ExceptionAssert.Throws(typeof(ArgumentException), delegate { });
        //}
        //[Test, ExpectedException(typeof(MbUnit.Core.Exceptions.AssertionException))]
        //public void FailsOnWrongTypeOfException()
        //{
        //    ExceptionAssert.Throws(
        //        typeof(ArgumentException),
        //        delegate
        //            {
        //                throw new Exception("rebuke me");
        //            });
        //}
        [Test]
        public void PassesOnExceptionTrown()
        {
            ExceptionAssert.Throws(
                typeof(ArgumentException),
                delegate
                {
                    throw new ArgumentException("catch me");
                });
        }
        [Test]
        public void ReturnsTheException()
        {
            Exception ex = ExceptionAssert.Throws(
                typeof(ArgumentException),
                delegate
                {
                    throw new ArgumentException("return me");
                });
            Assert.AreEqual("return me", ex.Message);
        }

        //[Test, ExpectedException(typeof(MbUnit.Core.Exceptions.AssertionException))]
        //public void FailsOnExceptionNotTrown_generic()
        //{
        //    ExceptionAssert.Throws<ArgumentException>(delegate { });
        //}
        //[Test, ExpectedException(typeof(MbUnit.Core.Exceptions.AssertionException))]
        //public void FailsOnWrongTypeOfException_generic()
        //{
        //    ExceptionAssert.Throws<ArgumentException>(
        //        delegate
        //            {
        //                throw new Exception("rebuke me");
        //            });
        //}
        [Test]
        public void PassesOnExceptionTrown_generic()
        {
            ExceptionAssert.Throws<ArgumentException>(
                delegate
                {
                    throw new ArgumentException("catch me");
                });
        }
        [Test]
        public void ReturnsTheException_generic()
        {
            var ex = ExceptionAssert.Throws<ArgumentException>(
                delegate
                {
                    throw new ArgumentException("return me");
                });
            Assert.AreEqual("return me", ex.Message);
        }
    }
}
