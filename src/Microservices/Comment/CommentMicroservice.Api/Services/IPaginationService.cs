﻿using CommentMicroservice.Api.Models;

namespace CommentMicroservice.Api.Services
{
    public interface IPaginationService
    {
        Task<bool> DoesNextCommentsPageExistAsync(CommentParameters commentParameters);
        Task<bool> DoesNextCommentsByUserNamePageExistAsync(string userName, CommentParameters commentParameters);
        Task<bool> DoesNextSuggestedCommentsByUserNamePageExistAsync(string userName, CommentParameters commentParameters);
        Task<bool> DoesNextSuggestedCommentsPageExistAsync(CommentParameters commentParameters);
        Task<bool> DoesNextCommentsByDiscussionIdPageExistAsync(CommentParameters commentParameters, Guid discussionsId);
    }
}
