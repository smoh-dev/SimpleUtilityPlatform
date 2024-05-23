@echo off

REM Check args.
if "%1"=="" (
    exit /b 1
)

REM Build docker image.
call docker build -f .\Sup.Np.Api\Dockerfile -t smoh92/np-api:%1 .
call docker build -f .\Sup.Np.IssueLoader\Dockerfile -t smoh92/np-ldr:%1 .
call docker build -f .\Sup.Np.PagePublisher\Dockerfile -t smoh92/np-pub:%1 .

REM Set tag to latest.
call docker tag smoh92/np-api:%1 smoh92/np-api:latest
call docker tag smoh92/np-ldr:%1 smoh92/np-ldr:latest
call docker tag smoh92/np-pub:%1 smoh92/np-pub:latest

REM Push docker image.
call docker push smoh92/np-api:%1
call docker push smoh92/np-ldr:%1
call docker push smoh92/np-pub:%1
call docker push smoh92/np-api:latest
call docker push smoh92/np-ldr:latest
call docker push smoh92/np-pub:latest