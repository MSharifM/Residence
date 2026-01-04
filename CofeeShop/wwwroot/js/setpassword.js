const $ = document
const setPasswordForm = $.getElementById("set-password-form")
const eyeButtons = $.querySelectorAll(".eye-btn")

const newPasswordInput = $.getElementById("new-password-input")
const newPasswordAlert = $.getElementById("new-password-alert")
const confirmPasswordInput = $.getElementById("confirm-password-input")
const confirmPasswordAlert = $.getElementById("confirm-password-alert")

eyeButtons.forEach((eye) => {
  eye.addEventListener("click", () => {
    const input = eye.previousElementSibling

    if (input.type === "password") {
      input.type = "text"
      eye.firstElementChild.classList.replace("fa-eye", "fa-eye-slash")
    } else {
      input.type = "password"
      eye.firstElementChild.classList.replace("fa-eye-slash", "fa-eye")
    }
  })
})

// اعتبارسنجی رمز عبور جدید
newPasswordInput.addEventListener("input", () => {
  if (newPasswordInput.value.length < 6) {
    newPasswordInput.classList.add("wrong-input")
    newPasswordAlert.innerText = "رمز عبور باید حداقل 6 کاراکتر باشد"
  } else {
    newPasswordInput.classList.remove("wrong-input")
    newPasswordAlert.innerText = ""
  }
})

// اعتبارسنجی تکرار رمز عبور
confirmPasswordInput.addEventListener("input", () => {
  if (confirmPasswordInput.value !== newPasswordInput.value) {
    confirmPasswordInput.classList.add("wrong-input")
    confirmPasswordAlert.innerText = "رمزهای عبور مطابقت ندارند"
  } else {
    confirmPasswordInput.classList.remove("wrong-input")
    confirmPasswordAlert.innerText = ""
  }
})

// ارسال فرم
setPasswordForm.addEventListener("submit", (e) => {
  e.preventDefault()

  let isValid = true

  if (newPasswordInput.value.length < 6) {
    newPasswordInput.classList.add("wrong-input")
    newPasswordAlert.innerText = "رمز عبور باید حداقل 6 کاراکتر باشد"
    isValid = false
  }

  if (confirmPasswordInput.value !== newPasswordInput.value) {
    confirmPasswordInput.classList.add("wrong-input")
    confirmPasswordAlert.innerText = "رمزهای عبور مطابقت ندارند"
    isValid = false
  }

  if (isValid) {
    alert("رمز عبور با موفقیت تغییر یافت")
    // اینجا می‌توانید کاربر را هدایت کنید:
    window.location.href = "/login.html"
  }
})
