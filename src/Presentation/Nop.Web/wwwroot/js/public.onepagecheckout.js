/*
** nopCommerce one page checkout
*/


var Checkout = {
    loadWaiting: false,
    failureUrl: false,

    init: function (failureUrl) {
        this.loadWaiting = false;
        this.failureUrl = failureUrl;

        Accordion.disallowAccessToNextSections = true;
    },

    ajaxFailure: function () {
        location.href = Checkout.failureUrl;
    },

    _disableEnableAll: function (element, isDisabled) {
        var descendants = element.find('*');
        $(descendants).each(function () {
            if (isDisabled) {
                $(this).prop("disabled", true);
            } else {
                $(this).prop("disabled", false);
            }
        });

        if (isDisabled) {
            element.prop("disabled", true);
        } else {
            $(this).prop("disabled", false);
        }
    },

    setLoadWaiting: function (step, keepDisabled) {
        var container;
        if (step) {
            if (this.loadWaiting) {
                this.setLoadWaiting(false);
            }
            container = $('#' + step + '-buttons-container');
            container.addClass('disabled');
            container.css('opacity', '.5');
            this._disableEnableAll(container, true);
            $('#' + step + '-please-wait').show();
        } else {
            if (this.loadWaiting) {
                container = $('#' + this.loadWaiting + '-buttons-container');
                var isDisabled = keepDisabled ? true : false;
                if (!isDisabled) {
                    container.removeClass('disabled');
                    container.css('opacity', '1');
                }
                this._disableEnableAll(container, isDisabled);
                $('#' + this.loadWaiting + '-please-wait').hide();
            }
        }
        this.loadWaiting = step;
    },

    gotoSection: function (section) {
        section = $('#opc-' + section);
        section.addClass('allow');
        Accordion.openSection(section);
    },

    back: function () {
        if (this.loadWaiting) return;
        Accordion.openPrevSection(true, true);
    },

    setStepResponse: function(response) {
      if (response.update_section) {
        $('#checkout-' + response.update_section.name + '-load').html(response.update_section.html);
      }
      if (response.allow_sections) {
        response.allow_sections.each(function(e) {
          $('#opc-' + e).addClass('allow');
        });
      }

      //TODO move it to a new method
      if ($("#billing-address-select").length > 0) {
        Billing.newAddress(!$('#billing-address-select').val());
      } else {
        Billing.newAddress(true);
      }

      if ($("#shipping-address-select").length > 0) {
        Shipping.newAddress(response.selected_id == undefined ? $('#shipping-address-select').val() : response.selected_id, $('#billing-address-select').children("option:selected").val());
      }

      if (response.goto_section) {
        Checkout.gotoSection(response.goto_section);
        return true;
      }
      if (response.redirect) {
        location.href = response.redirect;
        return true;
      }
      return false;
    }
};


