const button = document.getElementById('dropdownProfileButton');
const list = document.getElementById('dropdownProfileList');

const secondButton = document.getElementById('secondDropdownProfileButton');
const secondList = document.getElementById('secondDropdownProfileList');

button.addEventListener('click', function (event) {
    event.stopPropagation();
    if (list.style.display === 'none' || list.style.display === '') {
        list.style.display = 'block';
    } else {
        list.style.display = 'none';
    }
});

secondButton.addEventListener('click', function (event) {
    event.stopPropagation();
    if (secondList.style.display === 'none' || secondList.style.display === '') {
        secondList.style.display = 'block';
    } else {
        secondList.style.display = 'none';
    }
});

window.addEventListener('click', function () {
    list.style.display = 'none';
    secondList.style.display = 'none';
});