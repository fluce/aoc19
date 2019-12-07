param ($day) 

dotnet new console -n $day
dotnet sln add $day
dotnet new xunit -n "$($day).Tests"
dotnet sln add "$($day).Tests"
dotnet add "$($day).Tests" reference $day
