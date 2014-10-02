using System;
using System.Linq;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Localization;
using Nop.Core.Domain.Polls;
using Nop.Tests;
using NUnit.Framework;

namespace Nop.Data.Tests.Polls
{
    [TestFixture]
    public class PollPersistenceTests : PersistenceTest
    {
        [Test]
        public void Can_save_and_load_poll()
        {
            var poll = new Poll
            {
                Name = "Name 1",
                SystemKeyword = "SystemKeyword 1",
                Published = true,
                ShowOnHomePage = true,
                DisplayOrder = 1,
                StartDateUtc = new DateTime(2010, 01, 01),
                EndDateUtc = new DateTime(2010, 01, 02),
                Language = new Language
                {
                    Name = "English",
                    LanguageCulture = "en-Us",
                }
            };

            var fromDb = SaveAndLoadEntity(poll);
            fromDb.ShouldNotBeNull();
            fromDb.Name.ShouldEqual("Name 1");
            fromDb.SystemKeyword.ShouldEqual("SystemKeyword 1");
            fromDb.Published.ShouldEqual(true);
            fromDb.ShowOnHomePage.ShouldEqual(true);
            fromDb.DisplayOrder.ShouldEqual(1);
            fromDb.StartDateUtc.ShouldEqual(new DateTime(2010, 01, 01));
            fromDb.EndDateUtc.ShouldEqual(new DateTime(2010, 01, 02));

            fromDb.Language.ShouldNotBeNull();
            fromDb.Language.Name.ShouldEqual("English");
        }

        [Test]
        public void Can_save_and_load_poll_with_answers()
        {
            var poll = new Poll
            {
                Name = "Name 1",
                SystemKeyword = "SystemKeyword 1",
                Published = true,
                ShowOnHomePage = true,
                DisplayOrder = 1,
                StartDateUtc = new DateTime(2010, 01, 01),
                EndDateUtc = new DateTime(2010, 01, 02),
                Language = new Language
                {
                    Name = "English",
                    LanguageCulture = "en-Us",
                }
            };
            poll.PollAnswers.Add
                (
                    new PollAnswer
                    {
                        Name = "Answer 1",
                        NumberOfVotes = 1,
                        DisplayOrder = 2,
                    }
                );
            var fromDb = SaveAndLoadEntity(poll);
            fromDb.ShouldNotBeNull();


            fromDb.PollAnswers.ShouldNotBeNull();
            (fromDb.PollAnswers.Count == 1).ShouldBeTrue();
            fromDb.PollAnswers.First().Name.ShouldEqual("Answer 1");
            fromDb.PollAnswers.First().NumberOfVotes.ShouldEqual(1);
            fromDb.PollAnswers.First().DisplayOrder.ShouldEqual(2);
        }

        [Test]
        public void Can_save_and_load_poll_with_answer_and_votingrecord()
        {
            var poll = new Poll
            {
                Name = "Name 1",
                SystemKeyword = "SystemKeyword 1",
                Published = true,
                ShowOnHomePage = true,
                DisplayOrder = 1,
                StartDateUtc = new DateTime(2010, 01, 01),
                EndDateUtc = new DateTime(2010, 01, 02),
                Language = new Language
                {
                    Name = "English",
                    LanguageCulture = "en-Us",
                }
            };
            poll.PollAnswers.Add
                (
                    new PollAnswer
                    {
                        Name = "Answer 1",
                        NumberOfVotes = 1,
                        DisplayOrder = 2,
                    }
                );
            poll.PollAnswers.First().PollVotingRecords.Add
                (
                    new PollVotingRecord
                    {
                        Customer = GetTestCustomer(),
                        CreatedOnUtc = DateTime.UtcNow
                    }
                );
            var fromDb = SaveAndLoadEntity(poll);
            fromDb.ShouldNotBeNull();


            fromDb.PollAnswers.ShouldNotBeNull();
            (fromDb.PollAnswers.Count == 1).ShouldBeTrue();

            fromDb.PollAnswers.First().PollVotingRecords.ShouldNotBeNull();
            (fromDb.PollAnswers.First().PollVotingRecords.Count == 1).ShouldBeTrue();
        }

        protected Customer GetTestCustomer()
        {
            return new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                CreatedOnUtc = new DateTime(2010, 01, 01),
                LastActivityDateUtc = new DateTime(2010, 01, 02)
            };
        }
    }
}
