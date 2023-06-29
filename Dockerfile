FROM mono:latest
 
EXPOSE 9000

ADD . ./

ARG rel_type=STAGING
ARG pg_ip

ENV env_release_type=$rel_type

RUN apt-get update
RUN apt-get install -y --reinstall ca-certificates

RUN apt-get install -y ca-certificates-mono
RUN cert-sync /etc/ssl/certs/ca-certificates.crt

RUN msbuild CGRPG-Tournament.sln -t:restore -p:RestorePackagesConfig=true
RUN msbuild CGRPG-Tournament.sln -t:Build -p:Configuration=Release -p:DefineConstants="DOCKER $env_release_type"

ENV PG_ADDR "$pg_ip"
ENV PG_PORT "5432"
ENV PG_USER "citrus"
ENV PG_DB "postgres"
ENV PG_PASS "7r17y1pu77y"

CMD mono /CGRPG-Tournament/bin/Release/CGRPG_Tournament.exe --pgaddr $PG_ADDR --pgport $PG_PORT --pgdb $PG_DB --pguser $PG_USER --pgpass $PG_PASS
