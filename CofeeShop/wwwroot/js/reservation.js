// Guest list
const guests = [];
let guestForms = []; // Array to track form numbers
let nextFormId = countGuest + 1; // Track next form ID

// Initialize existing forms from the page
function initializeExistingForms() {
    const existingForms = document.querySelectorAll('.guest-form-section[data-index]');
    
    existingForms.forEach((formElement, index) => {
        const formIndex = formElement.getAttribute('data-index');
        const formId = parseInt(formIndex);
        
        // Add form ID to array
        if (!guestForms.includes(formId)) {
            guestForms.push(formId);
        }
        
        // Update form header number
        const header = formElement.querySelector('.guest-form-header h3');
        if (header) {
            header.textContent = `مهمان ${index + 1}`;
        }
        
        // Add event listener to remove button
        const removeBtn = formElement.querySelector('.btn-remove-guest');
        if (removeBtn) {
            removeBtn.addEventListener('click', () => {
                removeGuestForm(formId);
            });
        }
        
        // Store initial guest data
        saveGuestDataFromForm(formId);
    });
    
    // Update counter
    updateGuestCounter();
}

// Save guest data from an existing form
function saveGuestDataFromForm(formId) {
    const formElement = document.querySelector(`.guest-form-section[data-index="${formId}"]`);
    if (!formElement) return;
    
    const firstNameInput = formElement.querySelector('input[name$="FirstName"]');
    const lastNameInput = formElement.querySelector('input[name$="LastName"]');
    const birthDateInput = formElement.querySelector('input[name$="BirthDate"]');
    const genderSelect = formElement.querySelector('select[name$="IsMan"]');
    const nationalIdInput = formElement.querySelector('input[name$="Pin"]');
    
    if (!firstNameInput || !lastNameInput) return;
    
    const firstName = firstNameInput.value;
    const lastName = lastNameInput.value;
    const birthDate = birthDateInput ? birthDateInput.value : '';
    const gender = genderSelect ? genderSelect.value : '';
    const nationalId = nationalIdInput ? nationalIdInput.value : '';
    
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
        isExisting: true // Mark as existing form from server
    };
    
    if (existingGuestIndex !== -1) {
        // Update existing guest
        guests[existingGuestIndex] = guestData;
    } else {
        // Add new guest
        guests.push(guestData);
    }
}

