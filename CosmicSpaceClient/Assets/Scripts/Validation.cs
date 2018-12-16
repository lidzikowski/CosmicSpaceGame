public class Validation
{
    public static bool Text(string text, int min_lengh = 3, int max_lengh = 20, bool civility = false)
    {
        if (string.IsNullOrEmpty(text))
            return false;

        if (text.Length <= min_lengh && text.Length >= max_lengh)
            return false;

        if (civility && Civility(text))
            return false;

        return true;
    }

    /// <summary>
    /// Sprawdzanie niedozwolonych slow.
    /// </summary>
    /// <returns>Prawda jezeli niedozwolone slowo istnieje w slowniku.</returns>
    public static bool Civility(string text)
    {
        // TO DO
        return true;
    }

    public static bool Email(string text)
    {
        // TO DO
        return true;
    }
}