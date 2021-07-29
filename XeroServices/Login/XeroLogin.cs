namespace XeroServices.Login
{
    public class XeroLogin 
    {
        public XeroLogin(string email, string password)
        {
            Email = email;
            Password = password;
        }
        public string Email { get; set; }
        public string Password { get; set; }

    }
}