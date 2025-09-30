document.addEventListener("DOMContentLoaded", async () => {
    const dishId = getDishId();
    if (!dishId) {
        alert("ID блюда не найден в URL");
        return;
    }

    const form = document.getElementById("editDishForm");
    const currentImageContainer = document.getElementById("currentImageContainer");
    const removeCheckbox = document.getElementById("removeImage");

    // Загружаем данные блюда
    const response = await fetch(`/api/Menu/${dishId}`);
    if (response.ok) {
        const dish = await response.json();

        document.getElementById("DishID").value = dish.dishID;
        document.getElementById("DishName").value = dish.dishName;
        document.getElementById("Price").value = dish.price;
        document.getElementById("IngredientNames").value = dish.ingredients.join(", ");

        if (dish.dishImage) {
            currentImageContainer.innerHTML =
                `<img src="${dish.dishImage}" alt="Текущее изображение" style="max-width:200px;">`;

            // Если есть картинка, чекбокс по умолчанию не отмечен
            removeCheckbox.checked = false;
        } else {
            currentImageContainer.innerHTML = "<p>Картинка отсутствует</p>";
            removeCheckbox.checked = false;
        }
    } else {
        alert("Ошибка при загрузке данных блюда");
        return;
    }

    // Сабмит формы
    form.addEventListener("submit", async (e) => {
        e.preventDefault();

        const formData = new FormData(form);

        const res = await fetch(`/api/Menu/${dishId}`, {
            method: "PUT",
            body: formData
        });

        if (res.ok) {
            alert("Блюдо обновлено!");
            window.location.href = "/Admin/MenuAdmin";
        } else {
            const err = await res.json();
            alert(err.message || "Ошибка при сохранении");
        }
    });
});

function getDishId() {
    // /Admin/EditMenu/12
    const parts = window.location.pathname.split('/').filter(Boolean);
    const last = parts[parts.length - 1];
    if (/^\d+$/.test(last)) return last;

    // /Admin/EditMenu?id=12
    const params = new URLSearchParams(window.location.search);
    return params.get("id");
}
