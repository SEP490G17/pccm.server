
namespace API.DTOs;

public class UserResponseDto
{
    public string DisplayName { get; set; }
    public string Token { get; set; }
    public string Image { get; set; }
    public string UserName { get; set; }
    public string PhoneNumber { get; set; }

    public IList<string>? Roles  { get; set; }

}
