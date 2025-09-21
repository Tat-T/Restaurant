document.getElementById("addDishForm").addEventListener("submit", async (e) => {
    e.preventDefault();

    const form = e.target;
    const formData = new FormData(form);

    const res = await fetch("/api/Menu", {
        method: "POST",
        body: formData
    });

    if (res.ok) {
        window.location.href = "/Admin/MenuAdmin";
    } else {
        alert("Ошибка при добавлении блюда");
    }
});
