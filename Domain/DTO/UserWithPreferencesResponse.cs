using Domain.Entities;

namespace Domain.DTO;

public class UserWithPreferencesResponse : UserResponse
{
    public List<GenrePreference> GenrePreferences { get; set; }
}
