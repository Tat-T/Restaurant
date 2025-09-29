document.addEventListener("DOMContentLoaded", async () => {
    // Проверяем авторизацию
    const resp = await fetch("/api/auth/user", { credentials: "include" });
    const user = await resp.json();

    if (!user.isAuthenticated || user.role !== "Admin") {
        // Если не админ — выкидываем на логин
        window.location.href = "/Account/Login";
    }
});

document.getElementById("logout").addEventListener("click", async function () {
    const response = await fetch("/api/auth/logout", {
        method: "POST",
        credentials: "include"
    });

    if (response.ok) {
        window.location.href = "/Account/Login";
    } else {
        alert("Ошибка выхода из системы");
    }
});