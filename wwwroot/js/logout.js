document.getElementById("logoutBtn").addEventListener("click", async function () {
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