/*account scripts*/

var companyValue, cityValue, phoneValue, addressesValue, regionValue, mailValue, postCodeValue, countryValue, formTypeValue; //popuplate edit/new forms
var accCompanyValue, accLastnameValue, accCountryValue, accZipValue, accMobileValue, accFirstnameValue, accEmailValue, accProvinceValue, 
    accCityValue, accFirstlettersValue, accGenderValue, accStreetValue, accPhoneValue; //popuplate edit account form
var btnType;
var elemEmpty = false;
var accTab = "accountTab";

$(window).on("load", function () {

    $(".panel-body").on("click", ".clickable-row", function () {
        $(".clickable-row").removeClass("defaultRow");
        $(this).addClass("defaultRow");
        var index = $(this).index();
        $(".find_ids").hide();

        $(this).closest(".tab-pane").find(".find_ids").eq(index).show();
    });

    // the tabs
    $(".hk_order.hk_checkout .nav-tabs.tabsTop .nav-link").on("click", function () {
        
        $('.cancelBtn').click();

        var index = $(this).parent().index();
        var tabContent = $(".hk_order.hk_checkout .tab-content.tabContentTop .tab-pane").eq(index);
        if (tabContent.length) {
            tabContent.siblings().hide();
            tabContent.show();
        }
        
        var clickRow = $(".hk_order.hk_checkout .clickable-row:visible:first-child");
        if (clickRow.length) {
            clickRow.click();
        }

        if ($(this).hasClass("account-tab")) {
            $(".newFormBtn").hide();
        } else {
            $(".newFormBtn").show();
        }
    });

    $(".clickable-row").on("click", function () {
        populateFormFields();
    });

    $(".editFormBtn").on("click", function () {
        buttonsFunctions("editForm", accTab);
        $(".hk_order.hk_checkout .clickable-row").css("pointer-events","none");
    });
    $(".newFormBtn").on("click", function () {
        buttonsFunctions("newForm", accTab);
        $(".hk_order.hk_checkout .clickable-row").css("pointer-events", "none");
    });

    $(".cancelBtn").on("click", function () {
        buttonsFunctions("cancelBtn", accTab);
        $(".hk_order.hk_checkout .clickable-row").css("pointer-events", "all");
    });

    var firstTab = $(".hk_order.hk_checkout .account-tab");
    if (firstTab.length) {
        firstTab.click();
    }

    //find out which tab is selected and disable/enable buttons accordingly
    $('.hk_order.hk_checkout .nav-item > a').on("click", function () {
        
        var isInvoice = $(this).hasClass('invoice-tab');
        var isShipping = $(this).hasClass('shipping-tab');
        var isMyAccount = $(this).hasClass('account-tab');

        if (isInvoice || isShipping) {
            $('.newFormBtn').show();
            $('.editFormBtn').show();
        }
        else {
            btnHidden();
        }

        if (isShipping) {
            emptyshippingAddress();
            accTab = 'shipping';
        } else {
            accTab = 'invoice';
        }

        if (isMyAccount) {
            $('.editFormBtn').show();
            accTab = 'accountTab';
        }

        $('.cancelBtn').click();
    });
});

function buttonsFunctions(btnType, accTab) {

    if (btnType == 'newForm') {

        clearFields();
        $('.extra-info-row').hide();
        $('.new-form').show();
        $('.edit-form').hide();

        $('.cancelBtn').not(".cancelAcc").css('display', 'block');

        $('.editFormBtn').prop('disabled', true);

        /*11/09/2019*/ //prefill typeform so they can be displayed in mail
        if (accTab == 'shipping') {
            $(".formtype").find("input").val("shipping");
        } else if (accTab == 'invoice') {
            $(".formtype").find("input").val("invoice");
        }

        if ($(".liNewAddres").length === 0) {
            var adrList = $("ul.address-list");
            adrList.append('<li class="clearfix clickable-row liNewAddres"><div class="row p-adr h-adr">New address</div><div class="liArrowContainer"></div></li>');
            var lastLi = $('li:last-child');

            lastLi.on("click", function () {

             if ($('.clickable-row').hasClass('defaultRow')) {
                  $('.clickable-row').removeClass('defaultRow');
                  $('.liNewAddres').addClass('defaultRow');
               }
             else {
                 $('.clickable-row').addClass('defaultRow');
                    $('.liNewAddres').removeClass('defaultRow');
              }
            });
            lastLi.click();
        }
    }

    if (btnType == 'editForm') {

        if (accTab == 'shipping' || accTab == 'invoice') {

            $('.extra-info-row').hide();
            $('.edit-form').show();
            $('.new-form').hide();

            $('.newFormBtn').prop('disabled', true);
            //populateFormFields(); //populate form fields 
        }

        if (accTab == 'accountTab') {

            $('.editAccount').show();
            $('.tab-account-content').hide();
            $('.cancelAcc').css('display', 'block');
            populateAccountFields(); //populate fields in account tab
        }
        $('.cancelBtn').not(".cancelAcc").css('display', 'block');
    }

    if (btnType == 'cancelBtn') {

        formsHidden();
        if (accTab == 'shipping' || accTab == 'invoice') {

            $('.extra-info-row').show();
            $('.editFormBtn').prop('disabled', false);  //enable buttons
            $('.newFormBtn').prop('disabled', false);  //enable buttons
            $('.liNewAddres').remove();
         }
        
        if (accTab == 'accountTab') {

            $('.tab-account-content').show();
            $('.editAccount').hide();
        }
        $('.cancelBtn').css('display', 'none');
        $(".clickable-row").removeClass("defaultRow");
        $(".clickable-row:visible").first().click();
    }
}

