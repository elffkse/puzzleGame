# Squares

Squares is a one-player console puzzle game developed for the CME1251 course.

The player enters the number of squares for each piece and a regularity range. The program generates unique pieces, forms a puzzle, and shows the puzzle shape with `X` symbols. The player tries to form the same shape by using all pieces.

## Rules

- A piece can contain between 2 and 12 squares.
- The game can contain at most 20 pieces and 160 squares.
- Pieces can be rotated and reversed.
- All placed pieces must form the shown puzzle shape.
- A new piece list and regularity range are entered for every round.

## Controls

- Arrow keys: Move the selected piece
- Piece letter: Select a piece
- Z: Rotate the selected piece
- X: Reverse the selected piece
- Enter: Place the selected piece or check the completed puzzle
- P: Pick up a placed piece
- Q: Continue editing or quit when shown in the message

## Run

```bash
dotnet run --project Squares.csproj
```

The current game source is `Program.cs`. Other source-like files in the repository are previous project versions and are not included in the build.
