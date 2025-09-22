const userId = document.getElementById("UserId").value;

// Получаем список ролей и данные пользователя
async function loadData() {
    try {
        const rolesRes = await fetch("/api/users/roles");
        const roles = await rolesRes.json();
        const roleSelect = document.getElementById("IdRole");
        roles.forEach(r => {
            const option = document.createElement("option");
            option.value = r.id;
            option.text = r.name;
            roleSelect.add(option);
        });

        const userRes = await fetch(`/api/users/${userId}`);
        const user = await userRes.json();

        document.getElementById("SurName").value = user.surName;
        document.getElementById("Name").value = user.name;
        document.getElementById("Patronomic").value = user.patronomic ?? "";
        document.getElementById("UserName").value = user.userName;
        document.getElementById("Email").value = user.email ?? "";
        document.getElementById("PhoneNumber").value = user.phoneNumber ?? "";
        document.getElementById("Birthdate").value = user.birthdate?.split("T")[0] ?? "";
        document.getElementById("IdRole").value = user.idRole;
        document.getElementById("IsActive").checked = user.isActive;
    } catch(err) {
        console.error(err);
        alert("Ошибка загрузки данных пользователя или ролей");
    }
}

// Отправка формы через API
document.getElementById("editUserForm").addEventListener("submit", async function(e){
    e.preventDefault();

    const payload = {
        surName: document.getElementById("SurName").value,
        name: document.getElementById("Name").value,
        patronomic: document.getElementById("Patronomic").value,
        userName: document.getElementById("UserName").value,
        newPassword: document.getElementById("NewPassword").value,
        email: document.getElementById("Email").value,
        phoneNumber: document.getElementById("PhoneNumber").value,
        birthdate: document.getElementById("Birthdate").value || null,
        idRole: parseInt(document.getElementById("IdRole").value),
        isActive: document.getElementById("IsActive").checked
    };

    try {
        const res = await fetch(`/api/users/${userId}`, {
            method: "PUT",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(payload)
        });

        if(res.ok){
            alert("Пользователь обновлён!");
            window.location.href = "/Admin/AdminUsers";
        } else {
            const error = await res.json();
            alert("Ошибка: " + (error.message ?? "Не удалось обновить пользователя"));
        }
    } catch(err){
        console.error(err);
        alert("Ошибка запроса к API");
    }
});

// Запускаем загрузку данных при открытии страницы
loadData();