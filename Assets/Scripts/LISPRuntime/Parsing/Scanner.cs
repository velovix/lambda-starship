using UnityEngine;
using System;
using System.Collections.Generic;

/**
 * A character decorated with position information.
 */
public struct Scaneme
{
    public char val;
    public int pos;
    public int line;
}

/**
 * Emits scanemes one-by-one. Comments are not emitted.
 */
public class Scanner
{
    private List<Scaneme> scanemes;

    private int currScaneme;

    private bool ignoringLine;
    private int lineNumber;
    private int linePos;

    public Scanner()
    {
        scanemes = new List<Scaneme>();
    }

    public void Add(string data)
    {
        foreach (char c in data)
        {
            if (c == ';')
            {
                // Beginning of a comment, ignore the rest of the line
                ignoringLine = true;
            }

            if (!ignoringLine)
            {
                Scaneme scaneme = new Scaneme
                {
                    val = c,
                    pos = linePos,
                    line = lineNumber
                };
                scanemes.Add(scaneme);
            }
            linePos++;

            if (c == '\n')
            {
                // Go to the next line and reset in-line position
                lineNumber++;
                linePos = 0;
                // Newline marks the end of a comment
                ignoringLine = false;
            }

        }
    }

    public bool HasNext()
    {
        bool hasNonSpaceChar = false;
        for (int i=currScaneme; i<scanemes.Count; i++)
        {
            if (!Char.IsWhiteSpace(scanemes[i].val))
            {
                hasNonSpaceChar = true;
                break;
            }
        }

        return currScaneme < scanemes.Count && hasNonSpaceChar;
    }

    public Scaneme Next()
    {
        Scaneme output = scanemes[currScaneme];

        currScaneme++;

        return output;
    }

    public void BackUp()
    {
        currScaneme--;
    }
}
