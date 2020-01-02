using System;
using System.Collections.Generic;
using System.Linq;
using Nop.Core;
using Nop.Core.Data;
using Nop.Core.Domain.Messages;
using Nop.Services.Events;

namespace Nop.Services.Messages
{
    /// <summary>
    /// Email account service
    /// </summary>
    public partial class EmailAccountService : IEmailAccountService
    {
        #region Fields

        private readonly IEventPublisher _eventPublisher;
        private readonly IRepository<EmailAccount> _emailAccountRepository;

        #endregion

        #region Ctor

        public EmailAccountService(IEventPublisher eventPublisher,
            IRepository<EmailAccount> emailAccountRepository)
        {
            _eventPublisher = eventPublisher;
            _emailAccountRepository = emailAccountRepository;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Inserts an email account
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        public virtual void InsertEmailAccount(EmailAccount emailAccount)
        {
            if (emailAccount == null)
                throw new ArgumentNullException(nameof(emailAccount));

            emailAccount.Email = CommonHelper.EnsureNotNull(emailAccount.Email);
            emailAccount.DisplayName = CommonHelper.EnsureNotNull(emailAccount.DisplayName);
            emailAccount.Host = CommonHelper.EnsureNotNull(emailAccount.Host);
            emailAccount.Username = CommonHelper.EnsureNotNull(emailAccount.Username);
            emailAccount.Password = CommonHelper.EnsureNotNull(emailAccount.Password);

            emailAccount.Email = emailAccount.Email.Trim();
            emailAccount.DisplayName = emailAccount.DisplayName.Trim();
            emailAccount.Host = emailAccount.Host.Trim();
            emailAccount.Username = emailAccount.Username.Trim();
            emailAccount.Password = emailAccount.Password.Trim();

            emailAccount.Email = CommonHelper.EnsureMaximumLength(emailAccount.Email, 255);
            emailAccount.DisplayName = CommonHelper.EnsureMaximumLength(emailAccount.DisplayName, 255);
            emailAccount.Host = CommonHelper.EnsureMaximumLength(emailAccount.Host, 255);
            emailAccount.Username = CommonHelper.EnsureMaximumLength(emailAccount.Username, 255);
            emailAccount.Password = CommonHelper.EnsureMaximumLength(emailAccount.Password, 255);

            _emailAccountRepository.Insert(emailAccount);

            //event notification
            _eventPublisher.EntityInserted(emailAccount);
        }

        /// <summary>
        /// Updates an email account
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        public virtual void UpdateEmailAccount(EmailAccount emailAccount)
        {
            if (emailAccount == null)
                throw new ArgumentNullException(nameof(emailAccount));

            emailAccount.Email = CommonHelper.EnsureNotNull(emailAccount.Email);
            emailAccount.DisplayName = CommonHelper.EnsureNotNull(emailAccount.DisplayName);
            emailAccount.Host = CommonHelper.EnsureNotNull(emailAccount.Host);
            emailAccount.Username = CommonHelper.EnsureNotNull(emailAccount.Username);
            emailAccount.Password = CommonHelper.EnsureNotNull(emailAccount.Password);

            emailAccount.Email = emailAccount.Email.Trim();
            emailAccount.DisplayName = emailAccount.DisplayName.Trim();
            emailAccount.Host = emailAccount.Host.Trim();
            emailAccount.Username = emailAccount.Username.Trim();
            emailAccount.Password = emailAccount.Password.Trim();

            emailAccount.Email = CommonHelper.EnsureMaximumLength(emailAccount.Email, 255);
            emailAccount.DisplayName = CommonHelper.EnsureMaximumLength(emailAccount.DisplayName, 255);
            emailAccount.Host = CommonHelper.EnsureMaximumLength(emailAccount.Host, 255);
            emailAccount.Username = CommonHelper.EnsureMaximumLength(emailAccount.Username, 255);
            emailAccount.Password = CommonHelper.EnsureMaximumLength(emailAccount.Password, 255);

            _emailAccountRepository.Update(emailAccount);

            //event notification
            _eventPublisher.EntityUpdated(emailAccount);
        }

        /// <summary>
        /// Deletes an email account
        /// </summary>
        /// <param name="emailAccount">Email account</param>
        public virtual void DeleteEmailAccount(EmailAccount emailAccount)
        {
            if (emailAccount == null)
                throw new ArgumentNullException(nameof(emailAccount));

            if (GetAllEmailAccounts().Count == 1)
                throw new NopException("You cannot delete this email account. At least one account is required.");

            _emailAccountRepository.Delete(emailAccount);

            //event notification
            _eventPublisher.EntityDeleted(emailAccount);
        }

        /// <summary>
        /// Gets an email account by identifier
        /// </summary>
        /// <param name="emailAccountId">The email account identifier</param>
        /// <returns>Email account</returns>
        public virtual EmailAccount GetEmailAccountById(int emailAccountId)
        {
            if (emailAccountId == 0)
                return null;

            return _emailAccountRepository.GetById(emailAccountId);
        }

        /// <summary>
        /// Gets all email accounts
        /// </summary>
        /// <returns>Email accounts list</returns>
        public virtual IList<EmailAccount> GetAllEmailAccounts()
        {
            var query = from ea in _emailAccountRepository.Table
                        orderby ea.Id
                        select ea;
            var emailAccounts = query.ToList();
            return emailAccounts;
        }

        #endregion
    }
}