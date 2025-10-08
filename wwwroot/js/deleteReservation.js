async function loadReservation(id) {
    console.log('Loading reservation id=', id);
    const res = await fetch(`/api/reservations/${id}`, { credentials: 'same-origin' });
    if (!res.ok) {
        document.getElementById('reservationInfo').innerHTML =
            `<tr><td colspan="8" class="text-danger">Ошибка: бронирование не найдено (status ${res.status}).</td></tr>`;
        console.error('Get reservation failed', res.status, await res.text().catch(()=>null));
        document.querySelector('#deleteForm button[type="submit"]').disabled = true;
        return;
    }

    const data = await res.json();
    const reservation = data.reservation ?? data;
    console.log('Loaded reservation', reservation);

    document.getElementById('reservationId').value = reservation.id;

    // преобразуем дату и время
    const date = new Date(reservation.reservationDate);
    const time = reservation.reservationTime
        ? new Date(`1970-01-01T${reservation.reservationTime}`).toLocaleTimeString('ru-RU', { hour: '2-digit', minute: '2-digit' })
        : '';

    document.getElementById('reservationInfo').innerHTML = `
        <tr>
            <td>${reservation.id}</td>
            <td>${reservation.name}</td>
            <td>${reservation.email}</td>
            <td>${reservation.phone ?? '-'}</td>
            <td>${date.toLocaleDateString('ru-RU')}</td>
            <td>${time}</td>
            <td>${reservation.guests}</td>
            <td>${reservation.message ?? '-'}</td>
        </tr>
    `;
}


document.getElementById('deleteForm').addEventListener('submit', async (e) => {
    e.preventDefault();
    const id = document.getElementById('reservationId').value;
    console.log('Deleting id=', id);
    if (!id) { alert('id пустой'); return; }

    const res = await fetch(`/api/reservations/${id}`, {
    method: 'DELETE',
    credentials: 'same-origin',
    headers: { 'Accept': 'application/json' }
    });

    console.log('DELETE status', res.status);
    let body = null;
try {
    body = res.headers.get("content-type")?.includes("application/json")
        ? await res.json()
        : await res.text();
} catch (err) {
    console.warn("Не удалось разобрать ответ", err);
}
    if (res.ok) {
        alert("Бронирование удалено");
        window.location.href = "/Admin/ZakazAdmin";
    } else {
        let msg = text;
        try {
            const json = JSON.parse(text);
            msg = json?.message || JSON.stringify(json) || text;
        } catch {}
        alert(msg || "Ошибка при удалении");
    }
});
