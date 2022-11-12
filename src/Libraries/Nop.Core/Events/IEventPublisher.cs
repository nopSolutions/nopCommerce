<<<<<<< HEAD
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
=======
=======
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
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
>>>>>>> 974287325803649b246516d81982b95e372d09b9
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
<<<<<<< HEAD
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
=======
<<<<<<< HEAD
=======
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
>>>>>>> 974287325803649b246516d81982b95e372d09b9
 }