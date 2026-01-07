// Mock user data
const currentUser = {
  firstName: "علی",
  lastName: "محمدی",
  email: "ali.mohammadi@example.com",
};

// Mock accommodation data
const accommodationData = {
  name: "ویلا لوکس شمال",
  pricePerNight: 3500000,
  checkInDate: "۱۴۰۳/۱۰/۱۵",
  checkOutDate: "۱۴۰۳/۱۰/۲۰",
  numberOfNights: 5,
};

// Guest list
const guests = [];
let guestForms = []; // Array to track form numbers
let nextFormId = 1; // Track next form ID

function updatePriceCalculation() {
  const pricePerNight = accommodationData.pricePerNight;
  const numberOfNights = accommodationData.numberOfNights;
  const totalPrice = pricePerNight * numberOfNights;

  // Format numbers with Persian digits
  document.getElementById("pricePerNight").textContent =
    formatPrice(pricePerNight);
  document.getElementById("totalPrice").textContent = formatPrice(totalPrice);
}

function formatPrice(price) {
  const formatted = new Intl.NumberFormat("fa-IR").format(price);
  return formatted;
}

function createGuestForm(formId) {
  const guestNumber = guestForms.indexOf(formId) + 1;

  const guestFormHTML = `
        <div class="guest-form-section" id="guestForm_${formId}">
            <div class="guest-form-header">
                <h3>مهمان ${guestNumber}</h3>
                <button type="button" class="btn-remove-guest" data-form-id="${formId}">× حذف</button>
            </div>
            <form class="guest-form" id="form_guestForm_${formId}">
                <div class="form-group">
                    <label for="guestFirstName_${formId}">نام مهمان</label>
                    <input
                        type="text"
                        id="guestFirstName_${formId}"
                        placeholder="نام را وارد کنید"
                        required
                    />
                </div>

                <div class="form-group">
                    <label for="guestLastName_${formId}">نام خانوادگی مهمان</label>
                    <input
                        type="text"
                        id="guestLastName_${formId}"
                        placeholder="نام خانوادگی را وارد کنید"
                        required
                    />
                </div>

                <div class="form-group">
                    <label for="guestBirthDate_${formId}">تاریخ تولد</label>
                    <input type="date" id="guestBirthDate_${formId}" required />
                </div>

                <div class="form-group">
                    <label for="guestGender_${formId}">جنسیت</label>
                    <select id="guestGender_${formId}" required>
                        <option value="">انتخاب کنید</option>
                        <option value="male">مرد</option>
                        <option value="female">زن</option>
                    </select>
                </div>

                <div class="form-group">
                    <label for="guestNationalId_${formId}">کد ملی</label>
                    <input
                        type="text"
                        id="guestNationalId_${formId}"
                        placeholder="شماره ملی را وارد کنید"
                        required
                        pattern="\\d{10}"
                    />
                </div>

                <button type="submit" class="btn btn-primary btn-submit-guest btn-reserve">
                    ذخیره اطلاعات مهمان
                </button>
            </form>
        </div>
    `;

  return guestFormHTML;
}

function addGuestForm() {
  const guestFormsContainer = document.getElementById("guestFormsContainer");
  if (!guestFormsContainer) return;

  // Add form ID to array
  const formId = nextFormId++;
  guestForms.push(formId);

  const guestFormHTML = createGuestForm(formId);
  guestFormsContainer.insertAdjacentHTML("beforeend", guestFormHTML);

  // Add event listener to the remove button
  const newForm = document.getElementById(`guestForm_${formId}`);
  const removeBtn = newForm.querySelector(".btn-remove-guest");
  removeBtn.addEventListener("click", () => {
    removeGuestForm(formId);
  });

  // Add event listener to the form
  const form = document.getElementById(`form_guestForm_${formId}`);
  form.addEventListener("submit", (e) => {
    e.preventDefault();
    saveGuestData(formId);
  });

  // Update all form numbers and counter
  updateAllFormNumbers();
  updateGuestCounter();

  // Scroll to the new form
  newForm.scrollIntoView({ behavior: "smooth", block: "nearest" });
}

