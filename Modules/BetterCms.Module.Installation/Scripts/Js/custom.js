$(window).on('load', function () {
    $("#nav").sticky({
        topSpacing: 0,
        zIndex: 9
    });
	/**/
    $('.mobile-button').click(function () {
        $('.mobile-menu-list').toggle('slow', function () {
            return false;
        });
    });

    $('.mobile-menu-list a').click(function () {
        $('.mobile-menu-list').toggle('slow', function () {
            return false;
        });
    });
	/**/
    $('#nav').onePageNav({
        currentClass: 'active',
        changeHash: false,
        scrollSpeed: 3000,
        scrollOffset: 30,
        scrollThreshold: 0.5,
        filter: ':not(.external)',
        easing: 'swing',
        begin: function () {},
        end: function () {},
        scrollChange: function ($currentListItem) {

        }
    });
});

$(function(){  
	createSticky($("#nav"));
});

function createSticky(sticky) {	
	if (typeof sticky !== "undefined") {
		var	pos = sticky.offset().top,
				win = $(window);			
		win.on("scroll", function() {
    		win.scrollTop() >= pos ? sticky.addClass("fixed") : sticky.removeClass("fixed");  					
		});			
	}
}

/***************************************************
2. SLICKER
***************************************************/
jQuery(document).ready(function ($) {
    /*
    	var newStickies = new stickyTitles(jQuery(".followMeBar"));		
    	newStickies.load();		
    	jQuery('.main').on("scroll", function() {
    		  newStickies.scroll();	
    	});
    */
    $('.slicker').slick({
        dots: false,
        infinite: true,
        speed: 1000,
        slidesToShow: 3,
        slidesToScroll: 3,
        autoplay: false,
        cssEase: 'ease',
        easing: 'linear',
        responsive: [{
            breakpoint: 1024,
            settings: {
                slidesToShow: 3,
                slidesToScroll: 3,
                infinite: true,
                dots: false
            }
        }, {
            breakpoint: 600,
            settings: {
                slidesToShow: 2,
                slidesToScroll: 2
            }
        }, {
            breakpoint: 480,
            settings: {
                slidesToShow: 1,
                slidesToScroll: 1
            }
        }]
    });

    $('.slicker1').slick({
        dots: false,
        infinite: true,
        speed: 300,
        slidesToShow: 1,
        slidesToScroll: 1,
        autoplay: false
            /*,
		responsive: [{
            breakpoint: 1024,
            settings: {
                slidesToShow: 3,
                slidesToScroll: 1,
                infinite: true,
                dots: true
            }
        }, {
            breakpoint: 600,
            settings: {
                slidesToShow: 2,
                slidesToScroll: 1
            }
        }, {
            breakpoint: 480,
            settings: {
                slidesToShow: 1,
                slidesToScroll: 1
            }
        }]*/
    });
	$('.slicker2').slick({
        dots: false,
        infinite: true,
        speed: 300,
        slidesToShow: 1,
        slidesToScroll: 1,
        autoplay: false
            /*,
		responsive: [{
            breakpoint: 1024,
            settings: {
                slidesToShow: 3,
                slidesToScroll: 1,
                infinite: true,
                dots: true
            }
        }, {
            breakpoint: 600,
            settings: {
                slidesToShow: 2,
                slidesToScroll: 1
            }
        }, {
            breakpoint: 480,
            settings: {
                slidesToShow: 1,
                slidesToScroll: 1
            }
        }]*/
    });
    $('.slicker3').slick({
        arrows: false,
        dots: true,
        infinite: true,
        speed: 3000,
        slidesToShow: 1,
        slidesToScroll: 1,
        autoplay: true,
        cssEase: 'ease',
        easing: 'linear',
        responsive: [{
            breakpoint: 1024,
            settings: {
                slidesToShow: 1,
                slidesToScroll: 1,
                infinite: true,
                dots: true
            }
        }, {
            breakpoint: 600,
            settings: {
                slidesToShow: 1,
                slidesToScroll: 1
            }
        }, {
            breakpoint: 480,
            settings: {
                slidesToShow: 1,
                slidesToScroll: 1
            }
        }]
    });
    $('form').h5Validate();	
	$(".slicker .slick-slide").css({ "display": "none" });
	function Timer() {
		$(".slicker .slick-slide").css({ "display": "block" });
		clearInterval(First);
	}
	var First = setInterval(function(){ Timer() }, 10);
});

