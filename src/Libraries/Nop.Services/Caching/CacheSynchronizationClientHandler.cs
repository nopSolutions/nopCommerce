using System;
using System.Collections.Generic;
using System.Text;
using DotNetty.Buffers;
using DotNetty.Transport.Channels;

namespace Nop.Services.Caching
{
    /// <summary>
    /// Client handler for cache synchronization.
    /// </summary>
    internal sealed class CacheSynchronizationClientHandler : ChannelHandlerAdapter
    {
        private Action<string> _handleReturnMessage;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheSynchronizationClientHandler"/> class.
        /// </summary>
        /// <param name="handleReturnMessage">The delegate to handle the return message.</param>
        public CacheSynchronizationClientHandler(Action<string> handleReturnMessage)
        {
            _handleReturnMessage = handleReturnMessage;
        }

        /// <summary>
        /// Read a reply from the server.
        /// </summary>
        /// <param name="context">The channel handler context.</param>
        /// <param name="message">The received message.</param>
        public override void ChannelRead(IChannelHandlerContext context, object message)
        {
            if (message is IByteBuffer byteBuffer)
            {
                var str = byteBuffer.ToString(Encoding.UTF8);
                _handleReturnMessage(str);
            }
        }

        /// <summary>
        /// Runs when the read on the channel is complete.
        /// </summary>
        /// <param name="context">The channel handler context.</param>
        public override void ChannelReadComplete(IChannelHandlerContext context)
        {
            context.Flush();
        }

        /// <summary>
        /// Runs when an exception was caught.
        /// </summary>
        /// <param name="context">The channel handler context.</param>
        /// <param name="exception">The exception that occured.</param>
        public override void ExceptionCaught(IChannelHandlerContext context, Exception exception)
        {
            context.CloseAsync();
        }
    }
}
