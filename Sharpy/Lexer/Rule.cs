namespace Sharpy.Lexer
{
    public interface Rule
    {
        UnboundToken? Apply(string s);
    }
}
