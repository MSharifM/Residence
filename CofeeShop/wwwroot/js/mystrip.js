// هندل مودال ثبت نظر
function setupReviewModal() {
    const reviewModal = document.getElementById("review-modal");
    const ratingSpans = document.querySelectorAll(".rating-selector span");
    const reviewText = document.getElementById("review-text");
    let activeTripId = null;

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

    // انتخاب امتیاز
    ratingSpans.forEach((span) => {
        span.addEventListener("click", () => {
            const value = Number(span.dataset.value);
            reviewModal.dataset.rating = value;

            ratingSpans.forEach((s, i) => {
                s.classList.toggle("active", i < value);
            });
        });
    });

    // ثبت نظر
    document.getElementById("submit-review-btn").addEventListener("click", () => {
        const rating = reviewModal.dataset.rating || 0;
        const text = reviewText.value.trim();

        const trip = tripsData.find((t) => t.id == activeTripId);

        console.log("Submitting review:", {
            tripId: activeTripId,
            accommodation: trip?.accommodationName,
            rating,
            text,
        });

        alert(`نظر شما برای «${trip.accommodationName}» ثبت شد 🌟`);
        reviewModal.classList.add("hidden");
    });
}

// اجرا هنگام لود صفحه
document.addEventListener("DOMContentLoaded", () => {
    setupReviewModal();
});