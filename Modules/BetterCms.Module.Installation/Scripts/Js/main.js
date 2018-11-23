(function ($) {
    "use strict";

    jQuery(document).ready(function ($) {


        $(".embed-responsive iframe").addClass("embed-responsive-item");
        $(".carousel-inner .item:first-child").addClass("active");
        $('[data-toggle="tooltip"]').tooltip();


        //Numaric Counter
        $('.counter').counterUp({
            delay: 10,
            time: 1000
        });

        //Portfolio Popup
        $('.magnific-popup').magnificPopup({ type: 'image' });

        //Smooth Scroll
        smoothScroll.init();

        //Countdown
        (function () {

            $("#countdown").countdown({
                date: "31 december 2016 12:00:00",
                format: "on"
            });

        }());


    });


    jQuery(window).load(function () {

        //Preloader
        $('.preloader').delay(500).fadeOut('slow');
        $('body').delay(500).css({ 'overflow': 'visible' });


        //Portfolio container			
        var $container = $('.portfolioContainer');
        $container.isotope({
            filter: '*',
            animationOptions: {
                duration: 750,
                easing: 'linear',
                queue: false
            }
        });

        $('.portfolioFilter a').on('click', function () {
            $('.portfolioFilter a').removeClass('current');
            $(this).addClass('current');

            var selector = $(this).attr('data-filter');
            $container.isotope({
                filter: selector,
                animationOptions: {
                    duration: 750,
                    easing: 'linear',
                    queue: false
                }
            });
            return false;
        });



    });

    //Wow js
    new WOW().init();


}(jQuery));