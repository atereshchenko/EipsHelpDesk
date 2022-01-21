/**
 * �������� ������ � ������������ �� �������� �������������� �����-������� � ������ ��������������
 */
var input = document.getElementById('textarea');
input.oninput = function () {
    document.getElementById('result').innerHTML = input.value;
    getkeyparams(input.value);
};

/**
 * �������� ������ � ������������ �� �������� �������������� �����-������� ��� �������� ��������
 */
document.addEventListener("DOMContentLoaded", () => {
    document.getElementById('result').innerHTML = input.value;
    getkeyparams(input.value);
});

/**
 * ������� ���������� ������� � ����������� � ��������� card
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