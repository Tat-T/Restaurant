const id = window.location.pathname.split("/").pop();

async function loadDish() {
    const res = await fetch(`/api/Menu/${id}`);
    if (!res.ok) {
        alert("Ошибка загрузки блюда");
        return;
    }

    const dish = await res.json();

    document.getElementById("DishID").value = dish.dishID;
    document.getElementById("DishName").value = dish.dishName;
    document.getElementById("Price").value = dish.price;
    document.getElementById("IngredientNames").value = dish.ingredients.join(", ");

    if (dish.dishImage) {
        document.getElementById("currentImageContainer").innerHTML = `
            <img src="${dish.dishImage}" alt="Current" style="max-width:200px;" />
        `;
    }
}

document.getElementById("editDishForm").addEventListener("submit", async (e) => {
    e.preventDefault();

    const form = e.target;
    const formData = new FormData(form);

    const res = await fetch(`/api/Menu/${id}`, {
        method: "PUT",
        body: formData
    });

    if (res.ok) {
        window.location.href = "/Admin/MenuAdmin";
    } else {
        alert("Ошибка при обновлении блюда");
    }
});

document.addEventListener("DOMContentLoaded", loadDish);
