using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Square_Puzzle_Game
{
    internal class Program
    {
        static private Random random = new Random();
        static private int[,,,] ArrayOfPieces = new int[8, 20, 5, 5];
        static private int[] ArrayOfLetterNumbers = new int[20];

        static void Main(string[] args)
        {
            string[] letterSequenceString;
            int[] letterSequenceInt;
            bool isWithinRange = true;
            bool isOverCount = false;
            do
            {
                Console.SetCursorPosition(0, 0);
                if (isOverCount == true)
                {
                    Console.Write("Entry limit (20) exceeded. Try Again: ");
                }
                else if (isWithinRange == true)
                {
                    Console.Write("Pieces: ");
                }
                else if (isWithinRange == false)
                {
                    Console.Write("Out of the range! Please enter the values between 2 and 12: ");
                }
                string letterSequence = Console.ReadLine();
                Console.SetCursorPosition(0, 0);
                Console.Write(new string(' ', 200));
                letterSequenceString = letterSequence.Split(' ');
                letterSequenceInt = new int[letterSequenceString.Length];

                isOverCount = letterSequenceInt.Length > 20;

                for (int i = 0; i < letterSequenceString.Length; i++)
                {
                    letterSequenceInt[i] = int.Parse(letterSequenceString[i]);
                }

                for (int i = 0; i < letterSequenceInt.Length; i++)
                {
                    if (letterSequenceInt[i] > 12 || letterSequenceInt[i] < 2)
                    {
                        isWithinRange = false;
                        break;
                    }
                    else
                    {
                        isWithinRange = true;
                    }
                }
            } while (isWithinRange == false || isOverCount == true);

            string[] alphabet = {"A", "B", "C", "D", "E", "F", "G", "H", "I", "J",
            "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T" };

            int positionRow = 1;
            int pieceCounter = 0;
            for (int i = 0; i < 4; i++, positionRow += 7)
            {
                int positionColumn = 150;
                for (int j = 0; j < 5; j++, positionColumn += 10)
                {
                    Generate_Piece(letterSequenceInt[pieceCounter], positionColumn, positionRow, alphabet[pieceCounter], pieceCounter);
                    pieceCounter++;

                    if (pieceCounter > letterSequenceString.Length - 1)
                    {
                        break;
                    }
                }
                if (pieceCounter > letterSequenceString.Length - 1)
                {
                    break;
                }
            }
            Console.ReadKey();
        }
        //------GENERATE_PIECE_FUNCTION---------------------------------------------------------------
        static void Generate_Piece(int letterNumber, int positionColumn, int positionRow, string alphabet, int pieceCounter)
        {
            ArrayOfLetterNumbers[pieceCounter] = letterNumber;
            //------VARIABLES-------------------------------------------------------------------------
            int[,] pieces = new int[5, 5];
            int[] letterRows = new int[letterNumber];
            int[] letterColumns = new int[letterNumber];
            int counter = 0;
            bool isLetter = false;
            int rowFirstLetter = random.Next(0, 5);
            int columnFirstLetter = random.Next(0, 5);
            letterRows[counter] = rowFirstLetter;
            letterColumns[counter] = columnFirstLetter;
            pieces[rowFirstLetter, columnFirstLetter] = 1; // that area is full
            counter++;

            //------RANDOM LETTER PLACEMENT MECHANİC--------------------------------------------------

            for (int i = 1; i < letterNumber; i++)
            {
                int randomChooseLetter = random.Next(0, counter);
                int direction = random.Next(1, 5); // up, right, down, left

                if (direction == 1 && letterRows[randomChooseLetter] != 0 && pieces[letterRows[randomChooseLetter] - 1, letterColumns[randomChooseLetter]] != 1)
                {
                    letterRows[counter] = letterRows[randomChooseLetter] - 1;
                    letterColumns[counter] = letterColumns[randomChooseLetter];
                    pieces[letterRows[counter], letterColumns[counter]] = 1;
                    counter++;
                }
                else if (direction == 2 && letterColumns[randomChooseLetter] != 4 && pieces[letterRows[randomChooseLetter], letterColumns[randomChooseLetter] + 1] != 1)
                {
                    letterColumns[counter] = letterColumns[randomChooseLetter] + 1;
                    letterRows[counter] = letterRows[randomChooseLetter];
                    pieces[letterRows[counter], letterColumns[counter]] = 1;
                    counter++;
                }
                else if (direction == 3 && letterRows[randomChooseLetter] != 4 && pieces[letterRows[randomChooseLetter] + 1, letterColumns[randomChooseLetter]] != 1)
                {
                    letterRows[counter] = letterRows[randomChooseLetter] + 1;
                    letterColumns[counter] = letterColumns[randomChooseLetter];
                    pieces[letterRows[counter], letterColumns[counter]] = 1;
                    counter++;
                }
                else if (direction == 4 && letterColumns[randomChooseLetter] != 0 && pieces[letterRows[randomChooseLetter], letterColumns[randomChooseLetter] - 1] != 1)
                {
                    letterColumns[counter] = letterColumns[randomChooseLetter] - 1;
                    letterRows[counter] = letterRows[randomChooseLetter];
                    pieces[letterRows[counter], letterColumns[counter]] = 1;
                    counter++;
                }
                else
                {
                    i--;
                }
            }

            //------MOVE THE LETTERS TO THE TOP LEFT--------------------------------------------------
            int current_distance_up = 10; // random number that grater than 5
            int current_distance_left = 10; // random number that grater than 5
            for (int letter = 0; letter < letterRows.Length; letter++)
            {
                if (letterRows[letter] < current_distance_up)
                {
                    current_distance_up = letterRows[letter];
                }
                if (letterColumns[letter] < current_distance_left)
                {
                    current_distance_left = letterColumns[letter];
                }
            }
            for (int letter = 0; letter < letterRows.Length; letter++)
            {
                letterRows[letter] -= current_distance_up;
                letterColumns[letter] -= current_distance_left;
            }

            //------SAVE THE PİECE--------------------------------------------------------------------
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 5; j++)
                {
                    for (int k = 0; k < letterNumber; k++)
                    {
                        if (i == letterRows[k] && j == letterColumns[k])
                        {
                            ArrayOfPieces[0, pieceCounter, i, j] = 1; //that area is full
                        }
                    }
                }
            }

            //------PRINT THE PİECE ON THE SCREEN----------------------------------------------------
            for (int row = 0; row < 5; row++, positionRow++)
            {
                Console.SetCursorPosition(positionColumn, positionRow);
                for (int column = 0; column < 5; column++)
                {
                    for (int i = 0; i < letterNumber; i++)
                    {
                        if (row == letterRows[i] && column == letterColumns[i])
                        {
                            Console.Write(alphabet);
                            isLetter = true;
                            break;
                        }
                        else
                        {
                            isLetter = false;
                        }
                    }
                    if (isLetter == false)
                    {
                        Console.Write(".");
                    }
                }
            }
        }
        static bool ComparePieces(int pieceCounter)
        {

        }
    }
}
