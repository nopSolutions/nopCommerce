var CheckoutBilling = {
  form: false,
  selectedStateId: 0,

  init: function (form) {
    this.form = form;
  },

  editAddress: function(url, addressId) {
    CheckoutBilling.resetBillingForm();

    var prefix = 'BillingNewAddress_';
    $.ajax({
      cache: false,
      type: "GET",
      url: url,
      data: {
        "addressId": addressId
      },
      success: function(data, textStatus, jqXHR) {
        $.each(data,
          function(id, value) {
            //console.log("id:" + id + "\nvalue:" + value);
            if (value !== null) {
              var val = $(`#${prefix}${id}`).val(value);
              if (id.indexOf('CountryId') >= 0) {
                val.trigger('change');
              }
              if (id.indexOf('StateProvinceId') >= 0) {
                CheckoutBilling.setSelectedStateId(value);
              }
            }
          });
      },
      complete: function(jqXHR, textStatus) {
        $('#toggle-billing-address').hide();
        $('#billingaddress-next-button').hide();
        $('#billingaddress-save-button').show();
        $('#billingaddress-cancel-button').show();
        $('#billingaddress-new-form .title-text').html('Edit address');
      },
      error: function(err) {
        alert(err);
      }
    });
  },

  saveEditAddress: function (url) {
    $.ajax({
      cache: false,
      url: url,
      data: $(this.form).serialize(),
      type: "POST",
      success: function(response) {
        location.href = response.redirect;
        return true;
      },
      error: function (err) {
        alert(err);
      }
    });
  },

  deleteEditAddress: function(url, addressId) {
    $.ajax({
      cache: false,
      type: "GET",
      url: url,
      data: {
        "addressId": addressId
      },
      success: function(response) {
        location.href = response.redirect;
        return true;
      },
      error: function(err) {
        alert(err);
      }
    });
  },

  resetBillingForm: function() {
    $(':input', '#billingaddress-new-form')
      .not(':button, :submit, :reset, :hidden').removeAttr('checked').removeAttr('selected')
      .not(':checkbox, :radio, select').val('');

    $('select option[value="0"]').prop('selected', true);
  },

  setSelectedStateId: function(id) {
    this.selectedStateId = id;
  }
}