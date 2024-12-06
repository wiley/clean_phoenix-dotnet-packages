namespace WLS.Authorization.Models
{
    public class Token
    {
        public readonly string UserApiToken;
        public Token(string userApiToken)
        {
            UserApiToken = userApiToken;
        }
    }
}
