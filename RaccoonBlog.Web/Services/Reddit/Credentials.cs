namespace RaccoonBlog.Web.Services.Reddit
{
    public class Credentials
    {
        public string User { get; set; }

        public string Password { get; set; }

        public bool IsValid => string.IsNullOrWhiteSpace(User) == false &&
                               string.IsNullOrWhiteSpace(Password) == false;
    }
}