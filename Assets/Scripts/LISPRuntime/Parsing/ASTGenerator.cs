using UnityEngine;
using System.Collections.Generic;

public class ASTGenerator
{
    private Lexer lexer;

    public ASTGenerator(Lexer lexer)
    {
        this.lexer = lexer;
    }

    public List<LispExpression> Generate()
    {
        List<LispExpression> ast = new List<LispExpression>();

        while (lexer.HasNext())
        {
            Lexeme lexeme = lexer.Next();
            
            if (lexeme.val == "(")
            {
                ast.Add(ListParser.Parse(lexer));
            }
            else if (lexeme.val[0] == '\"')
            {
                ast.Add(StringLiteralParser.Parse(lexeme.val));
            }
            else if ("1234567890+-".Contains(lexeme.val[0].ToString()))
            {
                ast.Add(NumberLiteralParser.Parse(lexeme.val));
            }
            else
            {
                ast.Add(SymbolParser.Parse(lexeme.val));
            }
        }

        return ast;
    }

}
