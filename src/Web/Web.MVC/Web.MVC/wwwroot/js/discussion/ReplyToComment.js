const replyToCommentBtns = document.querySelectorAll('.open-reply-to-comment');
const replyToCommentForms = document.querySelectorAll('.reply-to-comment-form');
const replyToCommentInputs = document.querySelectorAll('.reply-to-comment-textarea');

replyToCommentBtns.forEach((replyToCommentBtn, index) => {
    replyToCommentBtn.addEventListener('click', function () {
        replyToCommentBtn.style.display = 'none';
        replyToCommentForms[index].style.display = 'flex';
    });
});

replyToCommentInputs.forEach(replyToCommentInput => {
    replyToCommentInput.addEventListener('input', function () {
        this.style.height = 'auto';
        this.style.height = this.scrollHeight + 'px';
    });
});