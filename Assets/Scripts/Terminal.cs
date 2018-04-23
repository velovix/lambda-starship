using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Some kind of displayable text element in the terminal.
/// </summary>
public interface TerminalObject
{
    string GetText();
}

/// <summary>
/// A generic text block in the terminal with no behavior.
/// </summary>
public class TextBlock : TerminalObject
{
    private string text;

    public TextBlock(string text)
    {
        this.text = text;
    }

    public string GetText()
    {
        return text;
    }
}

/// <summary>
/// An editable text block representing an in-progress command in the terminal.
/// </summary>
public class Command : TerminalObject
{
    private string command;

    public Command()
    {
        command = "";
    }

    /// <summary>
    /// Adds a character to the command. Might be called when the user types
    /// something.
    /// </summary>
    /// <param name="c">the character to add</param>
    public void AddChar(char c)
    {
        command += c;
    }

    /// <summary>
    /// Deletes the previous character in the command.
    /// </summary>
    public void Backspace()
    {
        if (command.Length != 0)
        {
            command = command.Substring(0, command.Length-1);
        }
    }

    /// <summary>
    /// Returns the resulting command as text.
    /// </summary>
    /// <returns>text of command</returns>
    public string GetCommand()
    {
        return command;
    }

    /// <summary>
    /// Returns the command formatted with prompts, ready to be displayed to
    /// the user.
    /// </summary>
    /// <returns>displayable representation</returns>
    public string GetText()
    {
        string output = "> ";
        foreach (char c in command)
        {
            if (c == '\n')
            {
                output += "\n... ";
            }
            else
            {
                output += c;
            }
        }

        return output;
    }

    /// <summary>
    /// Deletes the current command text.
    /// </summary>
    public void Clear()
    {
        command = "";
    }

    /// <summary>
    /// Checks if every opening paran in a command has a corresponding closing
    /// paran.
    /// </summary>
    /// <returns>true if command is closed</returns>
    public bool IsClosed()
    {
        int openParans = 0;
        foreach (char c in command)
        {
            if (c == '(')
            {
                openParans++;
            }
            else if (c == ')')
            {
                openParans--;
            }
        }

        return openParans <= 0;
    }
}

/// <summary>
/// Keeps track of the state of the terminal.
/// </summary>
public class Terminal
{
    /// <summary>
    /// The current state of the terminal. Controls what operations may be
    /// performed.
    /// </summary>
    public enum State
    {
        TakingInput,
        Evaluating,
    }

    private static char CURSOR = '';
    private static int WIDTH = 50;
    private static int HEIGHT = 38;

    private List<TerminalObject> termObjects;
    private Command command;
    private bool commandReady;
    private State state;

    private bool cursorVisible;
    private float lastCursorChange;

    private bool scrollingInHistory;
    private int historyIndex;
    private List<string> history;

    public Terminal()
    {
        termObjects = new List<TerminalObject>();

        TextBlock startingBlock = new TextBlock(
                "      *** LCM PowerTerminal Lisp Prompt ***\n" +
                "16G RAM SYSTEM 64K LISP BYTES FREE");
        termObjects.Add(startingBlock);

        // Add a warning about the main generator if it's off
        if (MainGeneratorState.Get() != LispBoolean.T)
        {
            TextBlock generatorWarning = new TextBlock(
                    "WARNING: Main generator is offline!");
            termObjects.Add(generatorWarning);
        }

        TextBlock readyBlock = new TextBlock("READY");
        termObjects.Add(readyBlock);

        command = new Command();
        termObjects.Add(command);

        state = State.TakingInput;
        commandReady = false;

        cursorVisible = false;
        lastCursorChange = Time.time;

        scrollingInHistory = false;
        historyIndex = 0;
        history = new List<string>();
    }

    public State GetState()
    {
        return state;
    }

    public void SetState(State state)
    {
        this.state = state;
    }

    public void Update()
    {
        if (Time.time - lastCursorChange >= 0.5)
        {
            cursorVisible = !cursorVisible;
            lastCursorChange = Time.time;
        }
    }

    public void Backspace()
    {
        if (state == State.TakingInput)
        {
            scrollingInHistory = false;
            command.Backspace();
        }
    }

    public void Enter()
    {
        if (state == State.TakingInput)
        {
            scrollingInHistory = false;
            if (command.IsClosed())
            {
                commandReady = true;
            }
            else
            {
                command.AddChar('\n');
            }
        }
    }

    public void PreviousCommand()
    {
        if (state != State.TakingInput)
        {
            return;
        }

        if (history.Count > 0)
        {
            if (scrollingInHistory)
            {
                if (historyIndex > 0)
                {
                    historyIndex--;
                }
            }
            else
            {
                scrollingInHistory = true;
                historyIndex = history.Count - 1;
            }

            command.Clear();
            foreach (char c in history[historyIndex])
            {
                command.AddChar(c);
            }
        }
    }

    public void NextCommand()
    {
        if (state != State.TakingInput)
        {
            return;
        }

        if (scrollingInHistory)
        {
            if (historyIndex < history.Count-1)
            {
                historyIndex++;
                command.Clear();
                foreach (char c in history[historyIndex])
                {
                    command.AddChar(c);
                }
            }
            else
            {
                command.Clear();
            }
        }
    }

    public void Key(char key)
    {
        if (state == State.TakingInput)
        {
            scrollingInHistory = false;
            command.AddChar(key);
        }
    }

    public bool IsCommandReady()
    {
        return commandReady;
    }

    public string GetCommand()
    {
        if (commandReady)
        {
            return command.GetCommand();
        }
        else
        {
            throw new SystemException("Command is not ready");
        }
    }

    public void PromptForCommand()
    {
        history.Add(command.GetCommand());

        command = new Command();
        termObjects.Add(command);
        commandReady = false;
    }

    public void AddTextBlock(string text)
    {
        TextBlock block = new TextBlock(text);
        termObjects.Add(block);
        if (state == State.TakingInput)
        {
            PromptForCommand();
        }
    }

    public string GetContent()
    {
        string output = "";

        foreach (TerminalObject obj in termObjects)
        {
            output += "\n";
            output += obj.GetText();
        }

        if (state == State.TakingInput && cursorVisible)
        {
            output += CURSOR;
        }

        return Clamp(output);
    }

    public string Clamp(string input)
    {
        List<string> lines = new List<string>();

        string line = "";
        foreach (char c in input)
        {
            if (c == '\n')
            {
                lines.Add(line);
                line = "";
            }
            else
            {
                line += c;
                if (line.Length == WIDTH)
                {
                    lines.Add(line);
                    line = "";
                }
            }
        }
        if (line.Length != 0)
        {
            lines.Add(line);
        }

        if (lines.Count > HEIGHT)
        {
            lines = lines.GetRange(lines.Count - HEIGHT, HEIGHT);
        }

        string output = "";
        foreach (string l in lines)
        {
            output += l + "\n";
        }

        return output;
    }
}