var Billing = {
  form: false,
  saveUrl: false,
  disableBillingAddressCheckoutStep: false,
  guest: false,
  selectedStateId: 0,

  init: function(form, saveUrl, disableBillingAddressCheckoutStep, guest) {
    this.form = form;
    this.saveUrl = saveUrl;
    this.disableBillingAddressCheckoutStep = disableBillingAddressCheckoutStep;
    this.guest = guest;
  },

  newAddress: function(isNew) { 
    $('#save-billing-address-button').hide();

    if (isNew) {
      $('#billing-new-address-form').show();
      $('#edit-billing-address-button').hide();
      $('#delete-billing-address-button').hide();
    } else {
      $('#billing-new-address-form').hide();
      $('#edit-billing-address-button').show();
      $('#delete-billing-address-button').show();
    }
    $(document).trigger({ type: "onepagecheckout_billing_address_new" });
    Billing.initializeCountrySelect();
  },

  resetSelectedAddress: function() {
    var selectElement = $('#billing-address-select');
    if (selectElement) {
      selectElement.val('');
    }
    $(document).trigger({ type: "onepagecheckout_billing_address_reset" });
  },

  save: function() {
    if (Checkout.loadWaiting !== false) return;

    Checkout.setLoadWaiting('billing');

    $.ajax({
      cache: false,
      url: this.saveUrl,
      data: $(this.form).serialize(),
      type: "POST",
      success: this.nextStep,
      complete: this.resetLoadWaiting,
      error: Checkout.ajaxFailure
    });
  },

  resetLoadWaiting: function() {
    Checkout.setLoadWaiting(false);
  },

  nextStep: function(response) {
    //ensure that response.wrong_billing_address is set
    //if not set, "true" is the default value
    if (typeof response.wrong_billing_address === 'undefined') {
      response.wrong_billing_address = false;
    }
    if (Billing.disableBillingAddressCheckoutStep) {
      if (response.wrong_billing_address) {
        Accordion.showSection('#opc-billing');
      } else {
        Accordion.hideSection('#opc-billing');
      }
    }


    if (response.error) {
      if (typeof response.message === 'string') {
        alert(response.message);
      } else {
        alert(response.message.join("\n"));
      }

      return false;
    }

    Checkout.setStepResponse(response);
    Billing.initializeCountrySelect();
  },

  initializeCountrySelect: function() {
    if ($('#opc-billing').has('select[data-trigger="country-select"]')) {
      $('#opc-billing select[data-trigger="country-select"]').countrySelect();
    }
  },

  editAddress: function(url) {
    Billing.resetBillingForm();
    //Billing.initializeStateSelect();

    var prefix = 'BillingNewAddress_';
    var selectedItem = $('#billing-address-select').children("option:selected").val();
    $.ajax({
      cache: false,
      type: "GET",
      url: url,
      data: {
        addressId: selectedItem,
      },
      success: function (data, textStatus, jqXHR) {
        $.each(data, function (id, value) {
          if (value === null)
            return;

          if (id.indexOf("CustomAddressAttributes") >= 0 && Array.isArray(value)) {
            $.each(value, function (i, customAttribute) {
              if (customAttribute.DefaultValue) {
                $(`#${customAttribute.ControlId}`).val(
                  customAttribute.DefaultValue
                );
              } else {
                $.each(customAttribute.Values, function (j, attributeValue) {
                  if (attributeValue.IsPreSelected) {
                    $(`#${customAttribute.ControlId}`).val(attributeValue.Id);
                    $(
                      `#${customAttribute.ControlId}_${attributeValue.Id}`
                    ).prop("checked", attributeValue.Id);
                  }
                });
              }
            });

            return;
          }

          var val = $(`#${prefix}${id}`).val(value);
          if (id.indexOf("CountryId") >= 0) {
            val.trigger("change");
          }
          if (id.indexOf("StateProvinceId") >= 0) {
            Billing.setSelectedStateId(value);
          }
        });
      },
      complete: function (jqXHR, textStatus) {
        $("#billing-new-address-form").show();
        $("#edit-billing-address-button").hide();
        $("#delete-billing-address-button").hide();
        $("#save-billing-address-button").show();
      },
      error: Checkout.ajaxFailure,
    });
  },

  saveEditAddress: function(url) {
    var selectedId;
    $.ajax({
      cache: false,
      url: url + '?opc=true',
      data: $(this.form).serialize(),
      type: "POST",
      success: function (response) {
        if (response.error) {
          alert(response.message);
          return false;
        } else {
          selectedId = response.selected_id;
          Checkout.setStepResponse(response);
          Billing.resetBillingForm();
        }        
      },
      complete: function() {
        var selectElement = $('#billing-address-select');
        if (selectElement && selectedId) {
          selectElement.val(selectedId);
        }
      },
      error: Checkout.ajaxFailure
    });
  },

  deleteAddress: function (url) {
    var selectedAddress = $('#billing-address-select').children("option:selected").val();
    $.ajax({
      cache: false,
      type: "GET",
      url: url,
      data: {
        "addressId": selectedAddress,
        "opc": 'true'
      },
      success: function (response) {
        Checkout.setStepResponse(response);
      },
      error: Checkout.ajaxFailure
    });
  },

  setSelectedStateId: function (id) {
    this.selectedStateId = id;
  },

  resetBillingForm: function() {
    $(':input', '#billing-new-address-form')
      .not(':button, :submit, :reset, :hidden')
      .removeAttr('checked').removeAttr('selected')
    $(':input', '#billing-new-address-form')
      .not(':checkbox, :radio, select')
      .val('');

    $('.address-id', '#billing-new-address-form').val('0');
    $('select option[value="0"]', '#billing-new-address-form').prop('selected', true);
  }
};