function createGuestForm(formId) {
    const guestNumber = guestForms.indexOf(formId) + 1;

    // Generate unique names for the form to work with ASP.NET model binding
    const uniqueName = `NewClients_${formId}_`;
    
    const guestFormHTML = `
        <div class="guest-form-section" data-index="${formId}">
            <div class="guest-form-header">
                <h3>مهمان ${guestNumber}</h3>
                <button type="button" class="btn-remove-guest">× حذف</button>
            </div>
            <div class="guest-form">
                <input type="hidden" name="NewClients.Index" value="${formId}" />
                
                <div class="form-group">
                    <label>نام مهمان</label>
                    <input
                        type="text"
                        name="NewClients[${formId}].FirstName"
                        placeholder="نام را وارد کنید"
                        required
                    />
                </div>

                <div class="form-group">
                    <label>نام خانوادگی مهمان</label>
                    <input
                        type="text"
                        name="NewClients[${formId}].LastName"
                        placeholder="نام خانوادگی را وارد کنید"
                        required
                    />
                </div>

                <div class="form-group">
                    <label>تاریخ تولد</label>
                    <input 
                        type="date" 
                        name="NewClients[${formId}].BirthDate"
                        required 
                    />
                </div>

                <div class="form-group">
                    <label>جنسیت</label>
                    <select name="NewClients[${formId}].IsMan" required>
                        <option value="">انتخاب کنید</option>
                        <option value="true">مرد</option>
                        <option value="false">زن</option>
                    </select>
                </div>

                <div class="form-group">
                    <label>کد ملی</label>
                    <input
                        type="text"
                        name="NewClients[${formId}].Pin"
                        placeholder="شماره ملی را وارد کنید"
                        required
                        pattern="\\d{10}"
                    />
                </div>
            </div>
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
    const formElement = document.querySelector(`.guest-form-section[data-index="${formId}"]`);
    const removeBtn = formElement.querySelector(".btn-remove-guest");
    removeBtn.addEventListener("click", () => {
        removeGuestForm(formId);
    });

    // Update all form numbers and counter
    updateAllFormNumbers();
    updateGuestCounter();

    // Scroll to the new form
    formElement.scrollIntoView({ behavior: "smooth", block: "nearest" });
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
    const formToRemove = document.querySelector(`.guest-form-section[data-index="${formId}"]`);
    if (formToRemove) {
        // If it's an existing form from server, add a hidden field to mark it for removal
        if (formToRemove.querySelector('input[name^="Clients"]')) {
            const removeInput = document.createElement('input');
            removeInput.type = 'hidden';
            removeInput.name = 'RemovedClientIndices';
            removeInput.value = formId;
            formToRemove.parentNode.appendChild(removeInput);
            
            // Hide the form instead of removing it
            formToRemove.style.display = 'none';
        } else {
            // For dynamically added forms, remove completely
            formToRemove.remove();
        }
    }

    // Update all form numbers and counter
    updateAllFormNumbers();
    updateGuestCounter();
}

function updateAllFormNumbers() {
    guestForms.forEach((formId, index) => {
        const formElement = document.querySelector(`.guest-form-section[data-index="${formId}"]`);
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


function setupEventListeners() {
    // Add guest form button
    const toggleGuestFormBtn = document.getElementById("toggleGuestForm");
    
    if (toggleGuestFormBtn) {
        // Remove existing onclick handler
        toggleGuestFormBtn.removeAttribute("onclick");
        toggleGuestFormBtn.addEventListener("click", () => {
            addGuestForm();
        });
    }

    // Handle payment button
    const proceedBtn = document.querySelector(".btn-reserve");
    if (proceedBtn) {
        // Change from click to form submission
        proceedBtn.addEventListener("click", (e) => {
            e.preventDefault();
            handlePayment();
        });
    }
    
    // Add save button listeners to existing forms
    document.querySelectorAll('.guest-form-section[data-index]').forEach(formElement => {
        const formId = formElement.getAttribute('data-index');
        const inputs = formElement.querySelectorAll('input, select');
        
        inputs.forEach(input => {
            input.addEventListener('change', () => {
                saveGuestData(parseInt(formId));
            });
        });
    });
}

function handlePayment() {
    // First validate all visible forms
    const visibleForms = document.querySelectorAll('.guest-form-section:not([style*="display: none"])');
    
    let allValid = true;
    visibleForms.forEach((form) => {
        const formInputs = form.querySelectorAll('input[required], select[required]');
        formInputs.forEach(input => {
            if (!input.value.trim()) {
                allValid = false;
                input.focus();
                input.reportValidity();
            }
        });
    });

    if (!allValid) {
        alert("لطفا اطلاعات تمام مهمانان را کامل کنید");
        return;
    }

    if (guests.length === 0) {
        alert("لطفا حداقل یک مهمان اضافه کنید");
        return;
    }

    // Submit the form
    const form = document.querySelector('form');
    if (form) {
        form.submit();
    }
}

// Initialize
document.addEventListener("DOMContentLoaded", () => {
    // Initialize existing forms first
    initializeExistingForms();
    
    // Then setup event listeners
    setupEventListeners();
    
    // Update counter
    updateGuestCounter();
});

//// Guest list
//const guests = [];
//let guestForms = []; // Array to track form numbers
//let nextFormId = countGuest + 1; // Track next form ID

//function createGuestForm(formId) {
//    const guestNumber = guestForms.indexOf(formId) + 1;

//    const guestFormHTML = `
//        <div class="guest-form-section">
//            <div class="guest-form-header">
//                <h3>مهمان </h3>
//                <button type="button" class="btn-remove-guest">× حذف</button>
//            </div>
//            <div class="guest-form">
//                <div class="form-group">
//                    <label>نام مهمان</label>
//                    <input
//                        type="text"
//                        placeholder="نام را وارد کنید"
//                        required
//                    />
//                </div>

//                <div class="form-group">
//                    <label>نام خانوادگی مهمان</label>
//                    <input
//                        type="text"
//                        placeholder="نام خانوادگی را وارد کنید"
//                        required
//                    />
//                </div>

//                <div class="form-group">
//                    <label>تاریخ تولد</label>
//                    <input type="date" required />
//                </div>

//                <div class="form-group">
//                    <label>جنسیت</label>
//                    <select required>
//                        <option value="">انتخاب کنید</option>
//                        <option value="male">مرد</option>
//                        <option value="female">زن</option>
//                    </select>
//                </div>

//                <div class="form-group">
//                    <label>کد ملی</label>
//                    <input
//                        type="text"
//                        placeholder="شماره ملی را وارد کنید"
//                        required
//                        pattern="\\d{10}"
//                    />
//                </div>
//            </div>
//        </div>
//    `;

//    return guestFormHTML;
//}

//function addGuestForm() {
//    const guestFormsContainer = document.getElementById("guestFormsContainer");
//    if (!guestFormsContainer) return;

//    // Add form ID to array
//    const formId = nextFormId++;
//    guestForms.push(formId);

//    const guestFormHTML = createGuestForm(formId);
//    guestFormsContainer.insertAdjacentHTML("beforeend", guestFormHTML);

//    // Add event listener to the remove button
//    const newForm = document.getElementById(`guestForm_${formId}`);
//    const removeBtn = newForm.querySelector(".btn-remove-guest");
//    removeBtn.addEventListener("click", () => {
//        removeGuestForm(formId);
//    });

//    // Add event listener to the form
//    const form = document.getElementById(`form_guestForm_${formId}`);
//    form.addEventListener("submit", (e) => {
//        e.preventDefault();
//        saveGuestData(formId);
//    });

//    // Update all form numbers and counter
//    updateAllFormNumbers();
//    updateGuestCounter();

//    // Scroll to the new form
//    newForm.scrollIntoView({ behavior: "smooth", block: "nearest" });
//}

//function removeGuestForm(formId) {
//    // Find form in array
//    const index = guestForms.indexOf(formId);
//    if (index === -1) return;

//    // Remove from array
//    guestForms.splice(index, 1);

//    // Remove guest data from guests array
//    const guestIndex = guests.findIndex((guest) => guest.formId === formId);
//    if (guestIndex !== -1) {
//        guests.splice(guestIndex, 1);
//    }

//    // Remove from DOM
//    const formToRemove = document.getElementById(`guestForm_${formId}`);
//    if (formToRemove) {
//        formToRemove.remove();
//    }

//    // Update all form numbers and counter
//    updateAllFormNumbers();
//    updateGuestCounter();
//}

//function updateAllFormNumbers() {
//    guestForms.forEach((formId, index) => {
//        const formElement = document.getElementById(`guestForm_${formId}`);
//        if (formElement) {
//            const header = formElement.querySelector(".guest-form-header h3");
//            if (header) {
//                header.textContent = `مهمان ${index + 1}`;
//            }
//        }
//    });
//}

//function updateGuestCounter() {
//    const guestCounterContainer = document.getElementById(
//        "guestCounterContainer"
//    );
//    const guestCountElement = document.getElementById("guestCount");

//    if (!guestCounterContainer || !guestCountElement) return;

//    const count = guestForms.length;

//    guestCountElement.textContent = count;

//    if (count > 0) {
//        guestCounterContainer.style.display = "flex";
//    } else {
//        guestCounterContainer.style.display = "none";
//    }
//}

//function saveGuestData(formId) {
//    const firstName = document.getElementById(`guestFirstName_${formId}`).value;
//    const lastName = document.getElementById(`guestLastName_${formId}`).value;
//    const birthDate = document.getElementById(`guestBirthDate_${formId}`).value;
//    const gender = document.getElementById(`guestGender_${formId}`).value;
//    const nationalId = document.getElementById(`guestNationalId_${formId}`).value;

//    // Validate national ID (must be 10 digits)
//    if (!/^\d{10}$/.test(nationalId)) {
//        alert("شماره ملی باید ۱۰ رقم باشد");
//        return;
//    }

//    // Check if guest already exists
//    const existingGuestIndex = guests.findIndex(
//        (guest) => guest.formId === formId
//    );

//    const guestData = {
//        formId: formId,
//        firstName,
//        lastName,
//        birthDate,
//        gender,
//        nationalId,
//        savedAt: new Date().toISOString(),
//    };

//    if (existingGuestIndex !== -1) {
//        // Update existing guest
//        guests[existingGuestIndex] = guestData;
//        alert(`اطلاعات مهمان "${firstName} ${lastName}" با موفقیت بروزرسانی شد`);
//    } else {
//        // Add new guest
//        guests.push(guestData);
//        alert(`مهمان "${firstName} ${lastName}" با موفقیت ثبت شد`);
//    }

//    // Log guests list for debugging
//    console.log("[v1] Current guests:", guests);
//}

//function setupEventListeners() {
//    // Add guest form button
//    const toggleGuestFormBtn = document.getElementById("toggleGuestForm");

//    if (toggleGuestFormBtn) {
//        toggleGuestFormBtn.addEventListener("click", () => {
//            addGuestForm();
//        });
//    }

//    // Handle payment button
//    const proceedBtn = document.querySelector(".btn-reserve");
//    if (proceedBtn) {
//        proceedBtn.addEventListener("click", () => {
//            handlePayment();
//        });
//    }
//}

//function handlePayment() {
//    // First validate all forms
//    const guestFormsContainer = document.getElementById("guestFormsContainer");
//    const forms = guestFormsContainer.querySelectorAll(".guest-form");

//    let allValid = true;
//    forms.forEach((form) => {
//        if (!form.checkValidity()) {
//            allValid = false;
//            form.reportValidity();
//        }
//    });

//    if (!allValid) {
//        alert("لطفا اطلاعات تمام مهمانان را کامل کنید");
//        return;
//    }

//    // Check if all guests have saved data
//    if (guests.length !== guestForms.length) {
//        alert("لطفا برای تمام مهمانان اطلاعات را ذخیره کنید");
//        return;
//    }

//    if (guests.length === 0) {
//        alert("لطفا حداقل یک مهمان اضافه کنید");
//        return;
//    }

//    // Proceed with payment
//    alert(`پرداخت برای ${guests.length} مهمان با موفقیت انجام شد!\n\nمهمانان:`);
//    guests.forEach((guest, index) => {
//        console.log(
//            `${index + 1}. ${guest.firstName} ${guest.lastName} - ${guest.nationalId}`
//        );
//    });
//}

//// Initialize
//document.addEventListener("DOMContentLoaded", () => {
//    setupEventListeners();
//    updatePriceCalculation();
//    updateGuestCounter();
//});