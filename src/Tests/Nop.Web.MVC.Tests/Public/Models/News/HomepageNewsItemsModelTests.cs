using System;
using Nop.Tests;
using Nop.Web.Models.News;
using NUnit.Framework;

namespace Nop.Web.MVC.Tests.Public.Models.News
{
    [TestFixture]
    public class HomepageNewsItemsModelTests
    {
        [Test]
        public void Can_clone()
        {	    
            //create
            var model1 = new HomepageNewsItemsModel
            {
                WorkingLanguageId = 1,
            };
            var newsItemModel1 = new NewsItemModel
            {
                Id = 1,
                SeName = "SeName 1",
                Title = "Title 1",
                Short = "Short 1",
                Full = "Full 1",
                AllowComments = true,
                NumberOfComments = 2,
                CreatedOn = new DateTime(2010, 01, 01),
                AddNewComment = new AddNewsCommentModel
                {
                    CommentTitle = "CommentTitle 1",
                    CommentText = "CommentText 1",
                    DisplayCaptcha = true
                }
            };
            newsItemModel1.Comments.Add(new NewsCommentModel
            {
                Id = 3,
                CustomerId = 4,
                CustomerName = "CustomerName 1",
                CustomerAvatarUrl = "CustomerAvatarUrl 1",
                CommentTitle = "CommentTitle 1",
                CommentText = "CommentText 1",
                CreatedOn = new DateTime(2010, 01, 02),
                AllowViewingProfiles = true
            });
            model1.NewsItems.Add(newsItemModel1);

            //clone
            var model2 = (HomepageNewsItemsModel)model1.Clone();
            model2.WorkingLanguageId.ShouldEqual(1);
            model2.NewsItems.ShouldNotBeNull();
            model2.NewsItems.Count.ShouldEqual(1);
            var newsItemModel2 = model2.NewsItems[0];
            newsItemModel2.Id.ShouldEqual(1);
            newsItemModel2.SeName.ShouldEqual("SeName 1");
            newsItemModel2.Title.ShouldEqual("Title 1");
            newsItemModel2.Short.ShouldEqual("Short 1");
            newsItemModel2.Full.ShouldEqual("Full 1");
            newsItemModel2.AllowComments.ShouldEqual(true);
            newsItemModel2.NumberOfComments.ShouldEqual(2);
            newsItemModel2.CreatedOn.ShouldEqual(new DateTime(2010, 01, 01));
            newsItemModel2.Comments.ShouldNotBeNull();
            newsItemModel2.Comments.Count.ShouldEqual(1);
            newsItemModel2.Comments[0].Id.ShouldEqual(3);
            newsItemModel2.Comments[0].CustomerId.ShouldEqual(4);
            newsItemModel2.Comments[0].CustomerName.ShouldEqual("CustomerName 1");
            newsItemModel2.Comments[0].CustomerAvatarUrl.ShouldEqual("CustomerAvatarUrl 1");
            newsItemModel2.Comments[0].CommentTitle.ShouldEqual("CommentTitle 1");
            newsItemModel2.Comments[0].CommentText.ShouldEqual("CommentText 1");
            newsItemModel2.Comments[0].CreatedOn.ShouldEqual(new DateTime(2010, 01, 02));
            newsItemModel2.Comments[0].AllowViewingProfiles.ShouldEqual(true);
            newsItemModel2.AddNewComment.ShouldNotBeNull();
            newsItemModel2.AddNewComment.CommentTitle.ShouldEqual("CommentTitle 1");
            newsItemModel2.AddNewComment.CommentText.ShouldEqual("CommentText 1");
            newsItemModel2.AddNewComment.DisplayCaptcha.ShouldEqual(true);
        }
    }
}
