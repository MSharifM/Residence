const newImages = [];
const removedImages = [];
console.log(accommodationData);
// Initialize the page
document.addEventListener("DOMContentLoaded", () => {
    const accommodationNameEl = document.getElementById("accommodationName");
    const imageInput = document.getElementById("imageInput");
    const addImageBtn = document.getElementById("addImageBtn");
    const sliderPrev = document.getElementById("sliderPrev");
    const sliderNext = document.getElementById("sliderNext");

    // Set accommodation name
    accommodationNameEl.textContent = accommodationData.name;

    // Initial render of slider
    renderSlider();

    // Event Listeners
    addImageBtn.addEventListener("click", () => {
        imageInput.click();
    });

    imageInput.addEventListener("change", handleImageUpload);
    sliderPrev.addEventListener("click", previousSlide);
    sliderNext.addEventListener("click", nextSlide);
});

function renderSlider(addImage = false) {
    const sliderWrapper = document.getElementById("sliderWrapper");
    sliderWrapper.innerHTML = "";

    if (accommodationData.images.length === 0) {
        sliderWrapper.innerHTML =
            '<div class="empty-slider">هیچ تصویری موجود نیست</div>';
        updateSliderButtons();
        return;
    }

    accommodationData.images.forEach((imageSrc, index) => {
        const slide = document.createElement("div");
        slide.className = "slider-slide";
        //TODO: Edit this conditon to display existing images
        if (addImage) {
            slide.innerHTML = `
                <img src="${imageSrc}" alt = "preview" />
                             `;
        }
        else {
            if (index === 0) {
                slide.innerHTML = `
          <img src="/residence_images/${imageSrc}" alt="تصویر اصلی">
          <input hidden="true" type="text" name="ExistingImages[${index}]" value="${imageSrc}"/>
      `;
            }
            else {
                slide.innerHTML = `
          <img src="/residence_images/otherImages/${imageSrc}" alt="تصویر ${index + 1}">
            <button type="button" class="delete-btn" onclick="deleteImage(${index})">
                حذف عکس
            </button>
          <input hidden="true" type="text" name="ExistingImages[${index}]" value="${imageSrc}"/>
      `;
            }
        }

        sliderWrapper.appendChild(slide);
    });

    // Reset to first slide after rendering
    updateSliderPosition();
    updateSliderButtons();
}

// Get current slide index from slider position
let currentSlideIndex = 0;

// Update slider position based on currentSlideIndex
function updateSliderPosition() {
    const sliderWrapper = document.getElementById("sliderWrapper");
    const slides = document.querySelectorAll(".slider-slide");

    if (!slides.length) return;

    const slideWidth = slides[0].offsetWidth;
    const offset = currentSlideIndex * slideWidth;

    sliderWrapper.style.transform = `translateX(${offset}px)`;
}

// Update slider button states
function updateSliderButtons() {
    const sliderPrev = document.getElementById("sliderPrev");
    const sliderNext = document.getElementById("sliderNext");
    const totalSlides = accommodationData.images.length;

    sliderPrev.disabled = currentSlideIndex === 0;
    sliderNext.disabled = currentSlideIndex === totalSlides - 1;
}

// Navigate to previous slide
function previousSlide() {
    if (currentSlideIndex > 0) {
        currentSlideIndex--;
        updateSliderPosition();
        updateSliderButtons();
    }
}

// Navigate to next slide
function nextSlide() {
    if (currentSlideIndex < accommodationData.images.length - 1) {
        currentSlideIndex++;
        updateSliderPosition();
        updateSliderButtons();
    }
}

//function handleImageUpload(event) {
//    const file = event.target.files[0];

//    if (file && file.type.startsWith("image/")) {
//        const reader = new FileReader();

//        reader.onload = (e) => {
//            // Add the new image to the array
//            accommodationData.images.push(e.target.result);

//            // Reset slider to show the newly added image
//            currentSlideIndex = accommodationData.images.length - 1;

//            // Re-render the slider
//            renderSlider(addImage = true);
//            updateSliderPosition();
//        };

//        reader.readAsDataURL(file);
//    } else {
//        alert("لطفاً یک فایل تصویری معتبر انتخاب کنید");
//    }

//    // Reset input
//    event.target.value = "";
//}

function handleImageUpload(event) {
    const file = event.target.files[0];
    if (!file || !file.type.startsWith("image/")) {
        alert("فایل تصویری معتبر نیست");
        return;
    }

    // ذخیره فایل واقعی برای ارسال
    newImages.push(file);

    // preview
    const reader = new FileReader();
    reader.onload = e => {
        accommodationData.images.push(e.target.result);
        currentSlideIndex = accommodationData.images.length - 1;
        renderSlider(true);
    };
    reader.readAsDataURL(file);

    event.target.value = "";
}

function deleteImage(index) {
    accommodationData.images.splice(index, 1);

    // Adjust currentSlideIndex if needed
    if (
        currentSlideIndex >= accommodationData.images.length &&
        accommodationData.images.length > 0
    ) {
        currentSlideIndex = accommodationData.images.length - 1;
    } else if (accommodationData.images.length === 0) {
        currentSlideIndex = 0;
    }

    removedImages.push(index);
    // Re-render the slider
    renderSlider();
}

//function beforeSubmit() {
//    const form = document.querySelector("form");

//    removedImages.forEach(img => {
//        const input = document.createElement("input");
//        input.type = "hidden";
//        input.name = "RemovedImages";
//        input.value = img;
//        form.appendChild(input);
//    });

//    newImages.forEach(file => {
//        const input = document.createElement("input");
//        input.type = "file";
//        input.name = "NewImages";
//        input.files = createFileList(file);
//        form.appendChild(input);
//    });
//}

function beforeSubmit() {
    const form = document.querySelector("form");

    // ارسال عکس‌های حذف‌شده
    removedImages.forEach(imgName => {
        const input = document.createElement("input");
        input.type = "hidden";
        input.name = "RemovedImages";
        input.value = imgName;
        form.appendChild(input);
    });

    // ساخت input[type=file] واقعی
    newImages.forEach(file => {
        const input = document.createElement("input");
        input.type = "file";
        input.name = "NewImages";

        const dt = new DataTransfer();
        dt.items.add(file);
        input.files = dt.files;

        form.appendChild(input);
    });
}