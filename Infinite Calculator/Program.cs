
List<char> equation = new List<char>();

while (true)
{
    Console.Write("Prompt: ");

    string? inputEquation = Console.ReadLine(); //type? is nullable

    if (!string.IsNullOrWhiteSpace(inputEquation) && CheckInputValidity(TrimString(inputEquation)))
    {
        equation = TrimString(inputEquation);
        break;
    }
    Console.Clear();
    Console.WriteLine("Invalid prompt, try again.");
}
// check div by zero
// check paranthesis validity
foreach (var item in FindParenthesisBlocks(equation))
{
    Console.WriteLine($"{item.Item1} - {item.Item2}");
}







static List<char> TrimString(string inputString)
{
    List<char> trimmed = new List<char>();
    char[] untrimmed = inputString.ToCharArray();

    for (int i = 0; i < untrimmed.Length; i++)
    {
        if (untrimmed[i] != ' ')
        {
            trimmed.Add(untrimmed[i]);
        }
    }

    return trimmed;
}

static bool CheckInputValidity(List<char> equation)
{
    int leftParenthesisCount = 0;
    int rightParenthesisCount = 0;

    for (int i = 0; i < equation.Count; i++)
    {
        switch (equation[i])
        {
            case '(':
                leftParenthesisCount++;
                break;
            case ')':
                rightParenthesisCount++;
                break;
        }
    }

    bool parenthesisValidity = leftParenthesisCount == rightParenthesisCount ? true : false;

    char[] validChars = { '.', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '(', ')', '+', '-', '*', '/', '%', '!' };
    bool charValidity = false;

    for (int i = 0; i < equation.Count; i++)
    {
        if (!validChars.Contains(equation[i]))
        {
            charValidity = false;
            break;
        }
        charValidity = true;
    }

    if (parenthesisValidity && charValidity)
    {
        return true;
    }
    return false;
}

static List<int> FindOperatorIndexes(List<char> equation)
{
    char[] operators = { '(', ')', '+', '-', '*', '/', '%', '!' };

    List<int> operatorIndexes = new List<int>();

    for (int i = 0; i < equation.Count; i++)
    {
        if (operators.Contains(equation[i]))
        {
            operatorIndexes.Add(i);
        }
    }
    return operatorIndexes;
}

static List<(int, int)> FindParenthesisBlocks(List<char> equation)
{
    //assuming all validity checks are successful (except for div by zero)

    List<(int, int)> blocks = new List<(int, int)>();

    int closeParaIndex = equation.IndexOf(')', 0);
    int openParaIndex = equation.LastIndexOf('(', closeParaIndex);

    for (int i = 0; i < equation.Count; i++)
    {
        equation[openParaIndex] = '_';

        if (blocks.Contains((openParaIndex, closeParaIndex)))
        {
            break;
        }

        blocks.Add((openParaIndex, closeParaIndex));

        closeParaIndex = equation.IndexOf(')', closeParaIndex + 1);
        if (closeParaIndex == -1) break;
        openParaIndex = equation.LastIndexOf('(', closeParaIndex);
    }
    return blocks;
}
