<<<<<<< HEAD
<<<<<<< HEAD
﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Forums;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Boards
{
    public partial record EditForumTopicModel : BaseNopModel
    {
        #region Ctor

        public EditForumTopicModel()
        {
            TopicPriorities = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        public bool IsEdit { get; set; }

        public int Id { get; set; }

        public int ForumId { get; set; }

        public string ForumName { get; set; }

        public string ForumSeName { get; set; }

        public int TopicTypeId { get; set; }

        public EditorType ForumEditor { get; set; }

        public string Subject { get; set; }

        public string Text { get; set; }

        public bool IsCustomerAllowedToSetTopicPriority { get; set; }

        public IEnumerable<SelectListItem> TopicPriorities { get; set; }

        public bool IsCustomerAllowedToSubscribe { get; set; }

        public bool Subscribed { get; set; }

        public bool DisplayCaptcha { get; set; }

        #endregion
    }
=======
=======
=======
<<<<<<< HEAD
﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Forums;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Boards
{
    public partial record EditForumTopicModel : BaseNopModel
    {
        #region Ctor

        public EditForumTopicModel()
        {
            TopicPriorities = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        public bool IsEdit { get; set; }

        public int Id { get; set; }

        public int ForumId { get; set; }

        public string ForumName { get; set; }

        public string ForumSeName { get; set; }

        public int TopicTypeId { get; set; }

        public EditorType ForumEditor { get; set; }

        public string Subject { get; set; }

        public string Text { get; set; }

        public bool IsCustomerAllowedToSetTopicPriority { get; set; }

        public IEnumerable<SelectListItem> TopicPriorities { get; set; }

        public bool IsCustomerAllowedToSubscribe { get; set; }

        public bool Subscribed { get; set; }

        public bool DisplayCaptcha { get; set; }

        #endregion
    }
=======
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
>>>>>>> 974287325803649b246516d81982b95e372d09b9
﻿using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Core.Domain.Forums;
using Nop.Web.Framework.Models;

namespace Nop.Web.Models.Boards
{
    public partial record EditForumTopicModel : BaseNopModel
    {
        #region Ctor

        public EditForumTopicModel()
        {
            TopicPriorities = new List<SelectListItem>();
        }

        #endregion

        #region Properties

        public bool IsEdit { get; set; }

        public int Id { get; set; }

        public int ForumId { get; set; }

        public string ForumName { get; set; }

        public string ForumSeName { get; set; }

        public int TopicTypeId { get; set; }

        public EditorType ForumEditor { get; set; }

        public string Subject { get; set; }

        public string Text { get; set; }

        public bool IsCustomerAllowedToSetTopicPriority { get; set; }

        public IEnumerable<SelectListItem> TopicPriorities { get; set; }

        public bool IsCustomerAllowedToSubscribe { get; set; }

        public bool Subscribed { get; set; }

        public bool DisplayCaptcha { get; set; }

        #endregion
    }
<<<<<<< HEAD
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
=======
<<<<<<< HEAD
=======
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
>>>>>>> 974287325803649b246516d81982b95e372d09b9
}