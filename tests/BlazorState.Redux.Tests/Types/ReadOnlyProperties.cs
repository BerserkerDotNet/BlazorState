namespace BlazorState.Redux.Tests.Types
{
    public class ReadOnlyProperties
    {
        public string Name { get; }

        public string Title => "Principal";

        public int Age { get; set; }
    }
}
