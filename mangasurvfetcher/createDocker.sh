dotnet restore
dotnet publish -c Release -o out

docker build -t mangasurv/fetcher .
docker save mangasurv/fetcher > mangasurvfetcher_runtime.tar

mv /home/paradise/Documents/mangasurv/mangasurvfetcher/mangasurvfetcher_runtime.tar /home/paradise/docker_homeautomationapi/mangasurvfetcher_runtime.tar