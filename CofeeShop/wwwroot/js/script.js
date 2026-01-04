// Dummy Data for Accommodations
const accommodationsData = [
    {
        id: 1,
        title: "ویلا لوکس شمال",
        location: "رامسر، مازندران",
        price: "۳,۵۰۰,۰۰۰",
        rating: "۴.۸",
        image: "/images/luxury-villa-north-iran.jpg",
    },
    {
        id: 2,
        title: "کلبه جنگلی",
        location: "نوشهر، مازندران",
        price: "۲,۲۰۰,۰۰۰",
        rating: "۴.۶",
        image: "/images/forest-cabin-iran.jpg",
    },
    {
        id: 3,
        title: "اقامتگاه سنتی",
        location: "یزد، یزد",
        price: "۱,۸۰۰,۰۰۰",
        rating: "۴.۹",
        image: "/images/traditional-house-yazd.jpg",
    },
    {
        id: 4,
        title: "ویلا ساحلی",
        location: "کیش، هرمزگان",
        price: "۴,۰۰۰,۰۰۰",
        rating: "۴.۷",
        image: "/images/beach-villa-kish.jpg",
    },
    {
        id: 5,
        title: "کلبه کوهستانی",
        location: "دیزین، البرز",
        price: "۲,۸۰۰,۰۰۰",
        rating: "۴.۵",
        image: "/images/mountain-cabin-dizin.jpg",
    },
    {
        id: 6,
        title: "اقامتگاه باغ",
        location: "شیراز، فارس",
        price: "۲,۵۰۰,۰۰۰",
        rating: "۴.۸",
        image: "/images/garden-house-shiraz.jpg",
    },
];

const popularData = [
    {
        id: 8,
        title: "ویلا استخردار",
        location: "چالوس، مازندران",
        price: "۴,۲۰۰,۰۰۰",
        rating: "۴.۹",
        image: "/images/villa-with-pool-chalus.jpg",
    },
    {
        id: 7,
        title: "پنت‌هاوس لوکس",
        location: "تهران، تهران",
        price: "۵,۵۰۰,۰۰۰",
        rating: "۵.۰",
        image: "/images/luxury-penthouse-tehran.jpg",
    },
    {
        id: 8,
        title: "ویلا استخردار",
        location: "چالوس، مازندران",
        price: "۴,۲۰۰,۰۰۰",
        rating: "۴.۹",
        image: "/images/villa-with-pool-chalus.jpg",
    },
    {
        id: 9,
        title: "بوم‌گردی سنتی",
        location: "اصفهان، اصفهان",
        price: "۱,۵۰۰,۰۰۰",
        rating: "۴.۷",
        image: "/images/traditional-eco-lodge-isfahan.jpg",
    },
    {
        id: 8,
        title: "ویلا استخردار",
        location: "چالوس، مازندران",
        price: "۴,۲۰۰,۰۰۰",
        rating: "۴.۹",
        image: "/images/villa-with-pool-chalus.jpg",
    },
];

const recommendedData = [
    {
        id: 10,
        title: "ویلا مدرن",
        location: "لواسان، تهران",
        price: "۳,۰۰۰,۰۰۰",
        rating: "۴.۶",
        image: "/images/modern-villa-lavasan.jpg",
    },
    {
        id: 11,
        title: "سوئیت دربستی",
        location: "کرج، البرز",
        price: "۲,۰۰۰,۰۰۰",
        rating: "۴.۴",
        image: "/images/private-suite-karaj.jpg",
    },
    {
        id: 12,
        title: "کلبه رویایی",
        location: "ماسال، گیلان",
        price: "۲,۷۰۰,۰۰۰",
        rating: "۴.۸",
        image: "/images/dream-cottage-masal.jpg",
    },
    {
        id: 13,
        title: "اقامتگاه تاریخی",
        location: "کاشان، اصفهان",
        price: "۲,۳۰۰,۰۰۰",
        rating: "۴.۹",
        image: "/images/historical-house-kashan.jpg",
    },
];

const exclusiveData = [
    {
        id: 14,
        title: "ویلا پنج ستاره",
        location: "رامسر، مازندران",
        price: "۸,۰۰۰,۰۰۰",
        rating: "۵.۰",
        image: "/images/five-star-villa-ramsar.jpg",
    },
    {
        id: 15,
        title: "قصر شاهانه",
        location: "شمیرانات، تهران",
        price: "۱۰,۰۰۰,۰۰۰",
        rating: "۵.۰",
        image: "/images/royal-palace-shemiranat.jpg",
    },
    {
        id: 16,
        title: "ویلا ساحلی لوکس",
        location: "نوشهر، مازندران",
        price: "۷,۵۰۰,۰۰۰",
        rating: "۴.۹",
        image: "/images/villa-with-pool-chalus.jpg",
    },
];

// Function to create accommodation card
function createAccommodationCard(data) {
    return `
        <div class="accommodation-card">
            <img src="${data.image}" alt="${data.title}" class="card-image">
            <div class="card-content">
                <h3 class="card-title">${data.title}</h3>
                <p class="card-location">${data.location}</p>
                <div class="card-footer">
                    <div class="card-price">${data.price} <span>تومان / شب</span></div>
                    <div class="card-rating">⭐ ${data.rating}</div>
                </div>
            </div>
        </div>
    `;
}

