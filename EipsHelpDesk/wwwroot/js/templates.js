/**
 * Загрузка данных в предпросмотр на странице редактирования емейл-шаблона в момент редактирования
 */
var input = document.getElementById('textarea');
input.oninput = function () {
    document.getElementById('result').innerHTML = input.value;
    getkeyparams(input.value);
};

/**
 * Загрузка данных в предпросмотр на странице редактирования емейл-шаблона при открытии страницы
 */
document.addEventListener("DOMContentLoaded", () => {
    document.getElementById('result').innerHTML = input.value;
    getkeyparams(input.value);
});

/**
 * Подсчет параметров шаблона и отображение в отдельной card
 * @param {any} text
 */
function getkeyparams(text) {
    let keys = [];
    var re = /\{(.*?)\}/g;
    var result = text.match(re);
    for (var i = 0; i < result.length; i++) {
        if (result[i].length < 20) {
            keys.push(result[i] + "<br />");
        }
    }
    document.getElementById('keyparams').innerHTML = keys;
}