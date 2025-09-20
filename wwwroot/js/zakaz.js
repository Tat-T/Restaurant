let bookedSlots = {};

async function loadSlots() {
    const res = await fetch('/api/reservations/slots');
    const data = await res.json();

    bookedSlots = data.bookedSlots || {};

    // Автозаполнение для авторизованного пользователя
    if (data.user) {
        document.getElementById('name').value = data.user.name || '';
        document.getElementById('email').value = data.user.email || '';
        document.getElementById('phone').value = data.user.phoneNumber || '';
    }

    const datesContainer = document.getElementById('datesContainer');
    datesContainer.innerHTML = '';

    data.availableDates.forEach(date => {
        const btn = document.createElement('button');
        btn.type = 'button';
        const dt = new Date(date);
        btn.textContent = dt.toLocaleDateString('ru-RU', { day: '2-digit', month: '2-digit', year: 'numeric' }); // dd.MM.yyyy
        btn.className = 'btn btn-dark m-1 date-btn';
        btn.style.minWidth = '100px';
        btn.style.height = '40px';
        btn.onclick = () => selectDate(date, data.availableTimes);
        datesContainer.appendChild(btn);
    });

    // Автоклик по сегодняшней дате
    const today = new Date().toISOString().split('T')[0];
    if (data.availableDates.includes(today)) selectDate(today, data.availableTimes);
}

function selectDate(date, times) {
    document.getElementById('selectedDate').value = date;

    // Подсветка выбранной даты
    document.querySelectorAll('.date-btn').forEach(btn => btn.classList.remove('active'));
    document.querySelectorAll('.date-btn').forEach(btn => {
        const btnDate = new Date(btn.textContent.split('.').reverse().join('-')).toISOString().split('T')[0];
        if (btnDate === date) btn.classList.add('active');
    });

    const timesContainer = document.getElementById('timesContainer');
    timesContainer.innerHTML = '';

    times.forEach(time => {
        const btn = document.createElement('button');
        btn.type = 'button';
        btn.textContent = time;
        btn.className = 'btn btn-dark m-1 time-btn';

        // Проверка забронированных слотов
        if (bookedSlots[date]?.includes(time)) {
            btn.disabled = true;
            btn.classList.add('disabled');
        }

        // Проверка прошедшего времени для сегодняшней даты
        const todayStr = new Date().toISOString().split('T')[0];
        if (date === todayStr) {
            const now = new Date();
            const [h, m] = time.split(':').map(Number);
            if (h < now.getHours() || (h === now.getHours() && m <= now.getMinutes())) {
                btn.disabled = true;
                btn.classList.add('disabled');
            }
        }

        btn.onclick = () => selectTime(time);
        timesContainer.appendChild(btn);
    });

    // Сбрасываем выбранное время
    document.getElementById('selectedTime').value = '';
}

function selectTime(time) {
    document.getElementById('selectedTime').value = time;

    document.querySelectorAll('.time-btn').forEach(btn => btn.classList.remove('active'));
    document.querySelectorAll('.time-btn').forEach(btn => {
        if (btn.textContent === time) btn.classList.add('active');
    });
}

document.getElementById('reservationForm').addEventListener('submit', async (e) => {
    e.preventDefault();

    const reservation = {
        Name: document.getElementById('name').value,
        Email: document.getElementById('email').value,
        Phone: document.getElementById('phone').value,
        ReservationDate: document.getElementById('selectedDate').value,
        ReservationTime: document.getElementById('selectedTime').value,
        Guests: +document.getElementById('guests').value,
        Message: document.getElementById('message').value
    };

    if (!reservation.ReservationDate || !reservation.ReservationTime) {
        alert('Пожалуйста, выберите дату и время');
        return;
    }

    const res = await fetch('/api/reservations', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(reservation)
    });

    const data = await res.json();
    alert(data.message);
    if (res.ok) window.location.href = '/Success';
});

loadSlots();
