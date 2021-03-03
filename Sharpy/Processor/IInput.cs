namespace Sharpy.Processor
{
    public interface IInput<TInput, TOutput>
    {
        TInput Advance(TOutput output);

        bool Empty();

        Lexer.Location? Location();
    }
}
