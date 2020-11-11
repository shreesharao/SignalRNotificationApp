dotnet publish -o publish -c release
docker build -t shreesharao/cardiac.notificationservice:latest -f Dockerfile .
docker login --username shreesharao --password docker@123
docker push shreesharao/cardiac.notificationservice:latest
REM docker kill cardiac.notificationservice
REM docker run -d --rm -p 443:5000/tcp --network sn --name cardiac.notificationservice shreesharao/cardiac.notificationservice:latest