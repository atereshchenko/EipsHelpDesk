/**
 * Функция для работы датой и временем. Русская локализация
 * 
 * @param {any} idInput - идентификатор Input-а
 * @param {any} value - значение даты и времени
 * @param {any} datetimePickerSection - идентификтаор секции для вставки кнопки вызова календаря
 */
function SetDateTimePickerValue(idInput, value, datetimePickerSection) {
    moment.locale('ru');
    var date = new Date(moment(value));
    $(idInput).val(moment(date).format('YYYY-MM-DD HH:mm'));
    $(datetimePickerSection).datetimepicker({
        icons: {
            time: "far fa-clock"
        },
        format: "YYYY-MM-DD HH:mm",
        locale: 'LL',
        timepicker: true
    });
}

/**
 * Функция преобразования даты и времени. Русская локализация
 * 
 * @param {any} idInput - идентификатор Input-а
 * @param {any} value - значение даты и времени 
 */
function TransformationDate(idInput, value) {
    moment.locale('ru');
    var date = new Date(moment(value));
    $(idInput).val(moment(date).format('YYYY-MM-DD HH:mm'));    
}