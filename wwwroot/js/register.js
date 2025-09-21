document.getElementById("registrationForm").addEventListener("submit", async (e) => {
    e.preventDefault();

    const payload = {
        surName: document.getElementById("surName").value,
        name: document.getElementById("name").value,
        patronomic: document.getElementById("patronomic").value,
        userName: document.getElementById("userName").value,
        email: document.getElementById("email").value,
        phoneNumber: document.getElementById("phoneNumber").value,
        password: document.getElementById("password").value
    };

    const res = await fetch("/api/Account/register", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify(payload)
    });

    const errorsDiv = document.getElementById("errors");
    errorsDiv.innerHTML = "";

    if (res.ok) {
        const data = await res.json();
        window.location.href = data.redirectUrl;
    } else {
        const errors = await res.json();
        if (Array.isArray(errors)) {
            errors.forEach(err => {
                errorsDiv.innerHTML += `<p class="text-danger">${err}</p>`;
            });
        } else {
            errorsDiv.innerHTML = `<p class="text-danger">${errors.message || "Ошибка регистрации"}</p>`;
        }
    }
});
