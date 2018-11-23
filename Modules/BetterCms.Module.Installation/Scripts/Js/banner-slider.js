var $interval;
var is_interval_running = false;

function renderSlider() {
    var $slider = $('.slider');
    var $slide = 'li';
    var $transition_time = 1000;
    var $time_between_slides = 5000;

    $interval = setInterval(change_slide, $transition_time + $time_between_slides);

    $(window).focus(function () {
        clearInterval($interval);
        if (!is_interval_running)
            $interval = setInterval(change_slide, $transition_time + $time_between_slides);
    }).blur(function () {
        clearInterval($interval);
        is_interval_running = false;
    });

    slides().fadeOut();
    slides().first().addClass('active');
    slides().first().css('display', 'block');
    slides().first().animate({
        opacity: 1
    }, function () {
        $(this).css('position', 'relative');
    });

    function slides() {
        return $slider.children($slide);
    }

    function change_slide() {
        is_interval_running = true; //Optional
        var $i = $slider.find($slide + '.active').index();
        hideSlide($i);
        if (slides().length == $i + 1) $i = -1;
        showSlide($i + 1);
    }

    function hideSlide($i) {
        slides().eq($i).removeClass('active');
        $(".bullet").eq($i).removeClass('active');
        slides().eq($i).animate({
            opacity: 0
        }, function () {
            $(this).css('position', 'absolute');
            $(this).find(".slide-text-wrapper").css('display', 'none');
        });
    }

    function showSlide($j) {
        slides().eq($j).addClass('active');
        $(".bullet").eq($j).addClass('active');
        slides().eq($j).show().animate({
            opacity: 1
        }, function () {
            $(this).css('position', 'relative');
            $(this).find(".slide-text-wrapper").css('display', 'block');
        });
    }

    $(".slider-prev").click(function () {
        var $i = $slider.find($slide + '.active').index();
        hideSlide($i);
        var $j = $i - 1;
        if ($j < 0) { $j = 9; }
        showSlide($j);
        clearInterval($interval);
    });

    $(".slider-next").click(function () {
        var $i = $slider.find($slide + '.active').index();
        hideSlide($i);
        var $j = $i + 1;
        if ($j > 9) { $j = 0; }
        showSlide($j);
        clearInterval($interval);
    });

    $(".bullet").click(function () {
        var $i = $slider.find($slide + '.active').index();
        hideSlide($i);
        var $j = $(this).index();
        showSlide($j);
        clearInterval($interval);
    });

    $('.slide').click(function () {
        clearInterval($interval);
    });
}

function renderSlideThumb() {
    var bannerSlider = $("#bannerSlider");
    $('.slider-bullets li', bannerSlider).hover(function () {
        if ($('.slide-thumb', this).length) {
            var slideThumbOuterWidth = $('.slide-thumb', this).outerWidth(),
                slideThumbOuterHeight = $('.slide-thumb', this).outerHeight(),
                slideNavBulletWidth = $(this).outerWidth();
            $('.slide-thumb', this).show().css({
                'top': '-' + slideThumbOuterHeight + 'px',
                'left': '-' + (slideThumbOuterWidth - slideNavBulletWidth) / 2 + 'px'
            }).animate({
                'opacity': 1,
                'margin-top': '-3px'
            }, 200);
            $('.thumb-arrow', this).show().animate({
                'opacity': 1,
                'margin-top': '-3px'
            }, 200);
        }
    }, function () {
        $('.slide-thumb', this).animate({
            'margin-top': '-20px',
            'opacity': 0
        }, 200, function () {
            $(this).css({
                marginTop: '5px'
            }).hide();
        });
        $('.thumb-arrow', this).animate({
            'margin-top': '-20px',
            'opacity': 0
        }, 200, function () {
            $(this).css({
                marginTop: '5px'
            }).hide();
        });
    });
}

$(document).ready(function () {
    renderSlider();
    renderSlideThumb();
});