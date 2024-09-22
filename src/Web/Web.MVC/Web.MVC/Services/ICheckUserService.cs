namespace Web.MVC.Services
{
    public interface ICheckUserService
    {
        Task<bool> HasUserCreatedSpecifiedDiscussionsCount(string userName, uint discussionsCount);
        Task<bool> HasUserCreatedSpecifiedCommentsCount(string userName, uint commentsCount);
    }
}
