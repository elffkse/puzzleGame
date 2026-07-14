using System;
using System.ComponentModel;
using System.Diagnostics.Eventing.Reader;
using System.Globalization;

namespace PBL
{
    internal class Program
    {
        static private Random random = new Random();

        static int[,,,] PieceMatrix = new int[8, 20, 5, 5];
        static char[] letterList = { 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 'Q', 'R', 'S', 'T' };

        static bool[] PieceUsed = new bool[20];
        static int[] PieceBlockCounts = new int[20];

        static int[,] PuzzleBoard = new int[20, 30];

        static int[] PiecePuzzleRow = new int[20];
        static int[] PiecePuzzleCol = new int[20];
        static int[] PieceUsedVariation = new int[20];

        static int[,] OriginalPuzzle = new int[20, 30];
        static int currentRound = 1;
        static int totalScore = 0;
        static int[,] bestPuzzleFound = new int[20, 30];
        static double bestRegularityFound = 0;
        static bool bestPuzzleSaved = false;

        static int UIWidth = 30;
        static int UIHeight = 20;

        static void CreatePiece(int letterCount, int pieceIndex)
        {
            PieceBlockCounts[pieceIndex] = letterCount;

            bool pieceCompleted = false;

            while (!pieceCompleted)
            {
                for (int r = 0; r < 5; r++)
                    for (int c = 0; c < 5; c++)
                        PieceMatrix[0, pieceIndex, r, c] = 0;

                int[,] letterCoordinates = new int[letterCount, 2];
                int counter = 0;

                int firstLetterRow = random.Next(0, 5);
                int firstLetterCol = random.Next(0, 5);

                letterCoordinates[counter, 0] = firstLetterRow;
                letterCoordinates[counter, 1] = firstLetterCol;
                PieceMatrix[0, pieceIndex, firstLetterRow, firstLetterCol] = 1;
                counter++;

                int totalAttempts = 0;
                int maxAttempts = 5000;

                while (counter < letterCount && totalAttempts < maxAttempts)
                {
                    int selectRandomLetter = random.Next(0, counter);
                    int direction = random.Next(1, 5);

                    int currentRow = letterCoordinates[selectRandomLetter, 0];
                    int currentCol = letterCoordinates[selectRandomLetter, 1];

                    if (direction == 1 && currentRow != 0 && PieceMatrix[0, pieceIndex, currentRow - 1, currentCol] != 1)
                    {
                        letterCoordinates[counter, 0] = currentRow - 1;
                        letterCoordinates[counter, 1] = currentCol;
                        PieceMatrix[0, pieceIndex, currentRow - 1, currentCol] = 1;
                        counter++;
                    }
                    else if (direction == 2 && currentCol != 4 && PieceMatrix[0, pieceIndex, currentRow, currentCol + 1] != 1)
                    {
                        letterCoordinates[counter, 0] = currentRow;
                        letterCoordinates[counter, 1] = currentCol + 1;
                        PieceMatrix[0, pieceIndex, currentRow, currentCol + 1] = 1;
                        counter++;
                    }
                    else if (direction == 3 && currentRow != 4 && PieceMatrix[0, pieceIndex, currentRow + 1, currentCol] != 1)
                    {
                        letterCoordinates[counter, 0] = currentRow + 1;
                        letterCoordinates[counter, 1] = currentCol;
                        PieceMatrix[0, pieceIndex, currentRow + 1, currentCol] = 1;
                        counter++;
                    }
                    else if (direction == 4 && currentCol != 0 && PieceMatrix[0, pieceIndex, currentRow, currentCol - 1] != 1)
                    {
                        letterCoordinates[counter, 0] = currentRow;
                        letterCoordinates[counter, 1] = currentCol - 1;
                        PieceMatrix[0, pieceIndex, currentRow, currentCol - 1] = 1;
                        counter++;
                    }

                    totalAttempts++;
                }

                if (counter == letterCount)
                    pieceCompleted = true;
            }

            AlignVariationLeft(0, pieceIndex);
        }

        static void DrawPiece(int pieceIndex, int posCol, int posRow, char pieceLetter)
        {
            for (int r = 0; r < 5; r++)
            {
                Console.SetCursorPosition(posCol, posRow + r);
                for (int c = 0; c < 5; c++)
                {
                    if (PieceMatrix[0, pieceIndex, r, c] == 1)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write(pieceLetter + "");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("*");
                        Console.ResetColor();
                    }
                }
            }
        }

        static void CreateVariations(int pieceIndex)
        {
            MirrorMatrix(pieceIndex, 0, 4);

            for (int i = 0; i < 3; i++)
            {
                RotateMatrix(pieceIndex, 0 + i, 0 + i + 1);

                RotateMatrix(pieceIndex, 4 + i, 4 + i + 1);
            }

            for (int v = 1; v < 8; v++) AlignVariationLeft(v, pieceIndex);
        }

        static void RotateMatrix(int pIndex, int src, int dest)
        {
            for (int r = 0; r < 5; r++)
                for (int c = 0; c < 5; c++)
                {
                    PieceMatrix[dest, pIndex, c, 4 - r] = 0;
                    if (PieceMatrix[src, pIndex, r, c] == 1)
                        PieceMatrix[dest, pIndex, c, 4 - r] = 1;
                }
        }

        static void MirrorMatrix(int pIndex, int src, int dest)
        {
            for (int r = 0; r < 5; r++)
                for (int c = 0; c < 5; c++)
                    PieceMatrix[dest, pIndex, r, 4 - c] = PieceMatrix[src, pIndex, r, c];
        }

