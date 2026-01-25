// DOM Elements
const guestSection = document.getElementById("guest-section");
const hostSection = document.getElementById("host-section");
const upgradecontainer = document.querySelector(".upgrade-container");
const upgradeBtn = document.getElementById("upgrade-btn");
const accordionItems = document.querySelectorAll(".accordion-item");
const contentPanel = document.getElementById("dynamic-content-panel");
const reviewModal = document.getElementById("review-modal");
const ratingSpans = document.querySelectorAll(".rating-selector span");
const numberModal = document.querySelector(".modal2");
const salaryModal = document.querySelector(".modal3");
const btnSubmit = document.querySelector(".btn-reserve");
const btnaccuont = document.getElementById("numberaccuont");
const salarytarget = document.getElementById("salary-target");
let accommodationId = document.getElementById("accommodation-select").value;

const modalcontent = document.querySelector(".modal-content");

const numberModalContent = numberModal.querySelector(".modal-content");
const salaryModalContent = salaryModal.querySelector(".modal-content");

numberModal.addEventListener("click", (e) => {
    if (numberModalContent.contains(e.target)) return;
    numberModal.classList.add("hidden");
});

salaryModal.addEventListener("click", (e) => {
    if (salaryModalContent.contains(e.target)) return;
    salaryModal.classList.add("hidden");
});

