/*Mobile navigation*/
const mainNavigation = document.querySelector(".main-navigation");
const overlay = mainNavigation.querySelector(".nav-overlay");
const toggler = mainNavigation.querySelector(".navbar-toggler");

const openSideNav = () => mainNavigation.classList.add("active");
const closeSideNav = () => mainNavigation.classList.remove("active");

toggler.addEventListener("click", openSideNav);
overlay.addEventListener("click", closeSideNav);

document.addEventListener("swiped-left", openSideNav);
document.addEventListener("swiped-right", closeSideNav);

/*Owl carousel*/
$(".owl-carousel").owlCarousel({
  rtl: true,
  loop: false,
  margin: 10,
  nav: false,
  responsive: {
    0: {
      items: 2,
    },
    660: {
      items: 3,
    },
    1000: {
      items: 5,
    },
  },
});

/* Preloader */
window.addEventListener('load', ()=> document.querySelector('.preloader').classList.add('hide-preloader'));

/*Single product*/

 
/*OTP*/
$(".otp-input").keyup(function () {
    if (this.value.length == this.maxLength) {
        $(this).next('.otp-input').focus();
    }
});
$(".otp-input").keyup(function () {
    var key = event.keyCode || event.charCode;
    if (key == 8 || key == 46) {
        $(this).prev("input[type='text']").focus();
    }
})
/*OTP expire countdown*/

var interval;
function countdown() {
    clearInterval(interval);
    interval = setInterval(function () {
        var timer = $('.js-timer').html();
        timer = timer.split(':');
        var minutes = timer[0];
        var seconds = timer[1];
        seconds -= 1;
        if (minutes < 0) return;
        else if (seconds < 0 && minutes != 0) {
            minutes -= 1;
            seconds = 59;
        }
        else if (seconds < 10 && length.seconds != 2) seconds = '0' + seconds;

        $('.js-timer').html(minutes + ':' + seconds);

        if (minutes == 0 && seconds == 0) clearInterval(interval);
    }, 1000);
}

function enableButton(id) {
    document.getElementById("resendCode").disabled = true;
    setTimeout(function () { document.getElementById("resendCode").disabled = false; }, 50000);
}

window.onload = function () {
    $('.js-timer').html("0:50");
    countdown();
    enableButton();
};

function resendCodeBtn() {
    $('.js-timer').text("0:50");
    countdown();
    enableButton();
};
