// Copyright [2011] [PagSeguro Internet Ltda.]
//
//   Licensed under the Apache License, Version 2.0 (the "License");
//   you may not use this file except in compliance with the License.
//   You may obtain a copy of the License at
//
//       http://www.apache.org/licenses/LICENSE-2.0
//
//   Unless required by applicable law or agreed to in writing, software
//   distributed under the License is distributed on an "AS IS" BASIS,
//   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//   See the License for the specific language governing permissions and
//   limitations under the License.

using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Diagnostics;

namespace Uol.PagSeguro
{
    static internal class PagSeguroTrace
    {
        private enum Level
        {
            None = 0,
            Info = 1,
            Warn = 2,
            Error = 3
        };

        static private string FormatMessage(Level level, string message)
        {
            return String.Format(
                CultureInfo.InvariantCulture,
                "{0} {1}: {2}",
                DateTime.Now,
                level,
                message);
        }

        static public void Info(string message)
        {
            Trace.TraceInformation(FormatMessage(Level.Info, message));
        }

        static public void Warn(string message)
        {
            Trace.TraceError(FormatMessage(Level.Warn, message));
        }

        static public void Error(string message)
        {
            Trace.TraceError(FormatMessage(Level.Error, message));
        }
    }
}
