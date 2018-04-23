using System;
using UnityEngine;

/**
 * A LISP lexical unit emitted by the lexer. It may be parenthesis or an
 * element.
 */
public struct Lexeme
{
    public int lineNumber;
    public int linePos;
    public string val;
}

/**
 * Provides a tool for building a lexeme character-by-character.
 */
class LexemeBuilder
{
    private bool initialized;
    private Lexeme output;

    public void Add(Scaneme scaneme)
    {
        if (initialized)
        {
            output.val += scaneme.val;
        }
        else
        {
            output = new Lexeme {
                lineNumber = scaneme.line,
                linePos = scaneme.pos,
                val = scaneme.val.ToString()
            };
            initialized = true;
        }
    }

    public Lexeme Build()
    {
        return output;
    }
}

/**
 * Emits lexemes using input from a scanner.
 */
public class Lexer
{

    enum State
    {
        Waiting,
        Element,
        String
    }

    private Scanner scanner;

    public Lexer(Scanner scanner)
    {
        this.scanner = scanner;
    }

    /**
     * Returns true if there are still lexemes to emit.
     */
    public bool HasNext()
    {
        return scanner.HasNext();
    }

    /**
     * Returns the next lexeme.
     */
    public Lexeme Next()
    {
        State state = State.Waiting;
        LexemeBuilder builder = new LexemeBuilder();

        while (scanner.HasNext())
        {
            Scaneme scaneme = scanner.Next();

            if (state == State.Waiting)
            {
                if (scaneme.val == '(' || scaneme.val == ')')
                {
                    // Immediately emit any parenthesis
                    builder.Add(scaneme);
                    return builder.Build();
                }
                else if (scaneme.val == '\"')
                {
                    // Marks the beginning of a string
                    state = State.String;
                    builder.Add(scaneme);
                }
                else if (!Char.IsWhiteSpace(scaneme.val))
                {
                    // Marks the beginning of some element
                    state = State.Element;
                    scanner.BackUp();
                }
            }
            else if (state == State.Element)
            {
                if (scaneme.val == ')' || scaneme.val == '(' || Char.IsWhiteSpace(scaneme.val))
                {
                    // Marks the termination of some element
                    scanner.BackUp();
                    return builder.Build();
                }
                builder.Add(scaneme);
            }
            else if (state == State.String)
            {
                builder.Add(scaneme);
                if (scaneme.val == '\"')
                {
                    // Marks the termination of a string
                    return builder.Build();
                }
            }
        }

        // Emit whatever we have at the end
        return builder.Build();
    }

}
