namespace ModernizeMe
{
    class Member
    {
        private readonly string _name;

        public Member(string name)
        {
            _name = name;
        }
        public string Name
        {
            get { return _name; }
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj.GetType() == typeof(Member))
            {
                return Name.Equals(((Member)obj).Name);
            }
            return false;
        }
    }
}
