const commentInput = document.querySelector('.leave-a-comment-textarea');

commentInput.addEventListener('input', function() {
    this.style.height = 'auto';
    this.style.height = this.scrollHeight + 'px';
});