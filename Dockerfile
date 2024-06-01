ARG REGISTRY
FROM $REGISTRY/dotnet/sdk:8.0 as builder 
# if changing version, change aspnet:8.0 below and in dbupdate\Dockerfile.run_ef as well
WORKDIR /app

COPY ["MyProgram.sln", "./MyProgram.sln"]
COPY ["src/MyProgram/MyProgram.csproj", "./src/MyProgram/MyProgram.csproj"]
COPY test/PlayWright.Tests ./test/PlayWright.Tests/

# secret folder should be available locally,but should not be copied on runtime
#COPY .secret .secret #required to uncomment in .dockerignore.secret to run UI tests locally 
ARG SKIP_UI_TESTS
RUN echo "SKIP_UI_TESTS is $SKIP_UI_TESTS"
RUN dotnet restore

ENV ASPNETCORE_ENVIRONMENT STAGING  # DEVELOPMENT is on local machine only
COPY . .

RUN dotnet publish src/MyProgram/MyProgram.csproj -c Release -o /app  # > /dev/null 2>&1

#https://stackoverflow.com/questions/61617941/how-to-run-multiples-services-in-parallel-in-a-single-docker-container
#RUN dotnet ./MyProgram.dll & wait
COPY test/PlayWright.Tests/*.runsettings /app/
#If you want to exclude some categories see https://docs.microsoft.com/en-us/dotnet/core/testing/selective-unit-tests, e.g.
# dotnet test --filter "FullyQualifiedName~UnitTestClass1&TestCategory=CategoryA"
RUN if [ -z "$SKIP_UI_TESTS" ] ; then \
        dotnet restore test/PlayWright.Tests/ && \
        dotnet publish test/PlayWright.Tests/PlayWright.Tests.sln -c Release -o /app  && \
        pwsh playwright.ps1 install && \
        pwsh playwright.ps1 install-deps chromium && \
        dotnet test PlayWright.Tests.dll  --settings:dockerfile.runsettings ; \
    else \
        echo "Skipping UI tests." ; \
    fi

RUN dotnet publish src/MyProgram/MyProgram.csproj --output /out/ --configuration Release

FROM $REGISTRY/dotnet/aspnet:8.0

WORKDIR /app

COPY --from=builder /out /app
COPY --from=builder /app/src /app/src

RUN useradd worker
RUN chown worker:worker /app
USER worker

ENTRYPOINT ["dotnet", "MyProgram.dll"]
