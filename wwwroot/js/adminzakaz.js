let bookedSlots = {};

async function loadReservation(id) {
    const res = await fetch(`/api/AdminReservations/${id}`);
    if (!res.ok) return alert('Ошибка загрузки бронирования');
    const data = await res.json();

    const r = data.reservation;
    document.getElementById('reservationId').value = r.id;
    document.getElementById('name').value = r.name;
    document.getElementById('email').value = r.email;
    document.getElementById('phone').value = r.phone;
    document.getElementById('guests').value = r.guests;
    document.getElementById('message').value = r.message;

    bookedSlots = data.bookedSlots;

    // рендер дат
    const datesContainer = document.getElementById('datesContainer');
    datesContainer.innerHTML = '';
    data.availableDates.forEach(date => {
        const btn = document.createElement('button');
        btn.type = 'button';
        const dt = new Date(date);
        btn.textContent = dt.toLocaleDateString('ru-RU', { day: '2-digit', month: '2-digit', year: 'numeric' });
        btn.className = 'btn btn-dark m-1 date-btn';
        btn.dataset.date = date;
        btn.onclick = () => selectDate(date, data.availableTimes);
        datesContainer.appendChild(btn);
    });

    // выбрать дату брони или сегодняшнюю
    const currentDate = r.reservationDate?.split('T')[0] || new Date().toISOString().split('T')[0];
    selectDate(currentDate, data.availableTimes);

    // выбрать время брони
    if (r.reservationTime) {
        const time = r.reservationTime.substring(0, 5);
        selectTime(time);
    }
}

function selectDate(date, times) {
    document.getElementById('selectedDate').value = date;

    // подсветка
    document.querySelectorAll('.date-btn').forEach(btn => btn.classList.remove('active'));
    const btn = document.querySelector(`.date-btn[data-date="${date}"]`);
    if (btn) btn.classList.add('active');

    // рендер времени
    const timesContainer = document.getElementById('timesContainer');
    timesContainer.innerHTML = '';

    const todayStr = new Date().toISOString().split('T')[0];
    const now = new Date();
    const currentMinutes = now.getHours() * 60 + now.getMinutes();

    times.forEach(time => {
        const btn = document.createElement('button');
        btn.type = 'button';
        btn.textContent = time;
        btn.className = 'btn btn-dark m-1 time-btn';
        btn.dataset.time = time;

        // занятые слоты
        if (bookedSlots[date]?.includes(time) && time !== document.getElementById('selectedTime').value) {
            btn.disabled = true;
            btn.classList.add('disabled');
        }

        // прошедшие часы сегодня
        if (date === todayStr) {
            const [h, m] = time.split(':').map(Number);
            const btnMinutes = h * 60 + m;
            if (btnMinutes <= currentMinutes) {
                btn.disabled = true;
                btn.classList.add('disabled');
            }
        }

        btn.onclick = () => selectTime(time);
        timesContainer.appendChild(btn);
    });
}

function selectTime(time) {
    document.getElementById('selectedTime').value = time;
    document.querySelectorAll('.time-btn').forEach(btn => btn.classList.remove('active'));
    const btn = document.querySelector(`.time-btn[data-time="${time}"]`);
    if (btn) btn.classList.add('active');
}

// отправка
document.getElementById('editReservationForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    const id = document.getElementById('reservationId').value;

    const updated = {
        Id: +id,
        Name: document.getElementById('name').value,
        Email: document.getElementById('email').value,
        Phone: document.getElementById('phone').value,
        ReservationDate: document.getElementById('selectedDate').value,
        ReservationTime: document.getElementById('selectedTime').value,
        Guests: +document.getElementById('guests').value,
        Message: document.getElementById('message').value
    };

    const res = await fetch(`/api/AdminReservations/${id}`, {
        method: 'PUT',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(updated)
    });

    const data = await res.json();
    alert(data.message);
    if (res.ok) window.location.href = '/Admin/ZakazAdmin';
});

// авто-загрузка по ?id=
const urlParams = new URLSearchParams(window.location.search);
const reservationId = urlParams.get('id');
if (reservationId) loadReservation(reservationId);

