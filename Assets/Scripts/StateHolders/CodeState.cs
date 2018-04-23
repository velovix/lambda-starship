public class CodeState
{
    private static string code;

    static CodeState()
    {
        code = "";
    }

    public static void Set(string code)
    {
        CodeState.code = code;
    }

    public static string Get()
    {
        return code;
    }
}