        static void AlignVariationLeft(int position, int pIndex)
        {
            int minRow = 5, minC = 5;
            for (int r = 0; r < 5; r++)
                for (int c = 0; c < 5; c++)
                    if (PieceMatrix[position, pIndex, r, c] == 1)
                    {
                        if (r < minRow) minRow = r;
                        if (c < minC) minC = c;
                    }
            if (minRow == 0 && minC == 0) return;

            int[,] temp = new int[5, 5];
            for (int r = 0; r < 5; r++)
                for (int c = 0; c < 5; c++)
                    if (PieceMatrix[position, pIndex, r, c] == 1)
                        temp[r - minRow, c - minC] = 1;
            for (int r = 0; r < 5; r++)
                for (int c = 0; c < 5; c++)
                    PieceMatrix[position, pIndex, r, c] = temp[r, c];
        }

        static bool HasDuplicate(int current)
        {
            for (int previous = 0; previous < current; previous++)
            {
                if (PieceBlockCounts[current] != PieceBlockCounts[previous])
                    continue;

                for (int v_current = 0; v_current < 8; v_current++)
                {
                    for (int v_previous = 0; v_previous < 8; v_previous++)
                    {

                        if (AreMatricesEqual(v_current, current, v_previous, previous))
                            return true;
                    }
                }
            }
            return false;
        }

        static bool AreMatricesEqual(int variation1, int pieceIdx1, int variation2, int pieceIdx2)
        {
            for (int r = 0; r < 5; r++)
                for (int c = 0; c < 5; c++)
                    if (PieceMatrix[variation1, pieceIdx1, r, c] != PieceMatrix[variation2, pieceIdx2, r, c])
                        return false;
            return true;
        }

        static bool CreatePuzzle(int pieceCount)
        {
            for (int r = 0; r < 20; r++)
                for (int c = 0; c < 30; c++)
                    PuzzleBoard[r, c] = 0;

            int centerRow = 10;
            int centerCol = 15;

            int variation = random.Next(0, 8);
            PieceUsedVariation[0] = variation;

            if (!PlacePiece(0, variation, centerRow, centerCol))
            {
                return false;
            }

            for (int p = 1; p < pieceCount; p++)
            {
                bool placed = false;
                int attemptCount = 0;
                int maxAttempts = 500;

                while (!placed && attemptCount < maxAttempts)
                {
                    variation = random.Next(0, 8);

                    int[]? position = FindCentralPosition(p, variation);

                    if (position != null)
                    {
                        int attemptRow = position[0];
                        int attemptCol = position[1];

                        if (PlacePiece(p, variation, attemptRow, attemptCol))
                        {
                            placed = true;
                            PieceUsedVariation[p] = variation;
                        }
                    }
                    attemptCount++;
                }

                if (!placed)
                {
                    return false;
                }
            }

            return true;
        }

        static bool PlacePiece(int pieceIndex, int variation, int startRow, int startCol)
        {
            for (int r = 0; r < 5; r++)
            {
                for (int c = 0; c < 5; c++)
                {
                    if (PieceMatrix[variation, pieceIndex, r, c] == 1)
                    {
                        int targetRow = startRow + r;
                        int targetCol = startCol + c;

                        if (targetRow < 0 || targetRow >= 20 || targetCol < 0 || targetCol >= 30)
                            return false;

                        if (PuzzleBoard[targetRow, targetCol] != 0)
                            return false;
                    }
                }
            }

            if (pieceIndex > 0)
            {
                bool hasNeighbor = false;
                for (int r = 0; r < 5; r++)
                {
                    for (int c = 0; c < 5; c++)
                    {
                        if (PieceMatrix[variation, pieceIndex, r, c] == 1)
                        {
                            int targetRow = startRow + r;
                            int targetCol = startCol + c;

                            if ((targetRow > 0 && PuzzleBoard[targetRow - 1, targetCol] != 0) ||
                                (targetRow < 19 && PuzzleBoard[targetRow + 1, targetCol] != 0) ||
                                (targetCol > 0 && PuzzleBoard[targetRow, targetCol - 1] != 0) ||
                                (targetCol < 29 && PuzzleBoard[targetRow, targetCol + 1] != 0))
                            {
                                hasNeighbor = true;
                                break;
                            }
                        }
                    }
                    if (hasNeighbor) break;
                }

                if (!hasNeighbor)
                    return false;
            }

            for (int r = 0; r < 5; r++)
            {
                for (int c = 0; c < 5; c++)
                {
                    if (PieceMatrix[variation, pieceIndex, r, c] == 1)
                    {
                        int targetRow = startRow + r;
                        int targetCol = startCol + c;
                        PuzzleBoard[targetRow, targetCol] = pieceIndex + 1;
                    }
                }
            }

            PiecePuzzleRow[pieceIndex] = startRow;
            PiecePuzzleCol[pieceIndex] = startCol;

            return true;
        }

        static void DrawPuzzleInUI(int UIWidth, int UIHeight, char[] letterList)
        {
            for (int r = 0; r < 20; r++)
            {
                for (int c = 0; c < 30; c++)
                {
                    Console.SetCursorPosition(2 + c, 2 + r);

                    if (PuzzleBoard[r, c] != 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.Write("X");
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write(".");
                        Console.ResetColor();
                    }
                }
            }
            UpdateRegularityDisplay();
        }

