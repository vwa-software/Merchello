
$(window).on("load", function (){
    
    $('.cat-link-inner-right').on('click', function () {
		$(this).next().toggleClass("cat-link-open");
		//$(this).next().find('> li > ul').toggleClass("cat-link-open");
		$(this).find('.pres-cat-dropdown').toggleClass("pres-dropdown-open");
    });


    /* other filter options */
    $('#hk_pressroom .r-catalog-left form input').on('click', function() {
        var value = $(this)[0]['value'];
        var name = $(this)[0]['name'];
        localStorage.setItem('pers_'+name, value);
        
        if (($this).parent().is('a')) {
            var link = ($this).parent().attr('href');  
            window.location.href = link;
        }
        
        if (name == "background-color:") {
            $('.hk_product-box').css('background-color', value);            
        }
    });
 
    
    /* on select product */
    $('.press-productbox-control-inner input[name=selectProduct]').on('click', function() {
        if ($(this).prop('checked') === true) {
            $(this).closest('.hk_product-box').find('.caption').css("outline", "1px dashed #444");
        } else {
            $(this).closest('.hk_product-box').find('.caption').css("outline", "0px dashed #444");
        }
    });
    
    /* when click on productbox send email */
	$('.press-productbox-control-inner:not(.crl-disabled) .producuctbox-control-enter-email-right-inner').on('click', function() { 
        var emailBox = $(this).closest(".producuctbox-control-enter-email").find('input');
		var emailAdr = emailBox.val();

        /* validate email */
        if (isEmail(emailAdr)) {
            emailBox.css("color","#444444");
			localStorage.setItem("productboxEmail", emailAdr);
			$(this).closest('.hk_product-box').find('.press-productbox-control-inner input[name=selectProduct]').prop('checked', true);
            $(this).closest(".producuctbox-control-enter-email").find('.producuctbox-control-email-sent').addClass('email-sent-show');

			presClickActionButton("sendMail");

        } else {
            emailBox.css("color","red");
		}
    });    

	/* when click on download all as zip */
	$('.press-productbox-control-inner .productbox-download-zip-all').on('click', function () {
		$(this).closest('.hk_product-box').find('.press-productbox-control-inner input[name=selectProduct]').prop('checked', true);
		presClickActionButton("downloadAll");
	});

	/* when click on download docs only */
	$('.press-productbox-control-inner:not(.crl-disabled) .productbox-download-docs-only').on('click', function () {
		$(this).closest('.hk_product-box').find('.press-productbox-control-inner input[name=selectProduct]').prop('checked', true);
		presClickActionButton("downloadDocs");
	});


	function presClickActionButton(action) {
		var emailAdr = localStorage.getItem("productboxEmail");
		if (!emailAdr) {
			emailAdr = "";
		}
		var qualityNode = $(".r-catalog-left input[name=quality]:checked");
		var quality = qualityNode.prop('value');

		var productId = getSelected();
		if (productId.length > 2) {
			presController(emailAdr, quality, productId, action);
		}		
	}

    /* restore email adres on hover */
	$('.press-productbox-control-inner:not(.crl-disabled)').on('mouseover', function() { 
        var emailAdr = localStorage.getItem("productboxEmail");
        if (emailAdr) {
            var emailBox = $(this).find(".producuctbox-control-enter-email input[name=emailaddress]");
            if (emailBox.prop("value") === "") {
                $(this).find(".producuctbox-control-enter-email input[name=emailaddress]").prop("value", emailAdr);
            }
        }    
    });
    

    /* activate front image overlay on click */
    $('.hk_product-box img').on('click', function () {
       $('.pres-fullsc-front-image-container').removeClass('pres-front-image-closed');
       var imgUrl = $(this).prop('src');
       console.log('src '+imgUrl);
       $('.pres-fullsc-front-image-container-inner').css('background-image', 'url(' + imgUrl + ')');
    });
 
    $('.pres-fullsc-front-image-container-close').on('click', function () {   
        $('.pres-fullsc-front-image-container').addClass('pres-front-image-closed');
    });

    
    
    /* restore selection values on page load, if any*/
    restoreMemory();
});


// function selectedMultiple() {
    // return ($('.press-productbox-control-inner input[name=selectProduct]:checked').length > 1);
// }
function getSelected() {
    var list = [];
    var checks = $('.press-productbox-control-inner input[name=selectProduct]:checked');
    checks.each( function() {
        list.push($(this).closest('.hk_product-box').attr('data-productId'));
        $(this).prop('checked', false);
        $(this).closest('.hk_product-box').find('.caption').css("outline", "0px dashed #444");
    });

	list = JSON.stringify(list);
    return list;
}


function presController(email, quality, productId, action) {
    var url = "/umbraco/Surface/PressroomMail/CreateZip?ProductId="+productId+"&Quality="+quality+"&Action="+action+"&EmailAdr="+email;
	url += "&guid=" + $('#guid').val();

	if (action != "sendMail") {
        $("#pres-dwnldframe").attr("src", url);
	} else {
		$.ajax({
			url: url,
			type: 'POST',
			data: {},
			dataType: "json",
			beforeSend: function () {

			},
			success: function (result) {
				var success = result.Success;
				var message = result.Message;
				if (!success) {
					alert(message);
				}
			},
			error: function (result) {
				//Handle error
			}
		});
		return false;
	}
	
}


function isEmail(email) {
    var regex = /^([a-zA-Z0-9_.+-])+\@(([a-zA-Z0-9-])+\.)+([a-zA-Z0-9]{2,4})+$/;
    return regex.test(email);    
}


function restoreMemory() {

    /* restore stored background color for product boxes */
    var style_value = localStorage.getItem('pers_background-color:');
    if (style_value) {
        $('.hk_product-box').css('background-color', style_value);
        $('input[name="background-color:"][value="'+style_value+'"]').prop('checked', true);        
    }
    
    /* restore quality value */
    var quality_value = localStorage.getItem('pers_quality');
    if (quality_value) {
        $('input[name="quality"][value="'+quality_value+'"]').prop('checked', true);        
    }
    
    
    /* checkbox selections */
    $('.press-productbox-control-inner input[name=selectProduct]:checked').closest('.hk_product-box').find('.caption').css("outline", "1px dashed #444");

}


