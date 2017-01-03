dotnet restore
dotnet publish -c Release -o out

docker build -t mangasurv/webapi .
docker save mangasurv/webapi > mangasurvwebapi_runtime.tar