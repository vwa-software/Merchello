// ------------------------------------------------------- //
//   Make a sticky navbar on scrolling
// ------------------------------------------------------ //
$(window).scroll(function () {

    function makeItFixed(x) {

        var body = $('body'),
            navbar = $('nav.navbar-sticky'),
            header = $('.header');

        if ($(window).scrollTop() >= x) {
            navbar.addClass('fixed-top');
            navbar.removeClass('fixed-top-null');
            if (!header.hasClass('header-absolute')) {
                body.css('padding-top', navbar.outerHeight());
                body.data('smooth-scroll-offset', navbar.outerHeight());
            }
        }
        if ($(window).scrollTop() <= x) {
            navbar.addClass('fixed-top-null');
            if (!header.hasClass('header-absolute')) {
                body.css('padding-top', navbar.outerHeight());
                body.data('smooth-scroll-offset', navbar.outerHeight());
            }
        }
    }

    // when we scroll past .top-bar, affix the navbar
    makeItFixed($('.top-bar').outerHeight());
});


// ------------------------------------------------------- //
//   Scroll to top button
// ------------------------------------------------------ //

$(window).on('scroll', function () {
    if ($(window).scrollTop() >= 1500) {
        $('#scrollTop').fadeIn();
    } else {
        $('#scrollTop').fadeOut();
    }
});


$('#scrollTop').on('click', function () {
    $('html, body').animate({
        scrollTop: 0
    }, 1000);
});


/*!
 *
 * Sell theme v1.0.0
 * Copyright 2018 - Bootstrapious.com
 * 
 */

