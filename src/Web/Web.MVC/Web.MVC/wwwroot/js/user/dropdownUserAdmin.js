function setupDropdown(buttonId, listId) {
    const button = document.getElementById(buttonId);
    const list = document.getElementById(listId);

    button.addEventListener('click', function (event) {
        event.stopPropagation();
        if (list.style.display === 'none' || list.style.display === '') {
            list.style.display = 'block';
        } else {
            list.style.display = 'none';
        }
    });
}

setupDropdown('dropdownAdminButton', 'dropdownAdminList');

window.addEventListener('click', function () {
    const dropdownAdminList = document.getElementById('dropdownAdminList');
    dropdownAdminList.style.display = 'none';
});