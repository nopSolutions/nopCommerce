var CheckoutManager = {
  urls: null,

  state: {
    billingAddressId: null,
    shippingAddressId: null,
    shippingOption: null,
    paymentMethodSystemName: null,
    shipToSameAddress: false,
    pickupInStore: false,
  },

  requirements: {
    shippingRequired: false,
    shippingMethodRequired: false,
    paymentRequired: false,
    paymentInfoRequired: false,
  },

  // Connect each "state component" with the sections that depends on it.
  dependencyGraph: {
    billingAddressId: ['paymentMethod', 'paymentInfo', 'confirmOrder'],
    shippingAddressId: ['shippingMethod', 'confirmOrder'],
    shippingOption: ['confirmOrder'],
    paymentMethodSystemName: ['paymentInfo', 'confirmOrder'],
    shipToSameAddress: ['shippingAddress', 'shippingMethod'],
    pickupInStore: ['shippingAddress', 'shippingMethod', 'confirmOrder'],
    pickupPoint: ['shippingAddress', 'shippingMethod', 'confirmOrder']
  },

  domIds: null,

  // Requirements determines visibility.
  activationRules: new Map([
    ['billingAddress', req => true],
    ['shippingAddress', req => req.shippingRequired],
    ['shippingMethod', req => req.shippingMethodRequired],
    ['paymentMethod', req => req.paymentRequired],
    ['paymentInfo', req => req.paymentInfoRequired],
    ['confirmOrder', req => true]
  ]),

  // A set deduplicates automatically so adding the same section key twice
  // won't cause any problems.
  pendingRenders: new Set(),

  // Connect each "section" with its render method.
  renderMap: null,

  domIds: {
    billingAddress: 'billing-address-section-content',
    shippingAddress: 'shipping-address-section-content',
    shippingMethod: 'shipping-methods-section-content',
    paymentMethod: 'payment-methods-section-content',
    paymentInfo: 'payment-info-section-content',
    confirmOrder: 'confirm-order-section-content'
  },

  config: {
    isCaptchaEnabled: false,
    isReCaptchaV3: false,
    recaptchaPublicKey: null,
  },

  // Bootstrapping

  init: async function (urls) {
    this.urls = urls;

    this.renderMap = new Map([
      ['billingAddress', () => this.renderBillingAddress()],
      ['shippingAddress', () => this.renderShippingAddress()],
      ['shippingMethod', () => this.renderShippingMethods()],
      ['paymentMethod', () => this.renderPaymentMethods()],
      ['paymentInfo', () => this.renderPaymentInfo()],
      ['confirmOrder', () => this.renderConfirmOrder()],
    ]);

    // TODO: Handle failure.
    const [config, checkout] = await Promise.all([
      fetch(this.urls.getCheckoutConfiguration).then(r => r.json()),
      fetch(this.urls.getCheckoutState).then(r => r.json())
    ]);

    // TODO: Fix naming. Either send camelCase from the sever or use PascalCase everywhere here.

    this.state.billingAddressId = checkout.state.BillingAddressId;
    this.state.shippingAddressId = checkout.state.ShippingAddressId;
    this.state.shippingOption = checkout.state.ShippingOption;
    this.state.paymentMethodSystemName = checkout.state.PaymentMethodSystemName;
    this.state.shipToSameAddress = checkout.state.ShipToSameAddress;

    this.requirements.shippingRequired = checkout.requirements.ShippingRequired;
    this.requirements.shippingMethodRequired = checkout.requirements.ShippingMethodRequired;
    this.requirements.paymentRequired = checkout.requirements.PaymentRequired;
    this.requirements.paymentInfoRequired = checkout.requirements.PaymentInfoRequired;

    this.config.isCaptchaEnabled = config.IsCaptchaEnabled;
    this.config.isReCaptchaV3 = config.IsReCaptchaV3;
    this.config.recaptchaPublicKey = config.RecaptchaPublicKey;
    this.config.shippingRequired = config.ShippingRequired;

    //this.initialActivation();
    await this.initialRender();
  },

  initialRender: async function () {
    for (const section of this.renderMap.keys()) {
      const isActive = this.activationRules.get(section)?.(this.requirements);

      if (isActive) {
        this.pendingRenders.add(section);
      } else {
        this.hideSection(section);
      }
    }

    await this.flushRenders();
  },

  // State management

  updateLocalRequirements: function (newRequirements) {
    const old = this.requirements;
    this.requirements = newRequirements;

    this.evaluateSections(old, newRequirements);
  },

  evaluateSections: function (oldRequirements, newRequirements) {
    for (const section of this.renderMap.keys()) {

      const rule = this.activationRules.get(section);
      const wasActive = rule?.(oldRequirements);
      const isActive = rule?.(newRequirements);

      if (wasActive && !isActive) {
        this.hideSection(section);
      }

      if (!wasActive && isActive) {
        this.showSection(section);
        this.pendingRenders.add(section);
      }
    }
  },

  updateLocalState: async function(newState) {
    for (const [key, value] of Object.entries(newState)) {
      if (this.state[key] === value) {
        continue;
      }

      this.state[key] = value;
      this.scheduleRenderingOfDependents(key);
    }

    await this.flushRenders();
  },

  scheduleRenderingOfDependents: function (stateKey) {
    const affectedSections = this.dependencyGraph[stateKey] ?? [];

    for (const section of affectedSections) {
      const isActive = this.activationRules.get(section)?.(this.requirements);
      if (isActive) {
        this.showSection(section);
        this.pendingRenders.add(section);
      } else {
        this.hideSection(section);
      }
    }
  },

  flushRenders: async function () {
    if (this.pendingRenders.size === 0) {
      return;
    }

    for (const [key, render] of this.renderMap) {
      if (this.pendingRenders.has(key)) {
        await render();
      }
    }

    this.pendingRenders.clear();
  },

  hideSection: function (sectionKey) {
    const id = this.domIds[sectionKey];
    const section = document.getElementById(id);
    if (!section) return;

    section.classList.add('hidden');
  },

  showSection: function (sectionKey) {
    const id = this.domIds[sectionKey];
    const section = document.getElementById(id);
    if (!section) return;

    section.classList.remove('hidden');
  },

  // Render methods (do not change state.)

  renderBillingAddress: async function () {
    WaitingManager.begin('billing-address');

    try {
      const html = await fetch(this.urls.renderBillingAddress).then(r => r.text());
      document.getElementById('billing-address-section-content').innerHTML = html;

      this.bindBillingAddressEvents();
    }
    catch {
      this.ajaxFailure();
    }
    finally {
      WaitingManager.end('billing-address');
    }
  },

  renderShippingAddress: async function () {
    WaitingManager.begin('shipping-address');

    try {
      const html = await fetch(this.urls.renderShippingAddress).then(r => r.text());
      document.getElementById('shipping-address-section-content').innerHTML = html;

      this.bindShippingAddressEvents();
    }
    catch {
      this.ajaxFailure();
    }
    finally {
      WaitingManager.end('shipping-address');
    }
  },

  renderShippingMethods: async function () {
    WaitingManager.begin('shipping-methods');

    try {
      const html = await fetch(this.urls.renderShippingMethods).then(r => r.text());
      document.getElementById('shipping-methods-section-content').innerHTML = html;

      this.bindShippingMethodEvents();
    }
    catch {
      this.ajaxFailure();
    }
    finally {
      WaitingManager.end('shipping-methods');
    }
  },

  renderPaymentMethods: async function () {
    WaitingManager.begin('payment-methods');

    try {
      const html = await fetch(this.urls.renderPaymentMethods).then(r => r.text());
      document.getElementById('payment-methods-section-content').innerHTML = html;

      this.bindPaymentMethodEvents();
    }
    catch {
      this.ajaxFailure();
    }
    finally {
      WaitingManager.end('payment-methods');
    }
  },

  renderPaymentInfo: async function () {
    WaitingManager.begin('payment-info');

    try {
      const html = await fetch(this.urls.renderPaymentInfo).then(r => r.text());
      document.getElementById('payment-info-section-content').innerHTML = html;
    }
    finally {
      WaitingManager.end('payment-info');
    }
  },

  renderConfirmOrder: async function () {
    WaitingManager.begin('confirm-order');

    try {
      const html = await fetch(this.urls.renderConfirmOrder).then(r => r.text());
      document.getElementById('confirm-order-section-content').innerHTML = html;
    }
    catch {
      this.ajaxFailure();
    }
    finally {
      WaitingManager.end('confirm-order');
    }
  },

  // Event binding

  bindBillingAddressEvents: function () {
    document
      .querySelector('input[type="checkbox"][name="ShipToSameAddress"]')
      ?.addEventListener('change', async e => {
        await this.updateCheckoutState({ shipToSameAddress: e.target.checked }, this.urls.toggleShipToSameAddress, 'billing-address');
      });

    document
      .getElementById('billing-address-select')
      ?.addEventListener('change', async e => {
        // TODO: Explain.
        const id = e.target.value || null;

        await this.updateCheckoutState({ billingAddressId: id }, this.urls.selectBillingAddress, 'billing-address');
      });
  },

  bindShippingAddressEvents: function () {
    document
      .getElementById('shipping-address-select')
      ?.addEventListener('change', async e => {
        const id = e.target.value || null;

        await this.updateCheckoutState({ shippingAddressId: id }, this.urls.selectShippingAddress, 'shipping-address');
      });

    document
      .querySelector('input[type="checkbox"][name="PickupInStore"]')
      ?.addEventListener('change', async e => {
        await this.updateCheckoutState({ pickupInStore: e.target.checked }, this.urls.togglePickupInStore, 'shipping-address');
      });

    document
      .getElementById('pickup-points-select')
      ?.addEventListener('change', async e => {
        const id = e.target.value || null;

        await this.updateCheckoutState({ pickupPoint: id }, this.urls.selectPickupPoint, 'shipping-address');
      });
  },

  bindShippingMethodEvents: function () {
    document
      .querySelectorAll('#shipping-method-block input[type="radio"][name="shippingoption"]')
      .forEach(radio => {
        radio.addEventListener('change', async e => {
          if (e.target.checked) {
            await this.updateCheckoutState({ shippingOption: e.target.value }, this.urls.selectShippingMethod, 'shipping-method');
          }
        });
      });
  },

  bindPaymentMethodEvents: function () {
    document
      .querySelectorAll('#payment-method-block input[type="radio"][name="paymentmethod"]')
      .forEach(radio => {
        radio.addEventListener('change', async e => {
          if (e.target.checked) {
            await this.updateCheckoutState({ paymentMethodSystemName: e.target.value }, this.urls.selectPaymentMethod, 'payment-method');
          }
        });
      });
  },

  // Order confirmation

  confirmOrder: async function () {
    WaitingManager.begin('confirm-button');

    var termOfServiceOk = true;

    // This element could appear in the confirm order section.
    if ($('#termsofservice').length > 0) {
      if (!$('#termsofservice').is(':checked')) {
        $("#terms-of-service-warning-box").dialog();
        termOfServiceOk = false;
      } else {
        termOfServiceOk = true;
      }
    }

    if (termOfServiceOk) {
      var form = $('#co-payment-info-form').serialize();

      if (this.isCaptchaEnabled) {
        var captchaTok = await this.getCaptchaToken('OpcConfirmOrder');
        form['g-recaptcha-response'] = captchaTok;
      }

      addAntiForgeryToken(form);
      $.ajax({
        cache: false,
        url: this.urls.confirmOrder,
        data: form,
        type: "POST",
        success: this.handleConfirmationSuccess,
        complete: WaitingManager.end('confirm-button'),
        error: this.ajaxFailure
      });
    } else {
      // TODO: Handle this.
      return false;
    }
  },

  getCaptchaToken: async function (action) {
    var recaptchaToken = '';

    if (this.isReCaptchaV3) {
      grecaptcha.ready(() => {
        grecaptcha.execute(this.recaptchaPublicKey, { action: action }).then((token) => {
          recaptchaToken = token;
        });
      });
      while (recaptchaToken == '') {
        await new Promise(t => setTimeout(t, 100));
      }
    } else {
      recaptchaToken = $(this.div).find('.captcha-box textarea[name="g-recaptcha-response"]').val();
    }

    return recaptchaToken;
  },

  handleConfirmationSuccess: function (response) {
    if (response.error) {
      if (typeof response.message === 'string') {
        alert(response.message);
      } else {
        alert(response.message.join("\n"));
      }

      return false;
    }

    if (response.redirect) {
      location.href = response.redirect;
      return;
    }
    if (response.success) {
      window.location = CheckoutManager.urls.confirmSuccess;
    }
  },

  // Address deletion

  deleteAddress: async function (addressId) {
    try {
      const token = document.querySelector(
        'input[name="__RequestVerificationToken"][value]:not([value=""])'
      )?.value;

      const response = await fetch(this.urls.deleteAddress, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'RequestVerificationToken': token
        },
        body: addressId
      });

      if (!response.ok) {
        throw new Error('HTTP error');
      }

      var result = await response.json();
      await this.resyncAddresses(result.state);
    }
    catch(e) {
      this.ajaxFailure();
    }
  },

  resyncAddresses: async function (state) {
    // Force re-rendering the addresses.

    this.pendingRenders.add('billingAddress');
    this.pendingRenders.add('shippingAddress');

    await this.updateLocalState({
      billingAddressId: state.BillingAddressId,
      shippingAddressId: state.ShippingAddressId,
      shippingOption: state.ShippingOption,
      paymentMethodSystemName: state.PaymentMethodSystemName,
      shipToSameAddress: state.ShipToSameAddress,
      pickupInStore: state.PickupInStore,
      pickupPoint: state.PickupPoint
    });
  },

  // Network requests

  updateCheckoutState: async function (patchRequest, url, waitingElementKey) {
    WaitingManager.begin(waitingElementKey);

    try {
      const token = document.querySelector(
        'input[name="__RequestVerificationToken"][value]:not([value=""])'
      )?.value;

      const response = await fetch(url, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          'RequestVerificationToken': token
        },
        body: JSON.stringify(patchRequest)
      });

      if (!response.ok) {
        throw new Error('HTTP error');
      }

      const result = await response.json();

      if (result.requirements) {
        this.updateLocalRequirements({
          shippingRequired: result.requirements.ShippingRequired,
          shippingMethodRequired: result.requirements.ShippingMethodRequired,
          paymentRequired: result.requirements.PaymentRequired,
          paymentInfoRequired: result.requirements.PaymentInfoRequired
        });
      }

      if (result.state) {
        await this.updateLocalState({
          billingAddressId: result.state.BillingAddressId,
          shippingAddressId: result.state.ShippingAddressId,
          shippingOption: result.state.ShippingOption,
          paymentMethodSystemName: result.state.PaymentMethodSystemName,
          shipToSameAddress: result.state.ShipToSameAddress,
          pickupInStore: result.state.PickupInStore,
          pickupPoint: result.state.PickupPoint
        });
      }
    }
    catch (e) {
      this.ajaxFailure();
    }
    finally {
      WaitingManager.end(waitingElementKey);
    }
  },

  // More like 'fetchFailure' in the current implementation.
  ajaxFailure: function () {
    location.href = this.urls.failureUrl;
  },
}

