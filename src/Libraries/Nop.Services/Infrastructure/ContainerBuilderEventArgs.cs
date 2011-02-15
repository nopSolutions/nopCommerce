using System;
using Autofac;

namespace Nop.Services.Infrastructure
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