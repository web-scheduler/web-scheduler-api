rmdir /S /Q Migrations


dotnet ef migrations add DataProtection -c DataProtectionKeysDbContext -o Data/Migrations/DataProtectionKeysDb

dotnet ef migrations script -c DataProtectionKeysDbContext -o Data/Migrations/DataProtectionKeysDb.sql