var WaitingManager = {
  waiting: new Set(),

  begin: function (sectionKey) {
    if (this.waiting.has(sectionKey)) return;

    this.waiting.add(sectionKey);

    const section = document.getElementById(`${sectionKey}-section`);
    if (!section) return;

    section.classList.add('is-waiting');

    const waitingEl = section.querySelector('.waiting-indicator');
    if (waitingEl) {
      waitingEl.classList.remove('hidden');
    }

    this.disableInputs(section, true);
  },

  end: function (sectionKey) {
    if (!this.waiting.has(sectionKey)) return;

    this.waiting.delete(sectionKey);

    const section = document.getElementById(`${sectionKey}-section`);
    if (!section) return;

    section.classList.remove('is-waiting');

    const waitingEl = section.querySelector('.waiting-indicator');
    if (waitingEl) {
      waitingEl.classList.add('hidden');
    }

    this.disableInputs(section, false);
  },

  disableInputs: function (container, disabled) {
    container
      .querySelectorAll('input, select, textarea, button')
      .forEach(el => {
        el.disabled = disabled;
      });
  }
}

var AddressEditor = {
  // In the OPC implementation, the form represented the whole billing section.
  // However, in our current implementation, it only represents the form that appears
  // inside the modal.
  form: false,
  addressType: '',
  urls: null,

  // Used in public.countryselect.js
  selectedStateId: 0,

  init: function (form, urls) {
    this.urls = urls;
    this.form = form;
  },

  editAddress: async function (addressId, addressType) {
    this.addressType = addressType;

    const params = new URLSearchParams({
      addressId: addressId,
      addressType: addressType
    });

    const response = await fetch(this.urls.renderEditor + `?${params.toString()}`, {
      method: 'GET',
      headers: { 'Content-Type': 'application/json' },
      credentials: 'same-origin',
    });

    const html = await response.text();

    document.getElementById('address-editor-content').innerHTML = html;

    this.initializeCountrySelect();

    $('#edit-address-form').dialog({ width: 700 });
  },

  saveEditAddress: function () {
    var dataArray = $(this.form).serializeArray();
    var data = {};
    dataArray.forEach(item => data[item.name] = item.value);

    const tokenInput = document.querySelector(
      'input[name="__RequestVerificationToken"][value]:not([value=""])'
    );
    data.__RequestVerificationToken = tokenInput.value;

    $.ajax({
      cache: false,
      url: (this.addressType === 'billing' ? this.urls.saveBillingAddress : this.urls.saveShippingAddress),
      data: data,
      type: "POST",
      success: async function (result) {
        if (result.error) {
          alert(result.message);
          return false;
        } else {
          await CheckoutManager.resyncAddresses(result.state);

          AddressEditor.closeModal();
        }
      },
      error: CheckoutManager.ajaxFailure
    });
  },

  resetAddressForm: function () {
    $(':input', '#edit-address-form')
      .not(':button, :submit, :reset, :hidden')
      .removeAttr('checked').removeAttr('selected')
    $(':input', '#edit-address-form')
      .not(':checkbox, :radio, select')
      .val('');

    $('.address-id', '#edit-address-form').val('0');
    $('select option[value="0"]', '#edit-address-form').prop('selected', true);
  },

  // Modal methods

  showModal: function () {
    $('#edit-address-form').dialog({ width: 700 });
  },

  closeModal: function () {
    $('#edit-address-form').dialog('close');
  },

  // Utilities

  initializeCountrySelect: function () {
    if ($('#edit-address-form').has('select[data-trigger="country-select"]')) {
      $('#edit-address-form select[data-trigger="country-select"]').countrySelect();
    }
  },

  setSelectedStateId: function (id) {
    this.selectedStateId = id;
  },
};