using DSA.Patterns;

namespace DSA
{
    internal class Program
    {   
        static void Main(string[] args)
        {
            Pattern_Enum pattern = Pattern_Enum.Character_Triangle;

            switch (pattern)
            {
                case Pattern_Enum.square:
                    Patterns_Problems.PrintSquarePattern(5);
                    break;
                case Pattern_Enum.pyramid:
                    Patterns_Problems.PrintPyramidPattern(5);
                    break;
                case Pattern_Enum.InvertedPyramid:
                    Patterns_Problems.PrintInvertedPyramidPattern(5);
                    break;
                case Pattern_Enum.Diamond:
                    Patterns_Problems.PrintDiamondPattern(5);
                    break;
                case Pattern_Enum.Half_Diamond:
                    Patterns_Problems.PrintHalfDiamondPattern(5);
                    break;
                case Pattern_Enum.Binary_Triangle:
                    Patterns_Problems.PrintBinary_TrianglePattern(5);
                    break;
                case Pattern_Enum.Symmetrical_Numbers:
                    Patterns_Problems.PrintSymmetrical_NumbersPattern(5);
                    break;
                case Pattern_Enum.Number_Triangle:
                    Patterns_Problems.PrintNumber_TrianglePattern(5);
                    break;
                case Pattern_Enum.Character_Triangle:
                    Patterns_Problems.PrintCharacter_TrianglePattern(5);
                    break;
                default:
                    break;
            }
        }
    }
}
