(function ($) {
"use strict";

/*------------------------------------
		Preloader
	--------------------------------------*/
	$(window).on('load', function () {
		$('#preloader').delay(350).fadeOut('slow');
		$('body').delay(350).css({'overflow': 'visible'});
	});

// meanmenu
$('#mobile-menu').meanmenu({
	meanMenuContainer: '.mobile-menu',
	meanScreenWidth: "991"
});

	/*------------------------------------
		Mobile Menu
	--------------------------------------*/

	$('#mobile-menu-active').metisMenu();

	$('#mobile-menu-active .has-dropdown > a').on('click', function (e) {
		e.preventDefault();
	});

	$(".hamburger-menu > a").on("click", function (e) {
		e.preventDefault();
		$(".slide-bar").toggleClass("show");
		$("body").addClass("on-side");
		$('.body-overlay').addClass('active');
		$(this).addClass('active');
	});

	$(".close-mobile-menu > a").on("click", function (e) {
		e.preventDefault();
		$(".slide-bar").removeClass("show");
		$("body").removeClass("on-side");
		$('.body-overlay').removeClass('active');
		$('.hamburger-menu > a').removeClass('active');
	});

	$('.body-overlay').on('click', function () {
		$(this).removeClass('active');
		$(".slide-bar").removeClass("show");
		$("body").removeClass("on-side");
		$('.hamburger-menu > a').removeClass('active');
	});

/* Search
	-------------------------------------------------------*/
	var $searchWrap = $('.search-wrap');
	var $navSearch = $('.nav-search');
	var $searchClose = $('#search-close');

	$('.search-trigger').on('click', function (e) {
		e.preventDefault();
		$searchWrap.animate({ opacity: 'toggle' }, 500);
		$navSearch.add($searchClose).addClass("open");
	});

	$('.search-close').on('click', function (e) {
		e.preventDefault();
		$searchWrap.animate({ opacity: 'toggle' }, 500);
		$navSearch.add($searchClose).removeClass("open");
	});

	function closeSearch() {
		$searchWrap.fadeOut(200);
		$navSearch.add($searchClose).removeClass("open");
	}

	$(document.body).on('click', function (e) {
		closeSearch();
	});

	$(".search-trigger, .main-search-input").on('click', function (e) {
		e.stopPropagation();
	});


//sidenav
 //extra_btn
 $('.extra_btn').click(function(){
	$('.extra_info').addClass('info-open');
});
$('.close_icon').click(function(){
	$('.extra_info').removeClass('info-open');
});

//sticky-menu
$(window).on('scroll', function () {
	var scroll = $(window).scrollTop();
	if (scroll < 200) {
		$(".main-header-area").removeClass("sticky");
	} else {
		$(".main-header-area").addClass("sticky");
	}
});

   // -------------------------- scroll animate
   var links = $('a.scroll-target');
   links.on('click', function() {
	   if (location.pathname.replace(/^\//,'') == this.pathname.replace(/^\//,'') || location.hostname == this.hostname) {
	   var target = $(this.hash);
		   target = target.length ? target : $('[name=' + this.hash.slice(1) +']');
		   if (target.length) {
		   $('html,body').animate({
			   scrollTop: target.offset().top - 75,
			   }, 1000);
			   return false;
		   }
	   }
   });


// mainSlider
function mainSlider() {
	var BasicSlider = $('.slider-active');
	BasicSlider.on('init', function (e, slick) {
		var $firstAnimatingElements = $('.single-slider:first-child').find('[data-animation]');
		doAnimations($firstAnimatingElements);
	});
	BasicSlider.on('beforeChange', function (e, slick, currentSlide, nextSlide) {
		var $animatingElements = $('.single-slider[data-slick-index="' + nextSlide + '"]').find('[data-animation]');
		doAnimations($animatingElements);
	});
	BasicSlider.slick({
		autoplay: false,
		autoplaySpeed: 10000,
		dots: false,
		fade: true,
		arrows: true,
		prevArrow:'<button type="button" class="slick-prev"><i class="far fa-long-arrow-left"></i></button>',
		nextArrow:'<button type="button" class="slick-next"><i class="far fa-long-arrow-right"></i></button>',
		responsive: [
			{ breakpoint: 767, settings: { dots: false, arrows: false } }
		]
	});

	function doAnimations(elements) {
		var animationEndEvents = 'webkitAnimationEnd mozAnimationEnd MSAnimationEnd oanimationend animationend';
		elements.each(function () {
			var $this = $(this);
			var $animationDelay = $this.data('delay');
			var $animationType = 'animated ' + $this.data('animation');
			$this.css({
				'animation-delay': $animationDelay,
				'-webkit-animation-delay': $animationDelay
			});
			$this.addClass($animationType).one(animationEndEvents, function () {
				$this.removeClass($animationType);
			});
		});
	}
}
mainSlider();


//team-active-slider
  $('.team-active').slick({
	infinite: true,
	dots: true,
	arrows: false,
	autoplay:true,
	autoplaySpeed:3000,
	slidesToShow: 3,
	slidesToScroll: 2,
	responsive: [
		{
		  breakpoint: 1250,
		  settings: {
			slidesToShow: 2,
		  }
		},
		{
		  breakpoint: 768,
		  settings: {
			slidesToShow: 1,
		  }
		},
	  ]
  });


//testimonial-slider
  $('.testimonial-active').slick({
	infinite: true,
	dots: false,
	arrows: false,
	autoplay:true,
	autoplaySpeed:3000,
	slidesToShow: 2,
	slidesToScroll: 2,
	responsive: [
		{
		  breakpoint: 1100,
		  settings: {
			slidesToShow: 1,
		  }
		},
	  ]
  });

//testimonial-slider
  $('.news-active').slick({
	loop:false,
	infinite: true,
	dots: false,
	arrows: true,
	prevArrow:'<button type="button" class="slick-prev"><i class="fal fa-arrow-left"></i></button>',
	nextArrow:'<button type="button" class="slick-next"><i class="fal fa-arrow-right"></i></button>',
	autoplay:true,
	autoplaySpeed:3000,
	slidesToShow: 3,
	slidesToScroll: 2,
	responsive: [
		{
		  breakpoint: 1300,
		  settings: {
			slidesToShow: 2,
		  }
		},
		{
		  breakpoint: 700,
		  settings: {
			slidesToShow: 1,
		  }
		},
	  ]
  });


//testtimonial-item-active2
  $('.brand-active').slick({
	infinite: true,
	arrows: false,
	slidesToShow: 4,
	slidesToScroll: 1,
	autoplay: true,
  	autoplaySpeed: 2000,
	responsive: [
	  {
		breakpoint: 1000,
		settings: {
		  slidesToShow: 3
		}
	  },
	  {
		breakpoint: 800,
		settings: {
		  slidesToShow: 2
		}
	  },
	  {
		breakpoint: 500,
		settings: {
		  slidesToShow: 1
		}
	  }
	]
  });

//testtimonial-item-active2
  $('.brand-active-02').slick({
	infinite: true,
	arrows: false,
	slidesToShow: 5,
	slidesToScroll: 1,
	autoplay: true,
  	autoplaySpeed: 2000,
	responsive: [
	  {
		breakpoint: 1200,
		settings: {
		  slidesToShow: 4
		}
	  },
	  {
		breakpoint: 1000,
		settings: {
		  slidesToShow: 3
		}
	  },
	  {
		breakpoint: 800,
		settings: {
		  slidesToShow: 2
		}
	  },
	  {
		breakpoint: 500,
		settings: {
		  slidesToShow: 1
		}
	  }
	]
  });

// owlCarousel
$('.owlcarousel').owlCarousel({
    loop:true,
    margin:0,
	items:2,
	navText:['<i class="fa fa-angle-left"></i>','<i class="fa fa-angle-right"></i>'],
    nav:true,
	dots:false,
    responsive:{
        0:{
            items:1
        },
        767:{
            items:2
        },
        992:{
            items:2
        }
    }
})

/* magnificPopup img view */
$('.popup-image').magnificPopup({
	type: 'image',
	gallery: {
	  enabled: true
	}
});


/* magnificPopup video view */
$('.popup-video').magnificPopup({
	type: 'iframe'
});

// active-class
$('.feature_box', '.features-item').on('mouseenter', function () {
	$(this).addClass('active').parent().siblings().find('.feature_box', '.features-item').removeClass('active');
})

 //extra_btn
 $('.extra_info').click(function(){
	$('.extra_info').addClass('info-open');
});
$('.close_icon').click(function(){
	$('.extra_info').removeClass('info-open');
});

// isotop
$('.grid').imagesLoaded( function() {
	// init Isotope
	var $grid = $('.grid').isotope({
	itemSelector: '.grid-item',
	percentPosition: true,
	masonry: {
		// use outer width of grid-sizer for columnWidth
		columnWidth: 1,
		gutter: 0
	}
	});
	// filter items on button click
	$('.portfolio-menu').on( 'click', 'button', function() {
		var filterValue = $(this).attr('data-filter');
		$grid.isotope({ filter: filterValue });
	});
});

//for menu active class
$('.portfolio-menu button').on('click', function(event) {
	$(this).siblings('.active').removeClass('active');
	$(this).addClass('active');
	event.preventDefault();
});

//counter
// $('.counter').counterUp({
//     delay: 10,
//     time: 3000
// });



// scrollToTop
$.scrollUp({
	scrollName: 'scrollUp', // Element ID
	topDistance: '300', // Distance from top before showing element (px)
	topSpeed: 500, // Speed back to top (ms)
	animation: 'fade', // Fade, slide, none
	animationInSpeed: 300, // Animation in speed (ms)
	animationOutSpeed: 300, // Animation out speed (ms)
	scrollText: '<i class="fas fa-chevron-double-up"></i>', // Text for element
	activeOverlay: false, // Set CSS color to display scrollUp active point, e.g '#00FFFF'
});

// WOW active
new WOW().init();


//nice-select
$(document).ready(function() {
	$('select').niceSelect();
  });   

})(jQuery);

// 27. Brand Logo
// --------------------------------------------------------- 
if($('.ltn__brand-logo-active').length){
	$('.ltn__brand-logo-active').slick({
		rtl: false,
		arrows: false,
		dots: false,
		infinite: true,
		speed: 300,
		slidesToShow: 4,
		slidesToScroll: 2,
		prevArrow: '<a class="slick-prev"><i class="icon-arrow-left" alt="Arrow Icon"></i></a>',
		nextArrow: '<a class="slick-next"><i class="icon-arrow-right" alt="Arrow Icon"></i></a>',
		responsive: [
			{
				breakpoint: 992,
				settings: {
					slidesToShow: 4,
					slidesToScroll: 1
				}
			},
			{
				breakpoint: 768,
				settings: {
					slidesToShow: 3,
					slidesToScroll: 1,
					arrows: false,
				}
			},
			{
				breakpoint: 580,
				settings: {
					slidesToShow: 2,
					slidesToScroll: 1
				}
			}
		]
	});
};