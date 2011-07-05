using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Forums;
using Nop.Services.Customers;
using Nop.Services.Forums;
using Nop.Services.Localization;
using Nop.Web.Framework.Controllers;
using Nop.Web.Models;
using Nop.Web.Models.PrivateMessages;

namespace Nop.Web.Controllers
{
    public class PrivateMessagesController : BaseNopController
    {
        private readonly IForumService _forumService;
        private readonly ICustomerService _customerService;
        private readonly ILocalizationService _localizationService;
        private readonly IWorkContext _workContext;
        private readonly ForumSettings _forumSettings;

        public PrivateMessagesController(IForumService forumService,
            ICustomerService customerService,
            ILocalizationService localizationService,
            IWorkContext workContext,
            ForumSettings forumSettings)
        {
            this._forumService = forumService;
            this._customerService = customerService;
            this._localizationService = localizationService;
            this._workContext = workContext;
            this._forumSettings = forumSettings;
        }

        [NonAction]
        private bool AllowPrivateMessages()
        {
            return _forumSettings.AllowPrivateMessages;
        }

        public ActionResult Index(int? page, string tab)
        {
            if (!AllowPrivateMessages())
            {
                return RedirectToAction("index", "home");
            }

            if (_workContext.CurrentCustomer.IsGuest())
            {
                return new HttpUnauthorizedResult();
            }

            int inboxPage = 0;
            int sentItemsPage = 0;
            bool sentItemsTabSelected = false;

            switch (tab)
            {
                case "inbox":
                    if (page.HasValue)
                    {
                        inboxPage = page.Value;
                    }
                    break;
                case "sent":
                    if (page.HasValue)
                    {
                        sentItemsPage = page.Value;
                    }
                    sentItemsTabSelected = true;
                    break;
                default:
                    break;
            }

            var model = new PrivateMessageIndexModel()
            {
                InboxPage = inboxPage,
                SentItemsPage = sentItemsPage,
                SentItemsTabSelected = sentItemsTabSelected
            };

            return View(model);
        }

        //inbox tab
        [ChildActionOnly]
        public ActionResult Inbox(int page, string tab)
        {
            if (page > 0)
            {
                page -= 1;
            }

            var pageSize = _forumSettings.PrivateMessagesPageSize;

            var list = _forumService.GetAllPrivateMessages(0, _workContext.CurrentCustomer.Id, null, null, false, string.Empty, page, pageSize);

            var inbox = new List<PrivateMessageModel>();

            foreach (var pm in list)
            {
                inbox.Add(new PrivateMessageModel()
                {
                    customerFromName = pm.FromCustomer.FormatUserName(),
                    customerToName = pm.ToCustomer.FormatUserName(),
                    Subject = pm.Subject,
                    Message = pm.Text,
                    CreatedOnUtc = pm.CreatedOnUtc,
                    Id = pm.Id,
                    IsRead = pm.IsRead,
                });
            }

            var pagerModel = new PagerModel()
            {
                PageSize = list.PageSize,
                TotalRecords = list.TotalCount,
                PageIndex = list.PageIndex,
                ShowTotalSummary = false,
                RouteActionName = "PrivateMessagesPaged",
                UseRouteLinks = true,
                RouteValues = new PrivateMessageRouteValues { page = page, tab = tab }
            };

            var model = new PrivateMessageListModel()
            {
                Messages = inbox,
                PagerModel = pagerModel
            };

            return PartialView(model);
        }

        //sent items tab
        [ChildActionOnly]
        public ActionResult SentItems(int page, string tab)
        {
            if (page > 0)
            {
                page -= 1;
            }

            var pageSize = _forumSettings.PrivateMessagesPageSize;

            var list = _forumService.GetAllPrivateMessages(_workContext.CurrentCustomer.Id, 0, null, false, null, string.Empty, page, pageSize);

            var sentItems = new List<PrivateMessageModel>();

            foreach (var pm in list)
            {
                sentItems.Add(new PrivateMessageModel()
                {
                    customerFromName = pm.FromCustomer.FormatUserName(),
                    customerToName = pm.ToCustomer.FormatUserName(),
                    Subject = pm.Subject,
                    Message = pm.Text,
                    CreatedOnUtc = pm.CreatedOnUtc,
                    Id = pm.Id,
                    IsRead = pm.IsRead,
                });
            }

            var pagerModel = new PagerModel()
            {
                PageSize = list.PageSize,
                TotalRecords = list.TotalCount,
                PageIndex = list.PageIndex,
                ShowTotalSummary = false,
                RouteActionName = "PrivateMessagesPaged",
                UseRouteLinks = true,
                RouteValues = new PrivateMessageRouteValues { page = page, tab = tab }
            };

            var model = new PrivateMessageListModel()
            {
                Messages = sentItems,
                PagerModel = pagerModel
            };

            return PartialView(model);
        }

