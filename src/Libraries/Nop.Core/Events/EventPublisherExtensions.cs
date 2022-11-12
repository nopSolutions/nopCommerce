<<<<<<< HEAD
<<<<<<< HEAD
﻿﻿using System.Threading.Tasks;

 namespace Nop.Core.Events
 {
     /// <summary>
     /// Event publisher extensions
     /// </summary>
     public static class EventPublisherExtensions
     {
         /// <summary>
         /// Entity inserted
         /// </summary>
         /// <typeparam name="T">Entity type</typeparam>
         /// <param name="eventPublisher">Event publisher</param>
         /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
         public static async Task EntityInsertedAsync<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
         {
             await eventPublisher.PublishAsync(new EntityInsertedEvent<T>(entity));
         }

         /// <summary>
         /// Entity updated
         /// </summary>
         /// <typeparam name="T">Entity type</typeparam>
         /// <param name="eventPublisher">Event publisher</param>
         /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
         public static async Task EntityUpdatedAsync<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
         {
             await eventPublisher.PublishAsync(new EntityUpdatedEvent<T>(entity));
         }

         /// <summary>
         /// Entity deleted
         /// </summary>
         /// <typeparam name="T">Entity type</typeparam>
         /// <param name="eventPublisher">Event publisher</param>
         /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
         public static async Task EntityDeletedAsync<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
         {
             await eventPublisher.PublishAsync(new EntityDeletedEvent<T>(entity));
         }
     }
=======
=======
=======
<<<<<<< HEAD
>>>>>>> 974287325803649b246516d81982b95e372d09b9
﻿﻿using System.Threading.Tasks;

 namespace Nop.Core.Events
 {
     /// <summary>
     /// Event publisher extensions
     /// </summary>
     public static class EventPublisherExtensions
     {
         /// <summary>
         /// Entity inserted
         /// </summary>
         /// <typeparam name="T">Entity type</typeparam>
         /// <param name="eventPublisher">Event publisher</param>
         /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
         public static async Task EntityInsertedAsync<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
         {
             await eventPublisher.PublishAsync(new EntityInsertedEvent<T>(entity));
         }

         /// <summary>
         /// Entity updated
         /// </summary>
         /// <typeparam name="T">Entity type</typeparam>
         /// <param name="eventPublisher">Event publisher</param>
         /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
         public static async Task EntityUpdatedAsync<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
         {
             await eventPublisher.PublishAsync(new EntityUpdatedEvent<T>(entity));
         }

         /// <summary>
         /// Entity deleted
         /// </summary>
         /// <typeparam name="T">Entity type</typeparam>
         /// <param name="eventPublisher">Event publisher</param>
         /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
         public static async Task EntityDeletedAsync<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
         {
             await eventPublisher.PublishAsync(new EntityDeletedEvent<T>(entity));
         }
     }
<<<<<<< HEAD
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
=======
=======
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
﻿﻿using System.Threading.Tasks;

 namespace Nop.Core.Events
 {
     /// <summary>
     /// Event publisher extensions
     /// </summary>
     public static class EventPublisherExtensions
     {
         /// <summary>
         /// Entity inserted
         /// </summary>
         /// <typeparam name="T">Entity type</typeparam>
         /// <param name="eventPublisher">Event publisher</param>
         /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
         public static async Task EntityInsertedAsync<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
         {
             await eventPublisher.PublishAsync(new EntityInsertedEvent<T>(entity));
         }

         /// <summary>
         /// Entity updated
         /// </summary>
         /// <typeparam name="T">Entity type</typeparam>
         /// <param name="eventPublisher">Event publisher</param>
         /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
         public static async Task EntityUpdatedAsync<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
         {
             await eventPublisher.PublishAsync(new EntityUpdatedEvent<T>(entity));
         }

         /// <summary>
         /// Entity deleted
         /// </summary>
         /// <typeparam name="T">Entity type</typeparam>
         /// <param name="eventPublisher">Event publisher</param>
         /// <param name="entity">Entity</param>
        /// <returns>A task that represents the asynchronous operation</returns>
         public static async Task EntityDeletedAsync<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
         {
             await eventPublisher.PublishAsync(new EntityDeletedEvent<T>(entity));
         }
     }
<<<<<<< HEAD
=======
>>>>>>> 174426a8e1a9c69225a65c26a93d9aa871080855
>>>>>>> cf758b6c548f45d8d46cc74e51253de0619d95dc
>>>>>>> 974287325803649b246516d81982b95e372d09b9
 }