/***************************************************
4. CONTACT FORM
***************************************************/
$(function () {
    $("#contact_button").click(function () {
        var contact_name = $("#contact_name").val();
        var contact_email = $("#contact_email").val();
        var contact_subject = $("#contact_subject").val();
        var contact_text = $("#contact_text").val();
        var emailReg = /^([a-zA-Z0-9_\.\-\+])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$/;
        var dataString = '&name=' + contact_name + '&email=' + contact_email + '&text=' + contact_text + '&subject=' + contact_subject;
        var go = true;
        var emailcheck = false;

        $('.name-error').removeClass('error-display');
        $('.email-error').removeClass('error-display');
        $('.topic-error').removeClass('error-display');
        $('.message-error').removeClass('error-display');
        $('.submit-wrapper').find('#contact_button').attr('value', 'Send Email');
        $('#contact_button').removeClass('skin-contact-done');

        if (contact_name == 'Name') {
            $('.name-error').addClass('error-display');
            go = false;
        }

        if (contact_name == '') {
            $('.name-error').addClass('error-display');
            go = false;
        }

        if (contact_email == 'Email') {
            $('.email-error').addClass('error-display');
            go = false;
        }

        if (contact_email == '') {
            $('.email-error').addClass('error-display');
            go = false;
        } else {
            if (emailReg.test(contact_email)) {
                emailcheck = true;
            } else {
                $('.email-error').addClass('error-display');
                go = false;
            }
        }

        if (contact_subject == 'Subject') {
            $('.topic-error').addClass('error-display');
            go = false;
        }

        if (contact_subject == '') {
            $('.topic-error').addClass('error-display');
            go = false;
        }

        if (contact_text == 'Email content') {
            $('.message-error').addClass('error-display');
            go = false;
        }

        if (contact_text == '') {
            $('.message-error').addClass('error-display');
            go = false;
        }

        if (go == false) {
            return false;
        }

        $.ajax({
            type: "POST",
            url: "email.php",
            data: dataString,
            success: function () {
                $('.submit-wrapper').find('#contact_button').attr('value', 'Thank you!');
                $('#contact_button').addClass('skin-contact-done');
            }
        });
        $('.contact').find('form')[0].reset();
        return false;
    });
});

/***************************************************
5. SCROLL TO
***************************************************/

function ScrollTo(id) {
    $('html,body').animate({
        scrollTop: $("#" + id).offset().top
    }, 3000);
}

/***********MENU NEVIGATION********************************
var menuNavigation = (function(){    
    jQuery( '#nav li:has(ul)' ).doubleTapToGo();
});
/********************************************************/



function slickReset() {
    //$('.slicker').slickGoTo(0);
    //$('.slicker1').slickGoTo(0);
}


$(function () {
    var $c = $('#carousel'),
        $w = $(window);

    $c.carouFredSel({
        responsive: true,
        align: true,
        items: {

            height: '50%',
            visible: {
                min: 1,
                max: 5
            }
        },
        scroll: {
            items: 1,
            duration: 8000,
            timeoutDuration: 0,
            easing: 'linear',
            pauseOnHover: 'immediate',
            fx: 'directscroll'
        }
    });
});


function validateUsername(obj) {
    obj.value = obj.value.trim();
}

function validateMessage() {
    $("#message").val($.trim($("#message").val()));
}

$('input, textarea').placeholder();

$(function () {
    var $c = $('#patner'),
        $w = $(window);

    $c.carouFredSel({
        responsive: true,
        align: true,
        items: {

            height: '100%',
            visible: {
                min: 2,
                max: 2
            }
        },
        scroll: {
            items: 2,
            duration: 8000,
            timeoutDuration: 0,
            easing: 'linear',
            pauseOnHover: 'immediate',
            fx: 'directscroll'
        }
    });
});

$(document).mouseup(function (e)
      {
			var container = $("#nav");
			if (!container.is(e.target) // if the target of the click isn't the container...
				&& container.has(e.target).length === 0) // ... nor a descendant of the container
			{
				if($('#menuOption').css('display') == 'block')
				{
					if (window.matchMedia("(max-width:959px)").matches) 
					{
						
					
				$("#menuOption").css("display","none");
					}
			//	($("#nav").removeClass("active"));
				}
				
					
			}
	
	  });
	  
	  $(window).resize(function() 
	  {

		           if (window.matchMedia("(min-width:960px)").matches) 
					 {		
						$("#menuOption").css("display","block");
					 }
					 
					 else
					 {
						
						 $("#menuOption").css("display","none");
					 }
	 });
	 
	 $( document ).ready(function() {
			//$('.g-recaptcha div:first').css('width', '100%');
			//$('.rc-anchor-normal').css('width', '100%');
			//$('.rc-anchor-logo-portrait').css('margin-left', '0');
    //console.log( "ready!" );
});
	