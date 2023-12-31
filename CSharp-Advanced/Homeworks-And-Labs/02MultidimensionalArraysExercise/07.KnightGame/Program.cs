﻿int size = int.Parse(Console.ReadLine());

char[][] matrix = new char[size][];

for (int row = 0; row < size; row++)
{
    matrix[row] = Console.ReadLine().ToCharArray();
}

int removedHorses = 0;

while (true)
{
    int knightRow = -1;
    int knightCol = -1;
    int maxAttacked = 0;

    for (int row = 0; row < size; row++)
    {
        for (int col = 0; col < size; col++)
        {
            if (matrix[row][col] == 'K')
            {
                int tempAttack = CountAttacks(matrix, row, col);

                if (tempAttack > maxAttacked)
                {
                    maxAttacked = tempAttack;
                    knightRow = row;
                    knightCol = col;
                }
            }
        }
    }

    if (maxAttacked > 0)
    {
        matrix[knightRow][knightCol] = '0';
        removedHorses++;
    }
    else
    {
        break;
    }
}

Console.WriteLine(removedHorses);

static int CountAttacks(char[][] matrix, int row, int col)
{
    int attacks = 0;
    if (IsInMatrix(row - 1, col - 2, matrix.Length) && matrix[row - 1][col - 2] == 'K')
    {
        attacks++;
    }
    if (IsInMatrix(row - 1, col + 2, matrix.Length) && matrix[row - 1][col + 2] == 'K')
    {
        attacks++;
    }
    if (IsInMatrix(row + 1, col - 2, matrix.Length) && matrix[row + 1][col - 2] == 'K')
    {
        attacks++;
    }
    if (IsInMatrix(row + 1, col + 2, matrix.Length) && matrix[row + 1][col + 2] == 'K')
    {
        attacks++;
    }
    if (IsInMatrix(row - 2, col - 1, matrix.Length) && matrix[row - 2][col - 1] == 'K')
    {
        attacks++;
    }
    if (IsInMatrix(row - 2, col + 1, matrix.Length) && matrix[row - 2][col + 1] == 'K')
    {
        attacks++;
    }
    if (IsInMatrix(row + 2, col - 1, matrix.Length) && matrix[row + 2][col - 1] == 'K')
    {
        attacks++;
    }
    if (IsInMatrix(row + 2, col + 1, matrix.Length) && matrix[row + 2][col + 1] == 'K')
    {
        attacks++;
    }

    return attacks;
}

static bool IsInMatrix(int row, int col, int length)
{
    return row >= 0 && row < length && col >= 0 && col < length;
}