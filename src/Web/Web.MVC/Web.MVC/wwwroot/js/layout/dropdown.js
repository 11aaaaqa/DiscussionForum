const button = document.getElementById('dropdownProfileButton');
const dropdownList = document.getElementById('dropdownProfileList');

button.addEventListener('click', function() {
    if (dropdownList.style.display === 'none' || dropdownList.style.display === '') {
        dropdownList.style.display = 'block';
    } else {
        dropdownList.style.display = 'none';
    }
});

window.addEventListener('click', function() {
    if (!event.target.matches('#dropdownProfileButton')) {
        dropdownList.style.display = 'none';
    }
});