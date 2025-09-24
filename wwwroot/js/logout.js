document.getElementById("logoutBtn").addEventListener("click", async function () {
    const response = await fetch("/api/auth/logout", {
        method: "POST",
        credentials: "include" // очень важно, чтобы куки сессии отправлялись
    });

    if (response.ok) {
        window.location.href = "/Account/Login"; // редирект на страницу логина
    } else {
        alert("Ошибка выхода из системы");
    }
});