(function ($) {
    "use strict";

    jQuery(document).ready(function ($) {        

        //Portfolio Popup
        $('.magnific-popup').magnificPopup({ type: 'image' }); 
    });


    jQuery(window).load(function () {      

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

}(jQuery));