var Shipping = {
    form: false,
    saveUrl: false,

    init: function (form, saveUrl) {
        this.form = form;
        this.saveUrl = saveUrl;
    },

  newAddress: function (id, billingAddressId) {
    isNew = !id;    
        if (isNew) {
          this.resetSelectedAddress();         
          $('#shipping-new-address-form').show();          
          $('#edit-shipping-address-button').hide();
          $('#delete-shipping-address-button').hide();
        } else {
          $('#shipping-new-address-form').hide();
          if (id == billingAddressId || (id != undefined && billingAddressId == undefined)) {
            $('#edit-shipping-address-button').hide();
            $("#save-shipping-address-button").hide();
            $('#delete-shipping-address-button').hide();            
          } else {
            $("#save-shipping-address-button").hide();
            $('#edit-shipping-address-button').show();
            $('#delete-shipping-address-button').show();
          }
        }
        $(document).trigger({ type: "onepagecheckout_shipping_address_new" });
        Shipping.initializeCountrySelect();
    },

    resetSelectedAddress: function () {
        var selectElement = $('#shipping-address-select');
        if (selectElement) {
            selectElement.val('');
        }
        $(document).trigger({ type: "onepagecheckout_shipping_address_reset" });
  },

    editAddress: function (url) {
      Shipping.resetShippingForm();

      var prefix = 'ShippingNewAddress_';
      var selectedItem = $('#shipping-address-select').children("option:selected").val();
      $.ajax({
        cache: false,
        type: "GET",
        url: url,
        data: {
          addressId: selectedItem,
        },
        success: function (data, textStatus, jqXHR) {
          $.each(data, function (id, value) {
            if (value === null)
              return;

            if (id.indexOf("CustomAddressAttributes") >= 0 && Array.isArray(value)) {
              $.each(value, function (i, customAttribute) {
                if (customAttribute.DefaultValue) {
                  $(`#${customAttribute.ControlId}`).val(
                    customAttribute.DefaultValue
                  );
                } else {
                  $.each(customAttribute.Values, function (j, attributeValue) {
                    if (attributeValue.IsPreSelected) {
                      $(`#${customAttribute.ControlId}`).val(attributeValue.Id);
                      $(
                        `#${customAttribute.ControlId}_${attributeValue.Id}`
                      ).prop("checked", attributeValue.Id);
                    }
                  });
                }
              });

              return;
            }

            var val = $(`#${prefix}${id}`).val(value);
            if (id.indexOf("CountryId") >= 0) {
              val.trigger("change");
            }    
            if (id.indexOf("StateProvinceId") >= 0) {
              Billing.setSelectedStateId(value);
            }
          });
        },
        complete: function (jqXHR, textStatus) {
          $("#shipping-new-address-form").show();
          $("#edit-shipping-address-button").hide();
          $("#delete-shipping-address-button").hide();
          $("#save-shipping-address-button").show();
        },
        error: Checkout.ajaxFailure,
      });
    },

    saveEditAddress: function (url) {
      var selectedId;
      $.ajax({
        cache: false,
        url: url + '?opc=true',
        data: $(this.form).serialize(),
        type: "POST",
        success: function (response) {
          if (response.error) {
            alert(response.message);
            return false;
          } else {
            selectedId = response.selected_id;
            Checkout.setStepResponse(response);
            Shipping.resetShippingForm();
          }
        },
        complete: function () {
          var selectElement = $('#shipping-address-select');
          if (selectElement && selectedId) {
            selectElement.val(selectedId);
          }
        },
        error: Checkout.ajaxFailure
      });
    },

    deleteAddress: function (url) {
      var selectedAddress = $('#shipping-address-select').children("option:selected").val();
      $.ajax({
        cache: false,
        type: "GET",
        url: url,
        data: {
          "addressId": selectedAddress,
          "opc": 'true'
        },
        success: function (response) {
          Checkout.setStepResponse(response);
        },
        error: Checkout.ajaxFailure
      });
    },

    save: function () {
        if (Checkout.loadWaiting !== false) return;

        Checkout.setLoadWaiting('shipping');

        $.ajax({
            cache: false,
            url: this.saveUrl,
            data: $(this.form).serialize(),
            type: "POST",
            success: this.nextStep,
            complete: this.resetLoadWaiting,
            error: Checkout.ajaxFailure
        });
    },

    resetLoadWaiting: function () {
        Checkout.setLoadWaiting(false);
    },

    nextStep: function (response) {
        if (response.error) {
            if (typeof response.message === 'string') {
                alert(response.message);
            } else {
                alert(response.message.join("\n"));
            }

            return false;
        }

        Checkout.setStepResponse(response);
    },

    initializeCountrySelect: function () {
        if ($('#opc-shipping').has('select[data-trigger="country-select"]')) {
            $('#opc-shipping select[data-trigger="country-select"]').countrySelect();
        }
  },

    resetShippingForm: function () {
      $(':input', '#shipping-new-address-form')
        .not(':button, :submit, :reset, :hidden')
        .removeAttr('checked').removeAttr('selected')
      $(':input', '#shipping-new-address-form')
        .not(':checkbox, :radio, select')
        .val('');

      $('.address-id', '#shipping-new-address-form').val('0');
      $('select option[value="0"]', '#shipping-new-address-form').prop('selected', true);
    }
};



