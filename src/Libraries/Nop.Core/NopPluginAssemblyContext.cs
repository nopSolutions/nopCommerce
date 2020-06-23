using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.Loader;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Nop.Core
{
    public class NopPluginAssemblyContext : AssemblyLoadContext
    {
        public string BasePath { get; set; }

        public NopPluginAssemblyContext() : base(false)
        {
            this.Resolving += ResolvingHandler;
        }

        public Assembly ResolvingHandler(AssemblyLoadContext context, AssemblyName assemblyName)
        {
            try
            {
                Assembly loadedAssembly = this.LoadFromAssemblyPath(Path.Combine(BasePath, assemblyName.Name + ".dll"));
                return loadedAssembly;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }    

        }
    }
}
