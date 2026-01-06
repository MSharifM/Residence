// Backend-controlled host state
let isHost = false;

// DOM Elements
const guestSection = document.getElementById("guest-section");
const hostSection = document.getElementById("host-section");
const upgradecontainer = document.querySelector(".upgrade-container");
const upgradeBtn = document.getElementById("upgrade-btn");
const accordionItems = document.querySelectorAll(".accordion-item");
const contentPanel = document.getElementById("dynamic-content-panel");
const reviewModal = document.getElementById("review-modal");
const ratingSpans = document.querySelectorAll(".rating-selector span");

// Initialization
function init() {
    if (isHost) {
        showHostView();
    }

    // Setup event listeners
    setupEventListeners();

    // Load default content if host
    if (isHost) {
        loadHostContent("upcoming-res");
    }
}

function setupEventListeners() {
    // Upgrade button
    upgradeBtn.addEventListener("click", () => {
        isHost = true;
        showHostView();
        loadHostContent("upcoming-res");
    });

    // Review Triggers
    document.querySelectorAll(".review-trigger").forEach((btn) => {
        btn.addEventListener("click", () => {
            reviewModal.classList.remove("hidden");
        });
    });

    // Modal Close
    document.getElementById("cancel-review").addEventListener("click", () => {
        reviewModal.classList.add("hidden");
    });

    // Rating Selector
    ratingSpans.forEach((span) => {
        span.addEventListener("click", function () {
            const val = Number.parseInt(this.dataset.value);
            ratingSpans.forEach((s, idx) => {
                s.classList.toggle("active", idx < val);
            });
            reviewModal.dataset.rating = val;
        });
    });

    // Submit Review
    document.getElementById("submit-review-btn").addEventListener("click", () => {
        const text = document.getElementById("review-text").value;
        const rating = reviewModal.dataset.rating || 0;

        console.log("Submitting review:", { text, rating });
        alert("از نظر شما سپاسگزاریم!");
        reviewModal.classList.add("hidden");
    });

    // Accordion Interaction
    accordionItems.forEach((item) => {
        item.addEventListener("click", function () {
            accordionItems.forEach((i) => i.classList.remove("active"));
            this.classList.add("active");
            loadHostContent(this.dataset.target);
        });
    });

    // Delete Accommodation
    document
        .getElementById("delete-accommodation")
        .addEventListener("click", () => {
            const select = document.getElementById("accommodation-select");
            const text = select.options[select.selectedIndex].text;
            if (confirm(`آیا از حذف "${text}" اطمینان دارید؟`)) {
                select.remove(select.selectedIndex);
            }
        });

    // Add Accommodation
    document.getElementById("add-accommodation").addEventListener("click", () => {
        alert("در حال باز کردن مراحل افزودن اقامتگاه جدید...");
    });
}

function showHostView() {
    upgradecontainer.classList.add("hidden");
    hostSection.classList.remove("hidden");
    upgradeBtn.classList.add("hidden");
}

function loadHostContent(type) {
    let html = "";

    switch (type) {
        case "upcoming-res":
            html = `

                <div class="data-row">
                    <div>
                        <strong>آپارتمان لوکس</strong><br>
                        <small>۹ دی ۱۴۰۴</small>
                    </div>
                    <div>+۹۸ ۹۱۱ ۱۱۱ ۱۱۱۱</div>
                </div>
            `;
            break;

        case "completed-res":
            html = `
                <h2>رزروهای تکمیل شده</h2>
                <div class="data-row">
                    <div>
                        <strong>خانه جنگلی</strong><br>
                        <small>۱۸ مهر - ۲۳ مهر ۱۴۰۲</small>
                    </div>
                    <strong>۴۵۰ دلار</strong>
                </div>
            `;
            break;

        case "mgmt":
            html = `
                <h2>مدیریت اقامتگاه</h2>
                <form id="mgmt-form" class="mt-4">
                    <div class="form-group">
                        <label>نام</label>
                        <input type="text" value="آپارتمان لوکس">
                    </div>
                    <div class="form-group">
                        <label>آدرس</label>
                        <input type="text" value="تهران، میدان ونک">
                    </div>
                    <div class="form-row">
                        <div class="form-group">
                            <label>شهر</label>
                            <input type="text" value="تهران">
                        </div>
                        <div class="form-group">
                            <label>قیمت هر شب</label>
                            <input type="number" value="150">
                        </div>
                    </div>
                    <div class="form-group">
                        <label>توضیحات</label>
                        <textarea rows="3">یک آپارتمان مدرن و زیبا در قلب شهر.</textarea>
                    </div>
                    <div class="form-row">
                        <div class="form-group">
                            <label>وضعیت</label>
                            <select><option>فعال</option><option>پیش‌نویس</option></select>
                        </div>
                        <div class="form-group">
                            <label>ظرفیت</label>
                            <input type="number" value="4">
                        </div>
                    </div>
                    <button type="button" onclick="alert('تغییرات ذخیره شد!')" class="btn-primary">ذخیره تغییرات</button>
                </form>
            `;
            break;

        case "reviews":
            html = `
                <h2>نظرات کاربران</h2>
                <div class="review-item">
                    <div class="review-meta">
                        <span>جان دو • ۵ ستاره</span>
                        <a href="/accommodation/1">مشاهده صفحه</a>
                    </div>
                    <p class="mt-2">جای فوق‌العاده‌ای بود! برای خانواده‌ها پیشنهاد می‌شود.</p>
                </div>
                <div class="review-item">
                    <div class="review-meta">
                        <span>سارا اسمیت • ۴ ستاره</span>
                        <a href="/accommodation/1">مشاهده صفحه</a>
                    </div>
                    <p class="mt-2">بسیار تمیز و در موقعیت مکانی عالی.</p>
                </div>
            `;
            break;
    }

    contentPanel.innerHTML = html;
}

// Run init
init();

{
    /* <a href="/accommodation/1">مشاهده صفحه</a> */
}