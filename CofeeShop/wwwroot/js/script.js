// Function to create accommodation card
//function createAccommodationCard(data) {
//    return `
//        <div class="accommodation-card">
//            <img src="${data.image}" alt="${data.title}" class="card-image">
//            <div class="card-content">
//                <h3 class="card-title">${data.title}</h3>
//                <p class="card-location">${data.location}</p>
//                <div class="card-footer">
//                    <div class="card-price">${data.price} <span>تومان / شب</span></div>
//                    <div class="card-rating">⭐ ${data.rating}</div>
//                </div>
//            </div>
//        </div>
//    `;
//}

// Render all accommodations
function renderAccommodations() {
    const accommodationsGrid = document.getElementById("accommodationsGrid");
    accommodationsGrid.innerHTML = accommodationsData
        .map((item) => createAccommodationCard(item))
        .join("");
}

// Render recommended accommodations
function renderRecommended() {
    const recommendedGrid = document.getElementById("recommendedGrid");
    recommendedGrid.innerHTML = recommendedData
        .map((item) => createAccommodationCard(item))
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
const backIsLogin = document.getElementById("isLogin");
const logoutBtn = document.getElementById("logoutBtn");

// change in backend
let isLoggedIn = true;
console.log("kfodjgd", isLoggedIn);
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