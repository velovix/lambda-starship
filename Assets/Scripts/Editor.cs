using UnityEngine;
using System.Collections.Generic;
using System;

public class Editor
{
    public static float SHORT_MESSAGE = 2.0f;

    private static char BORDER = '';
    private static char CURSOR = '';
    private static char LINE_CONTINUATION = '';

    private static char TOP_LEFT_PIPE = '';
    private static char TOP_RIGHT_PIPE = '';
    private static char BOTTOM_LEFT_PIPE = '';
    private static char BOTTOM_RIGHT_PIPE = '';
    private static char HORIZONTAL_PIPE = '';
    private static char VERTICAL_PIPE = '';

    private static int WIDTH = 50;
    private static int HEIGHT = 38;

    private List<string> lines;

    private int cursorPos;
    private int cursorLine;
    private int topLine;

    private string message;
    private float messageTimeout;

    public Editor(string code)
    {
        lines = GetLines(code);

        cursorPos = 0;
        cursorLine = 0;
        topLine = 0;

        message = "Welcome to the editor!";
        messageTimeout = SHORT_MESSAGE;
    }

    public void Update()
    {
        if (messageTimeout > 0)
        {
            messageTimeout -= 1.0f * Time.deltaTime;
            if (messageTimeout <= 0)
            {
                message = "";
            }
        }
    }

    public void SetMessage(string message, float timeout)
    {
        this.message = message;
        messageTimeout = timeout;
    }

    public void Right()
    {
        if (lines[cursorLine].Length > this.cursorPos)
        {
            this.cursorPos++;
        }
    }

    public void Left()
    {
        if (cursorPos > 0)
        {
            cursorPos--;
        }
    }

    public void Up()
    {
        if (cursorLine > 0)
        {
            cursorLine--;

            if (cursorPos >= lines[cursorLine].Length)
            {
                cursorPos = lines[cursorLine].Length;
            }
        }
    }

    public void Down()
    {
        if (cursorLine < lines.Count - 1)
        {
            cursorLine++;

            if (cursorPos >= lines[cursorLine].Length)
            {
                cursorPos = lines[cursorLine].Length;
            }
        }
    }

    public void Backspace()
    {
        if (cursorLine == 0 && cursorPos == 0)
        {
            // We're already at the end of the editor
            return;
        }
        else if (cursorPos > 0)
        {
            string currLine = lines[cursorLine];
            lines[cursorLine] = currLine.Substring(0, cursorPos-1) +
                currLine.Substring(cursorPos, currLine.Length-cursorPos);
            cursorPos--;
        }
        else if (cursorPos == 0)
        {
            string lineContents = lines[cursorLine];
            int oldLineLength = lines[cursorLine-1].Length;
            lines[cursorLine-1] += lineContents;
            lines.RemoveAt(cursorLine);

            cursorLine--;
            cursorPos = oldLineLength;
        }
        else
        {
            throw new SystemException("Unknown situation when doing backspace");
        }
    }

    public void Enter()
    {
        cursorLine++;
        cursorPos = 0;
        lines.Insert(cursorLine, "");
    }

    public void Key(char c)
    {
        string currLine = lines[cursorLine];
        lines[cursorLine] = currLine.Substring(0, cursorPos) + c +
            currLine.Substring(cursorPos, currLine.Length-cursorPos);
        cursorPos++;
    }

    public List<string> GetLines(string text)
    {
        List<string> lines = new List<string>(text.Split('\n'));

        return lines;
    }

    public string GetContent()
    {
        string output = "";

        // Add top of border
        for (int i=0; i<WIDTH; i++)
        {
            output += HORIZONTAL_PIPE;
        }
        output += "\n";

        int linesForText = HEIGHT-3;
        int linesUsedForText = 0;

        // Adjust the view if necessary so that the cursor is visible
        if (cursorLine < topLine)
        {
            topLine = cursorLine;
        }
        else if (cursorLine >= topLine+linesForText)
        {
            topLine = cursorLine-linesForText+1;
        }

        // Calculate what the last visible line will be
        int bottomLine;
        if (lines.Count - topLine <= linesForText)
        {
            bottomLine = lines.Count-1;
        }
        else
        {
            bottomLine = topLine + linesForText;
        }

        for (int i=topLine; i<=bottomLine; i++)
        {
            for (int j=0; j<lines[i].Length; j++)
            {
                if (i == cursorLine && j == cursorPos)
                {
                    output += CURSOR;
                }
                else
                {
                    output += lines[i][j];
                    if (j % (WIDTH-2) == 0 && j != 0)
                    {
                        output += LINE_CONTINUATION + "\n";
                    }
                }
            }

            if (i == cursorLine && cursorPos == lines[i].Length)
            {
                // Cursor is at the end of the line, draw it
                output += CURSOR;
            }

            output += "\n";
            linesUsedForText++;
        }

        // Fill up any unused text space
        int remainingLines = linesForText - linesUsedForText;
        for (int i=0; i<remainingLines; i++)
        {
            output += "\n";
        }

        // Add bottom of border
        for (int i=0; i<WIDTH; i++)
        {
            output += HORIZONTAL_PIPE;
        }
        output += "\n";

        output += "     C-S : Save   C-Q : Quit   C-X : Suspend \n";

        output += message;

        return output;
    }

    public string GetCode()
    {
        string code = "";

        foreach (string line in lines)
        {
            code += line + "\n";
        }

        return code;
    }

    public void SetCode(string code)
    {
        lines = GetLines(code);
    }
}
