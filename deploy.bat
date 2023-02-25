@rd /s /q nugets
call dotnet build Rougamo.Retry.sln --configuration Release
for %%f in (nugets/*.nupkg) do (
	call dotnet nuget push nugets/%%f -k %IH_API_KEY% -s https://api.nuget.org/v3/index.json
)