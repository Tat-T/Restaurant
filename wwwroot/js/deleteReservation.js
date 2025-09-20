async function loadReservation(id) {
    const res = await fetch(`/api/AdminReservations/${id}`);
    if (!res.ok) {
        document.getElementById('reservationInfo').innerHTML =
            `<p class="text-danger">Ошибка: бронирование не найдено.</p>`;
        return;
    }

    const data = await res.json();
    document.getElementById('reservationId').value = data.id;

    document.getElementById('reservationInfo').innerHTML = `
        <p><strong>Имя:</strong> ${data.name}</p>
        <p><strong>Email:</strong> ${data.email}</p>
        <p><strong>Дата:</strong> ${new Date(data.reservationDate).toLocaleDateString('ru-RU')}</p>
        <p><strong>Гости:</strong> ${data.guests}</p>
    `;
}

document.getElementById('deleteForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    const id = document.getElementById('reservationId').value;

    const res = await fetch(`/api/AdminReservations/${id}`, {
        method: 'DELETE'
    });

    if (res.ok) {
        alert("Бронирование удалено");
        window.location.href = "/Admin/ZakazAdmin";
    } else {
        const error = await res.json();
        alert(error.message || "Ошибка при удалении");
    }
});

// авто-загрузка по id из query
const urlParams = new URLSearchParams(window.location.search);
const reservationId = urlParams.get('id');
if (reservationId) {
    loadReservation(reservationId);
}
