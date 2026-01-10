// هندل مودال ثبت نظر
function setupReviewModal() {
    const reviewModal = document.getElementById("review-modal");
    const ratingSpans = document.querySelectorAll(".rating-selector span");
    const reviewText = document.getElementById("review-text");

    // باز کردن مودال
    document.addEventListener("click", (e) => {
        const btn = e.target.closest(".review-trigger");
        if (!btn) return;

        activeTripId = btn.dataset.tripId;
        reviewModal.classList.remove("hidden");
        reviewModal.dataset.rating = 0;
        reviewText.value = "";

        ratingSpans.forEach((s) => s.classList.remove("active"));
    });

    // بستن مودال
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
}

// اجرا هنگام لود صفحه
document.addEventListener("DOMContentLoaded", () => {
    setupReviewModal();
});