var ShippingMethod = {
    form: false,
    saveUrl: false,
    localized_data: false,

    init: function (form, saveUrl, localized_data) {
        this.form = form;
        this.saveUrl = saveUrl;
        this.localized_data = localized_data;
    },

    validate: function () {
        var methods = document.getElementsByName('shippingoption');
        if (methods.length === 0) {
            alert(this.localized_data.NotAvailableMethodsError);
            return false;
        }

        for (var i = 0; i < methods.length; i++) {
            if (methods[i].checked) {
                return true;
            }
        }
        alert(this.localized_data.SpecifyMethodError);
        return false;
    },

    save: function () {
        if (Checkout.loadWaiting !== false) return;

        if (this.validate()) {
            Checkout.setLoadWaiting('shipping-method');

            $.ajax({
                cache: false,
                url: this.saveUrl,
                data: $(this.form).serialize(),
                type: "POST",
                success: this.nextStep,
                complete: this.resetLoadWaiting,
                error: Checkout.ajaxFailure
            });
        }
    },

    resetLoadWaiting: function () {
        Checkout.setLoadWaiting(false);
    },

    nextStep: function (response) {
        if (response.error) {
            if (typeof response.message === 'string') {
                alert(response.message);
            } else {
                alert(response.message.join("\n"));
            }

            return false;
        }

        Checkout.setStepResponse(response);
    }
};



var PaymentMethod = {
    form: false,
    saveUrl: false,
    localized_data: false,

    init: function (form, saveUrl, localized_data) {
        this.form = form;
        this.saveUrl = saveUrl;
        this.localized_data = localized_data;
    },

    toggleUseRewardPoints: function (useRewardPointsInput) {
        if (useRewardPointsInput.checked) {
            $('#payment-method-block').hide();
        }
        else {
            $('#payment-method-block').show();
        }
    },

    validate: function () {
        var methods = document.getElementsByName('paymentmethod');
        if (methods.length === 0) {
            alert(this.localized_data.NotAvailableMethodsError);
            return false;
        }

        for (var i = 0; i < methods.length; i++) {
            if (methods[i].checked) {
                return true;
            }
        }
        alert(this.localized_data.SpecifyMethodError);
        return false;
    },

    save: function () {
        if (Checkout.loadWaiting !== false) return;

        if (this.validate()) {
            Checkout.setLoadWaiting('payment-method');
            $.ajax({
                cache: false,
                url: this.saveUrl,
                data: $(this.form).serialize(),
                type: "POST",
                success: this.nextStep,
                complete: this.resetLoadWaiting,
                error: Checkout.ajaxFailure
            });
        }
    },

    resetLoadWaiting: function () {
        Checkout.setLoadWaiting(false);
    },

    nextStep: function (response) {
        if (response.error) {
            if (typeof response.message === 'string') {
                alert(response.message);
            } else {
                alert(response.message.join("\n"));
            }

            return false;
        }

        Checkout.setStepResponse(response);
    }
};



