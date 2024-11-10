const replyToCommentBtn = document.getElementById('open-reply-to-comment');
const replyToCommentForm = document.getElementById('reply-to-comment-form');
const replyToCommentInput = document.querySelector('.reply-to-comment-textarea');

replyToCommentBtn.addEventListener('click', function() {
    replyToCommentBtn.style.display = 'none';
    replyToCommentForm.style.display = 'flex';
});

replyToCommentInput.addEventListener('input', function () {
    this.style.height = 'auto';
    this.style.height = this.scrollHeight + 'px';
});