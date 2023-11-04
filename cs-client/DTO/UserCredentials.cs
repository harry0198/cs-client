

namespace cs_client.DTO
{
    public class UserCredentials
    {
        public UserCredentials(string userName, string password)
        {
            Username = userName;
            Password = password;
        }

        public string Username { get;}
        public string Password { get;}
    }
}
