using System.Collections.Generic;
using Nop.Core.Domain.Messages;
using Nop.Services.Messages;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Services.Tests.Messages
{
    [TestFixture]
    public class TokenizerTests : ServiceTest
    {
        [Test]
        public void Can_replace_tokens_case_sensitive()
        {
            var messageTemplatesSettings = new MessageTemplatesSettings
            {
                CaseInvariantReplacement = false
            };
            var tokenizer = new Tokenizer(messageTemplatesSettings);

            var tokens = new List<Token>
            {
                new Token("Token1", "Value1")
            };
            //correct case
            tokenizer
                .Replace("Some text %Token1%", tokens, false)
                .ShouldEqual("Some text Value1");
            //wrong case
            tokenizer
                .Replace("Some text %TOKeN1%", tokens, false)
                .ShouldEqual("Some text %TOKeN1%");
        }

        [Test]
        public void Can_replace_tokens_case_invariant()
        {
            var messageTemplatesSettings = new MessageTemplatesSettings
            {
                CaseInvariantReplacement = true
            };
            var tokenizer = new Tokenizer(messageTemplatesSettings);

            var tokens = new List<Token>
            {
                new Token("Token1", "Value1")
            };
            tokenizer
                .Replace("Some text %TOKEn1%", tokens, false)
                .ShouldEqual("Some text Value1");
        }

        [Test]
        public void Can_html_encode()
        {
            var messageTemplatesSettings = new MessageTemplatesSettings
            {
                CaseInvariantReplacement = false
            };
            var tokenizer = new Tokenizer(messageTemplatesSettings);

            var tokens = new List<Token>
            {
                new Token("Token1", "<Value1>")
            };

            tokenizer
                .Replace("Some text %Token1%", tokens, true)
                .ShouldEqual("Some text &lt;Value1&gt;");
        }

        [Test]
        public void Should_not_html_encode_if_token_doesnt_allow_it()
        {
            var messageTemplatesSettings = new MessageTemplatesSettings
            {
                CaseInvariantReplacement = false
            };
            var tokenizer = new Tokenizer(messageTemplatesSettings);

            var tokens = new List<Token>
            {
                new Token("Token1", "<Value1>", true)
            };

            tokenizer
                .Replace("Some text %Token1%", tokens, true)
                .ShouldEqual("Some text <Value1>");
        }

        [Test]
        public void Can_replace_tokens_with_conditional_statements()
        {
            var tokenizer = new Tokenizer(new MessageTemplatesSettings
            {
                CaseInvariantReplacement = false
            });

            var tokens = new List<Token>
            {
                new Token("ConditionToken", true),
                new Token("ConditionToken2", 2),
                new Token("ThenToken", "value"),
                new Token("ThenToken2", "value2"),
                new Token("SomeValueToken", 10),
            };

            //simple condition
            tokenizer.Replace(@"Some text %if (%ConditionToken%) %ThenToken% endif% %SomeValueToken%", tokens, true)
                .ShouldEqual("Some text value  10");

            //broken token in condition
            tokenizer.Replace(@"Some text %if (ConditionToken%) %ThenToken% endif% %SomeValueToken%", tokens, true)
                .ShouldEqual("Some text  10");

            //multiple conditions
            tokenizer.Replace(@"Some text %if (%ConditionToken% && %ConditionToken2% > 1) %ThenToken% endif% %SomeValueToken%", tokens, true)
                .ShouldEqual("Some text value  10");

            //nested conditional statements
            tokenizer.Replace(@"Some text %if (%ConditionToken%) %ThenToken% %if (%ConditionToken2% > 1) %ThenToken2% endif% %if (!%ConditionToken%) %ThenToken% endif% %ThenToken% endif% %SomeValueToken%", tokens, true)
                .ShouldEqual("Some text value value2   value  10");

            //wrong condition
            tokenizer.Replace(@"Some text %if (%ConditionToken%) %ThenToken%", tokens, true)
                .ShouldEqual("Some text %if (True) value");

            //complex condition
            tokenizer.Replace(@"Some text %if (%ConditionToken%) %ThenToken% endif% %SomeValueToken% %if (%ConditionToken2% > 1) %ThenToken2% %if (!%ConditionToken%) %ThenToken% endif% endif% %SomeValueToken%", tokens, true)
                .ShouldEqual("Some text value  10 value2   10");
        }

        [Test]
        public void Can_replace_tokens_with_non_string_values()
        {
            var tokenizer = new Tokenizer(new MessageTemplatesSettings
            {
                CaseInvariantReplacement = false
            });

            var tokens = new List<Token>
            {
                new Token("Token1", true),
                new Token("Token2", 1),
                new Token("Token3", "value"),
            };

            tokenizer.Replace("Some text %Token1%, %Token2%, %Token3%", tokens, true)
                .ShouldEqual("Some text True, 1, value");
        }
    }
}
