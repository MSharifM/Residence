// Mobile Menu
const mobileMenuToggle = document.getElementById("mobileMenuToggle")
const navMenu = document.getElementById("navMenu")

if (mobileMenuToggle) {
    mobileMenuToggle.addEventListener("click", () => {
        navMenu.classList.toggle("active")
    })
}

let currentSlide = 0
const slides = document.querySelectorAll(".slide")
const prevBtn = document.getElementById("prevBtn")
const nextBtn = document.getElementById("nextBtn")
const sliderDots = document.getElementById("sliderDots")
const priceAmount = document.getElementById("just-price")

// DOTS
slides.forEach((_, index) => {
    const dot = document.createElement("div")
    dot.classList.add("dot")
    if (index === 0) dot.classList.add("active")
    dot.addEventListener("click", () => goToSlide(index))
    sliderDots.appendChild(dot)
})

const dots = document.querySelectorAll(".dot")

function showSlide(n) {
    slides.forEach((slide) => slide.classList.remove("active"))
    dots.forEach((dot) => dot.classList.remove("active"))

    if (n >= slides.length) currentSlide = 0
    if (n < 0) currentSlide = slides.length - 1

    slides[currentSlide].classList.add("active")
    dots[currentSlide].classList.add("active")
}

function nextSlide() {
    currentSlide++
    showSlide(currentSlide)
}

function prevSlide() {
    currentSlide--
    showSlide(currentSlide)
}

function goToSlide(n) {
    currentSlide = n
    showSlide(currentSlide)
}

if (prevBtn) prevBtn.addEventListener("click", prevSlide)
if (nextBtn) nextBtn.addEventListener("click", nextSlide)

// SLIDERS
setInterval(nextSlide, 3000)

const PRICE_PER_NIGHT = priceAmount.innerHTML
console.log(PRICE_PER_NIGHT)
const today = new Date().toISOString().split("T")[0]

const checkInDesktop = document.getElementById("checkIn")
const checkOutDesktop = document.getElementById("checkOut")
const checkInMobile = document.getElementById("checkInMobile")
const checkOutMobile = document.getElementById("checkOutMobile")

    // MIN DATE
    ;[checkInDesktop, checkOutDesktop, checkInMobile, checkOutMobile].forEach((input) => {
        if (input) {
            input.min = today
            input.addEventListener("change", handleDateChange)
        }
    })

function handleDateChange(e) {
    if (e.target === checkInDesktop && checkInMobile) {
        checkInMobile.value = e.target.value
    } else if (e.target === checkInMobile && checkInDesktop) {
        checkInDesktop.value = e.target.value
    } else if (e.target === checkOutDesktop && checkOutMobile) {
        checkOutMobile.value = e.target.value
    } else if (e.target === checkOutMobile && checkOutDesktop) {
        checkOutDesktop.value = e.target.value
    }

    calculatePrice()
}

function calculatePrice() {
    const checkIn = checkInDesktop?.value || checkInMobile?.value
    const checkOut = checkOutDesktop?.value || checkOutMobile?.value

    if (checkIn && checkOut) {
        const checkInDate = new Date(checkIn)
        const checkOutDate = new Date(checkOut)

        if (checkOutDate <= checkInDate) {
            alert("تاریخ خروج باید بعد از تاریخ ورود باشد")
                ;[checkOutDesktop, checkOutMobile].forEach((input) => {
                    if (input) input.value = ""
                })
            resetCalculation()
            return
        }

        const nights = Math.ceil((checkOutDate - checkInDate) / (1000 * 60 * 60 * 24))
        const total = nights * PRICE_PER_NIGHT

        const nightsDesktop = document.getElementById("nights")
        const totalDesktop = document.getElementById("total")
        if (nightsDesktop) nightsDesktop.textContent = `${nights} شب`
        if (totalDesktop) totalDesktop.textContent = `${total.toLocaleString("fa-IR")} تومان`

        const nightsMobile = document.getElementById("nightsMobile")
        const totalMobile = document.getElementById("totalMobile")
        if (nightsMobile) nightsMobile.textContent = `${nights} شب`
        if (totalMobile) totalMobile.textContent = `${total.toLocaleString("fa-IR")} تومان`
    } else {
        resetCalculation()
    }
}

function resetCalculation() {
    const nightsElements = [document.getElementById("nights"), document.getElementById("nightsMobile")]
    const totalElements = [document.getElementById("total"), document.getElementById("totalMobile")]

    nightsElements.forEach((el) => {
        if (el) el.textContent = "-"
    })

    totalElements.forEach((el) => {
        if (el) el.textContent = "-"
    })
}

// RESERVE
function handleReservation() {
    const checkIn = checkInDesktop?.value || checkInMobile?.value
    const checkOut = checkOutDesktop?.value || checkOutMobile?.value

    if (!checkIn || !checkOut) {
        alert("لطفاً تاریخ ورود و خروج را انتخاب کنید")
        return
    }

    const checkInDate = new Date(checkIn)
    const checkOutDate = new Date(checkOut)
    const nights = Math.ceil((checkOutDate - checkInDate) / (1000 * 60 * 60 * 24))
    const total = nights * PRICE_PER_NIGHT
}
// NOTACTIVE ACCOMMODATION
function updateReserveButton() {
    const statusText = document.getElementById("notactive-btn");
    const btnReserve = document.querySelector(".btn-reserve");

    if (!statusText || !btnReserve) {
        console.error("عناصر مورد نظر یافت نشدند!");
        return;
    }

    if (statusText.textContent.trim() === "غیرفعال") {
        btnReserve.classList.remove("btn-reserve");
        btnReserve.classList.add("notactive");
    } else {
        btnReserve.classList.remove("notactive");
        btnReserve.classList.add("btn-reserve");
    }
}

// اجرای تابع بعد از لود صفحه
document.addEventListener("DOMContentLoaded", function () {
    updateReserveButton();
});

window.addEventListener("DOMContentLoaded", updateReserveButton);
someElement.addEventListener("change", updateReserveButton);