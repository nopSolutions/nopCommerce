using FluentAssertions;
using Nop.Web.Models.Polls;
using NUnit.Framework;

namespace Nop.Tests.Nop.Web.Tests.Public.Models.Polls;

[TestFixture]
public class PollModelTests
{
    [Test]
    public void CanClone()
    {
        //create
        var model1 = new PollModel
        {
            Id = 1,
            Name = "Name 1",
            AlreadyVoted = true,
            TotalVotes = 2,
        };
        model1.Answers.Add(new PollAnswerModel
        {
            Id = 3,
            Name = "answer 1",
            NumberOfVotes = 4,
            PercentOfTotalVotes = 5
        });

        //clone
        var model2 = model1 with { };
        model2.Id.Should().Be(1);
        model2.Name.Should().Be("Name 1");
        model2.AlreadyVoted.Should().BeTrue();
        model2.TotalVotes.Should().Be(2);
        model2.Answers.Should().NotBeNull();
        model2.Answers.Count.Should().Be(1);
        model2.Answers[0].Id.Should().Be(3);
        model2.Answers[0].Name.Should().Be("answer 1");
        model2.Answers[0].NumberOfVotes.Should().Be(4);
        model2.Answers[0].PercentOfTotalVotes.Should().Be(5);
    }
}