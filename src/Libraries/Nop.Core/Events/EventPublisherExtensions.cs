﻿using System.Threading.Tasks;

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
         public static async Task EntityInserted<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
         {
             await eventPublisher.Publish(new EntityInsertedEvent<T>(entity));
         }

         /// <summary>
         /// Entity updated
         /// </summary>
         /// <typeparam name="T">Entity type</typeparam>
         /// <param name="eventPublisher">Event publisher</param>
         /// <param name="entity">Entity</param>
         public static async Task EntityUpdated<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
         {
             await eventPublisher.Publish(new EntityUpdatedEvent<T>(entity));
         }

         /// <summary>
         /// Entity deleted
         /// </summary>
         /// <typeparam name="T">Entity type</typeparam>
         /// <param name="eventPublisher">Event publisher</param>
         /// <param name="entity">Entity</param>
         public static async Task EntityDeleted<T>(this IEventPublisher eventPublisher, T entity) where T : BaseEntity
         {
             await eventPublisher.Publish(new EntityDeletedEvent<T>(entity));
         }
     }
 }