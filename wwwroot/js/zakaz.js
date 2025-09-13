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

    // Если выбрана сегодняшняя дата — блокируем прошедшие часы
    let todayStr = new Date().toISOString().split('T')[0]; // формат yyyy-MM-dd
    if (date === todayStr) {
        let now = new Date();
        let currentMinutes = now.getHours() * 60 + now.getMinutes();

        document.querySelectorAll(".time-btn").forEach(btn => {
            let [h, m] = btn.dataset.time.split(":").map(Number);
            let btnMinutes = h * 60 + m;

            if (btnMinutes <= currentMinutes) {
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
