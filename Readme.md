# Getting started
For first time overview you can use  <https://www.twilio.com/blog/test-web-apps-with-playwright-and-csharp-dotnet>
and chapters from tutorial https://www.lambdatest.com/learning-hub/how-to-install-playwright

## Local installation

Follow https://www.lambdatest.com/learning-hub/how-to-install-playwright

You may need to change a version
> pwsh bin/Debug/net8.0/playwright.ps1 install 

You may need to install latest PowerShell https://stackoverflow.com/questions/53849730/how-to-troubleshoot-the-error-the-term-pwsh-exe-is-not-recognized-as-the-name

## Writing tests
For writing initial tests use Playwright Test Generator and Test Inspector https://www.lambdatest.com/learning-hub/playwright-futuristic-features (see more in https://playwright.dev/dotnet/docs/debug#playwright-inspector  )
> npx playwright codegen #to record the test



# Debugging hints:
 When running from main solution and ServiceTestSetup.[AssemblyInitialize] is invoked, it's by some reason headless.
/// If you need to debug output, run standalone PlayWright.Tests and disable [AssemblyInitialize] in ServiceTestSetup

//If required to kill process see https://www.revisitclass.com/networking/how-to-kill-a-process-which-is-using-port-8080-in-windows/
        netstat -ano | findstr 5000  
        Run as Admin    taskkill /F /PID 12345  
# Future improvements

The current implementation is end-to-end tests that required running Apploication against backend environments.
If we will find that it is too flaky, we can mock the external storage, but it will require more work. If we will do it, we can use  [Mock APIs](https://playwright.dev/dotnet/docs/mock)


# Limitations
XUnit support is not complete (no Microsoft.Playwright.XUnit package), so MSTest was selected https://playwright.dev/dotnet/docs/test-runners#mstest

# Why Playwright, not Cypress

After reading the referred above links I see the following advantages to  use Playwright  compare to Cypress
 
* [Playwright is faster than Cypress](https://mtlynch.io/notes/cypress-vs-playwright/#playwright-is-significantly-faster-than-cypress) , [Parallel tests are free in Playwright](https://mtlynch.io/notes/cypress-vs-playwright/#parallel-tests-are-free-in-playwright)
* [Playwright requires less domain-specific knowledge](https://mtlynch.io/notes/cypress-vs-playwright/#playwright-requires-less-domain-specific-knowledge), [code is more straightforward](https://applitools.com/blog/cypress-vs-playwright/)
*  [Playwright supports s multiple languages](https://www.browserstack.com/guide/playwright-vs-cypress) including .NET C# and JavaScript
* [Debugging options include](https://www.browserstack.com/guide/playwright-vs-cypress) Playwright Inspector, VSCode Debugger, Browser Developer Tools, and Trace Viewers Console Logs.
* [Playwright supports multi-tabs and frames](https://medium.com/sparebank1-digital/playwright-vs-cypress-1e127d9157bd).
* [Better Support/t](https://mtlynch.io/notes/cypress-vs-playwright/#playwrights-team-doesnt-feel-resource-constrained)ime for response.
* Better Integration with ASP.NET  for Build/Deployment
 
