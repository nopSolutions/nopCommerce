using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FluentValidation.Attributes;
using Nop.Web.Framework.Mvc;
using Nop.Web.Validators.Blogs;
using Nop.Web.Validators.Install;

namespace Nop.Web.Models.Install
{
    [Validator(typeof(InstallValidator))]
    public class InstallModel : BaseNopModel
    {
        [AllowHtml]
        public string AdminEmail { get; set; }
        [AllowHtml]
        [DataType(DataType.Password)]
        public string AdminPassword { get; set; }
        [AllowHtml]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        [AllowHtml]
        public string DatabaseConnectionString { get; set; }
        public string DataProvider { get; set; }

        public bool InstallSampleData { get; set; }
    }
}