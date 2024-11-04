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

setupDropdown('dropdownSuggestionsButton', 'dropdownSuggestionsList');

window.addEventListener('click', function () {
    const dropdownSuggestionsList = document.getElementById('dropdownSuggestionsList');
    dropdownSuggestionsList.style.display = 'none';
});