        //updates inbox (deletes or marks PrivateMessages as unread)
        [HttpPost, FormValueExists("inboxupdate", "delete", "deleteMessages")]
        public ActionResult InboxUpdate(FormCollection formCollection, bool deleteMessages)
        {
            foreach (var key in formCollection.AllKeys)
            {
                var value = formCollection[key];

                if (value.Equals("on") && key.StartsWith("pm", StringComparison.InvariantCultureIgnoreCase))
                {
                    var id = key.Replace("pm", "").Trim();
                    int privateMessageId = 0;

                    if (Int32.TryParse(id, out privateMessageId))
                    {
                        var pm = _forumService.GetPrivateMessageById(privateMessageId);
                        if (pm != null)
                        {
                            if (pm.ToCustomerId == _workContext.CurrentCustomer.Id)
                            {
                                if (deleteMessages)
                                {
                                    pm.IsDeletedByRecipient = true;
                                }
                                else
                                {
                                    pm.IsRead = false;
                                }

                                _forumService.UpdatePrivateMessage(pm);
                            }
                        }
                    }
                }
            }
            return RedirectToAction("Index");
        }

        //updates sent items (deletes PrivateMessages)
        [HttpPost, FormValueExists("sentupdate", "delete", "deleteMessages")]
        public ActionResult SentUpdate(FormCollection formCollection, bool deleteMessages)
        {
            if (deleteMessages)
            {
                foreach (var key in formCollection.AllKeys)
                {
                    var value = formCollection[key];

                    if (value.Equals("on") && key.StartsWith("si", StringComparison.InvariantCultureIgnoreCase))
                    {
                        var id = key.Replace("si", "").Trim();
                        int privateMessageId = 0;

                        if (Int32.TryParse(id, out privateMessageId))
                        {
                            PrivateMessage pm = _forumService.GetPrivateMessageById(privateMessageId);
                            if (pm != null)
                            {
                                if (pm.FromCustomerId == _workContext.CurrentCustomer.Id)
                                {
                                    pm.IsDeletedByAuthor = true;
                                    _forumService.UpdatePrivateMessage(pm);
                                }
                            }
                        }
                    }

                }
            }
            return RedirectToAction("Index", new { tab = "sent" });
        }

