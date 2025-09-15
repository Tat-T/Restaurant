document.getElementById("loginForm").addEventListener("submit", async function (e) {
        e.preventDefault();

        const email = document.getElementById("email").value;
        const password = document.getElementById("password").value;

        const response = await fetch("/api/auth/login", {
            method: "POST",
            headers: { "Content-Type": "application/json" },
            body: JSON.stringify({ email, password })
        });

        if (response.ok) {
            const result = await response.json();
            if (result.role === "Admin") {
                window.location.href = "/Admin/AdminPanel";
            } else {
                window.location.href = "/Account/Index";
            }
        } else {
            const error = await response.json();
            document.getElementById("errorMessage").textContent = error.message;
        }
    });