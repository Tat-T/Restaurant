async function loadUser(id) {
    try {
        const res = await fetch(`/api/users/${id}`);
        if (!res.ok) {
            document.getElementById("userInfo").innerHTML =
                `<tr><td colspan="9" class="text-danger">Ошибка: пользователь не найден</td></tr>`;
            document.querySelector("#deleteForm button[type='submit']").disabled = true;
            return;
        }

        const user = await res.json();
        document.getElementById("userId").value = user.id;
        document.getElementById("deleteTitle").innerText = `Удалить пользователя ${user.userName}?`;

        document.getElementById("userInfo").innerHTML = `
            <tr>
                <td>${user.id}</td>
                <td>${user.surName}</td>
                <td>${user.name}</td>
                <td>${user.patronomic ?? "-"}</td>
                <td>${user.userName}</td>
                <td>${user.email ?? "-"}</td>
                <td>${user.phoneNumber ?? "-"}</td>
                <td>${user.userRole?.name ?? "-"}</td>
                <td>
                    <span class="badge ${user.isActive ? "bg-success" : "bg-danger"}">
                        ${user.isActive ? "Да" : "Нет"}
                    </span>
                </td>
            </tr>
        `;
    } catch (err) {
        console.error(err);
        alert("Ошибка загрузки пользователя");
    }
}

document.getElementById("deleteForm").addEventListener("submit", async e => {
    e.preventDefault();
    const id = document.getElementById("userId").value;

    if (!confirm("Вы уверены, что хотите удалить пользователя?")) return;

    try {
        const res = await fetch(`/api/users/${id}`, { method: "DELETE" });
        if (res.ok) {
            alert("Пользователь удалён");
            window.location.href = "/Admin/AdminUsers";
        } else {
            const error = await res.json();
            alert("Ошибка: " + (error.message ?? "Не удалось удалить пользователя"));
        }
    } catch (err) {
        console.error(err);
        alert("Ошибка запроса к API");
    }
});
