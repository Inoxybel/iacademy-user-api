namespace CrossCutting.Helpers;

public static class DocumentValidator
{
    public static bool IsValidCnpj(string Cnpj)
    {
        if (!Cnpj.All(char.IsDigit))
            return false;

        int[] firstMultiplier = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] secondMultiplier = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        string tempCnpj = Cnpj[..12];
        int sum = 0;

        for (int i = 0; i < 12; i++)
            sum += int.Parse(tempCnpj[i].ToString()) * firstMultiplier[i];

        int remainder = sum % 11;
        if (remainder < 2)
            remainder = 0;
        else
            remainder = 11 - remainder;

        string digit = remainder.ToString();
        tempCnpj += digit;
        sum = 0;

        for (int i = 0; i < 13; i++)
            sum += int.Parse(tempCnpj[i].ToString()) * secondMultiplier[i];

        remainder = sum % 11;
        if (remainder < 2)
            remainder = 0;
        else
            remainder = 11 - remainder;

        digit += remainder.ToString();

        return Cnpj.EndsWith(digit);
    }

    public static bool IsValidCpf(string Cpf)
    {
        if (!Cpf.All(char.IsDigit))
            return false;

        int[] firstMultiplier = { 10, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] secondMultiplier = { 11, 10, 9, 8, 7, 6, 5, 4, 3, 2 };

        if (Cpf.Length != 11)
            return false;

        for (var j = 0; j < 10; j++)
            if (j.ToString().PadLeft(11, char.Parse(j.ToString())) == Cpf)
                return false;

        var tempCpf = Cpf[..9];
        var sum = 0;

        for (var i = 0; i < 9; i++)
            sum += int.Parse(tempCpf[i].ToString()) * firstMultiplier[i];

        var remainder = sum % 11;
        if (remainder < 2)
            remainder = 0;
        else
            remainder = 11 - remainder;

        var digit = remainder.ToString();

        tempCpf += digit;
        sum = 0;

        for (var i = 0; i < 10; i++)
            sum += int.Parse(tempCpf[i].ToString()) * secondMultiplier[i];

        remainder = sum % 11;
        if (remainder < 2)
            remainder = 0;
        else
            remainder = 11 - remainder;

        digit += remainder.ToString();

        return Cpf.EndsWith(digit);
    }
}