var PaymentInfo = {
    form: false,
    saveUrl: false,

    init: function (form, saveUrl) {
        this.form = form;
        this.saveUrl = saveUrl;
    },

    save: function () {
        if (Checkout.loadWaiting !== false) return;

        Checkout.setLoadWaiting('payment-info');
        $.ajax({
            cache: false,
            url: this.saveUrl,
            data: $(this.form).serialize(),
            type: "POST",
            success: this.nextStep,
            complete: this.resetLoadWaiting,
            error: Checkout.ajaxFailure
        });
    },

    resetLoadWaiting: function () {
        Checkout.setLoadWaiting(false);
    },

    nextStep: function (response) {
        if (response.error) {
            if (typeof response.message === 'string') {
                alert(response.message);
            } else {
                alert(response.message.join("\n"));
            }

            return false;
        }

        Checkout.setStepResponse(response);
    }
};



var ConfirmOrder = {
    form: false,
    saveUrl: false,
    isSuccess: false,
    isCaptchaEnabled: false,
    isReCaptchaV3: false,
    recaptchaPublicKey: "",

    init: function (saveUrl, successUrl, isCaptchaEnabled, isReCaptchaV3, recaptchaPublicKey) {
        this.saveUrl = saveUrl;
        this.successUrl = successUrl;
        this.isCaptchaEnabled = isCaptchaEnabled;
        this.isReCaptchaV3 = isReCaptchaV3;
        this.recaptchaPublicKey = recaptchaPublicKey;
    },

  save: async function () {
    if (Checkout.loadWaiting !== false) return;

      //terms of service
        var termOfServiceOk = true;
        if ($('#termsofservice').length > 0) {
            //terms of service element exists
            if (!$('#termsofservice').is(':checked')) {
                $("#terms-of-service-warning-box").dialog();
                termOfServiceOk = false;
            } else {
                termOfServiceOk = true;
            }
        }
        if (termOfServiceOk) {
            Checkout.setLoadWaiting('confirm-order');
            var postData = {};

            if (ConfirmOrder.isCaptchaEnabled) {
                var captchaTok = await ConfirmOrder.getCaptchaToken('OpcConfirmOrder');
                postData['g-recaptcha-response'] = captchaTok;
            }

            addAntiForgeryToken(postData);
            $.ajax({
                cache: false,
                url: this.saveUrl,
                data: postData,
                type: "POST",
                success: this.nextStep,
                complete: this.resetLoadWaiting,
                error: Checkout.ajaxFailure
            });
        } else {
            return false;
        }
    },

    getCaptchaToken: async function (action) {
        var recaptchaToken = ''
        if (ConfirmOrder.isReCaptchaV3) {
            grecaptcha.ready(() => {
                grecaptcha.execute(this.recaptchaPublicKey, { action: action }).then((token) => {
                    recaptchaToken = token;
                });
            });
        } else {
            recaptchaToken = grecaptcha.getResponse();
        }

        while (recaptchaToken == '') {
            await new Promise(t => setTimeout(t, 100));
        }

        return recaptchaToken;
    },

    resetLoadWaiting: function (transport) {
        Checkout.setLoadWaiting(false, ConfirmOrder.isSuccess);
    },

    nextStep: function (response) {
        if (response.error) {
            if (typeof response.message === 'string') {
                alert(response.message);
            } else {
                alert(response.message.join("\n"));
            }

            return false;
        }

        if (response.redirect) {
            ConfirmOrder.isSuccess = true;
            location.href = response.redirect;
            return;
        }
        if (response.success) {
            ConfirmOrder.isSuccess = true;
            window.location = ConfirmOrder.successUrl;
        }

        Checkout.setStepResponse(response);
    }
};
