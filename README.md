# Environment
Install EF Core tools:
`dotnet tool install -g dotnet-ef --version 3.0.0-preview7.19362.6`

> If already installed, replace with `update`. Match with versions available at https://www.nuget.org/packages/Microsoft.EntityFrameworkCore/

# EF Core commands:

Add Migration for profiles:

`dotnet ef migrations add AllocationMigration --project Ata.Investment.Allocation.Data --startup-project Ata.Investment.Api --context AllocationContext`
`dotnet ef migrations add ProfileMigration --project Ata.Investment.Profile.Data --startup-project Ata.Investment.Api --context ProfileContext`

Update database schema for profiles (using Investment schema)

`dotnet ef database update --project Ata.Investment.Profile.Data --startup-project Ata.Investment.Api --context ProfileContext`

`dotnet ef migrations add AllocationMigration --project Ata.Investment.Allocation.Data --startup-project Ata.Investment.Api --context AllocationContext`
`dotnet ef database update --project Ata.Investment.Allocation.Data --startup-project Ata.Investment.Api --context AllocationContext`


https://github.com/aspnet/AspNetCore/issues/10448

dotnet ef migrations add AuthMigration --project Web/Ata.Investment.AuthCore --startup-project Ata.Investment.Api --context AuthDbContext
dotnet ef database update --project Web/Ata.Investment.AuthCore --startup-project Ata.Investment.Api --context AuthDbContext

@if (SignInManager.IsSignedIn(User))
{
    <p>Hello @UserManager.GetUserName(User)!</p>
}
</ul>