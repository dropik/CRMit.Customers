docker-compose -f docker-compose.yml -f docker-compose.override.yml build
docker-compose -f docker-compose.yml -f docker-compose.override.yml up --no-start
docker-compose -f docker-compose.yml -f docker-compose.override.yml start
timeout 30 > nul
UpdateDatabase.cmd > nul
