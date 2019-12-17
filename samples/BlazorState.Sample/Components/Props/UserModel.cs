namespace BlazorState.Sample.Components.Props
{
    public class UserModel
    {
        public UserModel(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }

        public string Address { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public UserModel WithFirstName(string newName)
        {
            return new UserModel(newName, this.LastName) { Address = this.Address };
        }

        public UserModel WithLastName(string newName)
        {
            return new UserModel(this.FirstName, newName) { Address = this.Address };
        }

        public UserModel WithAddress(string newAddress)
        {
            return new UserModel(this.FirstName, this.LastName) { Address = newAddress };
        }
    }
}
