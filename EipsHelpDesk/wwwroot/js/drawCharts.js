/**
 * Создание графика типа Pie
 * @param {any} section DIV-секция
 * @param {any} model Модель данных Json-формата. Пример: Json.Serialize(Model)
 */
function drawPie(section, model, backgroundColor) {
    var sessionCanvas = document.getElementById(section);    
    Chart.defaults.global.defaultFontFamily = "Source Sans Pro";
    Chart.defaults.global.defaultFontSize = 18;

    var labels = [];
    for (var i = 0; i < model.length; i++) {
        labels.push(model[i].worker.login);
    }

    var data = [];
    for (var i = 0; i < model.length; i++) {
        data.push(model[i].count);
    }

    var sessionData = {
        labels,
        datasets: [
            {
                data,
                backgroundColor
            }]
    };

    var pieChart = new Chart(sessionCanvas, {
        type: 'pie',
        data: sessionData
    });
}

/**
 * Генерация случаного цвета
 * @param {number} param параметр rgb
 */
function getRandomColor(param) {    
    return Math.floor(Math.random() * Math.floor(param));
}