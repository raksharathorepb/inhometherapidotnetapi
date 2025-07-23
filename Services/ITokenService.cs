public interface ITokenService
{
    string CreateToken(ApplicationUser user, string role);
}
