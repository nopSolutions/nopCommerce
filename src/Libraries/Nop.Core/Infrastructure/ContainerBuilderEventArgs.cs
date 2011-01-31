using System;
using Autofac;

namespace Nop.Core.Infrastructure
{
    public class ContainerBuilderEventArgs : EventArgs
    {
        public ContainerBuilderEventArgs(ContainerBuilder builder)
        {
            this.Builder = builder;
        }

        public ContainerBuilder Builder { get; private set; }
    }
}