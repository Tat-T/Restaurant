async function loadMenu() {
    const res = await fetch("/api/Menu");
    if (!res.ok) {
        alert("Ошибка загрузки меню");
        return;
    }

    const dishes = await res.json();
    const tbody = document.getElementById("menuBody");
    tbody.innerHTML = "";

    dishes.forEach(dish => {
        const row = document.createElement("tr");

        row.innerHTML = `
            <td><img src="${dish.dishImage}" alt="${dish.dishName}" width="100"></td>
            <td>${dish.dishName}</td>
            <td>${dish.ingredients.join(", ")}</td>
            <td>${dish.price}</td>
            <td>
                <a href="/Admin/EditMenu?id=${dish.dishID}" class="btn btn-danger">Редактировать</a>
                <button class="btn btn-secondary ms-1" onclick="deleteDish(${dish.dishID})">Удалить</button>
            </td>
        `;

        tbody.appendChild(row);
    });
}

async function deleteDish(id) {
    if (!confirm("Удалить блюдо?")) return;

    const res = await fetch(`/api/Menu/${id}`, { method: "DELETE" });
    if (res.ok) {
        loadMenu();
    } else {
        alert("Ошибка при удалении");
    }
}

document.addEventListener("DOMContentLoaded", loadMenu);
