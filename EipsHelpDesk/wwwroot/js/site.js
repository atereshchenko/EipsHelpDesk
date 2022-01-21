/**
 * Функция для работы с активными пунктами меню
 */
function menuopen(section) {
    document.getElementById(section).className = "nav-item has-treeview menu-open";
}

/**
 * Функция для работы с активными пунктами меню
 */
function menuactive(section) {
    document.getElementById(section).className = "nav-link active";
}