function emptyshippingAddress() {

    $('ul.address-list').each(function () {

        if (!$(this).has('li').length) $(this).html('<p>There is no shipping address added to your account yet.</p>');
    });

    $('ul.address-two').each(function () {

        if (!$(this).has('li').length) {
            $(this).html(''); 
            elemEmpty = true;
        }
        else {
            elemEmpty = false;
        }
    });

    if (elemEmpty) {
        $('.editFormBtn').hide();
        $('.extra-info-row').css('border-left', 'none');
    }
    else {
        $('.extra-info-row').css('border-left', '1px solid grey');
    }
}

function formsHidden() {
    $('.new-form').hide();
    $('.edit-form').hide();
}

function btnHidden() {
    $('.newFormBtn').hide();
    $('.editFormBtn').hide();
}

//complaint form to switch between options and render right form
$("select.mdb-select").change(function () {
    var selectedOption = $(this).children("option:selected").val();

    if (selectedOption == 'dGoods') {
        $('#option2').show();
        $('#option1').hide();
    }
    else {
        $('#option1').show();
        $('#option2').hide();
    }
});

//copy to clipboard /*12/09/2019*/
function copyToClipboard(element) {
    var $temp = $("<input>");
    console.log(element);
    $("body").append($temp);
    $temp.val($(element).html()).select();
    document.execCommand("copy");
    $temp.remove();
}

function populateFormFields() {

    $(".panel-body .clickable-row").click(function () {

        var visiblePanel = $(".panel-body .extra-info-row > ul > li").not(":hidden");
        var spans = visiblePanel.find("span");

        spans.each(function () {

            var allData = $(this).data();

            if (typeof allData.company != "undefined") {
                companyValue = allData.company;
            }

            if (typeof allData.city != "undefined") {
                cityValue = allData.city;
            }

            if (typeof allData.phone != "undefined") {
                phoneValue = allData.phone;
            }

            if (typeof allData.addresses != "undefined") {
                addressesValue = allData.addresses;
            }

            if (typeof allData.region != "undefined") {
                regionValue = allData.region;
            }

            if (typeof allData.mail != "undefined") {
                mailValue = allData.mail;
            }

            if (typeof allData.postalcode != "undefined") {
                postCodeValue = allData.postalcode;
            }

            if (typeof allData.country != "undefined") {
                countryValue = allData.country;
            }
        });

        $('.companyname').find('input').val(companyValue);
        $('.city').find('input').val(cityValue);
        $('.phone').find('input').val(phoneValue);
        $('.streetandnumber').find('input').val(addressesValue);
        $('.province').find('input').val(regionValue);
        $('.emailaddress').find('input').val(mailValue);
        $('.zipcode').find('input').val(postCodeValue);
        $('.country').find('input').val(countryValue);
    });
}

function clearFields() {

    $('.companyname').find('input').val("");
    $('.city').find('input').val("");
    $('.phone').find('input').val("");
    $('.streetandnumber').find('input').val("");
    $('.province').find('input').val("");
    $('.emailaddress').find('input').val("");
    $('.zipcode').find('input').val("");
    $('.country').find('input').val("");
}

function populateAccountFields() {

    $('.dataCol').each(function () {
         var personalData = $(this).data();
         
         if (typeof personalData.company != "undefined") {
            accCompanyValue = personalData.company;
        }

        if (typeof personalData.lastname != "undefined") {
            accLastnameValue = personalData.lastname;
        }

        if (typeof personalData.country != "undefined") {
            accCountryValue = personalData.country;
        }

        if (typeof personalData.zip != "undefined") {
            accZipValue = personalData.zip;
        }

        if (typeof personalData.mobile != "undefined") {
            accMobileValue = personalData.mobile;
        }

        if (typeof personalData.firstname != "undefined") {
            accFirstnameValue = personalData.firstname;
        }

        if (typeof personalData.email != "undefined") {
            accEmailValue = personalData.email;
        }

        if (typeof personalData.province != "undefined") {
            accProvinceValue = personalData.province;
        }
            
        if (typeof personalData.city != "undefined") {
            accCityValue = personalData.city;
        }
            
        if (typeof personalData.firstletters != "undefined") {
            accFirstlettersValue = personalData.firstletters;
        }
            
        if (typeof personalData.gender != "undefined") {
            accGenderValue = personalData.gender;
        }
            
        if (typeof personalData.street != "undefined") {
            accStreetValue = personalData.street;
        }
            
        if (typeof personalData.phone != "undefined") {
            accPhoneValue = personalData.phone;
        }
    });
    
    $('.companyname').find('input').val(accCompanyValue);
    $('.lastname').find('input').val(accLastnameValue);
    $('.country').find('input').val(accCountryValue);
    $('.zipcode').find('input').val(accZipValue);
    $('.mobile').find('input').val(accMobileValue);
    $('.firstname').find('input').val(accFirstnameValue);
    $('.emailaddress').find('input').val(accEmailValue);
    $('.province').find('input').val(accProvinceValue);
    $('.city').find('input').val(accCityValue);
    $('.firstletters').find('input').val(accFirstlettersValue);
    $('.gender').find('input').val(accGenderValue);
    $('.streetnr').find('input').val(accStreetValue);
    $('.phone').find('input').val(accPhoneValue);
}
