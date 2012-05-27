using Nop.Tests;
using Nop.Web.Models.Polls;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Public.Models
{
    [TestFixture]
    public class PollModelTests
    {
        [Test]
        public void Can_clone()
        {	    
            //create
            var pollModel1 = new PollModel()
            {
                Id = 1,
                Name = "Name 1",
                AlreadyVoted = true,
                TotalVotes = 2,
            };
            pollModel1.Answers.Add(new PollAnswerModel() {Id = 3, Name = "answer 1", NumberOfVotes = 4, PercentOfTotalVotes = 5});

            //clone
            var pollModel2 = (PollModel)pollModel1.Clone();
            pollModel2.Id.ShouldEqual(1);
            pollModel2.Name.ShouldEqual("Name 1");
            pollModel2.AlreadyVoted.ShouldEqual(true);
            pollModel2.TotalVotes.ShouldEqual(2);
            pollModel2.Answers.ShouldNotBeNull();
            pollModel2.Answers.Count.ShouldEqual(1);
            pollModel2.Answers[0].Id.ShouldEqual(3);
            pollModel2.Answers[0].Name.ShouldEqual("answer 1");
            pollModel2.Answers[0].NumberOfVotes.ShouldEqual(4);
            pollModel2.Answers[0].PercentOfTotalVotes.ShouldEqual(5);
        }
    }
}