// Function to create exclusive card
function createExclusiveCard(data) {
    return `
        <div class="exclusive-card">
            <img src="${data.image}" alt="${data.title}" class="card-image">
            <div class="card-content">
                <h3 class="card-title">${data.title}</h3>
                <p class="card-location">${data.location}</p>
                <div class="card-footer">
                    <div class="card-price">${data.price} <span>تومان / شب</span></div>
                    <div class="card-rating">⭐ ${data.rating}</div>
                </div>
            </div>
        </div>
    `;
}

// Render all accommodations
function renderAccommodations() {
    const accommodationsGrid = document.getElementById("accommodationsGrid");
    accommodationsGrid.innerHTML = accommodationsData
        .map((item) => createAccommodationCard(item))
        .join("");
}

// Render popular accommodations
function renderPopular() {
    const popularGrid = document.getElementById("popularGrid");
    popularGrid.innerHTML = popularData
        .map((item) => createPopularCard(item))
        .join("");
}

// Render recommended accommodations
function renderRecommended() {
    const recommendedGrid = document.getElementById("recommendedGrid");
    recommendedGrid.innerHTML = recommendedData
        .map((item) => createAccommodationCard(item))
        .join("");
}

// Render exclusive accommodations
function renderExclusive() {
    const exclusiveGrid = document.getElementById("exclusiveGrid");
    exclusiveGrid.innerHTML = exclusiveData
        .map((item) => createExclusiveCard(item))
        .join("");
}

// Mobile menu toggle
const mobileMenuToggle = document.getElementById("mobileMenuToggle");
const navMenu = document.getElementById("navMenu");

mobileMenuToggle.addEventListener("click", () => {
    navMenu.classList.toggle("active");
});

// Close mobile menu when clicking outside
document.addEventListener("click", (e) => {
    if (!navMenu.contains(e.target) && !mobileMenuToggle.contains(e.target)) {
        navMenu.classList.remove("active");
    }
});

// Sticky header effect
const header = document.getElementById("header");
let lastScroll = 0;

window.addEventListener("scroll", () => {
    const currentScroll = window.pageYOffset;

    if (currentScroll > 100) {
        header.style.boxShadow = "0 4px 20px rgba(0, 0, 0, 0.15)";
    } else {
        header.style.boxShadow = "0 2px 10px rgba(0, 0, 0, 0.1)";
    }

    lastScroll = currentScroll;
});

// User Menu Accordion Control
const userMenuButton = document.getElementById("userMenuButton");
const userMenuDropdown = document.getElementById("userMenuDropdown");
const authButtons = document.getElementById("authButtons");
const userMenuWrapper = document.getElementById("userMenuWrapper");
const logoutBtn = document.getElementById("logoutBtn");

// change in backend
let isLoggedIn = false;

function updateAuthUI() {
    if (isLoggedIn) {
        // اگر کاربر لاگین کرده
        authButtons.style.display = "none";
        userMenuWrapper.style.display = "block";
    } else {
        //اگر کاربر لاگین نکرده
        authButtons.style.display = "flex";
        userMenuWrapper.style.display = "none";
    }
}

if (userMenuButton) {
    userMenuButton.addEventListener("click", (e) => {
        e.stopPropagation();
        userMenuButton.classList.toggle("active");
        userMenuDropdown.classList.toggle("active");
    });
}

document.addEventListener("click", (e) => {
    if (userMenuWrapper && !userMenuWrapper.contains(e.target)) {
        userMenuButton.classList.remove("active");
        userMenuDropdown.classList.remove("active");
    }
});

if (logoutBtn) {
    logoutBtn.addEventListener("click", (e) => {
        e.preventDefault();
        isLoggedIn = false;
        updateAuthUI();
        alert("از حساب کاربری خارج شدید");
    });
}

// Initialize on page load
document.addEventListener("DOMContentLoaded", () => {
    renderAccommodations();
    renderPopular();
    renderRecommended();
    renderExclusive();
    updateAuthUI();
});

// Smooth scroll for anchor links
document.querySelectorAll('a[href^="#"]').forEach((anchor) => {
    anchor.addEventListener("click", function (e) {
        e.preventDefault();
        const target = document.querySelector(this.getAttribute("href"));
        if (target) {
            target.scrollIntoView({
                behavior: "smooth",
                block: "start",
            });
        }
    });
});

const popularGrid = document.getElementById("popularGrid");
const prevBtn = document.querySelector(".sliders.prev");
const nextBtn = document.querySelector(".sliders.next");

function getCardWidth() {
    const card = popularGrid.querySelector(".popular-card");
    const gap = parseInt(window.getComputedStyle(popularGrid).gap) || 0;
    return card.offsetWidth + gap;
}
// SLIDERS
function scrollNext() {
    const cardWidth = getCardWidth();
    const maxScrollLeft = popularGrid.scrollWidth - popularGrid.clientWidth;

    if (popularGrid.scrollLeft >= maxScrollLeft - 5) {
        popularGrid.scrollTo({
            left: 0,
            behavior: "smooth",
        });
    } else {
        popularGrid.scrollBy({
            left: cardWidth,
            behavior: "smooth",
        });
    }
}
// دکمه بعدی
nextBtn.addEventListener("click", () => {
    scrollNext();
    resetAutoplay(); // وقتی کاربر کلیک می‌کند، تایمر را ریست کن
});

// دکمه قبلی
prevBtn.addEventListener("click", () => {
    const cardWidth = getCardWidth();
    popularGrid.scrollBy({ left: -cardWidth, behavior: "smooth" });
    resetAutoplay();
});