        public ActionResult SendPM(int toCustomerId, int? replyToMessageId)
        {
            if (!AllowPrivateMessages())
            {
                return RedirectToAction("index", "home");
            }

            if (_workContext.CurrentCustomer.IsGuest())
            {
                return new HttpUnauthorizedResult();
            }

            var customerTo = _customerService.GetCustomerById(toCustomerId);

            if (customerTo == null)
            {
                return RedirectToAction("Index");
            }

            var model = new PrivateMessageModel();
            model.ToCustomerId = toCustomerId;
            model.customerToName = customerTo.FormatUserName();

            if (replyToMessageId.HasValue)
            {
                var replyToPM = _forumService.GetPrivateMessageById(replyToMessageId.Value);
                if (replyToPM == null)
                {
                    return RedirectToAction("Index");
                }

                if (replyToPM.ToCustomerId == _workContext.CurrentCustomer.Id || replyToPM.FromCustomerId == _workContext.CurrentCustomer.Id)
                {
                    model.ReplyToMessageId = replyToPM.Id;
                    model.Subject = string.Format("Re: {0}", replyToPM.Subject);
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult SendPM(PrivateMessageModel model)
        {
            if (!AllowPrivateMessages())
            {
                return RedirectToAction("index", "home");
            }

            if (_workContext.CurrentCustomer.IsGuest())
            {
                return new HttpUnauthorizedResult();
            }

            Customer toCustomer = null;
            var replyToPM = _forumService.GetPrivateMessageById(model.ReplyToMessageId);
            if (replyToPM != null)
            {
                if (replyToPM.ToCustomerId == _workContext.CurrentCustomer.Id || replyToPM.FromCustomerId == _workContext.CurrentCustomer.Id)
                {
                    toCustomer = replyToPM.FromCustomer;
                }
                else
                {
                    return RedirectToAction("Index");
                }
            }
            else
            {
                toCustomer = _customerService.GetCustomerById(model.ToCustomerId);
            }

            if (toCustomer == null || toCustomer.IsGuest())
            {
                return RedirectToAction("Index");
            }

            try
            {
                string subject = model.Subject;
                if (subject != null)
                {
                    subject = subject.Trim();
                }

                if (String.IsNullOrEmpty(subject))
                {
                    throw new NopException(_localizationService.GetResource("PrivateMessages.SubjectCannotBeEmpty"));
                }

                var maxSubjectLength = _forumSettings.PMSubjectMaxLength;
                if (maxSubjectLength > 0 && subject.Length > maxSubjectLength)
                {
                    subject = subject.Substring(0, maxSubjectLength);
                }

                var text = model.Message;
                if (text != null)
                {
                    text = text.Trim();
                }

                if (String.IsNullOrEmpty(text))
                {
                    throw new NopException(_localizationService.GetResource("PrivateMessages.MessageCannotBeEmpty"));
                }

                var maxPostLength = _forumSettings.PMTextMaxLength;
                if (maxPostLength > 0 && text.Length > maxPostLength)
                {
                    text = text.Substring(0, maxPostLength);
                }

                var nowUtc = DateTime.UtcNow;

                var privateMessage = new PrivateMessage
                {
                    ToCustomerId = toCustomer.Id,
                    FromCustomerId = _workContext.CurrentCustomer.Id,
                    Subject = subject,
                    Text = text,
                    IsDeletedByAuthor = false,
                    IsDeletedByRecipient = false,
                    IsRead = false,
                    CreatedOnUtc = nowUtc
                };

                _forumService.InsertPrivateMessage(privateMessage);

                return RedirectToAction("Index", new { tab = "sent" });
            }
            catch (Exception ex)
            {
                model.PostError = ex.Message;
            }

            return View(model);
        }

        public ActionResult ViewPM(int privateMessageId)
        {
            if (!AllowPrivateMessages())
            {
                return RedirectToAction("index", "home");
            }

            if (_workContext.CurrentCustomer.IsGuest())
            {
                return new HttpUnauthorizedResult();
            }

            var pm = _forumService.GetPrivateMessageById(privateMessageId);
            if (pm != null)
            {
                if (pm.ToCustomerId != _workContext.CurrentCustomer.Id && pm.FromCustomerId != _workContext.CurrentCustomer.Id)
                {
                    return RedirectToAction("Index");
                }

                if (!pm.IsRead && pm.ToCustomerId == _workContext.CurrentCustomer.Id)
                {
                    pm.IsRead = true;
                    _forumService.UpdatePrivateMessage(pm);
                }
            }
            else
            {
                return RedirectToAction("Index");
            }

            var model = new PrivateMessageModel()
            {
                customerFromName = pm.FromCustomer.FormatUserName(),
                customerToName = pm.ToCustomer.FormatUserName(),
                Subject = pm.Subject,
                Message = pm.FormatPrivateMessageText(),
                ToCustomerId = pm.FromCustomerId,
                Id = pm.Id,
            };

            return View(model);
        }

        public ActionResult DeletePM(int privateMessageId)
        {
            if (!AllowPrivateMessages())
            {
                return RedirectToAction("index", "home");
            }

            if (_workContext.CurrentCustomer.IsGuest())
            {
                return new HttpUnauthorizedResult();
            }

            var pm = _forumService.GetPrivateMessageById(privateMessageId);
            if (pm != null)
            {
                if (pm.FromCustomerId == _workContext.CurrentCustomer.Id)
                {
                    pm.IsDeletedByAuthor = true;
                    _forumService.UpdatePrivateMessage(pm);
                }

                if (pm.ToCustomerId == _workContext.CurrentCustomer.Id)
                {
                    pm.IsDeletedByRecipient = true;
                    _forumService.UpdatePrivateMessage(pm);
                }       
            }
            return RedirectToAction("Index");
        }
    }
}