        static void UpdateRegularityDisplay()
        {
            Console.SetCursorPosition(2, 30);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"Current Regularity: {calculate_regularity(PuzzleBoard):F4}          ");
            Console.ResetColor();
        }

        static void UI(int width, int height)
        {
            Console.SetCursorPosition(1, 1);
            Console.Write("+");
            for (int i = 0; i < width; i++) Console.Write("-");
            Console.Write("+");

            for (int i = 0; i < height; i++)
            {
                Console.SetCursorPosition(1, 2 + i);
                Console.Write("|");
                Console.SetCursorPosition(2 + width, 2 + i);
                Console.Write("|");
            }

            Console.SetCursorPosition(1, 2 + height);
            Console.Write("+");
            for (int i = 0; i < width; i++) Console.Write("-");
            Console.Write("+");

            Console.SetCursorPosition(0, 0);
        }

        static int[]? FindCentralPosition(int pieceIndex, int variation)
        {
            int[] filledRows = new int[600];
            int[] filledCols = new int[600];
            int filledCount = 0;

            for (int r = 0; r < 20; r++)
                for (int c = 0; c < 30; c++)
                    if (PuzzleBoard[r, c] != 0)
                    {
                        filledRows[filledCount] = r;
                        filledCols[filledCount] = c;
                        filledCount++;
                    }

            if (filledCount == 0) return null;

            int[] pieceRows = new int[25];
            int[] pieceCols = new int[25];
            int pieceFilledCount = 0;

            for (int r = 0; r < 5; r++)
                for (int c = 0; c < 5; c++)
                    if (PieceMatrix[variation, pieceIndex, r, c] == 1)
                    {
                        pieceRows[pieceFilledCount] = r;
                        pieceCols[pieceFilledCount] = c;
                        pieceFilledCount++;
                    }

            int attemptLimit = 100;
            for (int i = 0; i < attemptLimit; i++)
            {
                int randPuzzleIdx = random.Next(0, filledCount);
                int refR = filledRows[randPuzzleIdx];
                int refC = filledCols[randPuzzleIdx];

                int randPieceIdx = random.Next(0, pieceFilledCount);
                int pR = pieceRows[randPieceIdx];
                int pC = pieceCols[randPieceIdx];

                int direction = random.Next(0, 4);

                int targetR = refR;
                int targetC = refC;

                if (direction == 0) targetR--;
                else if (direction == 1) targetC++;
                else if (direction == 2) targetR++;
                else targetC--;

                int resultR = targetR - pR;
                int resultC = targetC - pC;

                if (resultR >= 0 && resultR <= 15 && resultC >= 0 && resultC <= 25)
                {
                    return new int[] { resultR, resultC };
                }
            }
            return null;
        }

        static double calculate_regularity(int[,] board)
        {
            int total_squares = 0;
            int total_perimeter = 0;
            int row = board.GetLength(0);
            int column = board.GetLength(1);

            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < column; j++)
                {
                    if (board[i, j] != 0)
                    {
                        total_squares++;

                        if (i == 0 || board[i - 1, j] == 0)
                            total_perimeter++;
                        if (i == row - 1 || board[i + 1, j] == 0)
                            total_perimeter++;
                        if (j == 0 || board[i, j - 1] == 0)
                            total_perimeter++;
                        if (j == column - 1 || board[i, j + 1] == 0)
                            total_perimeter++;
                    }
                }
            }
            if (total_perimeter == 0)
                return 0;

            double dividedBy4 = total_perimeter / 4.0;
            double denominator = Math.Pow(dividedBy4, 2);

            return total_squares / denominator;
        }

        static bool TryReadRegularity(string input, out double regularity)
        {
            input = input.Replace(',', '.');
            return double.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out regularity);
        }

        static bool HandleMovement(int totalPieces)
        {
            // Save the original puzzle layout
            for (int r = 0; r < 20; r++)
                for (int c = 0; c < 30; c++)
                    OriginalPuzzle[r, c] = PuzzleBoard[r, c];

            // Clear PuzzleBoard for user placements
            for (int r = 0; r < 20; r++)
                for (int c = 0; c < 30; c++)
                    PuzzleBoard[r, c] = 0;

            int cursorX = 10;
            int cursorY = 10;

            int selectedPieceIndex = 0;

            Console.CursorVisible = false;
            ConsoleKeyInfo pressedKey;

            for (int i = 0; i < totalPieces; i++)
            {
                PieceUsed[i] = false;
            }

            int currentVariation = PieceUsedVariation[selectedPieceIndex];
            for (int r = 0; r < 5; r++)
            {
                for (int c = 0; c < 5; c++)
                {
                    if (PieceMatrix[currentVariation, selectedPieceIndex, r, c] == 1)
                    {
                        DrawCharacter(cursorX + c, cursorY + r, letterList[selectedPieceIndex]);
                    }
                }
            }

            while (true)
            {
                if (AllPiecesPlaced(totalPieces))
                {
                    Console.SetCursorPosition(0, 26);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("All pieces placed! Press ENTER to check, Q to continue editing");
                    Console.ResetColor();

                    ConsoleKeyInfo finishKey = Console.ReadKey(true);
                    if (finishKey.Key == ConsoleKey.Enter)
                    {
                        if (IsPuzzleCorrect())
                        {
                            return true;
                        }

                        Console.SetCursorPosition(0, 26);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("The puzzle shape is not correct. Press any key to continue.      ");
                        Console.ResetColor();
                        Console.ReadKey(true);
                        Console.SetCursorPosition(0, 26);
                        Console.Write("                                                                     ");
                        continue;
                    }
                    else if (finishKey.Key == ConsoleKey.Q)
                    {
                        Console.SetCursorPosition(0, 26);
                        Console.Write("                                                                     ");
                    }
                }

                pressedKey = Console.ReadKey(true);

                // Save old state
                int oldX = cursorX;
                int oldY = cursorY;
                int oldPieceIndex = selectedPieceIndex;
                int oldVariationIndex = PieceUsedVariation[selectedPieceIndex];

                for (int i = 0; i < totalPieces; i++)
                {
                    if (pressedKey.Key.ToString() == letterList[i].ToString())
                    {
                        selectedPieceIndex = i;
                    }
                }

                switch (pressedKey.Key)
                {
                    case ConsoleKey.RightArrow:
                        if (cursorX < 26) cursorX++;
                        break;
                    case ConsoleKey.LeftArrow:
                        if (cursorX > 2) cursorX--;
                        break;
                    case ConsoleKey.UpArrow:
                        if (cursorY > 2) cursorY--;
                        break;
                    case ConsoleKey.DownArrow:
                        if (cursorY < 16) cursorY++;
                        break;

                    case ConsoleKey.Z:
                        int oldVariation = PieceUsedVariation[selectedPieceIndex];

                        // Normal 0-3 : 0>1>2>3>0
                        // Reverse 4-7 : 4>5>6>7>4
                        if (oldVariation < 4)
                            PieceUsedVariation[selectedPieceIndex] = (oldVariation + 1) % 4;
                        else
                            PieceUsedVariation[selectedPieceIndex] = 4 + ((oldVariation - 4 + 1) % 4);
                        break;

                    case ConsoleKey.X:
                        int currentVar = PieceUsedVariation[selectedPieceIndex];
                        int rotation = currentVar % 4;
                        bool isMirrored = currentVar >= 4;
                        int newRotation = rotation;
                        if (rotation == 1) newRotation = 3;
                        else if (rotation == 3) newRotation = 1;

                        if (isMirrored)
                            PieceUsedVariation[selectedPieceIndex] = newRotation;
                        else
                            PieceUsedVariation[selectedPieceIndex] = newRotation + 4;
                        break;

                    case ConsoleKey.P:
                        int checkR = cursorY - 2;
                        int checkC = cursorX - 2;

                        if (checkR >= 0 && checkR < 20 && checkC >= 0 && checkC < 30)
                        {
                            int pieceAtCursor = PuzzleBoard[checkR, checkC];

                            if (pieceAtCursor != 0)
                            {
                                int pieceIndex = pieceAtCursor - 1;

                                int usedVariation = PieceUsedVariation[pieceIndex];
                                int pieceStartR = -1, pieceStartC = -1;

                                for (int r = 0; r < 20; r++)
                                {
                                    for (int c = 0; c < 30; c++)
                                    {
                                        if (PuzzleBoard[r, c] == pieceAtCursor)
                                        {
                                            if (pieceStartR == -1 || r < pieceStartR || (r == pieceStartR && c < pieceStartC))
                                            {
                                                pieceStartR = r;
                                                pieceStartC = c;
                                            }
                                        }
                                    }
                                }

                                for (int r = 0; r < 20; r++)
                                {
                                    for (int c = 0; c < 30; c++)
                                    {
                                        if (PuzzleBoard[r, c] == pieceAtCursor)
                                        {
                                            PuzzleBoard[r, c] = 0;

                                            Console.SetCursorPosition(2 + c, 2 + r);
                                            if (OriginalPuzzle[r, c] != 0)
                                            {
                                                Console.ForegroundColor = ConsoleColor.Cyan;
                                                Console.Write("X");
                                            }
                                            else
                                            {
                                                Console.ForegroundColor = ConsoleColor.DarkGray;
                                                Console.Write(".");
                                            }
                                            Console.ResetColor();
                                        }
                                    }
                                }

                                PieceUsed[pieceIndex] = false;

                                Console.SetCursorPosition(0, 26);
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.Write($"Piece {letterList[pieceIndex]} picked up!        ");
                                Console.ResetColor();
                                System.Threading.Thread.Sleep(300);
                                Console.SetCursorPosition(0, 26);
                                Console.Write("                                                                     ");
                            }
                        }
                        continue;

                    case ConsoleKey.Enter:
                        currentVariation = PieceUsedVariation[selectedPieceIndex];

                        if (PieceUsed[selectedPieceIndex])
                        {
                            Console.SetCursorPosition(0, 26);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("This piece is already used! Press P to pick it up first.        ");
                            Console.ResetColor();
                            System.Threading.Thread.Sleep(800);
                            Console.SetCursorPosition(0, 26);
                            Console.Write("                                                                     ");
                            continue;
                        }

                        bool canBePlaced = true;
                        for (int r = 0; r < 5; r++)
                        {
                            for (int c = 0; c < 5; c++)
                            {
                                if (PieceMatrix[currentVariation, selectedPieceIndex, r, c] == 1)
                                {
                                    int targetRow = (cursorY + r) - 2;
                                    int targetCol = (cursorX + c) - 2;

                                    if (targetRow < 0 || targetRow >= 20 || targetCol < 0 || targetCol >= 30 || PuzzleBoard[targetRow, targetCol] != 0)
                                    {
                                        canBePlaced = false;
                                        break;
                                    }
                                }
                            }
                            if (!canBePlaced) break;
                        }

                        if (canBePlaced)
                        {
                            for (int r = 0; r < 5; r++)
                            {
                                for (int c = 0; c < 5; c++)
                                {
                                    if (PieceMatrix[currentVariation, selectedPieceIndex, r, c] == 1)
                                    {
                                        ClearBackground(cursorX + c, cursorY + r);
                                    }
                                }
                            }

                            char pieceLetter = letterList[selectedPieceIndex];
                            for (int r = 0; r < 5; r++)
                            {
                                for (int c = 0; c < 5; c++)
                                {
                                    if (PieceMatrix[currentVariation, selectedPieceIndex, r, c] == 1)
                                    {
                                        int targetRow = (cursorY + r) - 2;
                                        int targetCol = (cursorX + c) - 2;
                                        PuzzleBoard[targetRow, targetCol] = selectedPieceIndex + 1;

                                        Console.SetCursorPosition(2 + targetCol, 2 + targetRow);

                                        Console.ForegroundColor = ConsoleColor.DarkYellow;
                                        Console.Write(pieceLetter);
                                        Console.ResetColor();
                                    }
                                }
                            }

                            PieceUsed[selectedPieceIndex] = true;

                            Console.SetCursorPosition(0, 26);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write($"Piece {pieceLetter} placed!        ");
                            Console.ResetColor();
                            System.Threading.Thread.Sleep(300);
                            Console.SetCursorPosition(0, 26);
                            Console.Write("                                                                     ");
                            continue;
                        }
                        else
                        {
                            Console.SetCursorPosition(0, 26);
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.Write("Cannot place piece here!        ");
                            Console.ResetColor();
                            System.Threading.Thread.Sleep(800);
                            Console.SetCursorPosition(0, 26);
                            Console.Write("                                                                     ");
                        }
                        continue;

                    case ConsoleKey.Q:
                        Console.SetCursorPosition(0, 27);
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write("Are you sure you want to quit? (Y/N): ");
                        Console.ResetColor();
                        ConsoleKeyInfo quitConfirm = Console.ReadKey(true);
                        if (quitConfirm.Key == ConsoleKey.Y)
                        {
                            return false;
                        }
                        Console.SetCursorPosition(0, 27);
                        Console.Write("                                                                     ");
                        continue;
                }

                if (oldX != cursorX || oldY != cursorY || oldPieceIndex != selectedPieceIndex || oldVariationIndex != PieceUsedVariation[selectedPieceIndex])
                {
                    for (int r = 0; r < 5; r++)
                    {
                        for (int c = 0; c < 5; c++)
                        {
                            if (PieceMatrix[oldVariationIndex, oldPieceIndex, r, c] == 1)
                            {
                                ClearBackground(oldX + c, oldY + r);
                            }
                        }
                    }

                    currentVariation = PieceUsedVariation[selectedPieceIndex];

                    Console.SetCursorPosition(0, 25);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Selected: " + letterList[selectedPieceIndex] + " | Rotate(Z) | Reverse(X) | Place(ENTER) | Pickup(P)    ");
                    Console.ResetColor();

                    for (int r = 0; r < 5; r++)
                    {
                        for (int c = 0; c < 5; c++)
                        {
                            if (PieceMatrix[currentVariation, selectedPieceIndex, r, c] == 1)
                            {
                                DrawCharacter(cursorX + c, cursorY + r, letterList[selectedPieceIndex]);
                            }
                        }
                    }
                }
            }
        }

        static void DrawCharacter(int x, int y, char character)
        {
            if (x >= 0 && x < Console.WindowWidth && y >= 0 && y < Console.WindowHeight)
            {
                Console.SetCursorPosition(x, y);
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write(character);
                Console.ResetColor();
            }
        }

        static void ClearBackground(int x, int y)
        {
            if (x < 0 || y < 0 || x >= Console.WindowWidth || y >= Console.WindowHeight)
                return;

            int r = y - 2;
            int c = x - 2;

            if (r >= 0 && r < 20 && c >= 0 && c < 30)
            {
                int userPlacedValue = PuzzleBoard[r, c];
                int originalValue = OriginalPuzzle[r, c];

                Console.SetCursorPosition(x, y);

                if (userPlacedValue != 0)
                {
                    // User placed a piece here - show the letter
                    char letter = letterList[userPlacedValue - 1];
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.Write(letter);
                    Console.ResetColor();
                }
                else if (originalValue != 0)
                {
                    // Original puzzle had something here - show X
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("X");
                    Console.ResetColor();
                }
                else
                {
                    // Empty space - show dot
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                    Console.Write(".");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.SetCursorPosition(x, y);
                Console.Write(" ");
            }
        }

        static bool AllPiecesPlaced(int totalPieces)
        {
            for (int i = 0; i < totalPieces; i++)
            {
                if (!PieceUsed[i])
                    return false;
            }
            return true;
        }

        static bool IsPuzzleCorrect()
        {
            for (int r = 0; r < 20; r++)
            {
                for (int c = 0; c < 30; c++)
                {
                    bool userSquare = PuzzleBoard[r, c] != 0;
                    bool originalSquare = OriginalPuzzle[r, c] != 0;

                    if (userSquare != originalSquare)
                        return false;
                }
            }

            return true;
        }

        static void DisplayResults(int totalPieces)
        {
            double userRegularity = calculate_regularity(PuzzleBoard);
            int totalSquares = 0;
            for (int r = 0; r < 20; r++)
                for (int c = 0; c < 30; c++)
                    if (PuzzleBoard[r, c] != 0)
                        totalSquares++;

            int roundScore = (int)(totalSquares * Math.Pow(4 * userRegularity, 4));
            totalScore += roundScore;

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(" ======================================");
            Console.WriteLine("|             GAME OVER!               |");
            Console.WriteLine(" ======================================");
            Console.ResetColor();
            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("YOUR SOLUTION:");
            Console.ResetColor();

            for (int r = 0; r < 20; r++)
            {
                for (int c = 0; c < 30; c++)
                {
                    if (PuzzleBoard[r, c] != 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write(letterList[PuzzleBoard[r, c] - 1]);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write(".");
                    }
                }
                Console.WriteLine();
            }

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Your Regularity: {userRegularity:F4}");
            Console.ResetColor();

            // Show original puzzle
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("ORIGINAL PUZZLE:");
            Console.ResetColor();

            for (int r = 0; r < 20; r++)
            {
                for (int c = 0; c < 30; c++)
                {
                    if (OriginalPuzzle[r, c] != 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Magenta;
                        Console.Write(letterList[OriginalPuzzle[r, c] - 1]);
                        Console.ResetColor();
                    }
                    else
                    {
                        Console.Write(".");
                    }
                }
                Console.WriteLine();
            }

            double originalRegularity = calculate_regularity(OriginalPuzzle);
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"Original Regularity: {originalRegularity:F4}");
            Console.WriteLine($"Round Score: {roundScore}");
            Console.WriteLine($"Total Score: {totalScore}");
            Console.ResetColor();

            Console.WriteLine();
            if (Math.Abs(userRegularity - originalRegularity) < 0.01)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("★ PERFECT MATCH! ★");
            }
            else if (userRegularity > originalRegularity)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("★ YOU IMPROVED THE REGULARITY! ★");
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Good effort! Try to match the original regularity.");
            }
            Console.ResetColor();
        }
        /**/
        static void Main(string[] args)
        {

            while (true)
            {
                bestRegularityFound = 0;
                bestPuzzleSaved = false;
                string[] letterCountStringArray = new string[0];
                int[] letterCountIntArray = new int[0];
                int flag = 0;
                do
                {
                    Console.Clear();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    UI(UIWidth, UIHeight);
                    Console.ResetColor();

                    Console.SetCursorPosition(0, 0);
                    if (flag == 0)
                    {
                        Console.Write("Enter the number of squares for each piece (space-separated): ");
                    }
                    else if (flag == 1)
                    {
                        Console.Write("Please enter values between 2 and 12: ");
                    }
                    else if (flag == 2)
                    {
                        Console.Write("Total squares cannot exceed 160: ");
                    }
                    else
                    {
                        Console.Write("You can enter a maximum of 20 pieces: ");
                    }
                    string input = Console.ReadLine() ?? "";

                    letterCountStringArray = input.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    letterCountIntArray = new int[letterCountStringArray.Length];
                    int totalInputSquares = 0;
                    flag = 0;

                    if (letterCountStringArray.Length == 0)
                    {
                        flag = 1;
                    }
                    else if (letterCountStringArray.Length > 20)
                    {
                        flag = -1;
                    }
                    else
                    {
                        for (int i = 0; i < letterCountIntArray.Length; i++)
                        {
                            bool numberIsValid = int.TryParse(letterCountStringArray[i], out letterCountIntArray[i]);

                            if (!numberIsValid || letterCountIntArray[i] < 2 || letterCountIntArray[i] > 12)
                            {
                                flag = 1;
                                break;
                            }

                            totalInputSquares += letterCountIntArray[i];
                        }

                        if (flag == 0 && totalInputSquares > 160)
                        {
                            flag = 2;
                        }
                    }
                } while (flag != 0);

                double minReg = 0;
                double maxReg = 0;
                bool regularityIsValid = false;

                while (!regularityIsValid)
                {
                    Console.SetCursorPosition(0, UIHeight + 4);
                    Console.Write("Min. Regularity (0,1 - 1,0):                              ");
                    Console.SetCursorPosition(35, UIHeight + 4);
                    bool minIsValid = TryReadRegularity(Console.ReadLine() ?? "", out minReg);

                    Console.SetCursorPosition(0, UIHeight + 5);
                    Console.Write("Max. Regularity (0,1 - 1,0):                              ");
                    Console.SetCursorPosition(35, UIHeight + 5);
                    bool maxIsValid = TryReadRegularity(Console.ReadLine() ?? "", out maxReg);

                    regularityIsValid = minIsValid && maxIsValid &&
                                        minReg >= 0.1 && maxReg <= 1.0 && minReg <= maxReg;

                    if (!regularityIsValid)
                    {
                        Console.SetCursorPosition(0, UIHeight + 6);
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("Please enter a valid range between 0,1 and 1,0.           ");
                        Console.ResetColor();
                        System.Threading.Thread.Sleep(1000);
                        Console.SetCursorPosition(0, UIHeight + 6);
                        Console.Write("                                                                     ");
                    }
                }

                int posRow = 1;
                int pieceCounter = 0;
                int maxPieceGenerationAttempts = 50000;

                for (int i = 0; i < 4; i++, posRow += 7)
                {
                    if (pieceCounter >= letterCountStringArray.Length) break;

                    int posCol = UIWidth + 5;

                    for (int j = 0; j < 5;)
                    {
                        if (pieceCounter >= letterCountStringArray.Length) break;
                        int attemptCountPiece = 0;
                        bool isDuplicate = true;
                        while (isDuplicate && attemptCountPiece < maxPieceGenerationAttempts)
                        {
                            CreatePiece(letterCountIntArray[pieceCounter], pieceCounter);
                            CreateVariations(pieceCounter);

                            if (pieceCounter > 0)
                            {
                                isDuplicate = HasDuplicate(pieceCounter);
                            }
                            else
                            {
                                isDuplicate = false;
                            }
                            attemptCountPiece++;
                        }
                        if (isDuplicate)
                        {
                            Console.SetCursorPosition(0, UIHeight + 5);
                            Console.WriteLine($"Could not generate unique piece {letterList[pieceCounter]} after {maxPieceGenerationAttempts} attempts. Exiting.");
                            Console.ReadKey();
                            return;
                        }

                        DrawPiece(pieceCounter, posCol, posRow, letterList[pieceCounter]);

                        pieceCounter++;
                        j++;
                        posCol += 10;
                    }
                }

                bool suitablePuzzleFound = false;
                int attemptCount = 0;
                double currentRegularity = 0;

                int maxPuzzleAttempts = 50000;

                Console.SetCursorPosition(0, UIHeight + 6);
                Console.WriteLine("Generating puzzle, please wait...");

                while (!suitablePuzzleFound && attemptCount < maxPuzzleAttempts)
                {
                    attemptCount++;
                    bool puzzleCreated = CreatePuzzle(pieceCounter);
                    if (puzzleCreated)
                    {
                        currentRegularity = calculate_regularity(PuzzleBoard);

                        double currentDistance;
                        if (currentRegularity < minReg)
                            currentDistance = minReg - currentRegularity;
                        else if (currentRegularity > maxReg)
                            currentDistance = currentRegularity - maxReg;
                        else
                            currentDistance = 0;

                        double bestDistance;
                        if (bestRegularityFound < minReg)
                            bestDistance = minReg - bestRegularityFound;
                        else if (bestRegularityFound > maxReg)
                            bestDistance = bestRegularityFound - maxReg;
                        else
                            bestDistance = 0;

                        if (!bestPuzzleSaved || currentDistance < bestDistance)
                        {
                            bestRegularityFound = currentRegularity;
                            for (int r = 0; r < 20; r++)
                                for (int c = 0; c < 30; c++)
                                    bestPuzzleFound[r, c] = PuzzleBoard[r, c];
                            bestPuzzleSaved = true;
                        }

                        if (currentRegularity >= minReg && currentRegularity <= maxReg)
                        {
                            suitablePuzzleFound = true;
                            Console.SetCursorPosition(0, UIHeight + 7);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"✓ Suitable puzzle found! (Attempts: {attemptCount}, Regularity: {currentRegularity:F4})");
                            Console.ResetColor();
                            System.Threading.Thread.Sleep(2000);
                            break;
                        }
                    }
                    if (attemptCount % 100 == 0)
                    {
                        Console.SetCursorPosition(0, UIHeight + 6);
                        Console.Write($"Generating... Attempts: {attemptCount} | Best: {bestRegularityFound:F4}      ");
                    }
                }
                int totalPieceCount = pieceCounter;

                if (suitablePuzzleFound)
                {
                    for (int i = UIHeight + 6; i <= UIHeight + 10; i++)
                    {
                        Console.SetCursorPosition(0, i);
                        Console.Write(new string(' ', 80));
                    }

                    DrawPuzzleInUI(UIWidth, UIHeight, letterList);
                    System.Threading.Thread.Sleep(2000);
                }
                else
                {
                    Console.SetCursorPosition(0, UIHeight + 8);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"No puzzle found in range [{minReg:F2}-{maxReg:F2}] after {maxPuzzleAttempts} attempts.");

                    if (bestPuzzleSaved)
                    {
                        Console.SetCursorPosition(0, UIHeight + 9);
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine($"Best regularity found: {bestRegularityFound:F4}");
                        Console.ResetColor();

                        Console.SetCursorPosition(0, UIHeight + 11);
                        Console.WriteLine("Options:");
                        Console.SetCursorPosition(0, UIHeight + 12);
                        Console.WriteLine($"  [1] Use best puzzle ({bestRegularityFound:F4})");
                        Console.SetCursorPosition(0, UIHeight + 13);
                        Console.WriteLine("  [2] Enter new min/max and retry");
                        Console.SetCursorPosition(0, UIHeight + 14);
                        Console.WriteLine("  [Q] Enter new pieces");
                        Console.SetCursorPosition(0, UIHeight + 16);
                        Console.Write("Choice: ");

                        ConsoleKeyInfo choice = Console.ReadKey(true);
                        Console.Write(choice.KeyChar);

                        if (choice.Key == ConsoleKey.D1 || choice.Key == ConsoleKey.NumPad1)
                        {
                            for (int r = 0; r < 20; r++)
                                for (int c = 0; c < 30; c++)
                                    PuzzleBoard[r, c] = bestPuzzleFound[r, c];

                            suitablePuzzleFound = true;

                            for (int i = UIHeight + 6; i <= UIHeight + 16; i++)
                            {
                                Console.SetCursorPosition(0, i);
                                Console.Write(new string(' ', 80));
                            }

                            Console.SetCursorPosition(0, UIHeight + 8);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"✓ Using best puzzle (Regularity: {bestRegularityFound:F4})");
                            Console.ResetColor();

                            DrawPuzzleInUI(UIWidth, UIHeight, letterList);
                            System.Threading.Thread.Sleep(1500);
                        }
                        else if (choice.Key == ConsoleKey.D2 || choice.Key == ConsoleKey.NumPad2)
                        {
                            bool newRangeIsValid = false;
                            while (!newRangeIsValid)
                            {
                                Console.SetCursorPosition(0, UIHeight + 18);
                                Console.Write("New Min:                                               ");
                                Console.SetCursorPosition(9, UIHeight + 18);
                                bool newMinIsValid = TryReadRegularity(Console.ReadLine() ?? "", out minReg);

                                Console.SetCursorPosition(0, UIHeight + 19);
                                Console.Write("New Max:                                               ");
                                Console.SetCursorPosition(9, UIHeight + 19);
                                bool newMaxIsValid = TryReadRegularity(Console.ReadLine() ?? "", out maxReg);

                                newRangeIsValid = newMinIsValid && newMaxIsValid &&
                                                  minReg >= 0.1 && maxReg <= 1.0 && minReg <= maxReg;

                                if (!newRangeIsValid)
                                {
                                    Console.SetCursorPosition(0, UIHeight + 20);
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write("Please enter a valid range between 0,1 and 1,0.");
                                    Console.ResetColor();
                                    System.Threading.Thread.Sleep(1000);
                                }
                            }

                            for (int i = UIHeight + 6; i <= UIHeight + 20; i++)
                            {
                                Console.SetCursorPosition(0, i);
                                Console.Write(new string(' ', 80));
                            }

                            attemptCount = 0;
                            Console.SetCursorPosition(0, UIHeight + 6);
                            Console.WriteLine($"Retrying with [{minReg:F2}-{maxReg:F2}]...");

                            while (!suitablePuzzleFound && attemptCount < maxPuzzleAttempts)
                            {
                                attemptCount++;
                                bool puzzleCreated = CreatePuzzle(pieceCounter);

                                if (puzzleCreated)
                                {
                                    currentRegularity = calculate_regularity(PuzzleBoard);

                                    double currentDistance;
                                    if (currentRegularity < minReg)
                                        currentDistance = minReg - currentRegularity;
                                    else if (currentRegularity > maxReg)
                                        currentDistance = currentRegularity - maxReg;
                                    else
                                        currentDistance = 0;

                                    double bestDistance;
                                    if (bestRegularityFound < minReg)
                                        bestDistance = minReg - bestRegularityFound;
                                    else if (bestRegularityFound > maxReg)
                                        bestDistance = bestRegularityFound - maxReg;
                                    else
                                        bestDistance = 0;

                                    if (!bestPuzzleSaved || currentDistance < bestDistance)
                                    {
                                        bestRegularityFound = currentRegularity;
                                        for (int r = 0; r < 20; r++)
                                            for (int c = 0; c < 30; c++)
                                                bestPuzzleFound[r, c] = PuzzleBoard[r, c];
                                        bestPuzzleSaved = true;
                                    }

                                    if (currentRegularity >= minReg && currentRegularity <= maxReg)
                                    {
                                        suitablePuzzleFound = true;
                                        Console.SetCursorPosition(0, UIHeight + 7);
                                        Console.ForegroundColor = ConsoleColor.Green;
                                        Console.WriteLine($"✓ Found! Regularity: {currentRegularity:F4}");
                                        Console.ResetColor();

                                        for (int i = UIHeight + 6; i <= UIHeight + 10; i++)
                                        {
                                            Console.SetCursorPosition(0, i);
                                            Console.Write(new string(' ', 80));
                                        }

                                        DrawPuzzleInUI(UIWidth, UIHeight, letterList);
                                        System.Threading.Thread.Sleep(1500);
                                        break;
                                    }
                                }

                                if (attemptCount % 100 == 0)
                                {
                                    Console.SetCursorPosition(0, UIHeight + 6);
                                    Console.Write($"Retrying... Attempts: {attemptCount}      ");
                                }
                            }
                            if (!suitablePuzzleFound)
                            {
                                for (int i = UIHeight + 6; i <= UIHeight + 10; i++)
                                {
                                    Console.SetCursorPosition(0, i);
                                    Console.Write(new string(' ', 80));
                                }

                                Console.SetCursorPosition(0, UIHeight + 6);
                                Console.ForegroundColor = ConsoleColor.Yellow;
                                Console.WriteLine("Still not found. Using best available.");
                                Console.ResetColor();

                                for (int r = 0; r < 20; r++)
                                    for (int c = 0; c < 30; c++)
                                        PuzzleBoard[r, c] = bestPuzzleFound[r, c];

                                suitablePuzzleFound = true;
                                DrawPuzzleInUI(UIWidth, UIHeight, letterList);
                                System.Threading.Thread.Sleep(1500);
                            }
                        }
                        else
                        {
                            continue;
                        }
                    }
                    else
                    {
                        Console.WriteLine("No best puzzle saved. Try different parameters.");
                        Console.ReadKey();
                        continue;
                    }
                }

                if (!suitablePuzzleFound) continue;

                bool startNextRound = false;

                while (true)
                {
                    for (int i = UIHeight + 6; i <= UIHeight + 20; i++)
                    {
                        Console.SetCursorPosition(0, i);
                        Console.Write(new string(' ', 80));
                    }

                    Console.SetCursorPosition(0, UIHeight + 10);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("===============================================================");
                    Console.WriteLine($"       ROUND {currentRound} - Press ENTER to start, Q to quit");
                    Console.WriteLine($"       Current Score: {totalScore}");
                    Console.WriteLine($"       Best Regularity: {bestRegularityFound:F4}");
                    Console.WriteLine("===============================================================");
                    Console.ResetColor();

                    ConsoleKeyInfo startKey = Console.ReadKey(true);
                    if (startKey.Key == ConsoleKey.Q)
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Game Over! Final Score: {totalScore}");
                        Console.ResetColor();
                        Console.ReadKey();
                        break;
                    }

                    for (int i = UIHeight + 10; i <= UIHeight + 14; i++)
                    {
                        Console.SetCursorPosition(0, i);
                        Console.Write(new string(' ', 80));
                    }

                    bool roundCompleted = HandleMovement(totalPieceCount);

                    if (!roundCompleted)
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Game Over! Final Score: {totalScore}");
                        Console.ResetColor();
                        Console.ReadKey();
                        break;
                    }

                    DisplayResults(totalPieceCount);

                    Console.WriteLine();
                    Console.Write("Continue to next round? (Y/N): ");
                    ConsoleKeyInfo continueKey = Console.ReadKey(true);
                    if (continueKey.Key != ConsoleKey.Y)
                    {
                        Console.Clear();
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"Game Over! Final Score: {totalScore}");
                        Console.ResetColor();
                        Console.ReadKey();
                        break;
                    }

                    startNextRound = true;
                    break;
                }

                if (startNextRound)
                {
                    currentRound++;
                    continue;
                }

                Console.Write("Play new game? (Y/N): ");
                ConsoleKeyInfo playAgain = Console.ReadKey(true);
                if (playAgain.Key != ConsoleKey.Y)
                {
                    break;
                }
                totalScore = 0;
                currentRound = 1;
            }
        }
    }
}