const salarycontent = document.querySelector(".salary-content");
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
    btnSubmit.addEventListener("click", () => {
        numberModal.classList.add("hidden");
    });
    btnaccuont.addEventListener("click", (event) => {
        event.stopPropagation();
        shownumberView();
    });
    salarytarget.addEventListener("click", () => {
        salaryModal.classList.remove("hidden");
    });

    // Upgrade button
    upgradeBtn.addEventListener("click", (event) => {
        isHost = true;
        event.stopPropagation();
        showHostView();
        shownumberView();
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
        const rating = reviewModal.dataset.rating || 0;
        const inputRate = document.getElementById("rateInput");
        inputRate.value = rating;

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
}

function showHostView() {
    upgradecontainer.classList.add("hidden");
    hostSection.classList.remove("hidden");
    upgradeBtn.classList.add("hidden");
}

function shownumberView() {
    upgradecontainer.classList.add("hidden");
    numberModal.classList.remove("hidden");
    upgradeBtn.classList.add("hidden");
}

function loadHostContent(type) {
    contentPanel.innerHTML = `<p>در حال بارگذاری...</p>`;

    switch (type) {
        case "upcoming-res":
            fetch('/UserPanel/Home/GetFutureReservesForHost?ResidenceId=' + accommodationId)
                .then(res => res.json())
                .then(futureReserves => {
                    let html = `<h2>رزروهای پیش رو</h2>`;

                    if (futureReserves && futureReserves.length > 0) {
                        for (let i = 0; i < futureReserves.length; i++) {
                            html += `
                            <div class="data-row">
                                <div>
                                    <strong>${futureReserves[i].residenceName}</strong><br>
                                    <small>${futureReserves[i].startDate}</small>
                                </div>
                                <div>${futureReserves[i].phoneNumber}</div>
                            </div>
                            `;
                        }
                    } else {
                        html += `<p>هیچ رزرو پیش رویی یافت نشد.</p>`;
                    }

                    contentPanel.innerHTML = html;
                })
                .catch(error => {
                    console.error('خطا در دریافت رزروهای پیش رو:', error);
                    contentPanel.innerHTML = `<p>خطا در دریافت اطلاعات</p>`;
                });
            break;

        case "completed-res":
            fetch('/UserPanel/Home/GetCompletedReservesForHost?ResidenceId=' + accommodationId)
                .then(res => res.json())
                .then(completedReserves => {
                    let html = `<h2>رزروهای تکمیل شده</h2>`;

                    if (completedReserves && completedReserves.length > 0) {
                        for (let i = 0; i < completedReserves.length; i++) {
                            html += `
                            <div class="data-row">
                                <div>
                                    <strong>${completedReserves[i].residenceName}</strong><br>
                                    <small>${completedReserves[i].endDate}</small>
                                </div>
                                <strong>${completedReserves[i].price}</strong>
                            </div>
                            `;
                        }
                    } else {
                        html += `<p>هیچ رزرو تکمیل شده‌ای یافت نشد.</p>`;
                    }

                    contentPanel.innerHTML = html;
                })
                .catch(error => {
                    console.error('خطا در دریافت رزروهای تکمیل شده:', error);
                    contentPanel.innerHTML = `<p>خطا در دریافت اطلاعات</p>`;
                });
            break;

        case "mgmt":
            fetch('/Residence/GetResidenceDetailForHost?ResidenceId=' + accommodationId)
                .then(res => res.json())
                .then(residenceDetial => {
                    let mgmtHtml = `
                    <div class="flex-manage">
                        <h2>مدیریت اقامتگاه</h2>
                        </a>
                    </div>
                    `
                    if (residenceDetial != null) {
                        mgmtHtml += `
                     <div class="flex-manage">
                        <h2>مدیریت اقامتگاه</h2>
                        <a href="/Residence/EditResidenceImages?ResidenceId=${accommodationId}">
                            <button class="btn-outline">ویرایش تصاویر
                            </button>
                        </a>
                    </div>
                <form action="/Residence/EditResidenceDetail?residenceId=${accommodationId}" method="post" id="mgmt-form" class="mt-4">
                    <div class="form-group">
                        <label>نام</label>
                        <input type="text" value="${residenceDetial.name}" name="Name">
                    </div>
                    <div class="form-group">
                        <label>آدرس</label>
                        <input type="text" value="${residenceDetial.street}" name="Street">
                    </div>
                    <div class="form-row">
                        <div class="form-group">
                            <label>کد پستی</label>
                            <input type="number" value="${residenceDetial.postalCode}" name="PostalCode">
                        </div>
                        <div class="form-group">
                            <label>قیمت هر شب</label>
                            <input type="number" value="${residenceDetial.price}" name="Price">
                        </div>
                    </div>
                    <div class="form-group">
                        <label>توضیحات</label>
                        <textarea rows="3" name="Description">${residenceDetial.description}</textarea>
                    </div>
                    <div class="form-row">
                        <div class="form-group">
                            <label>وضعیت</label>
                    <select name="Status">
                        <option value="true" ${residenceDetial.status === true ? 'selected' : ''}>فعال</option>
                        <option value="false" ${residenceDetial.status === false ? 'selected' : ''}>غیرفعال</option>
                    </select>
                    </div>
                        <div class="form-group">
                            <label>ظرفیت</label>
                            <input type="number" name="Capacity" value="${residenceDetial.capacity}">
                        </div>
                    </div>
                    <button type="submit" class="btn-primary">ذخیره تغییرات</button>
                </form>
            `;
                    }
                    else {
                        mgmtHtml += `<p>اطلاعات یافت نشد</p>`;
                    }
                    contentPanel.innerHTML = mgmtHtml;
                });
            break;

        case "reviews":
            fetch('/UserPanel/Home/GetResidenceCommentsForHost?ResidenceId=' + accommodationId)
                .then(res => res.json())
                .then(comments => {
                    let html = `<h2>نظرات کاربران</h2>`;

                    if (comments && comments.length > 0) {
                        for (let i = 0; i < comments.length; i++) {
                            html += `
                            <div class="review-item">
                                <div class="review-meta">
                                    <span>⭐ ${comments[i].rate} • ${comments[i].userName}</span>
                                    <a href="/Residence/Detail/${accommodationId}">مشاهده صفحه</a>
                                </div>
                                <p class="mt-2">${comments[i].description}</p>
                            </div>
                            `;
                        }
                    } else {
                        html += `<p>هنوز نظری ثبت نشده است.</p>`;
                    }

                    contentPanel.innerHTML = html;
                })
                .catch(error => {
                    console.error('خطا در دریافت نظرات:', error);
                    contentPanel.innerHTML = `<p>خطا در دریافت نظرات</p>`;
                });
            break;

        case "salary":
            loadSalaryContent();
    }
}
// Run init
init();

function loadSalaryContent() {
    let slarydata = "";

    fetch('/UserPanel/Home/GetHostSalary')
        .then(res => res.json())
        .then(Data => {
            console.log(Data);
            let sum = 0;
            Data.forEach((item) => {
                slarydata += `
                  <div class="data-row">
                    <div>
                      <strong>${item.residenceName}</strong><br/>
                      <small>${item.fixedDate}</small>
                    </div>
                    <strong class="price-salary">${item.salary.toLocaleString('fa-IR')}</strong>
                  </div>
                `;
                sum = sum + item.salary;
            });

            salarycontent.innerHTML = slarydata;

            contentPanel.innerHTML =
                `
                   <div class="salary-content"></div>
                        <div class="total-salary">
                            <h3>درآمد کل</h3>
                            <span>${sum.toLocaleString('fa-IR')} تومان</span>
                        </div>
                    </div>
                `;

            const salaryTotal = document.getElementById("salary-total");
            salaryTotal.innerHTML = `${sum.toLocaleString('fa-IR')} تومان`;
        })
        .catch(error => {
            console.error('خطا در دریافت اطلاعات درآمد:', error);
            contentPanel.innerHTML = `<p>خطا در دریافت اطلاعات</p>`;
        });
}

const modalError = document.querySelector(".modal4");

function showError() {
    modalError.classList.add("show");

    setTimeout(() => {
        modalError.classList.remove("show");
    }, 2500);
}

if (needUpgradeRole) {
    showError();
}