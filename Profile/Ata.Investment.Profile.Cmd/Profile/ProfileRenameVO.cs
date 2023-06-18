namespace Ata.Investment.Profile.Cmd.Profile
{
    public class ProfileRenameVO
    {
        public ProfileRenameVO(Domain.Profile.Profile profile)
        {
            Profile = profile;
            Name = profile.Name;
            Icon = profile.Meta.Icon;
        }

        public string Name { get; set; }
        public string Icon { get; set; }
        public Domain.Profile.Profile Profile { get; private set; }
    }
}