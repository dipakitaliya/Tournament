#!/bin/bash
container_name=cgtournament
db_name=cgtournament-pg
image_name=cgrpg-tournament

#check if postgres image exists, if not, then download
if [[ "$(sudo docker images -q postgres:latest 2> /dev/null)" == "" ]]; then
  sudo docker pull postgres
fi

if [ "$( sudo docker container inspect -f '{{.State.Running}}' $db_name )" == "true" ]; then 
  echo "Database running"
else
  #restart if does exist, create if does not
  sudo docker start $db_name || sudo docker run -d $db_name -e POSTGRES_PASSWORD=7r17y1pu77y -e POSTGRES_USER=citrus -d postgres:latest
  echo "Database running"
fi

result=$( sudo docker images -q $image_name)
if [[ -n "$result" ]]; then
  echo "image exists, deleting old one"
  sudo docker rmi -f $image_name
else
  echo "No such image"
fi
echo "building $image_name"
pg_ipaddr=$(sudo docker inspect -f '{{range.NetworkSettings.Networks}}{{.IPAddress}}{{end}}' $(sudo docker ps | grep -i $db_name | awk '{print $1}'))
sudo docker build --build-arg rel_type=$RELEASE_TYPE --build-arg pg_ip=$pg_ipaddr --no-cache -t $image_name .
result=$( sudo docker ps -q -f name=$container_name )
if [[ $? -eq 0 ]]; then
  echo "Container exists"
  sudo docker container rm -f $container_name
  echo "Deleted the existing docker container"
else
  echo "No such container"
fi
echo "Deploying the updated container"
sudo docker run -itd -p 9000:9000 --name $container_name $image_name:latest