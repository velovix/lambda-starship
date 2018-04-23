using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using UnityEngine.SceneManagement;

public class ComputerBehaviour : MonoBehaviour
{
    private enum Mode { TERMINAL, EDITOR, ERROR31 };

    public GameObject focusCamera;
    public GameObject player;
    public GameObject book;
    public Text textbox;

    private bool inUse;
    private Error31Message error31Message;
    private Terminal terminal;
    private Editor editor;
    private Mode mode;

    private IEnumerator<LispExpression> switchEventChecker;

    private const String VALID_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890`~!@#$%^&*()_-+=[]{}\\|;:'\"'/?.>,< ";

    public void Start()
    {
        Runtime.Initialize();

        error31Message = new Error31Message();
        terminal = new Terminal();
        mode = Mode.ERROR31;

        textbox.text = terminal.GetContent();
        inUse = false;
    }

    public void OnInteraction()
    {
        player.SetActive(false);
        focusCamera.SetActive(true);

        focusCamera.GetComponent<RestrictedCameraMover>().Reset();

        book.GetComponent<BookBehavior>().SetUpright(true);

        inUse = true;
    }

    public void Update()
    {
        try
        {
            if (switchEventChecker == null || !switchEventChecker.MoveNext())
            {
                switchEventChecker = Runtime.CheckSwitchEvents().GetEnumerator();
            }
        }
        catch (RuntimeException e)
        {
            Debug.LogException(e, this);
            StartCoroutine(UsageStatsAPI.SendErrorInstance(e.Message));
            terminal.AddTextBlock(e.Message);
        }
        catch (SyntaxException e)
        {
            Debug.LogException(e, this);
            StartCoroutine(UsageStatsAPI.SendErrorInstance(e.Message));
            terminal.AddTextBlock(e.Message);
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            focusCamera.SetActive(false);
            player.SetActive(true);
            inUse = false;
            book.GetComponent<BookBehavior>().SetUpright(false);
        }

        if (mode == Mode.ERROR31)
        {
            Error31Update();
        }
        else if (mode == Mode.TERMINAL)
        {
            TerminalUpdate();
        }
        else if (mode == Mode.EDITOR)
        {
            EditorUpdate();
        }
    }

    public void Error31Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            mode = Mode.TERMINAL;
        }

        textbox.text = error31Message.GetContent();
    }

    public void TerminalUpdate()
    {
        try
        {
            foreach (PrintEvent e in PrintState.GetPrintEvents())
            {
                terminal.AddTextBlock(e.printable.ToString());
            }
        }
        catch (RuntimeException e)
        {
            Debug.LogException(e, this);
            StartCoroutine(UsageStatsAPI.SendErrorInstance(e.Message));
            terminal.AddTextBlock(e.Message);
        }
        catch (SyntaxException e)
        {
            Debug.LogException(e, this);
            StartCoroutine(UsageStatsAPI.SendErrorInstance(e.Message));
            terminal.AddTextBlock(e.Message);
        }

        if (!inUse)
        {
            textbox.text = terminal.GetContent();
            terminal.Update();
            return;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            terminal.PreviousCommand();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            terminal.NextCommand();
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            terminal.Backspace();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            terminal.Enter();
        }

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftAlt))
        {
            // A keyboard shortcut is being pressed
            if (Input.GetKeyDown(KeyCode.E))
            {
                Debug.Log("Going into editor");
                mode = Mode.EDITOR;
                if (editor == null)
                {
                    editor = new Editor(CodeState.Get());
                }
            }
            if (Input.GetKeyDown(KeyCode.R))
            {
                Debug.Log("Running code");
                EvaluateCode(CodeState.Get());
            }
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("Quitting a program");
                Runtime.Stop();
            }
        }
        else
        {
            foreach (char c in Input.inputString)
            {
                if (VALID_CHARS.Contains("" + c))
                {
                    terminal.Key(c);
                }
            }
        }

        if (terminal.IsCommandReady() && terminal.GetState() != Terminal.State.Evaluating)
        {
            string command = terminal.GetCommand();
            StartCoroutine(UsageStatsAPI.SendREPLCommand(command));

            EvaluateCode(command);
        }

        textbox.text = terminal.GetContent();

        terminal.Update();
    }

    public void EditorUpdate()
    {
        if (!inUse)
        {
            textbox.text = editor.GetContent();
            return;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            editor.Up();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            editor.Down();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            editor.Right();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            editor.Left();
        }
        else if (Input.GetKeyDown(KeyCode.Backspace))
        {
            editor.Backspace();
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            editor.Enter();
        }

        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl) || Input.GetKey(KeyCode.LeftAlt))
        {
            // A keyboard shortcut is being pressed
            if (Input.GetKeyDown(KeyCode.Q))
            {
                Debug.Log("Quitting editor");
                mode = Mode.TERMINAL;
                editor = null;
            }
            else if (Input.GetKeyDown(KeyCode.S))
            {
                Debug.Log("Saving");
                CodeState.Set(editor.GetCode());
                StartCoroutine(UsageStatsAPI.SendEditorContent(editor.GetCode()));
                editor.SetMessage("Saved!", Editor.SHORT_MESSAGE);
            }
            else if (Input.GetKeyDown(KeyCode.X))
            {
                Debug.Log("Suspending");
                mode = Mode.TERMINAL;
            }
        }
        else
        {
            foreach (char c in Input.inputString)
            {
                if (VALID_CHARS.Contains("" + c))
                {
                    editor.Key(c);
                }
            }
        }

        if (editor != null)
        {
            textbox.text = editor.GetContent();
            editor.Update();
        }
    }

    public void EvaluateCode(string code)
    {
        Scanner scanner = new Scanner();
        scanner.Add(code);
        Lexer lexer = new Lexer(scanner);
        ASTGenerator astGenerator = new ASTGenerator(lexer);


        // Parse the code
        Exception parseException = null;
        List<LispExpression> ast = null;
        try
        {
            ast = astGenerator.Generate();
        }
        catch (SyntaxException e)
        {
            parseException = e;
        }
        catch (RuntimeException e)
        {
            parseException = e;
        }

        if (parseException == null)
        {
            // Start running the code
            terminal.SetState(Terminal.State.Evaluating);
            StartCoroutine(RunExpressions(ast));
        }
        else
        {
            Debug.LogException(parseException, this);
            UsageStatsAPI.SendErrorInstance(parseException.Message);
            terminal.AddTextBlock(parseException.Message);
            terminal.PromptForCommand();
            terminal.SetState(Terminal.State.TakingInput);
        }
    }

    public IEnumerator<int> RunExpressions(List<LispExpression> expressions)
    {
        foreach (LispExpression expression in expressions)
        {
            string output = null;
            var enumerator = Runtime.Run(expression).GetEnumerator();
            while (true)
            {
                try
                {
                    if (!enumerator.MoveNext())
                    {
                        break;
                    }
                    LispExpression result = enumerator.Current;
                    if (result != null)
                    {
                        output = result.ToString();
                        break;
                    }
                }
                catch (SyntaxException e)
                {
                    Debug.LogException(e, this);
                    output = e.Message;
                    StartCoroutine(UsageStatsAPI.SendErrorInstance(e.Message));
                    break;
                }
                catch (RuntimeException e)
                {
                    Debug.LogException(e, this);
                    output = e.Message;
                    StartCoroutine(UsageStatsAPI.SendErrorInstance(e.Message));
                    break;
                }

                yield return 0;
            }

            terminal.AddTextBlock(output);
        }

        terminal.PromptForCommand();
        terminal.SetState(Terminal.State.TakingInput);
    }
}
