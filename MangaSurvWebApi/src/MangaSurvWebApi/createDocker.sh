dotnet restore
dotnet publish -c Release -o out

docker build -t mangasurv/webapi .
docker save mangasurv/webapi > mangasurvwebapi_runtime.tar

mv /home/paradise/Documents/mangasurv/MangaSurvWebApi/src/MangaSurvWebApi/mangasurvwebapi_runtime.tar /home/paradise/docker_homeautomationapi/mangasurvwebapi_runtime.tar