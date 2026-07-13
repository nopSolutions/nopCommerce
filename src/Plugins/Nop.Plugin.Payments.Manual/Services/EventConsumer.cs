using FluentValidation.Validators;
using Newtonsoft.Json;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Security;
using Nop.Data;
using Nop.Plugin.Payments.Manual.Validators;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Events;
using Nop.Services.Security;
using Nop.Web.Framework.Events;

namespace Nop.Plugin.Payments.Manual.Services;

/// <summary>
/// Represents plugin event consumer
/// </summary>
public class EventConsumer : IConsumer<SecuritySettingsChangedEvent>,
    IConsumer<ClientModelValidatorsCreatedEvent>,
    IConsumer<OrderPlacedEvent>
{
    #region Fields

    private readonly ICustomerService _customerService;
    private readonly IEncryptionService _encryptionService;
    private readonly IGenericAttributeService _genericAttributeService;
    private readonly IRepository<GenericAttribute> _genericAttributeRepository;
    private readonly IRepository<Order> _orderRepository;

    #endregion

    #region Ctor

    public EventConsumer(ICustomerService customerService,
        IEncryptionService encryptionService,
        IGenericAttributeService genericAttributeService,
        IRepository<GenericAttribute> genericAttributeRepository,
        IRepository<Order> orderRepository)
    {
        _customerService = customerService;
        _encryptionService = encryptionService;
        _genericAttributeService = genericAttributeService;
        _genericAttributeRepository = genericAttributeRepository;
        _orderRepository = orderRepository;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(SecuritySettingsChangedEvent eventMessage)
    {
        if (string.Equals(eventMessage.OldEncryptionPrivateKey, eventMessage.SecuritySettings.EncryptionKey))
            return;

        var items = _orderRepository.Table.Join(_genericAttributeRepository.Table,
                order => new { EntityId = order.Id, KeyGroup = nameof(Order), Key = nameof(CreditCardInfo) },
                attr => new { attr.EntityId, attr.KeyGroup, attr.Key }, (order, attr) => new { order, attr })
            .Where(t => !t.order.Deleted)
            .Select(t => new { t.order, t.attr });

        foreach (var item in await items.ToListAsync())
        {
            var json = item.attr.Value;

            if (string.IsNullOrEmpty(json))
                continue;

            var creditCardInfo = JsonConvert.DeserializeObject<CreditCardInfo>(json);

            var decryptedCardType = _encryptionService.DecryptText(creditCardInfo.CardType, eventMessage.OldEncryptionPrivateKey);
            var decryptedCardName = _encryptionService.DecryptText(creditCardInfo.CardName, eventMessage.OldEncryptionPrivateKey);
            var decryptedCardNumber = _encryptionService.DecryptText(creditCardInfo.CardNumber, eventMessage.OldEncryptionPrivateKey);
            var decryptedMaskedCreditCardNumber = _encryptionService.DecryptText(creditCardInfo.MaskedCreditCardNumber, eventMessage.OldEncryptionPrivateKey);
            var decryptedCardCvv2 = _encryptionService.DecryptText(creditCardInfo.CardCvv2, eventMessage.OldEncryptionPrivateKey);
            var decryptedCardExpirationMonth = _encryptionService.DecryptText(creditCardInfo.CardExpirationMonth, eventMessage.OldEncryptionPrivateKey);
            var decryptedCardExpirationYear = _encryptionService.DecryptText(creditCardInfo.CardExpirationYear, eventMessage.OldEncryptionPrivateKey);

            var encryptedCardType = _encryptionService.EncryptText(decryptedCardType, eventMessage.SecuritySettings.EncryptionKey);
            var encryptedCardName = _encryptionService.EncryptText(decryptedCardName, eventMessage.SecuritySettings.EncryptionKey);
            var encryptedCardNumber = _encryptionService.EncryptText(decryptedCardNumber, eventMessage.SecuritySettings.EncryptionKey);
            var encryptedMaskedCreditCardNumber = _encryptionService.EncryptText(decryptedMaskedCreditCardNumber, eventMessage.SecuritySettings.EncryptionKey);
            var encryptedCardCvv2 = _encryptionService.EncryptText(decryptedCardCvv2, eventMessage.SecuritySettings.EncryptionKey);
            var encryptedCardExpirationMonth = _encryptionService.EncryptText(decryptedCardExpirationMonth, eventMessage.SecuritySettings.EncryptionKey);
            var encryptedCardExpirationYear = _encryptionService.EncryptText(decryptedCardExpirationYear, eventMessage.SecuritySettings.EncryptionKey);

            creditCardInfo.CardType = encryptedCardType;
            creditCardInfo.CardName = encryptedCardName;
            creditCardInfo.CardNumber = encryptedCardNumber;
            creditCardInfo.MaskedCreditCardNumber = encryptedMaskedCreditCardNumber;
            creditCardInfo.CardCvv2 = encryptedCardCvv2;
            creditCardInfo.CardExpirationMonth = encryptedCardExpirationMonth;
            creditCardInfo.CardExpirationYear = encryptedCardExpirationYear;

            json = JsonConvert.SerializeObject(creditCardInfo);

            await _genericAttributeService.SaveAttributeAsync(item.order, nameof(CreditCardInfo), json);
        }
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public Task HandleEventAsync(ClientModelValidatorsCreatedEvent eventMessage)
    {
        eventMessage.AddClientModelValidator(typeof(ICreditCardValidator),
            new CreditCardClientValidator(eventMessage.Rule, eventMessage.Component));

        return Task.CompletedTask;
    }

    /// <summary>
    /// Handle event
    /// </summary>
    /// <param name="eventMessage">Event</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
    {
        var currentCustomer = await _customerService.GetCustomerByIdAsync(eventMessage.Order.CustomerId);
        var json = await _genericAttributeService.GetAttributeAsync<string>(currentCustomer, nameof(CreditCardInfo));

        if (string.IsNullOrEmpty(json))
            return;

        await _genericAttributeService.SaveAttributeAsync(eventMessage.Order, nameof(CreditCardInfo), json);
        await _genericAttributeService.SaveAttributeAsync<string>(currentCustomer, nameof(CreditCardInfo), null);
    }

    #endregion
}