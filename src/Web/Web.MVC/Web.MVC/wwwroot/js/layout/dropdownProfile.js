const button = document.getElementById('dropdownProfileButton');
const list = document.getElementById('dropdownProfileList');

button.addEventListener('click', function (event) {
    event.stopPropagation();
    if (list.style.display === 'none' || list.style.display === '') {
        list.style.display = 'block';
    } else {
        list.style.display = 'none';
    }
});

window.addEventListener('click', function () {
    list.style.display = 'none';
});