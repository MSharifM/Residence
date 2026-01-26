function setupReviewModal() {
    const reviewModal = document.getElementById("review-modal");
    const ratingSpans = document.querySelectorAll(".rating-selector span");
    const reviewText = document.getElementById("review-text");
    const residenceInput = document.getElementById("residenceId");
    const reservationInput = document.getElementById("reservationId");
    const rateInput = document.getElementById("rateInput");

    document.addEventListener("click", (e) => {
        const btn = e.target.closest(".review-trigger");
        if (!btn) return;

        residenceInput.value = btn.dataset.residenceId;
        reservationInput.value = btn.dataset.reservationId;

        reviewModal.classList.remove("hidden");
        reviewModal.dataset.rating = 0;
        reviewText.value = "";
        ratingSpans.forEach(s => s.classList.remove("active"));
    });

    document.getElementById("cancel-review").addEventListener("click", () => {
        reviewModal.classList.add("hidden");
    });

    ratingSpans.forEach(span => {
        span.addEventListener("click", function () {
            const val = parseInt(this.dataset.value);
            reviewModal.dataset.rating = val;
            rateInput.value = val;

            ratingSpans.forEach((s, i) =>
                s.classList.toggle("active", i < val)
            );
        });
    });
}

document.addEventListener("DOMContentLoaded", setupReviewModal);
