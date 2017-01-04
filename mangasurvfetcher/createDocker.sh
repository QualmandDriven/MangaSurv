dotnet restore
dotnet publish -c Release -o out

docker build -t mangasurv/fetcher .
docker save mangasurv/fetcher > mangasurvfetcher_runtime.tar