document.getElementById("loginForm")?.addEventListener("submit", async function (e) {
    e.preventDefault();

    const email = document.getElementById("email").value;
    const password = document.getElementById("password").value;

    const response = await fetch("/api/auth/login", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password }),
        credentials: "include"
    });

    if (response.ok) {
        const result = await response.json();
        window.location.href = result.redirectUrl;
    } else {
        const error = await response.json();
        document.getElementById("errorMessage").textContent = error.message;
    }
});

document.getElementById("logoutBtn")?.addEventListener("click", async function () {
    const response = await fetch("/api/auth/logout", {
        method: "POST",
        credentials: "include"
    });

    if (response.ok) {
        window.location.href = "/Account/Login";
    }
});
