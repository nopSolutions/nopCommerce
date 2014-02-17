using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Media;
using Nop.Services.Configuration;
using Nop.Services.Events;
using Nop.Services.Logging;
using Nop.Services.Seo;

namespace Nop.Services.Media
{
    /// <summary>
    /// Resize types
    /// </summary>
    public enum ResizeType
    {
        LongestSide,
        Width,
        Height
    }
}