$(function () {

    // ------------------------------------------------------- //
    //   Multilevel dropdowns
    // ------------------------------------------------------ //

    $(".dropdown-menu [data-toggle='dropdown']").on("click", function (event) {
        event.preventDefault();
        event.stopPropagation();

        $(this).siblings().toggleClass("show");


        if (!$(this).next().hasClass('show')) {
            $(this).parents('.dropdown-menu').first().find('.show').removeClass("show");
        }
        $(this).parents('li.nav-item.dropdown.show').on('hidden.bs.dropdown', function (e) {
            $('.dropdown-submenu .show').removeClass("show");
        });

    });

    // ------------------------------------------------------- //
    //   Transparent navbar dropdowns 
    //
    //   = do not make navbar 
    //   transparent if dropdown's still open 
    //   / make transparent again when dropdown's closed
    // ------------------------------------------------------ //

    var navbar = $('.navbar'),
        navbarCollapse = $('.navbar-collapse');

    $('.navbar.bg-transparent .dropdown').on('show.bs.dropdown', function () {
        makeNavbarWhite();
    });

    $('.navbar.bg-transparent .dropdown').on('hide.bs.dropdown', function () {
        // if we're not on mobile, make navbar transparent 
        // after we close the dropdown

        if (!navbarCollapse.hasClass('show')) {
            makeNavbarTransparent();
        }
    });

    $('.navbar.bg-transparent .navbar-collapse').on('show.bs.collapse', function () {
        makeNavbarWhite();
    });


    $('.navbar.bg-transparent .navbar-collapse').on('hide.bs.collapse', function () {
        makeNavbarTransparent();
    });

    function makeNavbarWhite() {
        navbar.addClass('bg-white');
        navbar.addClass('navbar-light');
        navbar.addClass('was-transparent');

        navbar.removeClass('bg-transparent');
        navbar.removeClass('navbar-dark');
    }

    function makeNavbarTransparent() {
        navbar.removeClass('bg-white');
        navbar.removeClass('navbar-light');
        navbar.removeClass('was-transparent');

        navbar.addClass('bg-transparent');
        navbar.addClass('navbar-dark');
    }


    // ------------------------------------------------------- //
    //   Open & Close Search Panel
    // ------------------------------------------------------ //
    $('[data-toggle="search"]').on('click', function () {
        $('.search-area-wrapper').show();
		$('.fa-search').css("visibility", "hidden" );
		$('#q.search-area-input').focus();
    });

    $('.search-area-wrapper .close-btn').on('click', function () {
        $('.search-area-wrapper').hide();
		$('.fa-search').css("visibility", "visible" );
    });
    //$(window).scroll(function(){$('.search-area-wrapper').fadeOut();});

    // ------------------------------------------------------- //
    //   Ekko Lightbox
    // ------------------------------------------------------ //

    $(document).on('click', '[data-toggle="lightbox"]', function (event) {
        event.preventDefault();
        $(this).ekkoLightbox({
            alwaysShowClose: true,
            leftArrow: '<span><img src="img/prev.svg" class="nav-arrow nav-arrow-left" alt="" width="50"></span>',
            rightArrow: '<span><img src="img/next.svg" class="nav-arrow nav-arrow-right" alt="" width="50"></span>'
        });
    });



    // ------------------------------------------------------- //
    //   Make a sticky navbar on scrolling
    // ------------------------------------------------------ //
    $(window).scroll(function () {

        function makeItFixed(x) {

            var body = $('body'),
                navbar = $('nav.navbar-sticky'),
                header = $('.header');

            if ($(window).scrollTop() >= x) {
                navbar.addClass('fixed-top');
                if (!header.hasClass('header-absolute')) {
                    body.css('padding-top', navbar.outerHeight());
                    body.data('smooth-scroll-offset', navbar.outerHeight());
                }
            } else {
                navbar.removeClass('fixed-top');
                body.css('padding-top', '0');
            }
        }

        // when we scroll past .top-bar, affix the navbar
        makeItFixed($('.top-bar').outerHeight());
    });


    // ------------------------------------------------------- //
    //   Increase/Decrease product amount
    // ------------------------------------------------------ //
    $('.btn-items-decrease').on('click', function () {
        var input = $(this).siblings('.input-items');
        if (parseInt(input.val(), 10) >= 1) {
            input.val(parseInt(input.val(), 10) - 1);
        }
    });

    $('.btn-items-increase').on('click', function () {
        var input = $(this).siblings('.input-items');
        input.val(parseInt(input.val(), 10) + 1);
    });

    // ------------------------------------------------------- //
    //   Scroll to top button
    // ------------------------------------------------------ //

    $(window).on('scroll', function () {
        if ($(window).scrollTop() >= 1500) {
            $('#scrollTop').fadeIn();
        } else {
            $('#scrollTop').fadeOut();
        }
    });

    $('#scrollTop').on('click', function () {
        $('html, body').animate({
            scrollTop: 0
        }, 1000);
    });

    // ------------------------------------------------------- //
    //    Colour form control 
    // ------------------------------------------------------ //

    $('.btn-colour').on('click', function (e) {
        var button = $(this);

        if (button.attr('data-allow-multiple') === undefined) {
            button.parents('.colours-wrapper').find('.btn-colour').removeClass('active');
        }

        button.toggleClass('active');
    });

    // ------------------------------------------------------- //
    //  Button-style form labels used in detail.html
    // ------------------------------------------------------ //

    $('.detail-option-btn-label').on('click', function () {
        var button = $(this);

        button.parents('.detail-option').find('.detail-option-btn-label').removeClass('active');

        button.toggleClass('active');
    });

    // ------------------------------------------------------- //
    //   Bootstrap tooltips
    // ------------------------------------------------------- //

  //  $('[data-toggle="tooltip"]').tooltip();

    // ------------------------------------------------------- //
    //   Smooth Scroll
    // ------------------------------------------------------- //

    var smoothScroll = new SmoothScroll('a[data-smooth-scroll]', {
        offset: 80
    });

// ------------------------------------------------------ //
// For demo purposes, can be deleted
// ------------------------------------------------------ //

/*
var stylesheet = $('link#theme-stylesheet');
$("<link id='new-stylesheet' rel='stylesheet'>").insertAfter(stylesheet);
var alternateColour = $('link#new-stylesheet');

if ($.cookie("theme_csspath")) {
    alternateColour.attr("href", $.cookie("theme_csspath"));
}

$("#colour").change(function () {

    if ($(this).val() !== '') {

        var theme_csspath = 'css/style.' + $(this).val() + '.css';

        alternateColour.attr("href", theme_csspath);

        $.cookie("theme_csspath", theme_csspath, {
            expires: 365,
            path: document.URL.substr(0, document.URL.lastIndexOf('/'))
        });

    }

    return false;
});
rtemp */ 


// Hotspot fade image
        
$(window).load(function(){
$(".imp-wrap").ready(function(){
      $(".imp-wrap img").fadeIn(1000);
});
});

// Hotspot fade image

// Hotspot pulse
/*
$(window).load(function(){
      
(function pulse(back) {
    $('.imp-shape-spot img').animate(
        { opacity: (back) ? 0.4 : 1 }, 1000, function ()
        { pulse(!back) });
        })(false);

    });
*/
// /Hotspot pulse


// Onclick & hover

    $(window).load(function () {
        
        if ( $(window).width() > 991.99) { 
    
            $('.nav-item.dropdown a').click(function () {
                if ($(this).next('.dropdown-menu.hk_Collections').is(':visible')) {
                    //window.location.replace("/collections/all/");
                    window.location = $(this).attr('href');
                }
                else if ($(this).next('.dropdown-menu.hk_HKliving').is(':visible')) {
                    window.location.replace("/hkliving/about/");
                }
                else if ($(this).next('.dropdown-menu.hk_Contact').is(':visible')) {
                    window.location = $(this).attr('href');
                }
            });
        }

});

// /Onclick & hover

// Accordion Account

   
$(document).ready(function () {

    //ACCORDION BUTTON ACTION (ON CLICK DO THE FOLLOWING)
    $('.accordionButton').click(function () {

        //REMOVE THE ON CLASS FROM ALL BUTTONS
        $('.accordionButton').removeClass('on');
        //$('i').removeClass('fa-sort-down');

        //NO MATTER WHAT WE CLOSE ALL OPEN SLIDES
        $('.accordionContent').slideUp('normal');

        //IF THE NEXT SLIDE WASN'T OPEN THEN OPEN IT
        if ($(this).next().is(':hidden') == true) {

            //ADD THE ON CLASS TO THE BUTTON
            $(this).addClass('on');

            //OPEN THE SLIDE
            $(this).next().slideDown('normal');
        }

    });

	// Toggle sort icon
	$('.tab-content .acc').click(function () {
		$(this).find('i.sort').toggleClass('fa-sort-down fa-sort-up');
	});

	// prevent accordion slide
	$('.fas.fa-file-pdf').click(function (event) {
		event.stopPropagation();
	});
	
    //ADDS THE .OVER CLASS FROM THE STYLESHEET ON MOUSEOVER 
    $('.accordionButton').mouseover(function () {
        $(this).addClass('over');

     //ON MOUSEOUT REMOVE THE OVER CLASS
    }).mouseout(function () {
        $(this).removeClass('over');
    });
  
	// CLOSES ALL S ON PAGE LOAD
    $('.accordionContent').hide();

    //pressroom or collection search
    $('input:radio[name="optradio"]').change(
        function () {
            if (this.checked && this.value == 'collection') {
                $("form").attr("action", '/collections/All-products/');
                $(".search-area-input").attr("name", "q");
            } else if (this.checked && this.value == 'pressroom') {
                $("form").attr("action", '/pressroom/');
                $(".search-area-input").attr("name", "category");
            }
        });

// /Accordion Account


/********************************************************************************************************************
    Prevent scrolling if mobile menu is loaded
*******************************************************************************************************************

$('.navbar-toggler').click(function(e) {
e.preventDefault();
$('body').css('overflow', 'hidden');
});

$('.navbar-toggler').click(function(e) {
e.preventDefault();
$('body').css('overflow', 'auto');
});
*/


/********************************************************************************************************************
    Prevent scrolling if mobile menu is loaded
********************************************************************************************************************/

});

});

$(document).ready(function () {

    //$('.options').first().attr("selected", true); //selected always th first address
    //var selected = false;

    //$('.shipping-addresses').change( function () {

    //    //change the selection if another option is selected
    //});

    //$('.options').each(function () {

    //    var addressFound = $(this).data("id"); //id of the address
    //    var selectedAddress = $(this).attr("selected");

    //    if (selectedAddress === "selected" && typeof selectedAddress != "undefined") {
    //        selected = true;
    //    }
    //});
    
});