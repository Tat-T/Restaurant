function selectDate(date) {
    document.getElementById("reservationDate").value = date;

    // Подсветим выбранную дату
    document.querySelectorAll(".date-btn").forEach(btn => btn.classList.remove("active"));
    document.querySelector(`.date-btn[data-date='${date}']`).classList.add("active");
}

function selectTime(time) {
    document.getElementById("reservationTime").value = time;

    // Подсветим выбранное время
    document.querySelectorAll(".time-btn").forEach(btn => btn.classList.remove("active"));
    document.querySelector(`.time-btn[data-time='${time}']`).classList.add("active");
}
