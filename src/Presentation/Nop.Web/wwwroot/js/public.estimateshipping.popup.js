function createEstimateShippingPopUp(settings) {
  var defaultSettings = {
    opener: false,
    form: false,
    requestDelay: 300,
    urlFactory: false,
    handlers: {},
    localizedData: false,
    contentEl: false,
    countryEl: false,
    stateProvinceEl: false,
    zipPostalCodeEl: false,
    useCity: false,
    cityEl: false,
    errorMessageBoxClass: 'message-failure',
  };

  return {
    settings: $.extend({}, defaultSettings, settings),
    params: {
      jqXHR: false,
      displayErrors: false,
      delayTimer: false,
      selectedShippingOption: false
    },

    init: function () {
      var self = this;
      var $content = $(this.settings.contentEl);

      $('.apply-shipping-button', $content).on('click', function () {
        var option = self.getActiveShippingOption();
        if (option && option.provider && option.price) {
          self.selectShippingOption(option);
          self.closePopup();
        }
      });

      $(this.settings.opener).magnificPopup({
        type: 'inline',
        removalDelay: 500,
        callbacks: {
          beforeOpen: function () {
            this.st.mainClass = this.st.el.attr('data-effect');
          },
          open: function () {
            if (self.settings.handlers.openPopUp)
              self.settings.handlers.openPopUp();

            self.params.displayErrors = true;
          }
        }
      });

      var addressChangedHandler = function () {
        self.clearShippingOptions();
        var address = self.getShippingAddress();
        self.getShippingOptions(address);
      };
      $(this.settings.countryEl, $content).on('change', function () {
        $(self.settings.stateProvinceEl, $content).val(0);
        addressChangedHandler();
      });
      $(this.settings.stateProvinceEl, $content).on('change', addressChangedHandler);

      if (this.settings.useCity) {
        $(this.settings.cityEl, $content).on('input propertychange paste', addressChangedHandler);
      } else {
        $(this.settings.zipPostalCodeEl, $content).on('input propertychange paste', addressChangedHandler);
      }
    },

    closePopup: function () {
      $.magnificPopup.close();
    },
    
    getShippingOptions: function (address) {
      if (!this.validateAddress(address))
        return;

      var self = this;

      self.setLoadWaiting();

      if (self.settings.handlers.load)
        self.settings.handlers.load();

      clearTimeout(self.params.delayTimer);
      self.params.delayTimer = setTimeout(function () {
        if (self.params.jqXHR && self.params.jqXHR.readyState !== 4)
          self.params.jqXHR.abort();

        var url = self.settings.urlFactory(address);
        if (url) {
          self.params.jqXHR = $.ajax({
            cache: false,
            url: url,
            data: $(self.settings.form).serialize(),
            type: 'POST',
            success: function (response) {
              self.successHandler(address, response);
            },
            error: function (jqXHR, textStatus, errorThrown) {
              self.errorHandler(jqXHR, textStatus, errorThrown);
            },
            complete: function (jqXHR, textStatus) {
              if (self.settings.handlers.complete)
                self.settings.handlers.complete(jqXHR, textStatus);
            }
          });
        }
      }, self.settings.requestDelay);
    },

    setLoadWaiting: function () {
      this.clearErrorMessage();
      $('.shipping-options-body', $(this.settings.contentEl)).html($('<div/>').addClass('shipping-options-loading'));
    },

    successHandler: function (address, response) {
      $('.shipping-options-body', $(this.settings.contentEl)).empty();

      if (response.Success) {
        var activeOption;

        var options = response.ShippingOptions;
        if (options && options.length > 0) {
          var self = this;
          var selectedShippingOption = this.params.selectedShippingOption;

          $.each(options, function (i, option) {
            // try select the shipping option with the same provider and address
            if (option.Selected ||
              (selectedShippingOption &&
                selectedShippingOption.provider === option.Name &&
                self.addressesAreEqual(selectedShippingOption.address, address))) {
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
          if (!$.magnificPopup.instance.isOpen && selectedShippingOption && this.addressesAreEqual(selectedShippingOption.address, address))
            this.selectShippingOption(activeOption);

          this.setActiveShippingOption(activeOption);
        } else {
          this.clearShippingOptions();
        }
      } else {
        this.params.displayErrors = true;
        this.clearErrorMessage();
        this.clearShippingOptions();
        this.showErrorMessage(response.Errors);
      }

      if (this.settings.handlers.success)
        this.settings.handlers.success(address, response);
    },

    errorHandler: function (jqXHR, textStatus, errorThrown) {
      if (textStatus === 'abort') return;

      this.clearShippingOptions();

      if (this.settings.handlers.error)
        this.settings.handlers.error(jqXHR, textStatus, errorThrown);
    },

    clearErrorMessage: function () {
      $('.' + this.settings.errorMessageBoxClass, $(this.settings.contentEl)).empty();
    },

    showErrorMessage: function (errors) {
      if (this.params.displayErrors) {
        var errorMessagesContainer = $('.' + this.settings.errorMessageBoxClass, $(this.settings.contentEl));
        $.each(errors, function (i, error) {
          errorMessagesContainer.append($('<div/>').text(error));
        });
      }
    },

    selectShippingOption: function (option) {
      if (option && option.provider && option.price && this.validateAddress(option.address))
        this.params.selectedShippingOption = option;

      if (this.settings.handlers.selectedOption)
        this.settings.handlers.selectedOption(option);
    },

    addShippingOption: function (name, deliveryDate, price) {
      if (!name || !price) return;

      var shippingOption = $('<div/>').addClass('estimate-shipping-row shipping-option');

      shippingOption
        .append($('<div/>').addClass('estimate-shipping-row-item-radio')
          .append($('<input/>').addClass('estimate-shipping-radio').attr({ 'type': 'radio', 'name': 'shipping-option' + '-' + this.settings.contentEl }))
          .append($('<label/>')))
        .append($('<div/>').addClass('estimate-shipping-row-item shipping-item').text(name))
        .append($('<div/>').addClass('estimate-shipping-row-item shipping-item').text(deliveryDate ? deliveryDate : '-'))
        .append($('<div/>').addClass('estimate-shipping-row-item shipping-item').text(price));

      var self = this;

      shippingOption.on('click', function () {
        $('input[name="shipping-option' + '-' + self.settings.contentEl + '"]', $(this)).prop('checked', true);
        $('.shipping-option.active', $(self.settings.contentEl)).removeClass('active');
        $(this).addClass('active');
      });

      $('.shipping-options-body', $(this.settings.contentEl)).append(shippingOption);
    },

    clearShippingOptions: function () {
      var noShippingOptionsMsg = this.settings.localizedData.noShippingOptionsMessage;
      $('.shipping-options-body', $(this.settings.contentEl)).html($('<div/>').addClass('no-shipping-options').text(noShippingOptionsMsg));
    },

    setActiveShippingOption: function (option) {
      $.each($('.shipping-option', $(this.settings.contentEl)), function (i, shippingOption) {
        var shippingItems = $('.shipping-item', shippingOption);

        var provider = shippingItems.eq(0).text().trim();
        var price = shippingItems.eq(2).text().trim();
        if (provider === option.provider && price === option.price) {
          $(shippingOption).trigger('click');
          return;
        }
      });
    },

    getActiveShippingOption: function () {
      var shippingItems = $('.shipping-item', $('.shipping-option.active', $(this.settings.contentEl)));

      return {
        provider: shippingItems.eq(0).text().trim(),
        deliveryDate: shippingItems.eq(1).text().trim(),
        price: shippingItems.eq(2).text().trim(),
        address: this.getShippingAddress()
      };
    },

    getShippingAddress: function () {
      var address = {};
      var $content = $(this.settings.contentEl);
      var selectedCountryId = $(this.settings.countryEl, $content).find(':selected');
      var selectedStateProvinceId = $(this.settings.stateProvinceEl, $content).find(':selected');
      var selectedZipPostalCode = $(this.settings.zipPostalCodeEl, $content);
      var selectedCity = $(this.settings.cityEl, $content);

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

      if (selectedCity && selectedCity.val()) {
        address.city = selectedCity.val();
      }

      return address;
    },

    addressesAreEqual: function (address1, address2) {
      return address1.countryId === address2.countryId &&
        address1.stateProvinceId === address2.stateProvinceId &&
          (this.settings.useCity || address1.zipPostalCode === address2.zipPostalCode) &&
            (!this.settings.useCity || address1.city === address2.city);
    },

    validateAddress: function (address) {
      this.clearErrorMessage();

      var errors = [];
      var localizedData = this.settings.localizedData;

      if (!(address.countryName && address.countryId > 0))
        errors.push(localizedData.countryErrorMessage);

      if (this.settings.useCity && !address.city)
        errors.push(localizedData.cityErrorMessage);

      if (!this.settings.useCity && !address.zipPostalCode)
        errors.push(localizedData.zipPostalCodeErrorMessage);

      if (errors.length > 0)
        this.showErrorMessage(errors);

      return errors.length === 0;
    }
  }
}