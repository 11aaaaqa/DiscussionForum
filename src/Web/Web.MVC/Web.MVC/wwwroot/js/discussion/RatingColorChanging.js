const ratingElements = document.querySelectorAll('.rating-value');
ratingElements.forEach(elem => {
    const value = parseInt(elem.dataset.value, 10);

    if (value === 0) {
        elem.classList.add('default');
    }
    else if (value > 0) {
        elem.classList.add('green');
    } else {
        elem.classList.add('red');
    }
});
