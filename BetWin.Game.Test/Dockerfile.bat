@echo on
RD /S /Q .\bin\publish
dotnet restore
dotnet publish -r linux-x64 --no-self-contained /p:PublishSingleFile=false -v quiet -c Release -o bin/publish
docker stop bw4-game-test
docker container rm bw4-game-test
docker image rmi betwin2015/betwin4-game-test
docker build -t betwin2015/betwin4-game-test -f Dockerfile .
docker push betwin2015/betwin4-game-test:latest
echo "BetWin.Game.Test ·¢²¼Íê±Ï"
pause