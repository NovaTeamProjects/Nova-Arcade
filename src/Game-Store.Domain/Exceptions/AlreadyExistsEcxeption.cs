namespace Game_Store.Domain.Exceptions
{
    public class AlreadyExistsEcxeption : Exception
    {
        public AlreadyExistsEcxeption(string message)
            : base(message) { }
    }
}
