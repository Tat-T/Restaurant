document.addEventListener("DOMContentLoaded", async () => {
    const form = document.getElementById("addUserForm");

    // Загружаем список ролей
    const roleSelect = form.querySelector("select[name=IdRole]");
    const rolesRes = await fetch("/api/Users/Roles");
    if (rolesRes.ok) {
        const roles = await rolesRes.json();
        roles.forEach(r => {
            const option = document.createElement("option");
            option.value = r.id;
            option.text = r.name;
            roleSelect.appendChild(option);
        });
    }

    form.addEventListener("submit", async (e) => {
        e.preventDefault();

        const formData = new FormData(form);
        const data = Object.fromEntries(formData.entries());
        data.IsActive = formData.get("IsActive") === "on";

        const res = await fetch("/api/Users", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify(data)
        });

        if (res.ok) {
            alert("Пользователь добавлен!");
            window.location.href = "/Admin/AdminUsers";
        } else {
            const err = await res.json();
            alert(err.message || "Ошибка при добавлении");
        }
    });
});
