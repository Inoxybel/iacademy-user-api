using CrossCutting.Enums;

namespace Domain.Entities;

public class User
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Cpf { get; set; }
    public string CellphoneNumberWithDDD { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public string CompanyRef { get; set; }
    public bool IsActivated { get; set; }
    public List<GenrePreference> GenrePreferences { get; set; }

    public User()
    {
        GenrePreferences = new List<GenrePreference>
        {
            new GenrePreference(TextGenre.Informativo, 0, 1.3),
            new GenrePreference(TextGenre.Explicativo, 0, 1.1),
            new GenrePreference(TextGenre.Narrativo, 0, 0.7),
            new GenrePreference(TextGenre.Argumentativo, 0, 0.9)
        };
    }

    public void RequestGenreChange(TextGenre genre)
    {
        var preference = GenrePreferences.FirstOrDefault(g => g.Genre == genre) ?? throw new ArgumentException("Gênero não reconhecido.");
        preference.IncrementChangeRequest();

        GenrePreferences = GenrePreferences
            .OrderBy(x => x.ChangeRequests * x.Weight)
            .ThenByDescending(x => x.Weight)
            .ToList();
    }

    public TextGenre GetNextGenre(TextGenre currentGenre)
    {
        int currentIndex = GenrePreferences.FindIndex(g => g.Genre == currentGenre);
        return currentIndex < GenrePreferences.Count - 1 ? GenrePreferences[currentIndex + 1].Genre : GenrePreferences[0].Genre;
    }
}

public class GenrePreference
{
    public TextGenre Genre { get; set; }
    public int ChangeRequests { get; private set; }
    public double Weight { get; set; }

    public GenrePreference(TextGenre genre, int changeRequests, double weight)
    {
        Genre = genre;
        ChangeRequests = changeRequests;
        Weight = weight;
    }

    public void IncrementChangeRequest()
    {
        ChangeRequests++;
    }

    public static explicit operator int(GenrePreference v)
    {
        throw new NotImplementedException();
    }
}
