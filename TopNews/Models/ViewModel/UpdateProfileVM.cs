using TopNews.Core.DTOs.User;

namespace TopNews.Web.Models.ViewModel
{
    public class UpdateProfileVM
    {
        public UpdateUserDto UserInfo { get; set; }
        public UpdatePasswordDto UpdatePassword { get; set; }
    }
}
