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

console.log(isLoggedIn);
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

// Initialize on page load
document.addEventListener("DOMContentLoaded", () => {
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