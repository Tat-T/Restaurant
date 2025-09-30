async function loadReservation(id) {
    console.log('Loading reservation id=', id);
    const res = await fetch(`/api/reservations/${id}`, { credentials: 'same-origin' });
    if (!res.ok) {
        document.getElementById('reservationInfo').innerHTML =
            `<p class="text-danger">Ошибка: бронирование не найдено (status ${res.status}).</p>`;
        console.error('Get reservation failed', res.status, await res.text().catch(()=>null));
        return;
    }

    const data = await res.json();
    const reservation = data.reservation ?? data;
    console.log('Loaded reservation', reservation);

    document.getElementById('reservationId').value = reservation.id;

    document.getElementById('reservationInfo').innerHTML = `
        <p><strong>Имя:</strong> ${reservation.name}</p>
        <p><strong>Email:</strong> ${reservation.email}</p>
        <p><strong>Дата:</strong> ${new Date(reservation.reservationDate).toLocaleDateString('ru-RU')}</p>
        <p><strong>Гости:</strong> ${reservation.guests}</p>
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
