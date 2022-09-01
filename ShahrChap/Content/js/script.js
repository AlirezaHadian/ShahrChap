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
/* Order */
const prevBtns = document.querySelectorAll(".btn-prev");
const nextBtns = document.querySelectorAll(".btn-next");
const progress = document.getElementById("progress");
const formSteps = document.querySelectorAll(".form-step");
const progressSteps = document.querySelectorAll(".progress-step");

let formStepsNum = 0;

nextBtns.forEach((btn) => {
  btn.addEventListener("click", () => {
    formStepsNum++;
    updateFormSteps();
    updateProgressbar();
  });
});

prevBtns.forEach((btn) => {
  btn.addEventListener("click", () => {
    formStepsNum--;
    updateFormSteps();
    updateProgressbar();
  });
});

function updateFormSteps() {
  formSteps.forEach((formStep) => {
    formStep.classList.contains("form-step-active") &&
      formStep.classList.remove("form-step-active");
  });

  formSteps[formStepsNum].classList.add("form-step-active");
}

function updateProgressbar() {
  progressSteps.forEach((progressStep, idx) => {
    if (idx < formStepsNum + 1) {
      progressStep.classList.add("progress-step-active");
    } else {
      progressStep.classList.remove("progress-step-active");
    }
  });

  const progressActive = document.querySelectorAll(".progress-step-active");

  progress.style.width =
    ((progressActive.length - 1) / (progressSteps.length - 1)) * 100 + "%";
}
/* Preloader */
window.addEventListener('load', ()=> document.querySelector('.preloader').classList.add('hide-preloader'));

/*Single product*/
const imgs = document.querySelectorAll('.img-select a');
const imgBtns = [...imgs];
let imgId = 1;

imgBtns.forEach((imgItem) => {
    imgItem.addEventListener('click', (event) => {
        event.preventDefault();
        imgId = imgItem.dataset.id;
        slideImage();
    });
});

function slideImage(){
    const displayWidth = document.querySelector('.img-showcase img:first-child').clientWidth;

    document.querySelector('.img-showcase').style.transform = `translateX(${+ (imgId - 1) * displayWidth}px)`;
}

window.addEventListener('resize', slideImage);

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
    setTimeout(function () { document.getElementById("resendCode").disabled = false; }, 20000);
}

window.onload = function () {
    $('.js-timer').html("0:20");
    countdown();
    enableButton();
};

function resendCodeBtn() {
    $('.js-timer').text("0:20");
    countdown();
    enableButton();
};
