function selectDate(date) {
    document.getElementById("reservationDate").value = date;

    // Подсветить выбранную дату
    document.querySelectorAll(".date-btn").forEach(btn => btn.classList.remove("active"));
    document.querySelector(`.date-btn[data-date='${date}']`).classList.add("active");

    // Сначала включим все кнопки времени
    document.querySelectorAll(".time-btn").forEach(btn => {
        btn.disabled = false;
        btn.classList.remove("disabled");
    });

    // Отключим занятые для этой даты
    if (bookedSlots[date]) {
        bookedSlots[date].forEach(time => {
            let btn = document.querySelector(`.time-btn[data-time='${time}']`);
            if (btn) {
                btn.disabled = true;
                btn.classList.add("disabled");
            }
        });
    }
}

function selectTime(time) {
    document.getElementById("reservationTime").value = time;

    // Подсветить выбранное время
    document.querySelectorAll(".time-btn").forEach(btn => btn.classList.remove("active"));
    document.querySelector(`.time-btn[data-time='${time}']`).classList.add("active");
}
