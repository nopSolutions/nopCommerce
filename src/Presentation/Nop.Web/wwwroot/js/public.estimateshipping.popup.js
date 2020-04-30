let EstimateShippingPopUp = {
    jqXHR: false,
    delayTimer: false,
    form: false,
    selectedShippingOption: false,
    urlFactory: false,
    handlers: false,
    errorMessageBoxSelector: '#estimate-shipping-popup .message-failure', 
    countryErrorMessage: '',
    zipPostalCodeErrorMessage: '',
    noShippingOptionsErrorMessage: '',
    isShown: false,

    init: function (form, urlFactory, handlers, localizedData) {
        this.form = form;
        this.urlFactory = urlFactory;
        this.handlers = handlers;
        this.countryErrorMessage = localizedData.countryErrorMessage;
        this.zipPostalCodeErrorMessage = localizedData.zipPostalCodeErrorMessage;
        this.noShippingOptionsErrorMessage = localizedData.NoShippingOptions;

        let self = this;

        $('#apply-shipping-button').on('click', function () {
            let option = self.getActiveShippingOption();
            if (option && option.provider && option.price) {
              self.selectShippingOption(option);
              self.closePopup();
            }
        });

        $('#open-estimate-shipping-popup').magnificPopup({
            type: 'inline',
            removalDelay: 500,
            callbacks: {
                beforeOpen: function () {
                    this.st.mainClass = this.st.el.attr('data-effect');
                },
                open: function () {
                    if (self.handlers && self.handlers.openPopUp)
                    self.handlers.openPopUp();
                    self.isShown = true;
                }
            }
        });

        let addressChangedHandler = function () {
            self.clearShippingOptions();
            let address = self.getShippingAddress();
            self.getShippingOptions(address);
        };
        $('#CountryId').on('change', function () {
            $("#StateProvinceId").val(0);
            addressChangedHandler();
        });
        $('#StateProvinceId').on('change', addressChangedHandler);
        $('#ZipPostalCode').on('input propertychange paste', addressChangedHandler);
    },

    closePopup: function () {
      $.magnificPopup.close();
    },

    getShippingOptions: function (address) {
        if (!this.validateAddress(address))
            return;

        let self = this;

        self.setLoadWaiting();

        if (self.handlers && self.handlers.load)
            self.handlers.load();

        clearTimeout(self.delayTimer);
        self.delayTimer = setTimeout(function () {
            if (self.jqXHR && self.jqXHR.readyState !== 4)
                self.jqXHR.abort();

            let url = self.urlFactory(address);
            if (url) {
                self.jqXHR = $.ajax({
                    cache: false,
                    url: url,
                    data: $(self.form).serialize(),
                    type: 'POST',
                    success: function (response) {
                        self.successHandler(address, response);
                    },
                    error: function (jqXHR, textStatus, errorThrown) {
                        self.errorHandler(jqXHR, textStatus, errorThrown);
                    },
                    complete: function (jqXHR, textStatus) {
                        if (self.handlers && self.handlers.complete)
                            self.handlers.complete(jqXHR, textStatus);
                    }
                });
            }
        }, 300);
    },

    setLoadWaiting: function () {
        this.clearErrorMessage();
        $('#shipping-options-body').html($('<div/>').addClass('shipping-options-loading'));
    },

    successHandler: function (address, response) {
      $('#shipping-options-body').empty();

        if (response.success) {
            let activeOption;

            let options = response.result.ShippingOptions;
            if (options && options.length > 0) {
                let self = this;

                $.each(options, function (i, option) {
                    // try select the shipping option with the same provider and address
                  if (option.Selected ||
                        (self.selectedShippingOption &&
                        self.selectedShippingOption.provider === option.Name &&
                        self.addressesAreEqual(self.selectedShippingOption.address, address))) {
                        activeOption = {
                            provider: option.Name,
                            price: option.Price,
                            address: address,
                            deliveryDate: option.DeliveryDateFormat
                        };
                    }
                    self.addShippingOption(option.Name, option.DeliveryDateFormat, option.Price);
                });

                // select the first option
                if (!activeOption) {
                    activeOption = {
                        provider: options[0].Name,
                        price: options[0].Price,
                        deliveryDate: options[0].DeliveryDateFormat,
                        address: address
                    };
                }

                // if we have the already selected shipping options with the same address, reload it
                if (!$.magnificPopup.instance.isOpen && this.selectedShippingOption && this.addressesAreEqual(this.selectedShippingOption.address, address))
                    this.selectShippingOption(activeOption);

                this.setActiveShippingOption(activeOption);
            }
        } else {
          this.clearErrorMessage();
          this.showErrorMessage(response.errors);
        }

        if (this.handlers && this.handlers.success)
            this.handlers.success(address, response);
    },

    errorHandler: function (jqXHR, textStatus, errorThrown) {
        if (textStatus === 'abort') return;

        if (jqXHR.status >= 400) {
            let response = jqXHR.responseJSON;
            if (response instanceof Object && response.hasOwnProperty('Errors')) {
                let errorBox = $('#estimate-shipping-popup .message-failure').empty();
                $.each(response.Errors, function (i, error) {
                    errorBox.append($('<div/>').text(error));
                });
            }
        }

        this.clearShippingOptions();

        if (this.handlers && this.handlers.error)
            this.handlers.error(jqXHR, textStatus, errorThrown);
    },

    clearErrorMessage: function () {
      $(this.errorMessageBoxSelector).empty();
    },

    showErrorMessage: function (errors) {
      if (this.isShown) {
        let errorMessageBox = $(this.errorMessageBoxSelector);
        $.each(errors, function (i, error) {
          errorMessageBox.append($('<div/>').text(error));
        });
      }
    },

    selectShippingOption: function (option) {
        if (option && option.provider && option.price && this.validateAddress(option.address))
          this.selectedShippingOption = option;

        if (this.handlers && this.handlers.selectedOption)
            this.handlers.selectedOption(option);
    },

    addShippingOption: function (name, deliveryDate, price) {
        if (!name || !price) return;

        let shippingOption = $('<div/>').addClass('estimate-shipping-row shipping-option');

        shippingOption
            .append($('<div/>').addClass('estimate-shipping-row-item-radio')
                .append($('<input/>').addClass('estimate-shipping-radio').attr({ 'type': 'radio', 'name': 'shipping-option' }))
                .append($('<label/>')))
            .append($('<div/>').addClass('estimate-shipping-row-item shipping-item').text(name))
            .append($('<div/>').addClass('estimate-shipping-row-item shipping-item').text(deliveryDate ? deliveryDate : '-'))
            .append($('<div/>').addClass('estimate-shipping-row-item shipping-item').text(price));

        shippingOption.on('click', function () {
            $('input[name="shipping-option"]', this).prop('checked', true);
            $('.shipping-option.active').removeClass('active');
            $(this).addClass('active');
        });

        $('#shipping-options-body').append(shippingOption);
    },

    clearShippingOptions: function () {
      $('#shipping-options-body').html($('<div/>').addClass('no-shipping-options').text(this.noShippingOptionsErrorMessage));
    },

    setActiveShippingOption: function (option) {
        $.each($('.shipping-option'), function (i, shippingOption) {
            let shippingItems = $('.shipping-item', shippingOption);

            let provider = shippingItems.eq(0).text().trim();
            let price = shippingItems.eq(2).text().trim();
            if (provider === option.provider && price === option.price) {
                $(shippingOption).trigger('click');
                return;
            }
        });
    },

    getActiveShippingOption: function () {
        let shippingItems = $('.shipping-item', $('.shipping-option.active'));

        return {
            provider: shippingItems.eq(0).text().trim(),
            deliveryDate: shippingItems.eq(1).text().trim(),
            price: shippingItems.eq(2).text().trim(),
            address: this.getShippingAddress()
        };
    },

    getShippingAddress: function () {
      let address = {};
      let selectedCountryId = $('#CountryId').find(':selected');
      let selectedStateProvinceId = $('#StateProvinceId').find(':selected');
      let selectedZipPostalCode = $('#ZipPostalCode');

      if (selectedCountryId && selectedCountryId.val() > 0) {
        address.countryId = selectedCountryId.val();
        address.countryName = selectedCountryId.text();
      }

      if (selectedStateProvinceId && selectedStateProvinceId.val() > 0) {
          address.stateProvinceId = selectedStateProvinceId.val();
          address.stateProvinceName = selectedStateProvinceId.text();
      }

      if (selectedZipPostalCode && selectedZipPostalCode.val()) {
          address.zipPostalCode = selectedZipPostalCode.val();
      }

      return address;
    },

    addressesAreEqual: function (address1, address2) {
        return address1.countryId === address2.countryId &&
                 address1.stateProvinceId === address2.stateProvinceId &&
                   address1.zipPostalCode === address2.zipPostalCode;
    },

    validateAddress: function (address) {
      this.clearErrorMessage();

      if (!(address.countryName && address.countryId > 0)) {
        this.showErrorMessage([this.countryErrorMessage]);
      }

      if (!address.zipPostalCode) {
        this.showErrorMessage([this.zipPostalCodeErrorMessage]);
      }

      return address &&
                 address.countryName &&
                   address.countryId > 0 &&
                     address.zipPostalCode;
    }
}