namespace BlazorState.Tests.Types
{
    public class NonPublicPropertiesState
    {
        public static string StaticProp { get; set; }

        public string Name { get; set; }

        internal string Company { get; set; }

        protected string Title { get; set; }

        private int Age { get; set; }

        public string GetCompany()
        {
            return Company;
        }

        public string GetTitle()
        {
            return Title;
        }

        public int GetAge()
        {
            return Age;
        }
    }
}
