namespace AnimalsSqlDb
{
    public class AnimalNotFoundException: Exception
    {
        public AnimalNotFoundException() { }
        public AnimalNotFoundException(string message) : base(message) { }
        public AnimalNotFoundException(string message, Exception inner) : base(message, inner) { }
    }
}