function removeGuestForm(formId) {
  // Find form in array
  const index = guestForms.indexOf(formId);
  if (index === -1) return;

  // Remove from array
  guestForms.splice(index, 1);

  // Remove guest data from guests array
  const guestIndex = guests.findIndex((guest) => guest.formId === formId);
  if (guestIndex !== -1) {
    guests.splice(guestIndex, 1);
  }

  // Remove from DOM
  const formToRemove = document.getElementById(`guestForm_${formId}`);
  if (formToRemove) {
    formToRemove.remove();
  }

  // Update all form numbers and counter
  updateAllFormNumbers();
  updateGuestCounter();
}

function updateAllFormNumbers() {
  guestForms.forEach((formId, index) => {
    const formElement = document.getElementById(`guestForm_${formId}`);
    if (formElement) {
      const header = formElement.querySelector(".guest-form-header h3");
      if (header) {
        header.textContent = `مهمان ${index + 1}`;
      }
    }
  });
}

function updateGuestCounter() {
  const guestCounterContainer = document.getElementById(
    "guestCounterContainer"
  );
  const guestCountElement = document.getElementById("guestCount");

  if (!guestCounterContainer || !guestCountElement) return;

  const count = guestForms.length;

  guestCountElement.textContent = count;

  if (count > 0) {
    guestCounterContainer.style.display = "flex";
  } else {
    guestCounterContainer.style.display = "none";
  }
}

function saveGuestData(formId) {
  const firstName = document.getElementById(`guestFirstName_${formId}`).value;
  const lastName = document.getElementById(`guestLastName_${formId}`).value;
  const birthDate = document.getElementById(`guestBirthDate_${formId}`).value;
  const gender = document.getElementById(`guestGender_${formId}`).value;
  const nationalId = document.getElementById(`guestNationalId_${formId}`).value;

  // Validate national ID (must be 10 digits)
  if (!/^\d{10}$/.test(nationalId)) {
    alert("شماره ملی باید ۱۰ رقم باشد");
    return;
  }

  // Check if guest already exists
  const existingGuestIndex = guests.findIndex(
    (guest) => guest.formId === formId
  );

  const guestData = {
    formId: formId,
    firstName,
    lastName,
    birthDate,
    gender,
    nationalId,
    savedAt: new Date().toISOString(),
  };

  if (existingGuestIndex !== -1) {
    // Update existing guest
    guests[existingGuestIndex] = guestData;
    alert(`اطلاعات مهمان "${firstName} ${lastName}" با موفقیت بروزرسانی شد`);
  } else {
    // Add new guest
    guests.push(guestData);
    alert(`مهمان "${firstName} ${lastName}" با موفقیت ثبت شد`);
  }

  // Log guests list for debugging
  console.log("[v1] Current guests:", guests);
}

function setupEventListeners() {
  // Add guest form button
  const toggleGuestFormBtn = document.getElementById("toggleGuestForm");

  if (toggleGuestFormBtn) {
    toggleGuestFormBtn.addEventListener("click", () => {
      addGuestForm();
    });
  }

  // Handle payment button
  const proceedBtn = document.querySelector(".btn-reserve");
  if (proceedBtn) {
    proceedBtn.addEventListener("click", () => {
      handlePayment();
    });
  }
}

function handlePayment() {
  // First validate all forms
  const guestFormsContainer = document.getElementById("guestFormsContainer");
  const forms = guestFormsContainer.querySelectorAll(".guest-form");

  let allValid = true;
  forms.forEach((form) => {
    if (!form.checkValidity()) {
      allValid = false;
      form.reportValidity();
    }
  });

  if (!allValid) {
    alert("لطفا اطلاعات تمام مهمانان را کامل کنید");
    return;
  }

  // Check if all guests have saved data
  if (guests.length !== guestForms.length) {
    alert("لطفا برای تمام مهمانان اطلاعات را ذخیره کنید");
    return;
  }

  if (guests.length === 0) {
    alert("لطفا حداقل یک مهمان اضافه کنید");
    return;
  }

  // Proceed with payment
  alert(`پرداخت برای ${guests.length} مهمان با موفقیت انجام شد!\n\nمهمانان:`);
  guests.forEach((guest, index) => {
    console.log(
      `${index + 1}. ${guest.firstName} ${guest.lastName} - ${guest.nationalId}`
    );
  });
}

// Initialize
document.addEventListener("DOMContentLoaded", () => {
  setupEventListeners();
  updatePriceCalculation();
  updateGuestCounter();
});
