<<<<<<< HEAD
﻿﻿using System.Threading.Tasks;

 namespace Nop.Core.Events
 {
     /// <summary>
     /// Represents an event publisher
     /// </summary>
     public partial interface IEventPublisher
     {
         /// <summary>
         /// Publish event to consumers
         /// </summary>
         /// <typeparam name="TEvent">Type of event</typeparam>
         /// <param name="event">Event object</param>
         Task PublishAsync<TEvent>(TEvent @event);
     }
=======
﻿﻿using System.Threading.Tasks;

 namespace Nop.Core.Events
 {
     /// <summary>
     /// Represents an event publisher
     /// </summary>
     public partial interface IEventPublisher
     {
         /// <summary>
         /// Publish event to consumers
         /// </summary>
         /// <typeparam name="TEvent">Type of event</typeparam>
         /// <param name="event">Event object</param>
         Task PublishAsync<TEvent>(TEvent @event);
     }
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
 }