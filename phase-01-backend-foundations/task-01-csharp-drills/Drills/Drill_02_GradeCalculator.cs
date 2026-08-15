public class Drill_02_GradeCalculator
{
    public static void GradeCalculator()
    {
        Console.Write("Enter your score (0-100): ");
        string? input = Console.ReadLine();

        if (!decimal.TryParse(input, out decimal score))
        {
            Console.WriteLine("Invalid score value.");
            return;
        }

        if (score < 0 || score > 100)
        {
            Console.WriteLine("Score must be between 0 and 100");
            return;
        }

        string grade = GetGrade(score);
        Console.WriteLine($"Grade: {grade}"); // return grade;
    

    }

    private static string GetGrade(decimal score)
    {
        if (score >= 90)
            return "A";
        else if (score >= 80)
            return "B";
        else if (score >= 70)
            return "C";
        else if (score >= 60)
            return "D";
        else
            return "F";
    }
}
