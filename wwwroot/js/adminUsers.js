async function loadUsers() {
    try {
        const res = await fetch('/api/users');
        const users = await res.json();
        const tbody = document.querySelector("#usersTable tbody");
        tbody.innerHTML = "";

        users.forEach(user => {
            const tr = document.createElement("tr");

            tr.innerHTML = `
                <td>${user.id}</td>
                <td>${user.surName}</td>
                <td>${user.name}</td>
                <td>${user.patronomic ?? "-"}</td>
                <td>${user.userName}</td>
                <td>${user.email ?? "-"}</td>
                <td>${user.phoneNumber ?? "-"}</td>
                <td>${user.birthdate ? new Date(user.birthdate).toLocaleDateString("ru-RU") : "-"}</td>
                <td>${new Date(user.creationDate).toLocaleDateString("ru-RU")}</td>
                <td>${user.userRole?.name ?? "—"}</td>
                <td>
                    <span class="badge ${user.isActive ? "bg-success" : "bg-danger"}">
                        ${user.isActive ? "Да" : "Нет"}
                    </span>
                </td>
                <td>
                    <a href="/Admin/EditUser/${user.id}" class="btn p-0 border-0 bg-transparent">
                        <img src="/images/pen.png" alt="Редактировать" style="width:30px;height:30px;">
                    </a>
                    <button class="btn p-0 border-0 bg-transparent" onclick="deleteUser(${user.id})">
                        <img src="/images/delete.png" alt="Удалить" style="width:30px;height:30px;">
                    </button>
                </td>
            `;
            tbody.appendChild(tr);
        });

    } catch(err) {
        console.error(err);
        alert("Ошибка загрузки списка пользователей");
    }
}

async function deleteUser(id) {
    if(!confirm("Вы уверены, что хотите удалить пользователя?")) return;

    try {
        const res = await fetch(`/api/users/${id}`, { method: "DELETE" });
        if(res.ok){
            alert("Пользователь удалён");
            loadUsers();
        } else {
            const error = await res.json();
            alert("Ошибка: " + (error.message ?? "Не удалось удалить пользователя"));
        }
    } catch(err) {
        console.error(err);
        alert("Ошибка запроса к API");
    }
}

// Загружаем пользователей при открытии страницы
loadUsers();