namespace ProcessProxy.Messages
{
    public class Create
    {
        public string Asm { get; }
        public string Type { get; }

        public Create(string asm, string type)
        {
            Asm = asm;
            Type = type;
        }
    }
}
